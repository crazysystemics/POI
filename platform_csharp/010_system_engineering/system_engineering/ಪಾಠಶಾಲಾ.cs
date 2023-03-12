using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

//TODO:
//Visual Studio: Updatable Iterators


//Tutorial is SOS
//It has Teachers, Students and Concepts as its Member Systems
//GOAL: To Have a Concept Distribution In Students
//CURRENT_STATE: TEACHERS and STUDENTS have current distribution
//BY STUDY OF (MONTE-CARLO??) SIMULATION BUILD PERFORMANCE PARAMETERS FOR A TUTORIAL OF
//[TEACHERS, TEACHER-[CONCEPTS, TOPICS]], [STUDENTS], WORLD[CONCEPTS, TOPICS],  [SYLLABUS]
//FITNESS 1:TOPOLOGICAL SIMILARITY BETWEEN STUDENTS AND TEACHERS KB.(HISTOGRAM OF STUDENT X CONCEPT x {1,0}}
//FITNESS 2:AVERAGE SCORE SCORED BY STUDENTS ACROSS TOPICS
//GOAL is EXPECTED_FITNESS1 AND EXPECTED_FITNESS2
//GAP = GOAL - CURRENT_STATE
//GAP_FILL_PLAN = PLAN TO FILL GAP (MANUAL AND/OR AUTOMATIC)
//FOREACH (GFP IN GAP_FILL_PLAN_SET)
//{
//   MANUALLY DESIGN/DECIDE CFG
//   SOS-CONFIGURATION SOS-CFG =  CONFIGURATION(TEACHERS, STUDENTS, CONCEPTS, SYLLABUS)
//   FOR GIVEN CFG
//       FOR VARIOUS IO, FUNCTIONAL, PERFORMANCE AND NONFUNCTIONAL BEHAVIOUR 
//       OF SYSTEM-X (TEACHER-X)
//       SCORE = EVALUATE(TEACHER_X.TEACH(CFG_SYLLABUS, CFG_STUDENTS))
//       TUNE_DESIGN(TEACHER_X, SC0RE)
//   TEACHER_X CONFIGURATION IS DEFINED    
//   TEACHER-X CONFIGURATION IS DECIDED AFTER EVALUATING OVER MULTIPLE TUTORIALS AND MULTIPLE FLOWS
//       
//   TEACHER-X LIFECYCLE (Modules->Lessons->Dictates, Students, Assignments)
//     UNDERSTAND CFG -- Requirements
//     Study  -- Build/Update personal KB - Requirements
//     Modules --  Organize Teaching into a Tree of Modules - Study
//     Lesson  --  Break Transfer into a  sequence of Dictates (direct_copy or stimulus whose response
//                 will change the matrix) - Code or Detailed Design
//     Deliver --  Execute/Simulate each dictate to concerned students
//             --  Design Assignments and make students solve them
//                 Evaluate
//     Improve, Optimize => Understand, Study, Break Into Modules, Generate Dictates, Delivers, 
//     Give and Evaluate Assignments
//   
//     TUTORIAL CYCLE
//      One Operation consists of running various GAP_FILL_PLAN
//      EXPERIMENT WITH CFG. IDEA IS TO GET OPTIMAL PLAN FOR TRANSFERRING THE MATRIX
//      OR CONCEPT DISTRIBUTION
//
/*     SCALING AND DIVERSITY OF TUTORIAL
 *     ==================================
 *     Village School to University to Education Ministry to SDG-Education Goals
 *     Employment Skillset for Daily Life to Philosophical Enlightenment
 *     Pre-Nursery to Post-Doctoral
 *     Medium of Instruction
 *     Teaching Methodology: Face to Face Upanishad to Blackboard, PPT, VR/AR, OCW
 
 *
 *    System Life Cycle:
 *    ==================
 *    Birth, Singularity, Boundary Conditions, Criticality, Avalanche: 
 *           Emerging into foreground from background. phi of a mechanism is much higher than background
 *           If you consider background as network. A new identity, new layer is borne - 
 *           System Definition or Technical Spec.
 *    Guided Replication (development) according to a blue-print
 *           genome to phenome
 *           divese components (organs) around single identity
 *           Proof of Concept = Birth of an infant
 *           Lab-Prototype    = Teenager - Early Twenties
 *    Dynamic Equillibrium - Steady State
 *           Operation/Maintenance
 *           20 - Till Death
 *    Reproduction (Random-Replication)
 *           separate idetity is created
 *           collective entity is also born
 *           sexual reproduction
 *           building of population
 *           Factory Production
 *           Forming/Tiling in  Background and hence disappearing (dropping in phi)
 *           
 *           SCALE UP and SCALE OUT
 *              1:1 Coaching, Class, Institution, University, Education Ministry, UNESCO-SDG-AI
 *      
 * 
 *    
 *    
 */
