using System;
using LeanCloud.Engine;

namespace AppHosting.Minimum
{
    class Program
    {
        public static void Main(string[] args)
        {
            Cloud cloud = new Cloud().UseLog();
            cloud.Start(args);
        }
    }
}
