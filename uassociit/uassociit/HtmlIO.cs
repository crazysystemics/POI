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


        public List<string> GetHtmlString(ConsoleColor bg, ConsoleColor fg, string text)
        {
            // string s = "<p style=\"bgcolor:" + chmap[bg] + "\"" + " style=\u0022color: " + chmap[fg] + "\"" + " > ";
            // "style=\"color: " + chmap[fg] + ";\"" + " > ";
            //string  = "<p style=\"background-color: " + chmap[bg] + ";\">";
            List<string> slist = new List<string>();
            //slist.Add("<!DOCTYPE html>");
            slist.Add("<html>");
            slist.Add("<head>");
            slist.Add("<style>");
            slist.Add("table,th,td");  
            slist.Add("{border: 2px solid black;}");
            slist.Add("table");
            slist.Add("{");
            slist.Add("border-collapse:collapse;");
            slist.Add("width:20%;");
            slist.Add("color:black;");
            slist.Add("<}>");
            slist.Add("<td>");
            slist.Add("{height: 40px;}");
            slist.Add("#orange");
            slist.Add("{background-color:orange;}");
            slist.Add("#green");
            slist.Add("{background-color:green;}");
            slist.Add("#blue");
            slist.Add("{background-color:blue;}");
            slist.Add("#darkblue");
            slist.Add("{background-color:darkblue;}");
            slist.Add("</style>");
            slist.Add("</head >");


            slist.Add("<body>");
            slist.Add("<table>");
            slist.Add("<tr>");
            slist.Add("<th>");
            slist.Add("<Roll No>");
            slist.Add("</th>");
            slist.Add("<th>Name</th>");
            slist.Add("<th>Team</th>");
            slist.Add("</tr>");

            slist.Add("<tr>");
            slist.Add("<td>1001</td>");
            slist.Add("<td>John</td>");
            slist.Add("<td>Red</td>");
            slist.Add("</tr>");


            slist.Add("<tr>");
            slist.Add("<td>1002</td>");
            slist.Add("<td id=\"blue\">RaviJ</td>");
            slist.Add("<td>Blue</td>");
            slist.Add("</tr>");

            slist.Add("<tr>");
            slist.Add("<td>1003</td>");
            slist.Add("<td id=\"darkblue\">Henry</td>");
            slist.Add("<td>Green</td>");
            slist.Add("/tr");

            slist.Add("</table>");
            slist.Add("/body");
            slist.Add("/html");    

                 

            //s += text;
            //s += "</p>";
            return slist;          
        }

        public List<string> GetHtmlHead()
        {
            List<string> slist = new List<string>();
            //slist.Add("<!DOCTYPE html>");
            slist.Add("<html>");
            slist.Add("<head>");
            slist.Add("<style>");
            slist.Add("table,th,td");
            slist.Add("{border: 2px solid black;}");
            slist.Add("table");
            slist.Add("{");
            slist.Add("border-collapse:collapse;");
            slist.Add("width:20%;");
            slist.Add("color:black;");
            slist.Add("<}>");
            slist.Add("< td >");
            slist.Add("<td>");
            slist.Add("{height: 40px;}");

            slist.Add("#black");slist.Add("{background-color:black; color:white;}");
            slist.Add("#blue");slist.Add("{background-color:blue; color:white;}");
            slist.Add("#cyan");slist.Add("{background-color:cyan; color:black}");
            slist.Add("#darkblue");slist.Add("{background-color:darkblue; color:white;}");

            slist.Add("#darkcyan");slist.Add("{background-color:darkcyan; color:white;}");
            slist.Add("#darkgray");slist.Add("{background-color:darkgray; color:white;}");
            slist.Add("#darkgreen"); slist.Add("{background-color:darkgreen; color:white;}");
            slist.Add("#darkmagenta");slist.Add("{background-color:darkmagenta;color:white;}");

            slist.Add("#darkred");slist.Add("{background-color:darkred;color:white;}");
            slist.Add("#darkyellow");slist.Add("{background-color:darkyellow; color:white;}");
            slist.Add("#gray"); slist.Add("{background-color:gray;color:black}");
            slist.Add("#green"); slist.Add("{background-color:green;color:white;}");

            slist.Add("#magenta");slist.Add("{background-color:magenta;color:white;}");
            slist.Add("#red");slist.Add("{background-color:red;color:white;}");
            slist.Add("#white");slist.Add("{background-color:white;color:black;}");
            slist.Add("#yellow");slist.Add("{background-color:yellow;color:black;}");
            
            slist.Add("</style>");
            slist.Add("</head >");

            return slist;
        }

        public List<string> GetHtmlforMat (string[] row_heading, string[] col_heading, 
                                           string[,] Mat, int order,  FgBg[,] fgbgmat, 
                                           int top = 0, int left = 0)
        {
            List<string> slist = new List<string>();

            slist = GetHtmlHead();

            slist.Add("<body>");
            slist.Add("<table>");
            slist.Add("<tr>");
            if (col_heading == null)
            {
                //don't add header
            }
            else if (col_heading[0] =="*")
            {
                //add column numbers as header
                slist.Add("<th>" + "*" + "</th>");
                for (int j = left; j < left + order; j++)
                {
                    slist.Add("<th>" + j + "</th>");
                }

            }
            else
            {
                slist.Add("<th>" + "*" + "</th>");
                for (int j = left ; j < left + order; j++)
                {
                    slist.Add("<th>" + col_heading[j] + "</th>");
                }
            }
            slist.Add("</tr>");


            for (int i = top; i < top + order; i++)
            {
                slist.Add("<tr>");
                if (row_heading == null)
                {
                    
                }
                else if (row_heading[0] == "*")
                {
                    slist.Add("<td>" + i + "</td>");
                }
                else
                {
                    slist.Add("<td>" + row_heading[i] +  "</td>");
                }
                for (int j = left; j < order; j++)
                {                    
                    slist.Add("<td id=\"" + chmap[fgbgmat[i,j].Bg].ToLower() +"\">" + Mat[i,j] + "</td>");                    
                }
                slist.Add("</tr>");
            }

            slist.Add("</table>");
            slist.Add("/body");
            slist.Add("/html");

            return slist;
        }
    }
}
