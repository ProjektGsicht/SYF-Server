using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using SYF_Server.Misc;

namespace SYF_Server
{
    class Logger
    {
        private string Filename;
        private FileStream FStream;

        bool WriteInProgress = false;

        public Logger(string Filename)
        {
            this.Filename = Filename;

            if (!File.Exists(Filename))
            {
                FStream = File.Create(Filename);
            }
            else
            {
                FStream = File.OpenWrite(Filename);
            }
        }

        public void Log(string Text, ConsoleColor Color = ConsoleColor.White, bool Debug = false)
        {
            if (Text.Length == 0)
                return;

            if (Debug)
            {
                if (!Settings.GetInstance().DebugEnabled)
                    return;
            }

            while (WriteInProgress)
            {
                Thread.Sleep(1);
            }

            WriteInProgress = true;

            DateTime Date = DateTime.Now;
            string DateString = Date.ToShortDateString();
            string TimeString = Date.ToLongTimeString() + "." + Date.Millisecond.ToString().PadLeft(3, '0');

            Console.ResetColor();

            Console.Write("[{0}] ", TimeString);
            Console.ForegroundColor = Color;
            Console.WriteLine(Text);

            Console.ResetColor();

            var Writer = new StreamWriter(FStream);
            Writer.WriteLine("[{0} {1}] {2}", DateString, TimeString, Text);
            Writer.Flush();

            WriteInProgress = false;
        }
    }
}
