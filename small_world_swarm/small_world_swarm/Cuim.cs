using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace small_world_swarm
{
    class ConsoleUiManager
    {
        public void PrintSimpleData(string title, string[] data,
                                    ConsoleColor tbg, ConsoleColor tfg,
                                    ConsoleColor dbg, ConsoleColor dfg)
        {
            //Title
            Console.BackgroundColor = tbg;
            Console.ForegroundColor = tfg;

            Console.WriteLine(title);

            Console.ResetColor();

            Console.BackgroundColor = dbg;
            Console.ForegroundColor = dfg;

            foreach (string d in data)
                Console.WriteLine(d);

            Console.ResetColor();
        }

        public void PrintLine(string[] Line)
        {
            string line = String.Empty;
            foreach (string l in Line)
            {
                line += l + "\t";
            }

            line.TrimEnd('\t');
            Console.WriteLine(line);
        }

        public void PrintTable(string title, string header, string[] datalines, char delim,
                               ConsoleColor tbg, ConsoleColor tfg,
                               ConsoleColor hbg, ConsoleColor hfg,
                               ConsoleColor bbg, ConsoleColor bfg)
        {
            //Title
            Console.BackgroundColor = tbg;
            Console.ForegroundColor = tfg;

            Console.WriteLine(title);

            Console.ResetColor();

            //Header
            Console.BackgroundColor = hbg;
            Console.ForegroundColor = hfg;

            string[] ha = header.Split(delim);
            PrintLine(ha);

            Console.ResetColor();

            //Body
            Console.BackgroundColor = bbg;
            Console.ForegroundColor = bfg;
            foreach (string sd in datalines)
            {
                string[] data = sd.Split(delim);
                PrintLine(data);
            }
            Console.ResetColor();


        }

        public void PrintMatrix(string title, string[,] mat, int rows, int cols,
                                 ConsoleColor tbg, ConsoleColor tfg,
                                 ConsoleColor bg, ConsoleColor fg
                                     )
        {
            //Title
            Console.BackgroundColor = tbg;
            Console.ForegroundColor = tfg;

            Console.WriteLine(title);

            Console.ResetColor();

            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(mat[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }


        public void PrintMatrix(string title, string[,] mat, int rows, int cols,
                        string[] disp, int[] posi, int[] posj,
                        ConsoleColor tbg, ConsoleColor tfg,
                        ConsoleColor defbg, ConsoleColor deffg,
                        ConsoleColor[] bg, ConsoleColor[] fg, string delim = " ")
        {

            //Title
            Console.BackgroundColor = tbg;
            Console.ForegroundColor = tfg;

            Console.WriteLine(title);

            Console.ResetColor();


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {

                    int k;
                    for (k = 0; k < disp.Length; k++)
                    {
                        if (i == posi[k] && j == posj[k])
                        {
                            Console.ForegroundColor = fg[k];
                            Console.BackgroundColor = bg[k];
                            Console.Write(disp[k] + delim);
                            Console.ResetColor();
                            break;
                        }
                    }

                    //not found
                    if (k == disp.Length)
                    {
                        Console.ForegroundColor = deffg;
                        Console.BackgroundColor = defbg;
                        Console.Write(mat[i, j] + delim);

                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }

        }

        public void PrintMatrix(string title, string[,] mat, int rows, int cols,
                string disp, int posi, int posj,
                ConsoleColor tbg, ConsoleColor tfg,
                ConsoleColor defbg, ConsoleColor deffg,
                ConsoleColor bg, ConsoleColor fg, string delim = " ")
        {
            //Title
            Console.BackgroundColor = tbg;
            Console.ForegroundColor = tfg;

            Console.WriteLine(title);

            Console.ResetColor();

            for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < cols; j++)
                {

                    if (i == posi && j == posj)
                    {
                        Console.ForegroundColor = fg;
                        Console.BackgroundColor = bg;
                        Console.Write(disp + delim);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = deffg;
                        Console.BackgroundColor = defbg;
                        Console.Write(mat[i, j] + delim);
                        Console.ResetColor();
                    }
                }

                Console.WriteLine();
            }
            Console.ResetColor();
        }


        public void PrintHistogram(string title, int[] hist, int max, char disp,
                                    ConsoleColor tbg, ConsoleColor tfg,
                                    ConsoleColor bg, ConsoleColor fg, string delim
                                  )
        {
            //Title
            Console.BackgroundColor = tbg;
            Console.ForegroundColor = tfg;

            Console.WriteLine(title);

            Console.ResetColor();

            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;

            for (int i = max - 1; i > 0; i--)
            {
                for (int j = 0; j < hist.Length; j++)
                {

                    if (i <= hist[j])
                    {
                        Console.Write(disp + delim);
                        hist[j]--;
                    }
                    else
                    {
                        Console.Write(' ' + delim);
                    }

                    //Console.Write(delim);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
            return;
        }

        public void PrintLinePlot(string title, int[] plot, int max, char disp,
                                   ConsoleColor tbg, ConsoleColor tfg,
                                  ConsoleColor bg, ConsoleColor fg,
                                  string delim = " ")
        {
            //Title
            Console.BackgroundColor = tbg;
            Console.ForegroundColor = tfg;

            Console.WriteLine(title);

            Console.ResetColor();

            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;

            for (int i = max - 1; i > 0; i--)
            {
                for (int j = 0; j < plot.Length; j++)
                {
                    if (i == plot[j])
                    {
                        Console.Write(disp + delim);
                    }
                    else
                    {
                        Console.Write(delim + delim);
                    }


                    //Console.WriteLine(delim);
                }
                Console.WriteLine();
            }

            Console.ResetColor();
        }

        public void RunTests()
        {
            ConsoleUiManager cum = this;

            double r = 10;
            double area = 2 * Math.PI * r;
            string[] sa = new string[2];

            //case-1
            sa[0] = "radius = " + r.ToString();
            sa[1] = "area = " + area.ToString();
            cum.PrintSimpleData("Simple Engineering Data", sa,
                                 ConsoleColor.Red, ConsoleColor.White,
                                 ConsoleColor.DarkYellow, ConsoleColor.Black);

            //case-2
            string[] h1 = new string[3];
            h1[0] = "name";
            h1[1] = "age";
            h1[2] = "salary";

            string[] d1 = new string[3];
            d1[0] = "so";
            d1[1] = "40";
            d1[2] = "100";

            cum.PrintLine(h1);
            cum.PrintLine(d1);

            //case-3
            string h2 = "product, quantity, price";
            string[] d2 = new string[2];
            d2[0] = "tv, 1, 25000";
            d2[1] = "charger, 3, 10000";
            cum.PrintTable("Table 1", h2, d2, ',',
                            ConsoleColor.Yellow, ConsoleColor.Black,
                            ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow,
                            ConsoleColor.Green, ConsoleColor.White);

            //case-4a            
            string[,] strmat = { { "0", "1", "2" }, { "3", "4", "5" }, { "6", "7", "8" } };
            cum.PrintMatrix("Simple Matrix", strmat, 3, 3,
                             ConsoleColor.Yellow, ConsoleColor.Black,
                             ConsoleColor.Blue, ConsoleColor.Cyan);

            //case-4b
            //string[,] intmat = { { "0", "1", "2" }, { "3", "4", "5" }, { "6", "7", "8" } };
            ConsoleColor cbg = ConsoleColor.Red;
            ConsoleColor cfg = ConsoleColor.Yellow;
            cum.PrintMatrix("Matrix with Multi Overwrites", strmat, 3, 3,
                            new string[] { "a", "b", "c" },
                            new int[] { 0, 1, 2 }, new int[] { 0, 1, 2 },
                            ConsoleColor.Yellow, ConsoleColor.Black,
                            ConsoleColor.DarkBlue, ConsoleColor.Yellow,
                            new ConsoleColor[] { cbg, cbg, cbg }, new ConsoleColor[] { cfg, cfg, cfg }
                            );

            //case-4c
            //string[,] intmat = { { "0", "1", "2" }, { "3", "4", "5" }, { "6", "7", "8" } };
            cbg = ConsoleColor.Red;
            cfg = ConsoleColor.Yellow;
            cum.PrintMatrix("Matrix with Single Overwrite", strmat, 3, 3,
                            "b", 1, 1,
                            ConsoleColor.Yellow, ConsoleColor.Black,
                            ConsoleColor.Black, ConsoleColor.White,
                            cbg, cfg, " ");

            //case-5
            cum.PrintHistogram("Histogram 1", new int[] { 3, 5, 7, 4, 2, 1 }, 9, '*',
                               ConsoleColor.Yellow, ConsoleColor.Black,
                               ConsoleColor.DarkBlue, cfg, "\t");


            //case-6
            cum.PrintLinePlot("Plot 1", new int[] { 3, 5, 7, 4, 2, 1 }, 9, '*',
                               ConsoleColor.Yellow, ConsoleColor.Black,
                               ConsoleColor.DarkMagenta, cfg, " ");

        }

    }
}

    