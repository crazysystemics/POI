using static System.Console;
using System;


int[] map = { 12, 21, 4 };


int teacher_x  = 1;
int student1_x = 0;
int student2_x = 2;
int[] student_x_list = new int[2];

student_x_list[0]  = student1_x;
student_x_list[1]  = student2_x;

int id = 0;
foreach (int student_x in student_x_list)
{
    Console.WriteLine("Student " + id++ + " at position " + student_x);
    int answer = map[student_x];
    int pos = Array.FindIndex(map,x => x==answer);

    
    Console.WriteLine("answer={0},pos={1}", answer, pos);
    string direction;
    direction = (pos < teacher_x) ? "right" : "left"; 
    WriteLine("Teacher: come " + direction);
}