namespace system_engineering
{
    public enum Mode { AUTO, MANUAL };
    public class ಪಾಠಶಾಲಾ
    {
        //Experiment No-1
        //Teacher and Student common frame of reference 
        //or rather, transformation logic between two frames
        //Let us restrict to Q1
        //two points a and b.
        //teacher_a_x, teacher_a_y; student_a_x, student_a_y
        //teacher_b_x, teacher_b_y; student_b_x, student_b_y
        //by solving for y=m*x + c, both get same m, but different = c1, c2
        //let origins be tox, toy and sox, soy
        //[tox, c1] [sox, c2]  

        public Tutorial point_is_atxy;


        /*cycle_number, concept(s), student(s),
        /*instruction(concept, example)
        /*assignments, recommendation
        /*Landscape = [students vs atomic_concepts]
        */
        public void Run()
        {
            Tutorial tutorial = new Tutorial();
            tutorial.engine.Run();
        }

        public ಪಾಠಶಾಲಾ()
        {
            point_is_atxy = new Tutorial();
            //TutorialDefinition point_is_atxy_definition = point_is_atxy.define(Mode.MANUAL);           
        }

    }

    public class TutorialDefinition : SystemDefinition
    {

    }

    public class Tutorial : SOS
    {
        class landscape_axes
        {
            concept x_concept;
            Student y_student = new Student();
        }
        public class WhiteBoard
        {
            public List<string> lines = new List<string>();

            public void erase()
            {
                lines = new List<string>();
            }

        }

        public class Engine
        {
            public WhiteBoard white_board = new WhiteBoard();
            public List<System> ಪಾಠಶಾಲಾ_Systems = new List<System>();
            public Engine()
            {
                ಪಾಠಶಾಲಾ_Systems.Add(new Teacher());
                ಪಾಠಶಾಲಾ_Systems.Add(new Student());
            }

            public void Fuse(System s)
            {

            }

            public void Run()
            {
                int tick = 0;
                Teacher teacher = new Teacher();
                Student student = new Student();
                


                while (true)
                {
                    //first, simple input-output
                    //whiteboard fusion is attempted later
                    //align frame-of-references
                    foreach (System pss in ಪಾಠಶಾಲಾ_Systems)
                    {
                        if ((Teacher)pss is Teacher)
                            teacher = (Teacher)pss;
                        else if (pss is Student)
                            student = (Student)pss;
                    }

                    teacher.student = student;
                    teacher.student.ox = student.px - teacher.px;
                    teacher.student.oy = student.py - teacher.py;

                    //Lesson taught: isin(Rectangle rectangle, Point point)
                    //Prerequisite functions:
                    //Data Types: Rectangle, Point, isin
                    //What is taught: Coordinate Transformation
                    //Common Lessons are moved to abstract base class

                }

                //whenever a point has to be specified in
                //in teachers' coordinates tx, ty
                //tx -= teacher.student.ox
                //ty -= teacher.student.oy

                //foreach (System tutorial_system in ಪಾಠಶಾಲಾ_Systems)
                //{
                //    //diffusion phase
                //    Query query = new Query();

                //    //which parameters to set
                //    Query set_param_query = new Query();
                //    set_param_query.set(new atomic_single_variable_concept
                //                        ("value_of(tutorial_system)",
                //                         "set-parameters", "?", false));
                //    object set_parameters = set_param_query.execute();

                //    //TODO: A System object has to be created and its 
                //    //set-parameters has to be filled. This is symbolically
                //    //shown by casting to System.
                //    tutorial_system.set((System)set_parameters);
                //    tutorial_system.update();
                //}


            }

        }

