using System;
using System.Threading.Tasks;

namespace DeveFFmpegCombiner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            MainAsync(args).GetAwaiter().GetResult();

            Console.WriteLine("Application done, press any key to exit . . .");
            Console.ReadKey();
        }

        public static async Task MainAsync(string[] args)
        {
            var ffmpegHandler = new FFmpegHandler(Constants.FfmpegPath, @"C:\TheCFolder\Avalan2018Timelapse", "OutputTimeLapse.mp4");

            ffmpegHandler.CreateFilesList();
        }
    }
}
