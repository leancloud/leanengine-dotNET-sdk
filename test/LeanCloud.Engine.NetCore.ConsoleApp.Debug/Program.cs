using System;

namespace LeanCloud.Engine.NetCore.ConsoleApp.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Cloud cloud = new Cloud();
            cloud.Start();
        }
    }
}
