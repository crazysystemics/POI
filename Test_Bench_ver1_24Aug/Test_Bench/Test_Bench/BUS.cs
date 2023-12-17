using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Bench
{
    public class BUS
    {
    }

    public interface IBC
    {
        void Register(SimulatedModel model);
        void UnRegister(SimulatedModel model);
        void Start();
        void Stop();
    }

    public class Schedule
    {

    }
}
