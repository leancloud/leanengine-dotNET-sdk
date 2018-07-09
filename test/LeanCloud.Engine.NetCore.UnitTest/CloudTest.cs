using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LeanCloud.Engine.NetCore.UnitTest
{
    public class CloudTest
    {
        [Fact]
        public void Init()
        {
            Cloud cloud = new Cloud().BeforeSave("Todo", todo =>
            {
                var title = todo.Get<string>("title");
                // reset value for title
                if (!string.IsNullOrEmpty(title))
                    if (title.Length > 20) todo["title"] = title.Substring(0, 20);
                // returning any value will be ok.
                return Task.FromResult(true);
            });

            cloud.Start();
        }

        [Fact]
        public void ThrowExeception()
        {
            
        }

        [Fact]
        public async Task CallFunction()
        {
            await AVCloud.CallFunctionAsync<double>("AverageStars", new Dictionary<string, object>()
            {
                { "movieName", "夏洛特烦恼" }
            });
        }
    }
}
