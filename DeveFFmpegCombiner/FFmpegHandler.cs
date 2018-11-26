using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DeveFFmpegCombiner
{
    public class FFmpegHandler
    {
        private readonly string ffmpegExe;

        public FFmpegHandler(string ffmpegExe)
        {
            this.ffmpegExe = ffmpegExe;
        }

        public void CreateTimeLapse(string pathToCombine, string outputFileName, Rectangle selectionRect, Rectangle pictureInPictureRect, int takeEveryNthPicture = 1)
        {
            var fileListPath = Path.Combine(pathToCombine, Constants.FileListFileName);
            var files = CreateFilesListInternal(pathToCombine).TakeNthItems(takeEveryNthPicture).ToList();

            using (var streamWriter = new StreamWriter(new FileStream(fileListPath, FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                foreach (var file in files)
                {
                    var toWrite = $"file '{file}'";
                    Console.WriteLine(toWrite);
                    streamWriter.WriteLine(toWrite);
                }
            }

            Console.WriteLine("File created with list of files");

            var outFile = Path.Join(pathToCombine, outputFileName);

            if (File.Exists(outFile))
            {
                File.Delete(outFile);
            }

            int frameRate = 60;
            int outWidth = 1920 * 2;
            int outHeight = 1080 * 2;

            int sizeOfPip = 120 * (outWidth / 1920);

            var arguments = "";
            arguments += $"-f concat ";
            arguments += $"-safe 0 ";
            arguments += $"-r {frameRate} ";
            arguments += $"-i \"{fileListPath}\" ";
            arguments += $"-f concat ";
            arguments += $"-safe 0 ";
            arguments += $"-r {frameRate} ";
            arguments += $"-i \"{fileListPath}\" ";
            arguments += $"-r {frameRate} ";
            //arguments += $"-filter_complex \"[1],crop = 2478:1205:40:40; [0] [pip] overlay=main_w-overlay_w-10:main_h-overlay_h-10\" ";
            if (pictureInPictureRect == Rectangle.Empty)
            {
                arguments += $"-filter_complex \"[0]crop={selectionRect.ToFFmpegStr()},scale={outWidth}:{outHeight}\" ";
            }
            else
            {
                arguments += $"-filter_complex \"[1]crop={pictureInPictureRect.ToFFmpegStr()},scale={sizeOfPip}:-1 [pip]; [0]crop={selectionRect.ToFFmpegStr()},scale={outWidth}:{outHeight} [cropped];[cropped][pip]overlay=10:10\" ";
            }
            arguments += $"-pix_fmt yuv420p ";
            arguments += $"-vcodec libx264 ";
            arguments += $"-crf 18 ";
            arguments += $"-preset slow "; //ultrafast,superfast,veryfast,faster,fast,medium,slow,slower,veryslow,placebo
            //arguments += $"-filter:v \"crop=4000:2250:0:660,scale=1920:1080\" ";
            arguments += $"\"{outFile}\"";

            Console.WriteLine($"Running command: ffmpeg {arguments}");

            var procStart = new ProcessStartInfo(ffmpegExe, arguments);
            using (var process = Process.Start(procStart))
            {
                process.WaitForExit();
            }

            Console.WriteLine("Completely done");
        }

        private IEnumerable<string> CreateFilesListInternal(string path)
        {
            var dirs = Directory.GetDirectories(path).CustomSort().ToList();

            foreach (var dir in dirs)
            {
                var foundStuff = CreateFilesListInternal(dir);
                foreach (var item in foundStuff)
                {
                    yield return item;
                }
            }

            var files = Directory.GetFiles(path).CustomSort().ToList();

            foreach (var fileItem in files)
            {
                var extension = Path.GetExtension(fileItem);
                if (Constants.ValidImageTypes.Any(t => t.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    yield return fileItem;
                }
            }
        }
    }
}
