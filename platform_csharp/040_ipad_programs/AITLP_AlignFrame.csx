
using System;
using System.Diagnostics;

class Teacher
{
    public Student s = new Student();
    
    public int sf_ox=0;
    public int sf_oy=0;
    
    int tf_anchor_x=0;
    int tf_anchor_y=0;

    int sf_anchor_x=0;
    int sf_anchor_y=0;
    
    public Teacher(int tapx = 0, int tapy = 0)
    {
        tf_anchor_x = tapx;
        tf_anchor_y = tapy;
    }
    
    public void align_frame(Student s)
    {
        s.ask_anchor_point_coords(ref sf_anchor_x, ref sf_anchor_y);
        sf_ox = tf_anchor_x - sf_anchor_x;
        sf_oy = tf_anchor_y - sf_anchor_y;
    } 
    
    public void specify(int px, int py, ref int sx, ref int sy)
    {
        sx = px - sf_ox;
        sy = py - sf_oy;
    }
}

class Student
{
    int x, y;
    
    
    int sf_anchor_x;
    int sf_anchor_y;
    
    public Student(int x=0, int y=0, int sax=0, int say=0)
    {
        this.x = x;
        this.y = y;
        sf_anchor_x = sax;
        sf_anchor_y = say;
        
    }
    
    public void ask_anchor_point_coords(ref int sfax, ref int sfay)
    {
        sfax = sf_anchor_x;
        sfay = sf_anchor_y;
    }
}


//Experiment-1: Aligning Frame
//set up
int teacher_x = 0;
int teacher_y = 0;

//consider a point p
//teacher sees it at
int tf_point_x = 40;
int tf_point_y = 40;

//student sees it at
int sf_point_x = 30;
int sf_point_y = 30;

//sf_point_(x,y) coordinates are communicated to teacher
//student's origin in teacher's frame of reference is calculated
int tf_student_ox = tf_point_x - sf_point_x;
int tf_student_oy = tf_point_y - sf_point_y;


//teacher expresses coordinates of pointw in terms of student
int new_sf_point_x = tf_point_x - tf_student_ox;
int new_sf_point_y = tf_point_y - tf_student_oy;
Debug.Assert(sf_point_x == new_sf_point_x && sf_point_y == new_sf_point_y);


//Experiment 2: Object Oriented way
Student s2 = new Student(25, 25, 30, 30);
Teacher teacher = new Teacher(50, 50);
teacher.align_frame(s2);
teacher.specify(35, 35, ref new_sf_point_x, ref new_sf_point_y);
var stud_ox = teacher.sf_ox;
var stud_oy = teacher.sf_oy; 












