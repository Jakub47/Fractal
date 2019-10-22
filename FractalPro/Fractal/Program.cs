using System;
using System.Collections.Generic;
using System.Drawing;
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

        static void Main(string[] args)
        {
            int iteration = 4;
            string initialString = "F+F+F+F";
            Dictionary<char, string> rules = new Dictionary<char, string>();
            rules.Add('B', "BWB"); rules.Add('W', "WWW"); rules.Add('F', "F+F-F-FF+F+F-F");
            StringBuilder sB = new StringBuilder();


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


            Bitmap mainImage = new Bitmap(5000, 5000);
            int currentX = 300;
            int currentY = 300;

            //up,right,down,left

            int iterate = 10;


            Graphics newGraphics = Graphics.FromImage(mainImage);
            newGraphics.Clear(Color.White); //Set background color to white
            using (Graphics g = Graphics.FromImage(mainImage))
            {

                for (int i = 0; i < initialString.Length; i++)
                {
                    if (initialString[i] == 'F')
                    {
                        for (int iA = 0; iA < iterateAxis.Length; iA++)
                        {
                            //up
                            if (iA == 0 && iterateAxis[iA])
                            {
                                g.DrawLine(new Pen(Color.Black), currentX, currentY, currentX, currentY - iterate);
                                currentY -= iterate;
                            }
                            //right
                            else if(iA == 1 && iterateAxis[iA])
                            {
                                g.DrawLine(new Pen(Color.Black), currentX, currentY, currentX + iterate, currentY);
                                currentX += iterate;
                            }
                            //down
                            else if (iA == 2 && iterateAxis[iA])
                            {
                                g.DrawLine(new Pen(Color.Black), currentX, currentY, currentX , currentY + iterate);
                                currentY += iterate;
                            }
                            //left
                            else if (iA == 3 && iterateAxis[iA])
                            {
                                g.DrawLine(new Pen(Color.Black), currentX, currentY, currentX - iterate, currentY);
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
            }

            mainImage.Save(@"C:\Users\Ragnus\Desktop\PI\Fractal\FractalPro\Fractal\yos.jpg");
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
    }
}
