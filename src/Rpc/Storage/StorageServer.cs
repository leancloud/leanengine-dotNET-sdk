using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeanCloud.Core.Internal;
using LeanCloud.Storage.Internal;

namespace LeanCloud.Engine.Rpc
{
    public class StorageServer : RpcServer
    {
        public StorageServer()
        {

        }

        public override Task<RpcConnection> OnConnected(RpcConnection connection)
        {
            connection.Client = new StorageClient();
            return Task.FromResult(connection);
        }

        public override Task OnDisconnected(RpcConnection connection)
        {
            return Task.FromResult(false);
        }

        public override Task OnReceived(RpcConnection sender, string message)
        {
            try
            {
                var context = CreateContext(sender, message);
                var functionName = context.Request.Url.Split("/").Last();
                if (!RpcFuncs.ContainsKey(functionName)) throw new MissingMethodException($"MissingMethodException OnRpcRedicrect on {functionName}");
                return InvokeRpc(functionName, context);
            }
            catch (ArgumentException ax)
            {
                Console.WriteLine(ax.Message);
                return base.OnReceived(sender, message);
            }
        }

        public StorageRpcContext CreateContext(RpcConnection sender, string message)
        {
            var context = new StorageRpcContext();
            context.Connection = sender;
            context.Message = message;
            if (Json.Parse(message) is IDictionary<string, object> result)
            {
                if (result.ContainsKey("results") && result.ContainsKey("si"))
                {
                    var responseState = AVObjectCoder.Instance.Decode(result, AVDecoder.Instance);
                    context.Response = AVObject.FromState<StorageResponse>(responseState, result["className"] as string);
                }
                else
                {
                    var state = AVObjectCoder.Instance.Decode(result, AVDecoder.Instance);
                    context.Request = AVObject.FromState<StorageRequest>(state, result["className"] as string);
                }
            }
            return context;
        }
    }
}
