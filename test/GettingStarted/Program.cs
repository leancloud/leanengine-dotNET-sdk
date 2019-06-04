using System;
using System.Collections.Generic;
using LeanCloud;
using LeanCloud.Core.Internal;
using LeanCloud.Engine;
using LeanCloud.Storage.Internal;
using Microsoft.AspNetCore.Hosting;

namespace GettingStarted {
    class Program {
        static void Main(string[] args) {
            AVObject.RegisterSubclass<AVUser>();
            Cloud cloud = new Cloud();
            cloud.OnLogIn((AVUser user) => {
                Console.WriteLine($"{user.Username} loged in");
            });
            cloud.UseLog();
            cloud.Start(args);
        }
    }
}
