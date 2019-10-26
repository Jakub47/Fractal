using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    /// <summary>
    /// Draw Fractal using rule
    /// </summary>
    class Program
    {
        //up,right,down,left
        public static bool[] iterateAxis = new bool[4] { false, true, false, false };
        private static List<CustomPoint> pointsOfImage = new List<CustomPoint>();

        static void Main(string[] args)
        {
            int iteration = 6;
            string initialString = "F-F-F-F";
            Dictionary<char, string> rules = new Dictionary<char, string>();
            rules.Add('B', "BWB"); rules.Add('W', "WWW"); rules.Add('F', "F-FF--F-F");
            StringBuilder sB = new StringBuilder();


            if (iteration == 0)
            {
                for (int y = 0; y < initialString.Length; y++)
                {
                    if (char.IsLetter(initialString[y]))
                        sB.Append(rules[initialString[y]]);
                    else
                        sB.Append(initialString[y]);
                }
                initialString = sB.ToString();
                sB = new StringBuilder();
            }
            else
            {
                for (int i = 0; i < iteration; i++)
                {
                    for (int y = 0; y < initialString.Length; y++)
                    {
                        if (char.IsLetter(initialString[y]))
                            sB.Append(rules[initialString[y]]);
                        else
                            sB.Append(initialString[y]);
                    }
                    initialString = sB.ToString();
                    sB = new StringBuilder();
                }
            }

            Bitmap mainImage = new Bitmap(1000, 1000);
            float currentX = 600f;
            float currentY = 700f;

            //up,right,down,left
            //The Algorithmic Beauty of Plants

            float iterate = 2f;


            Graphics newGraphics = Graphics.FromImage(mainImage);
            newGraphics.Clear(Color.Black); //Set background color to white

            using (Graphics g = Graphics.FromImage(mainImage))
            {
                //g.ScaleTransform(-1, -1);
                for (int i = 0; i < initialString.Length; i++)
                {
                    if (char.IsLetter(initialString[i]))
                    {
                        for (int iA = 0; iA < iterateAxis.Length; iA++)
                        {
                            //up
                            if (iA == 0 && iterateAxis[iA])
                            {
                                g.DrawLine(new Pen(Color.Red), currentX, currentY, currentX, currentY - iterate);
                                pointsOfImage.Add(new CustomPoint(currentX, currentY, currentX, currentY - iterate));
                                currentY -= iterate;
                            }
                            //right
                            else if (iA == 1 && iterateAxis[iA])
                            {
                                g.DrawLine(new Pen(Color.Red), currentX, currentY, currentX + iterate, currentY);
                                pointsOfImage.Add(new CustomPoint(currentX, currentY, currentX + iterate, currentY));
                                currentX += iterate;
                            }
                            //down
                            else if (iA == 2 && iterateAxis[iA])
                            {
                                g.DrawLine(new Pen(Color.Red), currentX, currentY, currentX, currentY + iterate);
                                pointsOfImage.Add(new CustomPoint(currentX, currentY, currentX, currentY + iterate));
                                currentY += iterate;
                            }
                            //left
                            else if (iA == 3 && iterateAxis[iA])
                            {
                                g.DrawLine(new Pen(Color.Red), currentX, currentY, currentX - iterate, currentY);
                                pointsOfImage.Add(new CustomPoint(currentX, currentY, currentX - iterate, currentY));
                                currentX -= iterate;
                            }
                        }
                    }
                    else if (initialString[i] == '+')
                    {
                        changeAxis(false);
                    }
                    else if (initialString[i] == '-')
                    {
                        changeAxis(true);
                    }
                }

                if (pointsOfImage.Max(a => a.x1) > 1000 || pointsOfImage.Min(a => a.x1) < 0 || pointsOfImage.Max(a => a.y1) > 1000 || pointsOfImage.Min(a => a.y1) < 0)
                {
                    g.Clear(Color.Black);
                    pointsOfImage.ForEach(a =>
                    {
                        g.DrawLine(new Pen(Color.Red), a.x1, a.x2, a.y1, a.y2);
                    });
                }
            }

            //Get First Pixe

            var allPixels = PixelOfGivenColor(mainImage);

            Point  leftPixel = allPixels.OrderBy(a => a.X).First();
            Point rightPixel  = allPixels.OrderByDescending(a => a.X).First();
            Point topPixel  = allPixels.OrderBy(a => a.Y).First();
            Point bottomPixel = allPixels.OrderByDescending(a => a.Y).First();

            //float distance1 = distanceBetweenTwoPoints(leftPixel, rightPixel);
            //float distance2 = distanceBetweenTwoPoints(bottomPixel, topPixel);
            float distance1 = (rightPixel.X - leftPixel.X) + 1;
            float distance2 = (bottomPixel.Y - topPixel.Y) + 1;

            Bitmap newImg1 = new Bitmap(Convert.ToInt32(distance1), Convert.ToInt32(distance2));
            Rectangle rectToPlace = new Rectangle(leftPixel.X, topPixel.Y, Convert.ToInt32(distance1), Convert.ToInt32(distance2));

            using (Graphics zxc = Graphics.FromImage(newImg1))
            {
                zxc.DrawImage(mainImage, new Rectangle(0, 0, newImg1.Width, newImg1.Height), rectToPlace, GraphicsUnit.Pixel);
            }
            //  mainImage = ResizeImage(mainImage, distance1, distance2);

            mainImage.Save(@"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\yos.jpg");
            newImg1 = ResizeImage(newImg1, 500, 500);
            newImg1.Save(@"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\yos2.jpg");
            // mainImage.Dispose();
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private static float distanceBetweenTwoPoints(Point point1, Point point2)
        {
            return (float)(Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2)));

            //var a = (double)(point2.X - point1.X);
            //var b = (double)(point2.Y - point1.Y);

            //return (float)Math.Sqrt(a * a + b * b);
        }
        private static List<Point> PixelOfGivenColor(Bitmap scrBitmap)
        {
            Color actualColor;
            List<Point> yos = new List<Point>();

            //make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < scrBitmap.Width; i++)
            {
                for (int j = 0; j < scrBitmap.Height; j++)
                {
                    //get the pixel from the scrBitmap image
                    actualColor = scrBitmap.GetPixel(i, j);

                    // > 150 because.. Images edges can be of low pixel colr. if we set all pixel color to new then there will be no smoothness left.
                    if (actualColor.ToArgb() == Color.Red.ToArgb())
                        yos.Add(new Point(i, j));
                }
            }

            return yos;
        }
        private static void changeAxis(bool nextAxis)
        {
            for (int i = 0; i < iterateAxis.Length; i++)
            {
                if (iterateAxis[i] && nextAxis)
                {
                    iterateAxis[i] = false;
                    if (i + 1 >= iterateAxis.Length)
                        iterateAxis[0] = true;
                    else
                        iterateAxis[i + 1] = true;

                    break;
                }
                else if (iterateAxis[i] && !nextAxis)
                {
                    iterateAxis[i] = false;
                    if (i == 0)
                        iterateAxis[iterateAxis.Length - 1] = true;
                    else
                        iterateAxis[i - 1] = true;

                    break;
                }
            }
        }

        struct CustomPoint
        {
            public float x1;
            public float x2;
            public float y1;
            public float y2;

            public CustomPoint(float x1, float x2, float y1, float y2)
            {
                this.x1 = x1;
                this.x2 = x2;
                this.y1 = y1;
                this.y2 = y2;
            }
        }
    }
}
