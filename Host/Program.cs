using System;
using Contracts;
using Contracts.HostSide;
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

                var producer = ProxyProducer.Create<IHostViewContract>((info, args) => consumer.Consume(info, args));

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
            
            Console.ReadLine();
        }

        #endregion
    }
}