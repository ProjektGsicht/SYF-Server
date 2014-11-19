using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SYF_Server
{
    class Logger
    {
        private string Filename;

        public Logger(string Filename)
        {
            this.Filename = Filename;

            if (!File.Exists(Filename))
            {
                File.Create(Filename);
            }
        }

        public void Log(string Text, ConsoleColor Color = ConsoleColor.White)
        {
            DateTime Date = DateTime.Now;
            string DateString = Date.ToShortDateString();
            string TimeString = Date.ToLongTimeString();

            ConsoleColor OldColor = Console.ForegroundColor;

            Console.ForegroundColor = Color;
            Console.WriteLine("[{0}] {1}", TimeString, Text);
            Console.ForegroundColor = OldColor;

            var Writer = File.AppendText(Filename);
            Writer.WriteLine("[{0} {1}] {2}", DateString, TimeString, Text);
            Writer.Flush();
            Writer.Close();
            Writer.Dispose();
        }
    }
}
