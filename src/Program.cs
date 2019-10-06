using System;

namespace QvCSGOConnector
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length >= 2) {
                new QvCSGOServer().Run(args[0], args[1]);
            }
        }
    }
}
