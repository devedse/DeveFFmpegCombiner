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

        public void CreateTimeLapse(string pathToCombine, string outputFileName, Rectangle selectionRect, Rectangle pictureInPictureRect)
        {
            var fileListPath = Path.Combine(pathToCombine, Constants.FileListFileName);
            var files = CreateFilesListInternal(pathToCombine).ToList();

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

            var arguments = "";
            arguments += $"-f concat ";
            arguments += $"-safe 0 ";
            arguments += $"-i \"{fileListPath}\" ";
            arguments += $"-f concat ";
            arguments += $"-safe 0 ";
            arguments += $"-i \"{fileListPath}\" ";
            arguments += $"-framerate 60 ";
            //arguments += $"-filter_complex \"[1],crop = 2478:1205:40:40; [0] [pip] overlay=main_w-overlay_w-10:main_h-overlay_h-10\" ";
            arguments += $"-filter_complex \"[1]crop={pictureInPictureRect.ToFFmpegStr()},scale=240:-2 [pip]; [0][pip] overlay=10:670,crop={selectionRect.ToFFmpegStr()},scale=1920:1080\" ";
            arguments += $"-pix_fmt yuv420p ";
            arguments += $"-vcodec libx264 ";
            arguments += $"-crf 18 ";
            arguments += $"-preset slow ";
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
            var dirs = Directory.GetDirectories(path);


            //var blah = dirs.Select(t => new
            //{
            //    Original = t,
            //    Dingetje = new string(Path.GetFileName(t).TakeWhile(c => char.IsDigit(c)).ToArray())
            //});

            //var dirsOrdered = dirs.OrderBy(t => int.Parse(new string(Path.GetFileName(t).TakeWhile(c => char.IsDigit(c)).ToArray()))).ToList();

            foreach (var dir in dirs)
            {
                var foundStuff = CreateFilesListInternal(dir);
                foreach (var item in foundStuff)
                {
                    yield return item;
                }
            }

            foreach (var fileItem in Directory.GetFiles(path))
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
