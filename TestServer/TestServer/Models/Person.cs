using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestServer.Models
{
    public class Person
    {
        public TcpClient Client = new TcpClient();//Program.server.AcceptTcpClient();
        public int Number { get; set; }
        public string Name { get; set; }
        public static int custID = 0;

        public Person()
        {
            Number = ++custID;
            Name = $"Person {Number}";
        }

        public void GetInLine()
        {
            Client.ConnectAsync("localhost",8000);
            Console.WriteLine($"{Name} got in line at {DateTime.Now}. Thread ID: {Thread.CurrentThread.ManagedThreadId}");
            Program.GrabNumber(this);
            Program.WriteName(this);
            Console.WriteLine($"{Name} got in line at {DateTime.Now}. Thread ID: {Thread.CurrentThread.ManagedThreadId}");
            Program.TotalCustomer++;
            Client.Close();
        }
    }
}