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

            int takeNthPicture = 100;


            var selectionRect1 = new Rectangle(0, 240, 4000, 2250);
            var pictureInPictureRect1 = Rectangle.Empty;

            ffmpegHandler.CreateTimeLapse(@"S:\Foto en Video\Amerika 2017 Rondreis Roy Davy\GoPro", "TheTimeLapse.mp4", selectionRect1, pictureInPictureRect1, takeNthPicture);

        }
    }
}
