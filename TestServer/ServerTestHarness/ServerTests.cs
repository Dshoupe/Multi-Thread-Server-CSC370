using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestServer;
using TestServer.Models;

namespace ServerTestHarness
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void ServerTestWith100PeopleWith1SecondTimeLimit()
        {
            Program.waitTimeMin = 100;
            Program.waitTimeMax = 1000;
            Program.writeTimeMin = 100;
            Program.writeTimeMax = 1000;

            Program.TakeNumberSemaphore = new Semaphore(Program.TotalNumOfCustomer, Program.TotalNumOfCustomer);
            Program.listener = new HttpListener();
            Program.listener.Prefixes.Add(Program.url);
            Program.listener.Start();
            Console.WriteLine("Listening for connections on {0}", Program.url);

            Task listenTask = Program.HandleIncomingConnections();
            
            for (int i = 0; i < 100; i++)
            {
                Person person = new Person(i + 1);
                person.GetInLine(person);
            }

            listenTask.GetAwaiter().GetResult();

            Program.listener.Close();

            while (Program.TotalCustomer < 100) { }
            Assert.Equals(Program.TotalCustomer, 100);
        }
    }
}
