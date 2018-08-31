using System;
namespace LeanCloud.Engine.Rpc
{
    public abstract class RpcContext
    {
        public abstract RpcConnection Connection { get; set; }
        public string Message { get; set; }
    }
}
