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
                //var assemblyLocation = typeof(RemoteInstance).Assembly.Location;
                //var typeName = typeof(RemoteInstance).FullName;

                var service = new HostService();

                var assemblyLocation = @"d:\GitHub\PS.Addins\AddIn1\bin\Debug\AddIn1.dll";
                var typeName = "AddIn1.AddIn1";

                //using (var pipeline = new DirectPipeline())
                using (var pipeline = new DomainPipeline())
                {
                    var facade = pipeline.CreateObject<IHostViewContract>(assemblyLocation, typeName);

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

                    Console.WriteLine("Spawn object...");
                    var spawned = facade.SpawnObject();
                    Console.WriteLine("Done.");

                    Console.WriteLine("Execute spawned object method");
                    spawned.Test();
                    Console.WriteLine("Done.");

                    Console.WriteLine("Detaching event handler...");
                    facade.Event -= EventHandler;
                    Console.WriteLine("Done.");

                    Console.WriteLine("Raising event...");
                    facade.RaiseEvent();
                    Console.WriteLine("Done.");

                    Console.WriteLine("Starting service processing...");
                    facade.SetService(service);
                    facade.RaiseServiceCall();
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