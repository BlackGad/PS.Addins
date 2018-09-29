using System;
using Contracts.HostSide;
using PS.Addins.Pipelines;

namespace Host
{
    class Program
    {
        #region Static members

        static void Main()
        {
            try
            {
                using (var pipeline = new DomainPipeline<Instance>())
                {
                    var facade = pipeline.Facade<IHostViewContract>();

                    void EventHandler(object sender, EventArgs eventArgs)
                    {
                        Console.WriteLine("Caught event, Domain: " + AppDomain.CurrentDomain.FriendlyName);
                    }

                    Console.WriteLine("Attaching event handler...");
                    facade.Event += EventHandler;
                    Console.WriteLine("Done.");

                    Console.WriteLine("Raising event...");
                    facade.RaiseEvent();
                    Console.WriteLine("Done.");

                    Console.WriteLine("Setting property...");
                    facade.Property = 1;
                    Console.WriteLine("Done.");

                    Console.WriteLine("Setting indexer...");
                    facade[0] = 1;
                    Console.WriteLine("Done.");

                    Console.WriteLine("Function result: " + facade.Function(2, "Hello"));
                    Console.WriteLine("Property result: " + facade.Property);
                    Console.WriteLine("Indexer result: " + facade[0]);

                    Console.WriteLine("Detaching event handler...");
                    facade.Event -= EventHandler;
                    Console.WriteLine("Done.");

                    Console.WriteLine("Raising event...");
                    facade.RaiseEvent();
                    Console.WriteLine("Done.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException());
            }

            Console.ReadLine();
        }

        #endregion
    }
}