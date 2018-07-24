using System;

namespace LeanCloud.Engine.NetCore.ConsoleApp.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Cloud cloud = new Cloud();
            cloud.OnVerifiedSMS((AVUser user) =>
            {
                Console.WriteLine("user verified by sms.");
                return user.SaveAsync();
            });
            cloud.OnLogIn((AVUser user) =>
            {
                Console.WriteLine("user logged in.");
            });
            cloud.Start();
        }
    }
}
