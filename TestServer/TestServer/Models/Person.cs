using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestServer.Models
{
    public class Person
    {
        public int Number { get; set; }
        public string Name { get; set; }

        public Person(int i)
        {
            Name = $"Person {i}";
        }

        public void GetInLine(Object stateInfo)
        {
            Console.WriteLine($"{Name} got in line at {DateTime.Now}. Thread ID: {Thread.CurrentThread.ManagedThreadId}");
            Program.GrabNumber(this);
            Program.WriteName(this);
            Console.WriteLine($"{Name} got in line at {DateTime.Now}. Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}