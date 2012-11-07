using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2CSV
{
    class Program
    {
        const string IMAGE_DIRECTORY = @"C:\temp\images";

        const string FILE_NAME = @"C:\temp\images.csv";

        const int REQUESTED_DIMENSIONS = 20;

        const string DELIMETER = ",";

        static void Main(string[] args)
        {
            if (File.Exists(FILE_NAME))
            {
                Console.WriteLine(string.Format("File {0} already exists.  Overwrite? (Y/n)", FILE_NAME));
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.N)
                {
                    Console.WriteLine("Closing");
                    return;
                }
                else
                {
                    File.Delete(FILE_NAME);
                    Console.WriteLine("File Deleted");
                }
            }

            //get all the image files in the directory
            IEnumerable<FileInfo> files = GetFiles(IMAGE_DIRECTORY);

            ProcessFiles(files);

            Console.WriteLine("Finished.");
            Console.WriteLine(string.Format("CSV Saved to {0}", FILE_NAME));
            Console.WriteLine("Press any key to continue..");
            Console.Read();
        }

        private static IEnumerable<FileInfo> GetFiles(string imageDirectory)
        {
            DirectoryInfo di = new DirectoryInfo(imageDirectory);
            return di.EnumerateFiles().Where(f => !f.Name.Equals("thumbs.db", StringComparison.InvariantCultureIgnoreCase));
        }

        private static void ProcessFiles(IEnumerable<FileInfo> files)
        {
            int filesComplete = 0;

            int totalFiles = files.Count();
            StringBuilder csv = new StringBuilder();

            foreach (var file in files)
            {
                try
                {
                    //rescale the image to requested dimensions
                    using (Bitmap image = new Bitmap(file.FullName))
                    using (Bitmap scaledImage = ResizeBitmap(image, REQUESTED_DIMENSIONS, REQUESTED_DIMENSIONS))
                    {
                        //warn them if the image is not square
                        if (image.Width != image.Height)
                        {
                            Console.WriteLine();
                            Console.WriteLine(string.Format("WARNING: Original image was not square.  Image: {0}", file.FullName));
                        }

                        //create a csv with the following structure:
                        //FileName, px1, px2, px3,...pxn
                        //pixels will start in the top left, make their way across the row, then to the next row (like you'd read a book in English)
                        csv.Append(file.FullName);
                        csv.Append(DELIMETER);

                        List<float> pixels = new List<float>(REQUESTED_DIMENSIONS * REQUESTED_DIMENSIONS);
                        for (int y = 0; y < REQUESTED_DIMENSIONS; y++)
                        {
                            for (int x = 0; x < REQUESTED_DIMENSIONS; x++)
                            {
                                //Get the brightness of the current pixel
                                pixels.Add(scaledImage.GetPixel(x, y).GetBrightness());
                            }
                        }

                        csv.AppendLine(string.Join(DELIMETER, pixels));

                        filesComplete++;

                        //log and write every 100 files
                        if (filesComplete % 100 == 0)
                        {
                            Console.Write("Processing file: {0}", file.Name);

                            System.IO.File.AppendAllText(FILE_NAME, csv.ToString());
                            csv.Clear();

                            Console.Write("...Done!");
                            Console.WriteLine(string.Format("[{0}/{1}] {2}%", filesComplete, totalFiles, (int)(((double)filesComplete / totalFiles) * 100)));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            //append the final lines
            System.IO.File.AppendAllText(FILE_NAME, csv.ToString());
            csv.Clear();
        }

        //From http://www.peterprovost.org/blog/2003/05/29/Resize-Image-in-C/
        public static Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            return result;
        }
    }
}