        //Implementation of abstract class System
        public override System get()
        {
            return this;
        }
        public override void set(System sys)
        {
            Tutorial temp_tutorial = (Tutorial)sys;
            this.description = temp_tutorial.description;
            this.engine = temp_tutorial.engine;
            this.teacher = temp_tutorial.teacher;
            this.teachers = temp_tutorial.teachers;
            this.concept_scape = temp_tutorial.concept_scape;
            this.landscape = temp_tutorial.landscape;
            this.Definition = temp_tutorial.Definition;
            this.id = temp_tutorial.id;
            this.students = temp_tutorial.students;
            this.syllabus = temp_tutorial.syllabus;
            this.white_board = temp_tutorial.white_board;
        }
        public override void update()
        { }

        //abstract public System query(atomic_single_variable_concept asvc);

        //discovery, vision of 
        List<concept> Definition = new List<concept>();

        public WhiteBoard white_board = new WhiteBoard();
        public string description;
        public concept[,] concept_scape;
        public Engine engine = new Engine();

        Dictionary<landscape_axes, bool> landscape = new Dictionary<landscape_axes, bool>();
        public List<Teacher> teachers;  //What Teacher Knows, Commander of Battle

        public int[] syllabus; //What Student should Know, Mission Objectives of Battle
        public List<Student> students; //Actors of Battle, different types of students may be there
        public Assignment assignment;
        public concept[,,] LessonPlan;
        public Teacher teacher = new Teacher(1);
        // public TutorialDefinition define(Mode m) { return null; }



        //teaching that object at bore-sight is object-to-be-tracked
        //Zone of Visibility is 90-degree. Main Azimuth turn-left, turn-right
        //Teacher has taught when "Student's Object to be Tracked" is aligned
        //with "Teacher's Object to be Tracked" (first POV will be same. then 
        //different POVs
        public Tutorial(int pid, Teacher pteacher, int[] psyllabus, Assignment passignment)
        {
            id = pid;
            teacher = pteacher;
            syllabus = psyllabus;
            assignment = passignment;
        }

        public Tutorial()
        {
            id = 0;
            description = "locating a point in stellar of points";


            teacher = new Teacher(16);

            for (int i = 0; i < teacher.kb.Length; i++)
            {
                if (i % 2 == 0)
                {
                    teacher.kb[i] = 1;
                }
                else
                {
                    teacher.kb[i] = 0;
                }
            }

            //syllabus is sub-set of teacher's knowledge base
            syllabus = new int[teacher.kb.Length];
            for (int i = 0; i < syllabus.Length; i++)
            {
                syllabus[i] = teacher.kb[i];
            }

            students = new List<Student>();
            assignment = new Assignment();
            concept_scape = new concept[syllabus.Length, students.Count];
        }

        public concept[] find_concept_gap(concept[,] student_concepts, concept[] desired_concepts)
        {
            return new concept[1];
        }

        //operation
        public void tutor()
        {
            teachers[0].teach(syllabus, students);
            for (int i = 0; i < students.Count; i++)
            {
                students[i].score = assignment.evaluate(students[i]);
            }
            //assert(score distribution is as expected)
        }
    }


    public class Principal : System
    {

        public override System get()
        { return null; }
        public override void set(System sys)
        { }
        public override void update()
        { }

