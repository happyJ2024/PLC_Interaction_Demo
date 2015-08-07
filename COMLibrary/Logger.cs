using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KepwareClientCOM
{
    class Logger
    {
        public static void Info(String msg)
        {
            Console.WriteLine(msg);
        }
        public static void Error(String msg)
        {
            Console.WriteLine(msg);
        }
    }
}
