using System;
using System.Threading.Tasks;

namespace LeanCloud.Engine.Rpc
{
    public abstract class RpcConnection
    {
        public RpcServer Server { get; set; }
        public RpcClient Client { get; set; }
        public abstract Task SendAsync(string message);
        public event EventHandler<string> OnMessage;
    }
}
