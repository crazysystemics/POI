
enum Token { ID, SCENARIO, T_STEP_LIST, T_STEP, LPAREN, LB, PARALLEL_INPUT, 
    PI_LIST, PI, MIN, MAX, STEP, INT_CONST, COMMA, RPAREN, PARALLEL_OUTPUT, PO_LIST,
    PO, RB, NULL, SKIP }

enum NonTerminalToken { SCENARIO, T_STEP, PI_LIST, PI, MIN, MAX, STEP, PO_LIST, PO };

enum TerminalToken{ID, SCENARIO, TSTEP, LPAREN, LB, PARALLEL_INPUT, INT_CONST, COMMA, RPAREN, PARALLEL_OUTPUT, RB, NULL, SKIP }

enum DataType {IDENTIFIER, INTEGER_CONSTRAINT };

namespace CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            TestHarness tc01 = new TestHarness();
            TestHarness tc02 = new TestHarness();
            TestHarness tc03 = new TestHarness();
            TestHarness tc04 = new TestHarness();
            //tc01.DriverTest_01();
            //tc02.DriverTest_02();
            //tc03.DriverTest_03();
            tc04.DriverTest_04();
        }
    }
}