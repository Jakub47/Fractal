using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    class Program
    {
        static void Main(string[] args)
        {
            int iteration = 3;
            string initialString = "B";
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

            Console.WriteLine(initialString);
            Console.ReadKey();


            //for (int steps = 1; steps < iteration; steps++)
            //{
            //    StringBuilder sB = new StringBuilder();

            //    foreach (var item in initialStringSubString)
            //    {
            //        if (rules.ContainsKey(Convert.ToChar(item)))
            //        {
            //            sB.Append(rules[Convert.ToChar(item)]);
            //            sB.Append("|");
            //        }
            //        else
            //            sB.Append(rules[Convert.ToChar(item)]);
            //    }

            //    //for (int i = 0; i < initialString.Length; i++)
            //    //{
            //    //    if(rules.ContainsKey(initialString[i]))
            //    //        sB.Append(rules[initialString[i]]);
            //    //}

            //    //initialString = sB.ToString();
            //    //sB = new StringBuilder();
            //}

            //BWBWWWBWB

        }
    }
}
