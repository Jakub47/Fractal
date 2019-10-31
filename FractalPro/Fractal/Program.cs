using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;




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
        private static List<string> emails = new List<string>();

        private static List<KeyValuePair<string, string>> axiomAndRules;


        static void Main(string[] args)
        {
            InitializeRulesAndAxioms();
            Random rand = new Random();
            var pathToNames = @"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\emails.csv";
            using (TextFieldParser ParserCsv = new TextFieldParser(pathToNames))
            {
                ParserCsv.CommentTokens = new string[] { "#" };
                ParserCsv.SetDelimiters(new string[] { ",", "'", });
                ParserCsv.HasFieldsEnclosedInQuotes = true;

                ParserCsv.ReadLine();

                while (!ParserCsv.EndOfData)
                {
                    string[] entites = ParserCsv.ReadFields();

                    //To make sure i will not get all 60000 records select only words starting with given words
                    emails.Add(entites[0]);
                }
            }


            // var watch = new System.Diagnostics.Stopwatch();
            // watch.Start();

            int ccounter = 0;
            StringBuilder sB = new StringBuilder();

            foreach (var email in emails)
            {
                if(ccounter == 20)
                    Console.WriteLine("da");
                Console.WriteLine(ccounter);
                ccounter++;

                sB = new StringBuilder();
                bool shouldGetFromRuleList = rand.NextDouble() < 0.3; //30% for user to get rule from list == nice image but repetable
                string emailOfUser = email;
                string hash = "";
                using (MD5 md5 = MD5.Create())
                {
                    hash = GetMd5Hash(md5, emailOfUser);
                }
                var source = hash.GetHashCode();
                int iterationOfFractal = source & 7 - 2;
                if (iterationOfFractal < 1)
                    iterationOfFractal = 1;

                string axiom = "";
                string rule = "";


                if (shouldGetFromRuleList)
                {
                    int index = source & 15;
                    if (index > 12) index = 12;
                    int bitEle = (source >> 4) & 15;
                    bitEle = bitEle > 10 ? 10 : bitEle;

                    var getElement = axiomAndRules[bitEle];
                    axiom = getElement.Key;
                    rule = getElement.Value;
                }
                else
                {
                    int amountForAxiom = (source & 3) + 1; //get next 3 bits do & operation on it and add since (since number will be from 0 to 3 and we want from 1 to 4
                    int amountForRule = 0;
                    switch (iterationOfFractal)
                    {
                        case 1: amountForRule = 30; break;
                        case 2: amountForRule = 26; break;
                        case 3: amountForRule = 23; break;
                        case 4: amountForRule = 19; break;
                        case 5: amountForRule = 13; break;
                        case 6: amountForRule = 11; break;
                        case 7: amountForRule = 9; break;
                    }

                    axiom = GenerateContent(axiom, amountForAxiom, rand);
                    if (!axiom.Contains('F'))
                    {
                        if (axiom.Length >= 2)
                            axiom = axiom.Remove(1, 1).Insert(1, "F");
                        else
                            axiom += "F";
                    }
                    //generate rule
                    rule = GenerateContent(rule, amountForRule, rand);

                }

                for (int i = 0; i <= iterationOfFractal; i++)
                {
                    for (int y = 0; y < axiom.Length; y++)
                    {
                        if (char.IsLetter(axiom[y]))
                            sB.Append(rule);
                        else
                            sB.Append(axiom[y]);
                    }
                    axiom = sB.ToString();
                    if (axiom.Length >= 10000000)
                        break;
                    sB = new StringBuilder();
                }
                if (axiom.Length >= 8500000)
                {
                    //Take 7000000 from end to end - 700000
                    Random rnd = new Random();
                    int numToTake = rnd.Next(3000000, 6000000);
                    var smallerInitialString = axiom.Substring(axiom.Length - numToTake, numToTake);
                    axiom = smallerInitialString;
                }

                Bitmap mainImage = new Bitmap(1000, 1000);
                float currentX = 500;
                float currentY = 500;

                float iterate = 10;

                int longWidth =  (source >> 8) & 1;
                if (longWidth > 1)
                    longWidth = rand.Next(20, 30);
                else
                    longWidth = rand.Next(2, 13);

                int widthOfPen = 3;

                GraphicsPath gP;

                var color1 = int.Parse(hash.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var color2 = int.Parse(hash.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var color3 = int.Parse(hash.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                Color foreGround = Color.FromArgb(color1, color2, color3);

                var color4 = int.Parse(hash.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                var color5 = int.Parse(hash.Substring(8, 2), System.Globalization.NumberStyles.HexNumber);
                var color6 = int.Parse(hash.Substring(10, 2), System.Globalization.NumberStyles.HexNumber);
                Color backGround = Color.FromArgb(color4, color5, color6);


                using (Graphics g = Graphics.FromImage(mainImage))
                {
                    g.Clear(backGround);
                    gP = new GraphicsPath();

                    //g.ScaleTransform(-1, -1);
                    for (int i = 0; i < axiom.Length; i++)
                    {
                        if (char.IsLetter(axiom[i]))
                        {
                            for (int iA = 0; iA < iterateAxis.Length; iA++)
                            {
                                //up
                                if (iA == 0 && iterateAxis[iA])
                                {
                                    gP.AddLine(currentX, currentY, currentX, currentY - iterate);
                                    currentY -= iterate;
                                }
                                //right
                                else if (iA == 1 && iterateAxis[iA])
                                {
                                    gP.AddLine(currentX, currentY, currentX + iterate, currentY);
                                    currentX += iterate;
                                }
                                //down
                                else if (iA == 2 && iterateAxis[iA])
                                {
                                    gP.AddLine(currentX, currentY, currentX, currentY + iterate);
                                    currentY += iterate;
                                }
                                //left
                                else if (iA == 3 && iterateAxis[iA])
                                {
                                    gP.AddLine(currentX, currentY, currentX - iterate, currentY);
                                    currentX -= iterate;
                                }
                            }
                        }
                        else if (axiom[i] == '+')
                        {
                            changeAxis(false);
                        }
                        else if (axiom[i] == '-')
                        {
                            changeAxis(true);
                        }
                    }
                    g.DrawPath(new Pen(foreGround, widthOfPen), gP);

                }

                var T = GetMatrixFitRectInBounds(gP.GetBounds(), new RectangleF(0, 0, mainImage.Width, mainImage.Height));
                gP.Transform(T);
                Graphics cz = Graphics.FromImage(mainImage);
                cz.Clear(backGround);
                cz.DrawPath(new Pen(foreGround, widthOfPen), gP);

                mainImage.Save(@"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\" + email + ".jpg");
            }
            /*
            int iteration = 5;
            string initialString = "F+F-F";
            string tempInitalString = initialString;
            Dictionary<char, string> rules = new Dictionary<char, string>();
            rules.Add('B', "BWB"); rules.Add('W', "WWW"); rules.Add('F', "-FF-FF+FF+FF");

            #region SaveToFile
            /* 
             string path = @"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\yos.txt";
             string path2 = @"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\yos2.txt";
             File.WriteAllText(path, String.Empty);
             File.WriteAllText(path2, String.Empty);


             //Initial String. Fist iteration will not throw memory excpetion

             for (int y = 0; y < initialString.Length; y++)
             {
                 if (char.IsLetter(initialString[y]))
                     sB.Append(rules[initialString[y]]);
                 else
                     sB.Append(initialString[y]);
             }
             initialString = sB.ToString();
             using (StreamWriter swInitial = new StreamWriter(path))
             {
                 for (int i = 0; i < initialString.Length; i++)
                 {
                     swInitial.Write(initialString[i]);
                 }
             }


             //Stream l_fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
             //Stream l_fileStream2 = File.Open(path2, FileMode.Open, FileAccess.ReadWrite);

             //StreamWriter sw = new StreamWriter(l_fileStream);
             //StreamReader sr = new StreamReader(l_fileStream);
             //StreamWriter sw2 = new StreamWriter(l_fileStream2);
             //StreamReader sr2 = new StreamReader(l_fileStream2);

            // bool nextLine = false;
             for (int i = 1; i <= iteration; i++)
             {

                 using (StreamWriter sw = new StreamWriter(path2))
                 using (StreamReader sr = new StreamReader(path))
                 {
                     //Write to second line
                     while (sr.Peek() >= 0 && sr.Peek() != '\n')
                     {
                         char currentChar = (char)sr.Read();
                         if (char.IsLetter(currentChar))
                             sw.Write(rules[currentChar]);
                         else
                             sw.Write(currentChar);
                     }

                 }

                 File.WriteAllText(path, String.Empty);
                 using (StreamWriter sw = new StreamWriter(path))
                 using (StreamReader sr = new StreamReader(path2))
                 {
                     sw.Write(sr.ReadLine());
                 }
                 File.WriteAllText(path2, String.Empty);
             }
             
            #endregion

            #region SaveInMemory

            iteration = iteration == 0 ? 1 : iteration;
            for (int i = 0; i <= iteration; i++)
            {
                for (int y = 0; y < initialString.Length; y++)
                {
                    if (char.IsLetter(initialString[y]))
                        sB.Append(rules[initialString[y]]);
                    else
                        sB.Append(initialString[y]);
                }
                initialString = sB.ToString();
                if (initialString.Length >= 10000000)
                    break;
                sB = new StringBuilder();
            }
            if (initialString.Length >= 8500000)
            {
                //Take 7000000 from end to end - 700000
                Random rnd = new Random();
                int numToTake = rnd.Next(3000000, 6000000);
                var smallerInitialString = initialString.Substring(initialString.Length - numToTake, numToTake);
                initialString = smallerInitialString;
                Console.WriteLine(initialString.Length);
            }

            #endregion

            Bitmap mainImage = new Bitmap(1000, 1000);
            float currentX = 500;
            float currentY = 500;

            float iterate = 10;

            int widthOfPen = 3;

            GraphicsPath gP;

            #region drawingFromMemoryString
            /*
            using (Graphics g = Graphics.FromImage(mainImage))
            {
                g.Clear(Color.Black);
                gP = new GraphicsPath();

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
                                gP.AddLine(currentX, currentY, currentX, currentY - iterate);
                                currentY -= iterate;
                            }
                            //right
                            else if (iA == 1 && iterateAxis[iA])
                            {
                                gP.AddLine(currentX, currentY, currentX + iterate, currentY);
                                currentX += iterate;
                            }
                            //down
                            else if (iA == 2 && iterateAxis[iA])
                            {
                                gP.AddLine(currentX, currentY, currentX, currentY + iterate);
                                currentY += iterate;
                            }
                            //left
                            else if (iA == 3 && iterateAxis[iA])
                            {
                                gP.AddLine(currentX, currentY, currentX - iterate, currentY);
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
                g.DrawPath(new Pen(Color.Red, widthOfPen), gP);
            }
            
            #endregion
            #region readingFromFile
            /*
            using (Graphics g = Graphics.FromImage(mainImage))
            using(StreamReader sr = new StreamReader(path))
            {
                g.Clear(Color.Black);
                gP = new GraphicsPath();
                char currentLetter;
                while (sr.Peek() >= 0 && sr.Peek() != '\n')
                {
                    currentLetter = (char)sr.Read();

                    if (char.IsLetter(currentLetter))
                    {
                        for (int iA = 0; iA < iterateAxis.Length; iA++)
                        {
                            //up
                            if (iA == 0 && iterateAxis[iA])
                            {
                                gP.AddLine(currentX, currentY, currentX, currentY - iterate);
                                currentY -= iterate;
                            }
                            //right
                            else if (iA == 1 && iterateAxis[iA])
                            {
                                gP.AddLine(currentX, currentY, currentX + iterate, currentY);
                                currentX += iterate;
                            }
                            //down
                            else if (iA == 2 && iterateAxis[iA])
                            {
                                gP.AddLine(currentX, currentY, currentX, currentY + iterate);
                                currentY += iterate;
                            }
                            //left
                            else if (iA == 3 && iterateAxis[iA])
                            {
                                gP.AddLine(currentX, currentY, currentX - iterate, currentY);
                                currentX -= iterate;
                            }
                        }
                    }
                    else if (currentLetter == '+')
                    {
                        changeAxis(false);
                    }
                    else if (currentLetter == '-')
                    {
                        changeAxis(true);
                    }
                }
                g.DrawPath(new Pen(Color.Red, widthOfPen), gP);
            }
            
            #endregion

            var T = GetMatrixFitRectInBounds(gP.GetBounds(), new RectangleF(0, 0, mainImage.Width, mainImage.Height));
            gP.Transform(T);
            Graphics cz = Graphics.FromImage(mainImage);
            cz.Clear(Color.Black);
            cz.DrawPath(new Pen(Color.Red, widthOfPen), gP);
            mainImage.Save(@"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\yos.jpg");
            return;
            */
            //SizeF sF = new SizeF(boundOfFractal.Size);
            //sF.Height *= .500f;
            //sF.Width *= .500f;

            //boundOfFractal.Size = sF;
            //gP.Dispose();
            //while ((boundOfFractal.Location.X + boundOfFractal.Size.Width) > 0
            //        || (boundOfFractal.Location.X + boundOfFractal.Size.Width) < 0
            //        || (boundOfFractal.Location.Y + boundOfFractal.Size.Height) > 0
            //        || (boundOfFractal.Location.Y + boundOfFractal.Size.Height) < 0
            //        )
            //{
            //    boundOfFractal.Top > 0 ? boundOfFractal.Offset()
            //}
            //// here


            //var zxz = cz.Top;
            //var zxz1 = cz.Bottom;
            //var zxz2 = cz.Right;
            //var zxz3 = cz.Left;

            //var zxc4 = cz.Location;
            //var zxc5 = cz.Size;
            //var zxc6 = cz.X;
            //var zxc7 = cz.Y;

            // here





            //float max = pointsOfImage.Max(a => a.x2);
            //bool wasScaled = false;

            //if (max  > 1000)
            //{
            //    using (Graphics g = Graphics.FromImage(mainImage))
            //    {
            //        g.Clear(Color.Black);

            //        var m = new System.Drawing.Drawing2D.Matrix();
            //        m.Translate(max - 1200,0); //z 600 da 1100
            //        if (!wasScaled)
            //        {
            //            m.Scale(0.9f, 0.9f);
            //            wasScaled = true;
            //        }
            //        gP.Transform(m);
            //        g.DrawPath(new Pen(Color.Red), gP);
            //    }
            //}



            //float min = pointsOfImage.Min(a => a.x2);

            //if (min < 0)
            //{
            //    using (Graphics g = Graphics.FromImage(mainImage))
            //    {
            //        g.Clear(Color.Black);

            //        var m = new System.Drawing.Drawing2D.Matrix();
            //        m.Translate(min + 600, 0); //z 600 da 1100
            //        if (!wasScaled) { 
            //            m.Scale(0.9f, 0.9f);
            //            wasScaled = true;
            //        }
            //        gP.Transform(m);
            //        g.DrawPath(new Pen(Color.Red), gP);
            //    }
            //}


            //max = pointsOfImage.Max(a => a.y2);


            //if (max > 1000)
            //{
            //    using (Graphics g = Graphics.FromImage(mainImage))
            //    {
            //        g.Clear(Color.Black);

            //        var m = new System.Drawing.Drawing2D.Matrix();
            //        m.Translate(0, max - 1300); //z 600 da 1100
            //        if (!wasScaled)
            //        {
            //            m.Scale(0.9f, 0.9f);
            //            wasScaled = true;
            //        }
            //        gP.Transform(m);
            //        g.DrawPath(new Pen(Color.Red), gP);
            //    }
            //}

            //min = pointsOfImage.Min(a => a.y1);

            //if (min < 0)
            //{
            //    using (Graphics g = Graphics.FromImage(mainImage))
            //    {
            //        g.Clear(Color.Black);

            //        var m = new System.Drawing.Drawing2D.Matrix();
            //        m.Translate(0, min + 600); //z 600 da 1100
            //        if (!wasScaled)
            //        {
            //            m.Scale(0.9f, 0.9f);
            //            wasScaled = true;
            //        }
            //        gP.Transform(m);
            //        g.DrawPath(new Pen(Color.Red), gP);
            //    }
            //}
            //if (pointsOfImage.Max(a => a.x1 > 1000) || pointsOfImage.Max(a => a.x2) > 1000)
            //{
            //    using (Graphics g = Graphics.FromImage(mainImage))
            //    {
            //        g.Clear(Color.Black);

            //        var m = new System.Drawing.Drawing2D.Matrix();
            //        m.Translate(-200, 0); //z 600 da 1100
            //        m.Scale(0.5f, 0.5f);
            //        gP.Transform(m);
            //        g.DrawPath(new Pen(Color.Red), gP);
            //    }
            //}



            ////Get First Pixe

            //var allPixels = PixelOfGivenColor(mainImage);

            //Point leftPixel = allPixels.OrderBy(a => a.X).First();
            //Point rightPixel = allPixels.OrderByDescending(a => a.X).First();
            //Point topPixel = allPixels.OrderBy(a => a.Y).First();
            //Point bottomPixel = allPixels.OrderByDescending(a => a.Y).First();

            ////float distance1 = distanceBetweenTwoPoints(leftPixel, rightPixel);
            ////float distance2 = distanceBetweenTwoPoints(bottomPixel, topPixel);
            //float distance1 = (rightPixel.X - leftPixel.X) + 40;
            //float distance2 = (bottomPixel.Y - topPixel.Y) + 40;
            //int position1 = leftPixel.X - 20;
            //int position2 = topPixel.Y - 20;


            //Bitmap newImg1 = new Bitmap(Convert.ToInt32(distance1), Convert.ToInt32(distance2));
            //Rectangle rectToPlace = new Rectangle(position1, position2, Convert.ToInt32(distance1), Convert.ToInt32(distance2));

            //using (Graphics zxc = Graphics.FromImage(newImg1))
            //{
            //    zxc.DrawImage(mainImage, new Rectangle(0, 0, newImg1.Width, newImg1.Height), rectToPlace, GraphicsUnit.Pixel);
            //}
            ////  mainImage = ResizeImage(mainImage, distance1, distance2);

            //mainImage.Save(@"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\yos.jpg");
            //newImg1 = ResizeImage(newImg1, 500, 500);
            //newImg1.Save(@"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\yos2.jpg");
            // mainImage.Dispose();
        }


        private static string GenerateContent(string initialString,int expLength,Random rand)
        {
            //Generate Axiom
            while (initialString.Length < 1)
            {
                //Initialize first character of string
                if (rand.NextDouble() <= 0.5)
                    initialString = "F";
                else if (rand.NextDouble() <= 0.5)
                    initialString = "-";
                else if (rand.NextDouble() <= 0.5)
                    initialString = "+";
            }
            while (initialString.Length < expLength)
            {
                switch (initialString[initialString.Length - 1])
                {
                    case 'F':
                        if (rand.NextDouble() <= 0.15)
                            initialString += 'F';
                        else if (rand.NextDouble() <= 0.45)
                            initialString += '-';
                        else if (rand.NextDouble() <= 0.45)
                            initialString += '+';
                        break;
                    case '+':
                    case '-':
                        if (rand.NextDouble() <= 0.8)
                            initialString += 'F';
                        else if (rand.NextDouble() <= 0.1)
                            initialString += '-';
                        else if (rand.NextDouble() <= 0.1)
                            initialString += '+';
                        break;
                }
            }
            return initialString;

        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
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

            public CustomPoint(float x1, float y1, float x2, float y2)
            {
                this.x1 = x1;
                this.x2 = x2;
                this.y1 = y1;
                this.y2 = y2;
            }
        }
        //https://stackoverflow.com/questions/20445474/matrix-and-graphicspath
        private static Matrix GetMatrixFitRectInBounds(RectangleF fitrect, RectangleF boundsrect)
        {
            var T = new Matrix();

            var bounds_center = new PointF(boundsrect.Width / 2, boundsrect.Height / 2);

            //Set translation centerpoint
            T.Translate(bounds_center.X, bounds_center.Y);

            //Get smallest size to scale to fit boundsrect
            float scale = Math.Min(boundsrect.Width / fitrect.Width, boundsrect.Height / fitrect.Height);

            T.Scale(scale, scale);

            //Move fitrect to center of boundsrect
            T.Translate(bounds_center.X - fitrect.X - fitrect.Width / 2f, bounds_center.Y - fitrect.Y - fitrect.Height / 2f);

            //Restore translation point
            T.Translate(-bounds_center.X, -bounds_center.Y);

            return T;

        }
        private static void InitializeRulesAndAxioms()
        {
            axiomAndRules = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("F","F+F−F−F+F"),
                new KeyValuePair<string, string>("F-F-F-F","F+FF-FF-F-F+F+F"),
                new KeyValuePair<string, string>("-F","F+F-F-F+F"),
                new KeyValuePair<string, string>("F-F-F-F","FF-F-F-F-F-F+F"),
                new KeyValuePair<string, string>("F-F-F-F","FF-F-F-F-FF"),
                new KeyValuePair<string, string>("F-F-F-F","FF-F+F-F-FF"),
                new KeyValuePair<string, string>("F-F-F-F","FF-F--F-F"),
                new KeyValuePair<string, string>("F-F-F-F","F-FF--F-F"),
                new KeyValuePair<string, string>("F-F-F-F","F-F+F-F-F"),
                new KeyValuePair<string, string>("F+F+F+F","F+F-F-FF+F+F-F"),
                new KeyValuePair<string, string>("F+F+F+F","FF+F-F+F+FF"),
            };
        }
    }
}
