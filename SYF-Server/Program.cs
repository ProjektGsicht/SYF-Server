using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYF_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server srv = new Server(12345);
            srv.Start();

            Console.Read();
        }
    }
}
