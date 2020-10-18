using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace uassociit
{
    class HtmlIO
    {

    }

    class HtmlWriter: HtmlIO
    {
        Dictionary<ConsoleColor, string> chmap = new Dictionary<ConsoleColor, string>();

        public HtmlWriter()
        {
            chmap.Add(ConsoleColor.Black,       "black");
            chmap.Add(ConsoleColor.Blue,        "blue");
            chmap.Add(ConsoleColor.Cyan,        "cyan");
            chmap.Add(ConsoleColor.DarkBlue,    "darkblue");

            chmap.Add(ConsoleColor.DarkCyan,    "darkcyan");
            chmap.Add(ConsoleColor.DarkGray,    "darkgray");
            chmap.Add(ConsoleColor.DarkGreen,   "darkgreen");
            chmap.Add(ConsoleColor.DarkMagenta, "darkmagenta");

            chmap.Add(ConsoleColor.DarkRed,     "darkred");
            chmap.Add(ConsoleColor.DarkYellow,  "darkyellow");
            chmap.Add(ConsoleColor.Gray,        "gray");
            chmap.Add(ConsoleColor.Green,       "green");

            chmap.Add(ConsoleColor.Magenta,     "magenta");
            chmap.Add(ConsoleColor.Red,         "red");
            chmap.Add(ConsoleColor.White,        "white");
            chmap.Add(ConsoleColor.Yellow,      "yellow");
        }


        public void GetHtmlHead(Log plog)
        {          
            //slist.Add("<!DOCTYPE html>");
            plog.slist.Add("<html>");
            plog.slist.Add("<head>");
            plog.slist.Add("<style>");
            plog.slist.Add("table,th,td");
            plog.slist.Add("{border: 2px solid black;}");
            plog.slist.Add("table");
            plog.slist.Add("{");
            plog.slist.Add("border-collapse:collapse;");
            plog.slist.Add("width:20%;");
            plog.slist.Add("color:black;");
            plog.slist.Add("}");
            plog.slist.Add("<td>");
            plog.slist.Add("{height: 40px;}");

            plog.slist.Add("#black");plog.slist.Add("{background-color:black; color:white;}");
            plog.slist.Add("#blue");plog.slist.Add("{background-color:blue; color:white;}");
            plog.slist.Add("#cyan");plog.slist.Add("{background-color:cyan; color:black}");
            plog.slist.Add("#darkblue");plog.slist.Add("{background-color:darkblue; color:white;}");

            plog.slist.Add("#darkcyan");plog.slist.Add("{background-color:darkcyan; color:white;}");
            plog.slist.Add("#darkgray");plog.slist.Add("{background-color:darkgray; color:white;}");
            plog.slist.Add("#darkgreen");plog.slist.Add("{background-color:darkgreen; color:white;}");
            plog.slist.Add("#darkmagenta");plog.slist.Add("{background-color:darkmagenta;color:white;}");

            plog.slist.Add("#darkred");   plog.slist.Add("{background-color:darkred;color:white;}");
            plog.slist.Add("#darkyellow");plog.slist.Add("{background-color:darkyellow; color:white;}");
            plog.slist.Add("#gray");      plog.slist.Add("{background-color:gray;color:black}");
            plog.slist.Add("#green");     plog.slist.Add("{background-color:green;color:white;}");

            plog.slist.Add("#magenta");plog.slist.Add("{background-color:magenta;color:white;}");
            plog.slist.Add("#red");    plog.slist.Add("{background-color:red;color:white;}");
            plog.slist.Add("#white");  plog.slist.Add("{background-color:white;color:black;}");
            plog.slist.Add("#yellow"); plog.slist.Add("{background-color:yellow;color:black;}");
            plog.slist.Add("</style>");
            plog.slist.Add("</head>");
            plog.slist.Add("<body>");
           
        }

        public void GetHtmlString(Log plog, ConsoleColor bg, ConsoleColor fg, string text)
        {
            string call_level_prefix = String.Empty;

            if (text.Contains("canBeCombined"))
            {
                
            }
            if (text.Contains("@Entry"))
            {
                sglobal.logger.call_level++;
            }

            for (int i = 0; i < sglobal.logger.call_level; i++)
            {
                call_level_prefix += "........";
            }

            plog.slist.Add("<p background-color: id=\"" + chmap[bg] + "\">" + call_level_prefix + text + "</p>");

            if (text.Contains("@Exit"))
            {
                sglobal.logger.call_level--;
            }
        }

        public void GetHtmlforMat (Log plog, string[] row_heading, string[] col_heading, 
                                           string[,] Mat, int matrows, int matcols, FgBg[,] fgbgmat, 
                                           int top = -1 , int left = -1, int bottom = -1 , int right = -1)
        {
            
            top    = (top     == -1 ? 0 : top);
            left   = (left    == -1 ? 0 : left);
            bottom = (bottom  == -1 ? matrows - 1 : bottom);
            right =  (right   == -1 ? matcols - 1 : right);

            //top, left, bottom, right are wrt to Mat 
            //Check whether they are out-of-bound
            Debug.Assert(top <= bottom && left <= right);
            // Debug.Assert(top > 0 && top < matrows && bott)


            plog.slist.Add("<table>");
            plog.slist.Add("<tr>");
            if (col_heading == null)
            {
                //don't add header
            }
            else if (col_heading[0] =="*")
            {
                //add column numbers as header
                plog.slist.Add("<th>" + "*" + "</th>");
                for (int j = left; j <= right ; j++)
                {
                    plog.slist.Add("<th>" + j + "</th>");
                }

            }
            else
            {
                plog.slist.Add("<th>" + "*" + "</th>");
                for (int j = left ; j <= right; j++)
                {
                    plog.slist.Add("<th>" + col_heading[j] + "</th>");
                }
            }
            plog.slist.Add("</tr>");

           
            for (int i = top; i <= bottom; i++)
            {
                plog.slist.Add("<tr>");
                if (row_heading == null)
                {
                    
                }
                else if (row_heading[0] == "*")
                {
                    plog.slist.Add("<td>" + i + "</td>");
                }
                else
                {
                    plog.slist.Add("<td>" + row_heading[i] +  "</td>");
                }

                //TODO: BUG: We are sending Colors for cluster and Mat for Sky.
                //Should sync both
                for (int j = left; j <= right; j++)
                {
                    plog.slist.Add("<td id=\"" + chmap[fgbgmat[i, j].Bg].ToLower() +"\">" + Mat[i, j] + "</td>");                    
                }
                plog.slist.Add("</tr>");
            }

            plog.slist.Add("</table>");           
        }
    }
}
