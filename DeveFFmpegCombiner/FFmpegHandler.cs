using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DeveFFmpegCombiner
{
    public class FFmpegHandler
    {
        private readonly string ffmpegExe;
        private readonly string pathToCombine;
        private readonly string outputFileName;
        private readonly string fileListPath;

        public FFmpegHandler(string ffmpegExe, string pathToCombine, string outputFileName)
        {
            this.ffmpegExe = ffmpegExe;
            this.pathToCombine = pathToCombine;
            this.outputFileName = outputFileName;
            fileListPath = Path.Combine(pathToCombine, Constants.FileListFileName);
        }

        public void CreateFilesList()
        {
            var files = CreateFilesListInternal(pathToCombine).Take(100).ToList();

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
            arguments += $"-framerate 30 ";
            arguments += $"-pix_fmt yuv420p ";
            arguments += $"-vcodec libx264 ";
            arguments += $"-crf 18 ";
            arguments += $"-preset slow ";
            arguments += $"-filter:v \"crop=4000:2250:0:660,scale=1920:1080\" ";
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
                yield return fileItem;
            }
        }
    }
}
