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

       public console_win(uint xoffs, uint yoffs, uint x_width, uint y_height, uint xid)
        {
            y_offs = xoffs;
            x_offs = yoffs;
            width = x_width; ;
            height = y_height;
            id = xid;
            y_orig = ((int)x_width + (int)x_offs) - 1;
            x_orig = (int)y_height + (int)y_offs - 1;
        }

    }

    class display
    {
        uint max_height = 48;
        uint max_width = 174;
        uint[] x_inc = new uint[50];
        uint[] y_inc = new uint[50];
        float theta = 0;
        char[,] framebuf = new char[256, 256];
        console_win view1 = new console_win(0, 0, 0, 0, 1); 
        console_win view2 = new console_win(0, 0, 0, 0, 2);
        console_win view3 = new console_win(0, 0, 0, 0, 3);
        console_win view4 = new console_win(0, 0, 0, 0, 4);

        public void init(uint xoffs, uint yoffs, uint x_width, uint y_height, uint view)
        {
            switch (view)
            {
                case 1:
                    view1.y_offs = yoffs;
                    view1.x_offs = xoffs;
                    view1.width = x_width; ;
                    view1.height = y_height;
                    view1.id = view;
                    view1.y_orig = (int)view1.x_offs;
                    view1.x_orig = (int)y_height + (int)view1.y_offs - 1;
                    break;
                case 2:
                    view2.y_offs = yoffs;
                    view2.x_offs = xoffs;
                    view2.width = x_width; ;
                    view2.height = y_height;
                    view2.id = view;
                    view2.y_orig = (int)view2.x_offs;
                    view2.x_orig = (int)y_height + (int)view2.y_offs - 1;
                    break;
                case 3:
                    view3.y_offs = yoffs;
                    view3.x_offs = xoffs;
                    view3.width = x_width; ;
                    view3.height = y_height;
                    view3.id = view;
                    view3.y_orig = (int)view3.x_offs;
                    view3.x_orig = (int)y_height + (int)view3.y_offs - 1;
                    break;
                case 4:
                    view4.y_offs = yoffs;
                    view4.x_offs = xoffs;
                    view4.width = x_width; ;
                    view4.height = y_height;
                    view4.id = view;
                    view4.y_orig = (int)view4.x_offs;
                    view4.x_orig = (int)y_height + (int)view4.y_offs - 1;
                    break;
            }
        }
        public display()
        {
            uint w = max_width / 2 ;
            uint h = max_height / 2 ;
            init_fb(0, 0, max_width, max_height, (char)0);

            init(1, 1, w, h, 1);
            init(w + 2, 1,  w,  h, 2);
            init(1,  h + 2,  w, h, 3);
            init(w + 2, h + 2, w, h, 4);
            view1_clear();
            view2_clear();
            view3_clear();
            view4_clear();
        }

        void init_fb(uint x, uint y, uint w, uint h, char ch)
        {
            uint i, j;

            for (i = x; i <  h; i++)
            {
                for (j = y; j < w; ++j)
                {
                    framebuf[i,  j] = ch;
                }
            }
        }

        public void clear()
        {
            uint i, j;

            Console.SetCursorPosition(0, 0);
            for (i = 0; i < max_height; i++)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                for (j = 0; j < max_width; ++j)
                    Console.Write(framebuf[i, j]);
                Console.WriteLine("");
            }
        }

        public void draw_view(uint view)
        {
            uint i, j;
            uint x_offs = 0, y_offs = 0, width, height, id;

            switch (view)
            {
                case 1:
                    x_offs = view1.x_offs;
                    y_offs = view1.y_offs;
                    width = view1.width;
                    height = view1.height;
                    id = view1.id;
                    break;
                case 2:
                    x_offs = view2.x_offs;
                    y_offs = view2.y_offs;
                    width = view2.width;
                    height = view2.height;
                    id = view2.id;
                    break;
                case 3:
                    x_offs = view3.x_offs;
                    y_offs = view3.y_offs;
                    width = view3.width;
                    height = view3.height;
                    id = view3.id;
                    break;
                case 4:
                    x_offs = view4.x_offs;
                    y_offs = view4.y_offs;
                    width = view4.width;
                    height = view4.height;
                    id = view4.id;
                    break;
                default:
                    return;

            }


            for (i = y_offs; i < height + y_offs; i++)
            {
                for (j = x_offs; j < width + x_offs; ++j)
                {
                    Console.SetCursorPosition((int)j, (int)i);
                    switch (id)
                    {
                        case 1:
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case 2:
                            Console.BackgroundColor = ConsoleColor.Cyan;
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case 3:
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case 4:
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        default:
                            break;
                    }
                   /* if ((uint)framebuf[i, j] >= (uint)(128)) {
                        //String str;
                        //byte[] bytes = new byte[10];
                        //str.ToString("%c",framebuf[i, j]);
                        var e = Encoding.GetEncoding("iso-8859-1");
                        var s = e.GetString(new byte[] { 128 });
                        //var s = e.Get framebuf[i, j]);
                        Console.Write(s);
                    } else */
                    Console.Write(framebuf[i, j]);
                }
                Console.WriteLine("");
            }
        }

        public void view1_clear()
        {
            init_fb(view1.x_offs, view1.y_offs, view1.width, view1.height, ' ');
            draw_view(1);
        }


        public void view2_clear()
        {
            init_fb(view2.x_offs, view2.y_offs, view2.width, view2.height, ' ');
            draw_view(2);
        }
        public void view3_clear()
        {//
            init_fb(view3.x_offs, view3.y_offs, view3.width, view3.height, ' ');
            draw_view(3);
        }
        public void view4_clear()
        {
            init_fb(view4.x_offs, view4.y_offs, view4.width, view4.height, ' ');
            draw_view(4);
        }

        public void putchar(uint x, uint y, char ch, uint view)
        {
            int xs = (int)x * -1; // + (int)x_offs;
            bool b1, b2;
            int x_pos, y_pos;

            switch (view)
            {
                case 1:
                    y_pos = (int)view1.x_orig - (int)y;
                    x_pos = (int)view1.y_orig + (int)x;
                    b1 = ((x_pos >= view1.x_offs) && (x_pos <= (view1.width + view1.x_offs)));
                    b2 = ((y_pos >= view1.y_offs) && (y_pos <= (view1.height + view1.y_offs)));
                    if (!(b1 || b2))
                        return;

                    break;
                case 2:
                    y_pos = (int)view2.x_orig - (int)y;
                    x_pos = (int)view2.y_orig + (int)x;
                    b1 = ((x_pos >= view2.x_offs) && (x_pos <= (view2.width + view2.x_offs)));
                    b2 = ((y_pos >= view2.y_offs) && (y_pos <= (view2.height + view2.y_offs)));
                    if (!(b1 || b2))
                        return;

                    break;
                case 3:
                    y_pos = (int)view3.x_orig - (int)y;
                    x_pos = (int)view3.y_orig + (int)x;

                    b1 = ((x_pos >= view3.x_offs) && (x_pos <= (view3.width + view3.x_offs)));
                    b2 = ((y_pos >= view3.y_offs) && (y_pos <= (view3.height + view3.y_offs)));
                    if (!(b1 || b2))
                        return;

                    break;
                case 4:
                    y_pos = (int)view4.x_orig - (int)y;
                    x_pos = (int)view4.y_orig + (int)x;

                    b1 = ((x_pos >= view4.x_offs) && (x_pos <= (view4.width + view4.x_offs)));
                    b2 = ((y_pos >= view4.y_offs) && (y_pos <= (view4.height + view4.y_offs)));
                    if (!(b1 || b2))
                        return;
                     break;
                default:
                    return;
            }
            if (x_pos < 0 || y_pos < 0)
                return;
            framebuf[y_pos, x_pos] = ch;
        }

        public void puttext(uint x, uint y, string str, uint view)
        {
            uint i;
            for (i = 0; i < str.Length; ++i)
            {
                putchar(x + i, y, (char)str.ElementAt((int)i), view);
            }
        }

        public void draw_curve(uint x_orig, uint y_orig, uint radius, uint view)
        {
            uint i = 0;// x_orig = 65, y_orig = 12;
            float r = (float)radius;
            uint[] x = new uint[400];
            uint[] y = new uint[400];

            puttext(x_orig, y_orig, "Circle", view);

            for (i = 0, theta = 0; theta < 360; ++theta)
            //theta++;
            //if (theta > 45)
              //  theta = 0;
            {
                x[i] = (uint)Math.Round((r * Math.Cos(theta * 3.14 / 180.0)));
                y[i] = (uint)Math.Round((r * Math.Sin(theta * 3.14 / 180.0)));
                putchar((uint)x[i] + x_orig, (uint)y[i] + y_orig, '*', view);
                i++;
            }

            draw_view(view);
        }

        public void draw_box(uint x, uint y, uint width, uint height, uint view)
        {
            uint i;
            for (i = 0; i < height; ++i)
            {
                putchar(x, y + (uint)i, '|', view);
                putchar(x + width - 1, y + (uint)i, '|', view);
            }

            for (i = 0; i < width; ++i)
            {
                putchar(x + (uint)i, y, '-', view);
                putchar(x + (uint)i, y + (uint)height - 1, '-', view);
            }

        }
        public void draw_line(uint view, char ch)
        {
            uint i = 0;

            //for (i = 0; i < 4; ++i)
             putchar((uint)(x_inc[view]+i), (uint)(y_inc[view]+i), (char)ch, view);

            if (x_inc[view] > 10)
                x_inc[view]--;
            else
                x_inc[view]++;

            if (y_inc[view] > 4)
                y_inc[view]--;
            else
                y_inc[view]++;

            ;// y_inc[view] = 0;
        }

        public void draw_view1()
        {
            draw_box(0, 0, view1.width, view1.height, 1);

            puttext(7, 6, "D1", 1);
            puttext(8, 8, "D2", 1);
            draw_box(5, 5, 15, 5, 1);

            puttext(21, 13, "A3", 1);
            puttext(24, 11, "A4", 1);
            draw_box(20, 10, 15, 5, 1);

            puttext(0, 0, "LEFT-BOT", 1);
            puttext(0, view1.height - 1, "LEFT-TOP", 1);
            puttext(view1.width-10, 0, "RIGHT-BOT", 1);
            puttext(view1.width-10, view1.height - 1, "RIGHT-TOP", 1);

            draw_view(1);
        }
        public void draw_view2()
        {
            draw_box(0, 0, view2.width, view2.height, 2);

            puttext(7, 6, "C1", 2);
            puttext(8, 8, "C2", 2);
            draw_box(5, 5, 15, 5, 2);

            puttext(21, 13, "C3", 2);
            puttext(24, 11, "C4", 2);
            draw_box(20, 10, 15, 5, 2);

            draw_curve(65, 12, 6, 2);

            puttext(0, 0, "LEFT-BOT", 2);
            puttext(0, view2.height - 1, "LEFT-TOP", 2);
            puttext(view2.width - 10, 0, "RIGHT-BOT", 2);
            puttext(view2.width - 10, view2.height - 1, "RIGHT-TOP", 2);
            draw_view(2);
        }

        public void draw_view3()
        {
            draw_box(0, 0, view3.width, view3.height, 3);
            puttext(7, 6, "A1", 3);
            puttext(8, 8, "A2", 3);
            draw_box(5, 5, 15, 5, 3);

            puttext(21, 13, "B3", 3);
            puttext(24, 11, "B4", 3);
            draw_box(20, 10, 15, 5, 3);

            draw_curve(50, 12, 8, 3);

            puttext(0, 0, "LEFT-BOT", 3);
            puttext(0, view3.height - 1, "LEFT-TOP", 3);
            puttext(view3.width - 10, 0, "RIGHT-BOT", 3);
            puttext(view3.width - 10, view3.height - 1, "RIGHT-TOP", 3);
            draw_view(3);
        }
        public void draw_view4()
        {
            draw_box(0, 0, view4.width, view4.height, 4);

            //display band-A
            puttext(7, 6, "R1", 4);
            puttext(8, 8, "R2", 4);
            draw_box(5, 5, 15, 5, 4);

            puttext(21, 13, "R4", 4);
            puttext(24, 11, "R5", 4);
            draw_box(20, 10, 15, 5, 4);

            draw_curve(10, 17, 4, 4);

            puttext(0, 0, "LEFT-BOT", 4);
            puttext(0, view4.height - 1, "LEFT-TOP", 4);
            puttext(view4.width - 10, 0, "RIGHT-BOT", 4);
            puttext(view4.width - 10, view4.height - 1, "RIGHT-TOP", 4);
            draw_view(4);
        }
    }
}
