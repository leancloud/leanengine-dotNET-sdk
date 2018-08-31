using System;
namespace LeanCloud.Engine.Rpc
{
    public abstract class RpcConnectionLayer : RpcServer
    {
        protected RpcServer BusinessServer { get; set; }

        public RpcConnectionLayer(RpcServer businessServer)
        {
            BusinessServer = businessServer;
        }
    }
}
