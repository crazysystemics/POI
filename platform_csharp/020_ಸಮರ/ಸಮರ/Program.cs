using system_engineering;

world w = new world();
Console.WriteLine("Hello, World!" + w.x + w.y);

namespace system_engineering
{
    class world
    {
        public int x = 2;
        public int y = 3;
    }
    

    class SystemEngineer
    {
        Domain          d;
        Technology[]    technologies;
        SOS             sos;
        System[]        systems;
        Operation[]     operations;
        Evaluation evaluation;
        
        public SystemSpecification  CreateSystemDefinition(object state_of_art,
                                                  object user_customer_requirements,
                                                  object user_customer_pain_points,
                                                  object market_analysis,
                                                  object competitive_analysis)
        { return null; }

        public void LifeOfSystem(TutorialLifeCycle slc, 
                                 SystemSpecification spec)
        {
            //slc.Realize(spec)
            //slc.Operate() will be called during operational phase
        }
    }

    class Domain
    { }

    class SystemSpecification
    {
        //In case System needs to be created, constructed or engineered
    }

    class TutorialLifeCycle
    {
        object Realization;//Realize();
        object Deployment;
        object Operation;
        object PreventiveMaintenance;
        object CorrectiveMaintenance;
        object AdaptiveMaintenance;
        object Retirement;

    }

    class SystemDefinition
    {
        //In case system is a entity/concept/phenomenon
        //that is discovered, perceived or understood
    }
      
    class Technology
    { }

    class SOS
    { }

    class System
    { }

    class SubSystem
    { }

    class Operation
    { }

    class Evaluation
    {
        double benchmark;
    }



}


