using System;
using System.Drawing;

namespace Maze1
{
    /// <summary>
    /// Static class for image processing and conversion
    /// </summary>
    internal static class ImageHelper
    {
        /// <summary>
        /// Function to Convert source image to 2D integer array
        /// </summary>
        /// <param name="sourceLocation"></param>
        /// <returns></returns>
        internal static int[,] Image2DIntArrayConverter(string sourceLocation)
        {
            try
            {
                // Saving input image using bitmap class
                Bitmap bitmap = new Bitmap(@sourceLocation);
                // Initiate corresponding 2D integer array for use throughout the application
                int[,] imgArray = new int[bitmap.Width, bitmap.Height];
                int whiteArgb = Color.White.ToArgb(), redArgb = Color.Red.ToArgb(), blueArgb = Color.Blue.ToArgb(), greenArgb = Color.Green.ToArgb(), pixelColArgb, i, j,
                    bitmapWidth = bitmap.Width, bitmapHeight = bitmap.Height;

                // Scan the image for White, Red and Blue pixels for efficient image processing
                for (i = 0; i < bitmapWidth; i++)
                {
                    for (j = 0; j < bitmapHeight; j++)
                    {
                        pixelColArgb = bitmap.GetPixel(i, j).ToArgb();

                        if (pixelColArgb == whiteArgb)
                        {
                            imgArray[i, j] = 0;
                        }
                        else if (pixelColArgb == redArgb)
                        {
                            imgArray[i, j] = 2;
                        }
                        else if (pixelColArgb == blueArgb)
                        {
                            imgArray[i, j] = 3;
                        }
                        // Handel any other color other than white, red and blue as the boundary of the maze
                        else if (pixelColArgb != whiteArgb && pixelColArgb != redArgb && pixelColArgb != blueArgb)
                        {
                            imgArray[i, j] = 1;
                        }
                    }
                }
                return imgArray;
            }
            catch(Exception e)
            {
                Console.WriteLine("Error with Source file: " + e.Message + ". Press Enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
                return null;
            }
        }

        /// <summary>
        /// Function to convert the 2D integer array to the destination image by checking the extension desired for the output file
        /// </summary>
        /// <param name="destinationLocation"></param>
        /// <param name="imgArray"></param>
        internal static void MakeandSaveImageinDesiredFormat(string destinationLocation, int[,] imgArray)
        {
            try
            {
                int bitmapWidth = imgArray.GetLength(0), bitmapHeight = imgArray.GetLength(1);
                // Initialize new bitmap image for saving the 2D integer array
                Bitmap img = new Bitmap(bitmapWidth, bitmapHeight);

                for (var i = 0; i < bitmapWidth; i++)
                {
                    for (var j = 0; j < bitmapHeight; j++)
                    {
                        switch (imgArray[i, j])
                        {
                            case 1:
                                img.SetPixel(i, j, Color.Black);
                                break;
                            case 0:
                                img.SetPixel(i, j, Color.White);
                                break;
                            case 2:
                                img.SetPixel(i, j, Color.Red);
                                break;
                            case 3:
                                img.SetPixel(i, j, Color.Blue);
                                break;
                            case 4:
                                img.SetPixel(i, j, Color.Green);
                                break;
                        }
                    }
                }

                switch (destinationLocation.Split('.')[1])
                {
                    case "bmp":
                        img.Save(@destinationLocation, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "png":
                        img.Save(@destinationLocation, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case "jpg":
                        img.Save(@destinationLocation, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + ", occurred while creating destination image. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }
    }
}
