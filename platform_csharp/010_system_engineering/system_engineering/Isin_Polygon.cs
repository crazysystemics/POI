using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system_engineering
{
    public class Tutorial_Isin_Polygon : Tutorial
    {
        public class RectanglePoint
        {
            public Rectangle r;
            public Point p;

            public RectanglePoint(Rectangle r, Point p, bool expected_answer)
            {
                this.r = r;
                this.p = p;
            }

        }

        public abstract class GeometricShape
        {

        }



        public class Point:GeometricShape
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        public class Rectangle:GeometricShape
        {
            public int top;
            public int left;
            public int bottom;
            public int right;
            public int height;
            public int width;

            public Rectangle(int top, int left, int bottom, int right)
            {
                this.top = top;
                this.left = left;
                this.bottom = bottom;
                this.right = right;
                this.height = (bottom - top + 1);
                this.width = (right - left + 1);
            }

            public bool isin(Rectangle r, Point p)
            {
                if (p.x >= r.left && p.x <= r.right &&
                    p.y >= r.top && p.y <= r.bottom)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public List<GeometricShape> syllabus = new List<GeometricShape>();
        public List<RectanglePoint>  assignment_questions
            = new List<RectanglePoint>();

        public class  Assignment
        {

        }

        public Tutorial_Isin_Polygon(List<GeometricShape> syllabus)
        {
            this.syllabus = syllabus;
        }

        public Tutorial_Isin_Polygon()
        {
            //Syllabus
            Rectangle r1 = new Rectangle(30, 30, 40, 40);
            Rectangle r2 = new Rectangle(50, 50, 60, 60);
            
            Point p1 = new Point(35, 35);
            Point p2 = new Point(55, 55);
            Point pout = new Point(45, 45);
            this.syllabus.Add(r1);
            this.syllabus.Add(r2);
            this.syllabus.Add(p1);  
            this.syllabus.Add(p2); 
            this.syllabus.Add(pout);

            assignment_questions.Add(new RectanglePoint(r1, p1));
            assignment_questions.Add(new RectanglePoint(r1, p2));
            assignment_questions.Add(new RectanglePoint(r2, p1));
            assignment_questions.Add(new RectanglePoint(r2, p2));
            assignment_questions.Add(new RectanglePoint(r1, pout));
            assignment_questions.Add(new RectanglePoint(r2, pout));


            { { r1, p1 }, { r1, p2 },
                                      { r2, p1 }, { r2, p1 },
                                      { r1, pout }, { r2, pout} }); ;

            this.engine.ಪಾಠಶಾಲಾ_Systems.Add(new Teacher(0,1));
            this.engine.ಪಾಠಶಾಲಾ_Systems.Add(new Student(1,10,10));

           
        }

        Teacher guru = new Teacher();
        Student shishya = new Student();
        int teacher_to_student_x;
        int teacher_to_student_y;



    }
}
