﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestServer.Models;

namespace TestServer
{
    public static class Program
    {
        public static int waitTimeMin = 500;
        public static int waitTimeMax = 1000;
        public static int writeTimeMin = 500;
        public static int writeTimeMax = 1000;
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static int pageViews = 0;
        public static int TotalCustomer = 1;
        public static int TotalNumOfCustomer = 50;
        public static List<string> CustomerNames = new List<string>();
        public static Semaphore TakeNumberSemaphore;
        public static Random randy = new Random();
        public static int requestCount = 0;
        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";

        public static void Main(string[] args)
        {
            TakeNumberSemaphore = new Semaphore(TotalNumOfCustomer, TotalNumOfCustomer);
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }

        public static void GrabNumber(Person p)
        {
            TakeNumberSemaphore.WaitOne();
            p.Number = TotalCustomer++;
            Thread.Sleep(randy.Next(500, 1000));
            Console.WriteLine($"{p.Name} has taken number {p.Number}");
            TakeNumberSemaphore.Release();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteName(Person p)
        {
            CustomerNames.Add(p.Name);
            Thread.Sleep(randy.Next(500, 1000));
            Console.WriteLine($"{p.Name} has added their name to the list");
        }

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                if (req.Url.AbsolutePath != "/favicon.ico")
                {
                    pageViews += 1;
                }

                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }
    }
}
