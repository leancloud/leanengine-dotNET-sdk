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
