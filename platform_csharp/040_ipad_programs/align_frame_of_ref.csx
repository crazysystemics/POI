
using System;


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


//Console.WriteLine((soffset_x + sf_point_x) + " " + (soffset_y + sf_point_y));

