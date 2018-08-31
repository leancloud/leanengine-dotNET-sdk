using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LeanCloud.Engine.Rpc
{
    public abstract class RpcServer
    {
        public virtual IDictionary<string, RpcFunctionDelegate> RpcFuncs { get; set; } = new Dictionary<string, RpcFunctionDelegate>();

        public virtual void Register(string functionName, RpcFunctionDelegate functionHandler)
        {
            RpcFuncs[functionName] = functionHandler;
            Console.WriteLine($"rpc.Register:{functionName}");
        }

        public virtual void Unregister(string functionName)
        {
            RpcFuncs.Remove(functionName);
            Console.WriteLine($"rpc.Unregister:{functionName}");
        }

        public virtual Task InvokeRpc(string functionName, RpcContext context)
        {
            if (RpcFuncs.ContainsKey(functionName))
            {
                var function = RpcFuncs[functionName];
                return function(context);
            }
            return Task.FromResult("");
        }

        public virtual string ServerUrl { get; set; }
        public virtual string ConnectingUrl { get; set; }
        public virtual IList<RpcConnection> Connections { get; set; }

        public virtual Task OnReceived(RpcConnection sender, string message)
        {
            return sender.SendAsync(message);
        }
        public abstract Task<RpcConnection> OnConnected(RpcConnection connection);
        public abstract Task OnDisconnected(RpcConnection connection);
    }
}
