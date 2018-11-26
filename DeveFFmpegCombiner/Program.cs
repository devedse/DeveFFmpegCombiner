using System;
using System.Drawing;
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
            var ffmpegHandler = new FFmpegHandler(Constants.FfmpegPath);


            var selectionRect1 = new Rectangle(0, 660, 4000, 2250);
            var pictureInPictureRect1 = new Rectangle(2479, 1201, 40, 40);

            ffmpegHandler.CreateTimeLapse(@"C:\TheCFolder\Avalan2018Timelapse\Part 1 Direct", "OutputTimeLapse.mp4", selectionRect1, pictureInPictureRect1);



            var selectionRect2 = new Rectangle(0, 700, 4000, 2250);
            var pictureInPictureRect2 = new Rectangle(2455, 1440, 40, 40);

            ffmpegHandler.CreateTimeLapse(@"C:\TheCFolder\Avalan2018Timelapse\Part 2 Direct", "OutputTimeLapse.mp4", selectionRect2, pictureInPictureRect2);
        }
    }
}
