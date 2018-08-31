using System;
namespace LeanCloud.Engine.Rpc
{
    public class StorageRpcContext : RpcContext
    {
        public StorageRpcContext()
        {

        }

        public override RpcConnection Connection { get; set; }

        public StorageRequest Request { get; set; }

        public StorageResponse Response { get; set; }
    }
}
