using System;
using System.Linq;
using Contracts;
using PS.Addins;
using PS.Addins.Host;

namespace Host
{
    class Program
    {
        #region Static members

        static void Main(string[] args)
        {
            try
            {
                var addInHost = new AddInHost();
                //TODO: setup discovery
                var addins = addInHost.FindAddIns();

                using (var addin = addins.First().Create())
                {
                    var facade = addin.Contract<ITestContract>();
                    EventHandler eventHandler = (sender, eventArgs) => { };

                    Console.WriteLine("Attaching event handler...");
                    facade.Event += eventHandler;
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
                    facade.Event -= eventHandler;
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