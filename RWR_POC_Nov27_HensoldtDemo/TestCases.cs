using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWR_POC_GUI
{
    public class TestCasesList
    {

        // Class containing fixed test cases for testing the Q-learning algorithm.

        public List<TestCaseSelector> TestCases = new List<TestCaseSelector>();

        public TestCasesList()
        {
            TestCases.Add(new TestCaseSelector(0, new List<bool> { true, false, }));

            TestCases.Add(new TestCaseSelector(1, new List<bool> { true, true, true,
                                                                   false, false, false, false,}));

            TestCases.Add(new TestCaseSelector(2, new List<bool> { true, false, false, false, false, false, false, false, false, false,
                                                                   true, false, false, false, false, false, false, false, false,
                                                                   true, false, false, false, false, false, false, false,
                                                                   true, false, false, false, false, false, false,
                                                                   true, false, false, false, false, false,
                                                                   true, false, false, false, false,
                                                                   true, false, false, false,
                                                                   true, false, false,
                                                                   true, false,}));

            TestCases.Add(new TestCaseSelector(3, new List<bool> { true, false,
                                                                   true, false, false,
                                                                   true, false, false, false,
                                                                   true, false, false, false, false,
                                                                   true, false, false, false, false, false,
                                                                   true, false, false, false, false, false, false,
                                                                   true, false, false, false, false, false, false, false,
                                                                   true, false, false, false, false, false, false, false, false,
                                                                   true, false, false, false, false, false, false, false, false, false}));

            TestCases.Add(new TestCaseSelector(4, new List<bool> { true, true, true, true,
                                                                   false, false, false, false, false,}));

            TestCases.Add(new TestCaseSelector(5, new List<bool> { true, false, true, false, true, false, true, false, true, false, true, false, true, false,
                                                                   true, false, true, false, false, false, false, false, false,
                                                                   true, false, true, false, false, false, false, false, false,
                                                                   true, false, true, false, false, false, false, false, false,}));

            TestCases.Add(new TestCaseSelector(6, new List<bool> { true, true, true, false, false, false, false, false, false, false, false,
                                                                   false, false, false, false, false, false, false, false,}));


            // Issue - in the case below, the track is never deleted for emitter with AgeOut = 3
            // and hence, no action is performed, which includes the ideal action of decrementing AgeOut

            // Case : Exact same scenario repeats in every episode
            // Randomness in the way we do exploration/exploitation
            // Explore QSA with least score

            TestCases.Add(new TestCaseSelector(7, new List<bool> { true, true, true, true, false, true }));

            TestCases.Add(new TestCaseSelector(8, new List<bool> { true, false, false, false, false, false, false, false, false, false, false }));

            TestCases.Add(new TestCaseSelector(9, new List<bool> { true, false, false, false, false, false, false, false, false, false,
                                                                   true, false, false, false, false, false, false, false, false, false,
                                                                   true, false, true, false, true, false, true, false, true, false, true, false, true, false,
                                                                   true, false, false, false, false, false, false, false, false, false,
                                                                   true, false, false, false, false, false, false, false, false, false}));

            TestCases.Add(new TestCaseSelector(10, new List<bool> { true, true, true, false, false, false, false, false, false, false,
                                                                    true, true, false, false, false, false, false, false, false, false,
                                                                    true, false, false, false, true, true, false, false, false, false, }));

            TestCases.Add(new TestCaseSelector(11, new List<bool> { true, true, true, false, false, true, true, true, true, false,
                                                                    true, false, false, false, false, false, true, true, false, false, false, false, false, false, true, false, false,
                                                                    true, true, true, false, false, true, true, true, true, false,}));

            TestCases.Add(new TestCaseSelector(12, new List<bool> { true, true, true, true, true, true, true, true, true, true,
                                                                    false, false, false, false, false, false, false, false, false, false,
                                                                    true, true, true, true, true, true, true, true, true, true,
                                                                    true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true,
                                                                    false, false, false, false, false, false, false, false,
                                                                    true, true, true, true, true, true, true, true, true, true,}));
        }
    }
}
