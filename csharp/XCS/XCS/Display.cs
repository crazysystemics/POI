using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace POI_XCS
{
    class console_win
    {
        public uint x_offs, y_offs;
        public uint width, height, id;
        public int y_orig, x_orig;
        ConsoleColor fg, bg;
        public char[,] frame_buf = new char[256, 256];
        public char[,] fb_color_bg = new char[256, 256];
        public char[,] fb_color_fg = new char[256, 256];
        public char[,] fbs_color_bg = new char[256, 256];
        public char[,] fbs_color_fg = new char[256, 256];

        public console_win()
        {

        }

        public console_win(uint xoffs, uint yoffs, uint x_width, uint y_height, System.ConsoleColor bg_color, System.ConsoleColor fg_color)
        {
            y_offs = yoffs;
            x_offs = xoffs;
            width = x_width; ;
            height = y_height;
            //y_orig = ((int)x_width + (int)x_offs) - 1;
            //x_orig = (int)y_height + (int)y_offs - 1;
            y_orig = (int)x_offs;
            x_orig = (int)y_height + (int)y_offs - 1;

            fg = fg_color;
            bg = bg_color;
            for (uint i = y_offs; i < height + y_offs; i++)
                for (uint j = x_offs; j < width + x_offs; ++j)
                {
                    frame_buf[i, j] = ' ';
                    fb_color_bg[i, j] = (char)bg_color;
                    fb_color_fg[i, j] = (char)fg_color;
                    fbs_color_bg[i, j] = (char)bg_color;
                    fbs_color_fg[i, j] = (char)fg_color;
                }
        }

        public void clear()
        {
            for (uint i = y_offs; i < height + y_offs; i++)
            {
                for (uint j = x_offs; j < width + x_offs; ++j)
                {
                    Console.SetCursorPosition((int)j, (int)i);

                    Console.BackgroundColor = bg; //(System.ConsoleColor)fbs_color_bg[i, j]; // ConsoleColor.Gray;
                    Console.ForegroundColor = fg; //(System.ConsoleColor)fbs_color_fg[i, j];  //ConsoleColor.Black;
                    frame_buf[i, j] = ' ';
                    //Console.Write(frame_buf[i, j]);
                    fb_color_fg[i, j] = (char)fg; // fbs_color_fg[i, j];
                    fb_color_bg[i, j] = (char)bg; // fbs_color_bg[i, j];
                }
                //Console.WriteLine("");
            }
        }

        public void draw()
        {
            for (uint i = y_offs; i < height + y_offs; i++)
            {
                for (uint j = x_offs; j < width + x_offs; ++j)
                {
                    Console.SetCursorPosition((int)j, (int)i);

                    Console.BackgroundColor = (System.ConsoleColor)fb_color_bg[i, j]; // ConsoleColor.Gray;
                    Console.ForegroundColor = (System.ConsoleColor)fb_color_fg[i, j];  //ConsoleColor.Black;

                    Console.Write(frame_buf[i, j]);
                    //fb_color_fg[i, j] = fbs_color_fg[i, j];
                    //fb_color_bg[i, j] = fbs_color_bg[i, j];
                }
                Console.WriteLine("");
            }
        }

        public void putchar(uint x, uint y, char ch)
        {
            int xs = (int)x * -1; // + (int)x_offs;
            bool b1, b2;
            int x_pos, y_pos;

            y_pos = (int)x_orig - (int)y;
            x_pos = (int)y_orig + (int)x;
            b1 = ((x_pos >= x_offs) && (x_pos <= (width + x_offs)));
            b2 = ((y_pos >= y_offs) && (y_pos <= (height + y_offs)));
            if (!(b1 || b2))
                return;

            if (x_pos < 0 || y_pos < 0)
                return;

            //fbs_color_fg[y_pos, x_pos] = fb_color_fg[y_pos, x_pos];
            //fbs_color_bg[y_pos, x_pos] = fb_color_bg[y_pos, x_pos];
            frame_buf[y_pos, x_pos] = ch;
        }

        public void putchar(uint x, uint y, char ch, System.ConsoleColor fg, System.ConsoleColor bg)
        {
            int xs = (int)x * -1; // + (int)x_offs;
            bool b1, b2;
            int x_pos, y_pos;

            y_pos = (int)x_orig - (int)y;
            x_pos = (int)y_orig + (int)x;
            b1 = ((x_pos >= x_offs) && (x_pos <= (width + x_offs)));
            b2 = ((y_pos >= y_offs) && (y_pos <= (height + y_offs)));
            if (!(b1 || b2))
                return;

            if (x_pos < 0 || y_pos < 0)
                return;

            if ((char)fb_color_fg[y_pos, x_pos] != (char)fg)
                fbs_color_fg[y_pos, x_pos] = fb_color_fg[y_pos, x_pos];

            if ((char)fb_color_bg[y_pos, x_pos] != (char)bg)
                fbs_color_bg[y_pos, x_pos] = fb_color_bg[y_pos, x_pos];

            fb_color_fg[y_pos, x_pos] = (char)fg;
            fb_color_bg[y_pos, x_pos] = (char)bg;
            frame_buf[y_pos, x_pos] = ch;
        }

        public void puttext(uint x, uint y, string str, System.ConsoleColor fg, System.ConsoleColor bg)
        {
            for (uint i = 0; i < str.Length; ++i)
                putchar(x + i, y, (char)str.ElementAt((int)i), fg, bg);
        }

        public void puttext(uint x, uint y, string str)
        {
            for (uint i = 0; i < str.Length; ++i)
                putchar(x + i, y, (char)str.ElementAt((int)i));
        }

        public void draw_curve(uint x_orig, uint y_orig, uint radius)
        {
            uint i = 0;// x_orig = 65, y_orig = 12;
            float theta;
            float r = (float)radius;
            uint[] x = new uint[400];
            uint[] y = new uint[400];

            for (i = 0, theta = 0; theta < 360; ++theta)
            {
                x[i] = (uint)Math.Round((r * Math.Cos(theta * 3.14 / 180.0)));
                y[i] = (uint)Math.Round((r * Math.Sin(theta * 3.14 / 180.0)));
                putchar((uint)x[i] + x_orig, (uint)y[i] + y_orig, '+');
                i++;
            }

            puttext(x_orig, y_orig, "Circle", ConsoleColor.White, ConsoleColor.Red);

            draw();
        }

        public void draw_box(uint x, uint y, uint width, uint height)
        {
            uint i;
            for (i = 0; i < height; ++i)
            {
                putchar(x, y + (uint)i, '|', ConsoleColor.White, ConsoleColor.Blue);
                putchar(x + width - 1, y + (uint)i, '|', ConsoleColor.White, ConsoleColor.Blue);
            }

            for (i = 0; i < width; ++i)
            {
                putchar(x + (uint)i, y, '-', ConsoleColor.White, ConsoleColor.Blue);
                putchar(x + (uint)i, y + (uint)height - 1, '-', ConsoleColor.White, ConsoleColor.Blue);
            }

        }

        public void show_view1()
        {
            draw_box(0, 0, width, height);

            puttext(7, 6, "D1");
            puttext(8, 8, "D2");
            draw_box(5, 5, 15, 5);

            puttext(21, 13, "A3");
            puttext(24, 11, "A4");
            draw_box(20, 10, 15, 5);

            puttext(0, 0, "LEFT-BOT");
            puttext(0, height - 1, "LEFT-TOP");
            puttext(width - 10, 0, "RIGHT-BOT");
            puttext(width - 10, height - 1, "RIGHT-TOP");

            draw();
        }
        public void show_view2()
        {
            draw_box(0, 0, width, height);

            puttext(7, 6, "C1");
            puttext(8, 8, "C2");
            draw_box(5, 5, 15, 5);

            puttext(21, 13, "C3");
            puttext(24, 11, "C4");
            draw_box(20, 10, 15, 5);

            draw_curve(65, 12, 6);

            puttext(0, 0, "LEFT-BOT");
            puttext(0, height - 1, "LEFT-TOP");
            puttext(width - 10, 0, "RIGHT-BOT");
            puttext(width - 10, height - 1, "RIGHT-TOP");
            draw();
        }

        public void show_view3()
        {
            draw_box(0, 0, width, height);
            puttext(7, 6, "A1");
            puttext(8, 8, "A2");
            draw_box(5, 5, 15, 5);

            puttext(21, 13, "B3");
            puttext(24, 11, "B4");
            draw_box(20, 10, 15, 5);

            draw_curve(50, 12, 8);

            puttext(0, 0, "LEFT-BOT");
            puttext(0, height - 1, "LEFT-TOP");
            puttext(width - 10, 0, "RIGHT-BOT");
            puttext(width - 10, height - 1, "RIGHT-TOP");
            draw();
        }
        public void show_view4()
        {
            draw_box(0, 0, width, height);

            //display band-A
            puttext(7, 6, "R1");
            puttext(8, 8, "R2");
            draw_box(5, 5, 15, 5);

            puttext(21, 13, "R4");
            puttext(24, 11, "R5");
            draw_box(20, 10, 15, 5);

            draw_curve(10, 17, 4);

            puttext(0, 0, "LEFT-BOT");
            puttext(0, height - 1, "LEFT-TOP");
            puttext(width - 10, 0, "RIGHT-BOT");
            puttext(width - 10, height - 1, "RIGHT-TOP");
            draw();
        }
    }
}