        public void lifecycle_requirements_understand(Tutorial tutorial)
        {
            //Assumption:Tutorial definition has already been made
            //Orient, Train each Teacher for the class
            Teacher temp_teacher = new Teacher(16);
            List<Teacher> temp_teacher_list = new List<Teacher>();
            foreach (Teacher teacher in tutorial.teachers)
            {
                List<concept> temp_teacher_concepts = teacher.define(Mode.MANUAL);
                temp_teacher = temp_teacher.train();
                temp_teacher_list.Add(temp_teacher);
            }
            tutorial.teachers = temp_teacher_list;

            //similarly for other subsystems i.e., students
            //gap-students' perspective           
        }

        //public List<Tutorial> lifecycle_architect_design_modules(Tutorial tutorial)
        //{
        //    //module tree, sub-tutorials
        //}



    }

    public class Teacher : System
    {
        public override System get()
        {
            outbox.Add("100,100");
            return this;
        }
        public override void set(System sys)
        { }
        public override void update()
        {
            //from white_board, get students coordinates
            //update students origin.
        }

        //attributes
        public Student student = new Student();
        public int num_kb_points;
        public int[] kb;

        public int px, py;
        public int? ox, oy;

        //methods
        public Teacher(int pid = 0, int pnum_kb_points = 1, int forx = 0, int fory = 0)
        {
            id = pid;
            num_kb_points = pnum_kb_points;
            kb = new int[num_kb_points];

            ox = null;
            oy = null;

            px = 100;
            py = 100;
        }
        public void dictate(int concept, List<Student> students, int mode = 0)
        {
            //mode = unicast, multicast or groupcast, broadcast
            foreach (Student student in students)
            {
                student.listen(concept);
            }
        }
        public void teach(int[] Syllabus, List<Student> students)
        {
            foreach (int syl in Syllabus)
            {
                dictate(syl, students);
            }
        }
        public Teacher train()
        { return null; }

    }

    public class Student : System
    {

        Random random = new Random();
        public int[] kb;
        public double score;

        public int ox, oy;
        //canvas
        public int px, py;

        public override System get()
        {
            outbox.Add("10,10");
            return this;
        }
        public override void set(System sys)
        { }
        public override void update()
        { }

        public Student(int pid = 1, int frx = 0, int fry = 0)
        {
            id = pid;
            ox = frx;
            oy = fry;
            px = 10;
            py = 10;
        }
        public void listen(int message)
        {
            foreach (int i in kb)
            {
                if (random.NextDouble() < 0.5)
                {
                    kb[message] = 1;
                }
            }

        }
    }
    public class Assignment
    {
        public int id;
        public string problem, solution;

        public double evaluate(Student student)
        {
            int num_right_answers = 0;
            int num_wrong_answers = 0;
            for (int i = 0; i < student.kb.Length; i++)
            {
                if (i % 2 == 0)
                {
                    if (student.kb[i] == 1)
                        num_right_answers++;
                    else
                        num_wrong_answers++;
                }
                else //i % 2 == 1
                {
                    if (student.kb[i] == 0)
                        num_right_answers++;
                    else
                        num_wrong_answers++;
                }
            }

            double score =
                        (num_right_answers - num_wrong_answers) / student.kb.Length;

            return score;
        }
    }






    //class LectureNotesSpecification : SystemSpecification
    //{
    //    //In case System needs to be created, constructed or engineered
    //}

    //class TutorialLifeCycle : system_engineering.TutorialLifeCycle
    //{
    //    //Architecture: Outline of lecture notes
    //    //Detailed Subsections, and interface of each subsection
    //    //Realization Writing Diagram and Bulletted Texts
    //    //Integration: Stitching together

    //    //Production: Trying on multiple students
    //    //Adaptive Maintenance: Integrating with new arrivals


    //}


    //class KnowledgeDiscovery : Technology
    //{ }

    //class WebOfTopics : SOS
    //{ }




}
