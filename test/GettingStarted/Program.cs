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
            GetHooks(cloud);
            cloud.Start(args);
        }

        static async void GetHooks(Cloud cloud) {
            var hooks = await cloud.QueryCloudFunctionsMetaDataAsync();
            foreach (var hook in hooks) {
                Console.WriteLine(hook);
            }
        }

    }
}
