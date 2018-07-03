using System;
using Contracts;
using PS.Addins;

namespace Host
{
    class Program
    {
        #region Static members

        static void Main()
        {
            try
            {
                var remoteInstance = new Instance();
                var consumer = ProxyConsumer.Create(remoteInstance);

                var producer = ProxyProducer.Create<ITestContract>((info, args) => consumer.Consume(info, args));

                EventHandler eventHandler = (sender, eventArgs) => { };

                Console.WriteLine("Attaching event handler...");
                producer.Event += eventHandler;
                Console.WriteLine("Done.");

                Console.WriteLine("Setting property...");
                producer.Property = 1;
                Console.WriteLine("Done.");

                Console.WriteLine("Setting indexer...");
                producer[0] = 1;
                Console.WriteLine("Done.");

                Console.WriteLine("Function result: " + producer.Function(2, "Hello"));
                Console.WriteLine("Property result: " + producer.Property);
                Console.WriteLine("Indexer result: " + producer[0]);

                Console.WriteLine("Detaching event handler...");
                producer.Event -= eventHandler;
                Console.WriteLine("Done.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException());
            }

            /*
            try
            {
                var addInHost = new AddInHost();
                //TODO: setup discovery
                //var addins = addInHost.FindAddIns();

                var addin = new AddIn(new Version(1, 0),
                                      @"d:\GitHub\PS.Addins\AddIn1\bin\Debug\AddIn1.dll",
                                      "AddIn1.Addin",
                                      new[]
                                      {
                                          typeof(ITestContract)
                                      });

                using (var adapter = new AddInPipelineSingleDomain())
                using (var addInInstance = addInHost.Create(addin, adapter))
                {
                    var facade = addInInstance.Contract<ITestContract>();
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
            */
            Console.ReadLine();
        }

        #endregion
    }
}