using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public void GetHtmlHead()
        {          
            //slist.Add("<!DOCTYPE html>");
            sglobal.slist.Add("<html>");
            sglobal.slist.Add("<head>");
            sglobal.slist.Add("<style>");
            sglobal.slist.Add("table,th,td");
            sglobal.slist.Add("{border: 2px solid black;}");
            sglobal.slist.Add("table");
            sglobal.slist.Add("{");
            sglobal.slist.Add("border-collapse:collapse;");
            sglobal.slist.Add("width:20%;");
            sglobal.slist.Add("color:black;");
            sglobal.slist.Add("}");
            sglobal.slist.Add("<td>");
            sglobal.slist.Add("{height: 40px;}");

            sglobal.slist.Add("#black");sglobal.slist.Add("{background-color:black; color:white;}");
            sglobal.slist.Add("#blue");sglobal.slist.Add("{background-color:blue; color:white;}");
            sglobal.slist.Add("#cyan");sglobal.slist.Add("{background-color:cyan; color:black}");
            sglobal.slist.Add("#darkblue");sglobal.slist.Add("{background-color:darkblue; color:white;}");

            sglobal.slist.Add("#darkcyan");sglobal.slist.Add("{background-color:darkcyan; color:white;}");
            sglobal.slist.Add("#darkgray");sglobal.slist.Add("{background-color:darkgray; color:white;}");
            sglobal.slist.Add("#darkgreen");sglobal.slist.Add("{background-color:darkgreen; color:white;}");
            sglobal.slist.Add("#darkmagenta");sglobal.slist.Add("{background-color:darkmagenta;color:white;}");

            sglobal.slist.Add("#darkred");   sglobal.slist.Add("{background-color:darkred;color:white;}");
            sglobal.slist.Add("#darkyellow");sglobal.slist.Add("{background-color:darkyellow; color:white;}");
            sglobal.slist.Add("#gray");      sglobal.slist.Add("{background-color:gray;color:black}");
            sglobal.slist.Add("#green");     sglobal.slist.Add("{background-color:green;color:white;}");

            sglobal.slist.Add("#magenta");sglobal.slist.Add("{background-color:magenta;color:white;}");
            sglobal.slist.Add("#red");    sglobal.slist.Add("{background-color:red;color:white;}");
            sglobal.slist.Add("#white");  sglobal.slist.Add("{background-color:white;color:black;}");
            sglobal.slist.Add("#yellow"); sglobal.slist.Add("{background-color:yellow;color:black;}");
            sglobal.slist.Add("</td>");
            sglobal.slist.Add("<body>");
           
        }

        public void GetHtmlString(ConsoleColor bg, ConsoleColor fg, string text)
        {
            sglobal.slist.Add("<p background-color:\"" + chmap[bg] + "\"" + text + "</p>");
        }

        public List<string> GetHtmlforMat (string[] row_heading, string[] col_heading, 
                                           string[,] Mat, int order,  FgBg[,] fgbgmat, 
                                           int top = 0, int left = 0)
        {
            sglobal.slist.Add("<table>");
            sglobal.slist.Add("<tr>");
            if (col_heading == null)
            {
                //don't add header
            }
            else if (col_heading[0] =="*")
            {
                //add column numbers as header
                sglobal.slist.Add("<th>" + "*" + "</th>");
                for (int j = left; j < left + order; j++)
                {
                    sglobal.slist.Add("<th>" + j + "</th>");
                }

            }
            else
            {
                sglobal.slist.Add("<th>" + "*" + "</th>");
                for (int j = left ; j < left + order; j++)
                {
                    sglobal.slist.Add("<th>" + col_heading[j] + "</th>");
                }
            }
            sglobal.slist.Add("</tr>");

           
            for (int i = top; i < top + order; i++)
            {
                sglobal.slist.Add("<tr>");
                if (row_heading == null)
                {
                    
                }
                else if (row_heading[0] == "*")
                {
                    sglobal.slist.Add("<td>" + i + "</td>");
                }
                else
                {
                    sglobal.slist.Add("<td>" + row_heading[i] +  "</td>");
                }
                for (int j = left; j < order; j++)
                {
                    sglobal.slist.Add("<td id=\"" + chmap[fgbgmat[i,j].Bg].ToLower() +"\">" + Mat[i,j] + "</td>");                    
                }
                sglobal.slist.Add("</tr>");
            }

            sglobal.slist.Add("</table>");

            return sglobal.slist;
        }
    }
}
