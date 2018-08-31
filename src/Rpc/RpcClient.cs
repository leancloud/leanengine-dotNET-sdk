using System;
using System.Threading.Tasks;

namespace LeanCloud.Engine.Rpc
{
    public abstract class RpcClient
    {
        public Func<string, Task> Courier { get; set; }

        public virtual Task SendAsync(string message)
        {
            return Courier(message);
        }
    }
}
