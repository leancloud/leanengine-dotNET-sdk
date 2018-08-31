using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LeanCloud.Core.Internal;
using LeanCloud.Storage.Internal;

namespace LeanCloud.Engine.Rpc
{
    public interface IRpc
    {

    }

    public static class RpcExtensions
    {
        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            dynamic awaitable = @this.Invoke(obj, parameters);
            await awaitable;
            return awaitable.GetAwaiter().GetResult();
        }

        public static RpcServer UseRpc(this IRpc host, RpcServer server)
        {
            var tupple = host.ReflectRpcFunctions();
            tupple.ForEach(t =>
            {
                server.Register(t.Item1, t.Item2);
            });
            return server;
        }

        public static List<(string, RpcFunctionDelegate)> ReflectRpcFunctions(this IRpc rpcHost)
        {
            var hostType = rpcHost.GetType();
            var methods = hostType.GetMethods();

            var tupple = new List<(string, RpcFunctionDelegate)>();

            foreach (var method in methods)
            {
                var hookAttributes = method.GetCustomAttributes(typeof(RpcAttribute), false);

                if (hookAttributes.Length == 1)
                {
                    var rpcAttribute = (RpcAttribute)hookAttributes[0];

                    RpcFunctionDelegate rpcFunction = async (context) =>
                    {
                        var pas = BindParamters(method, context);

                        object result = null;

                        object host = null;
                        if (!method.IsStatic)
                        {
                            host = rpcHost;
                        }

                        if (method.ReturnType == typeof(Task))
                        {
                            Task awaitable = (Task)method.Invoke(host, pas);
                            await awaitable;
                        }
                        else if (method.ReturnType == typeof(void))
                        {
                            method.Invoke(host, pas);
                        }
                        else if (method.ReturnType.IsGenericType)
                        {
                            if (method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                                result = await method.InvokeAsync(host, pas);
                            else
                            {
                                result = method.Invoke(host, pas);
                            }
                        }
                        else
                        {
                            result = method.Invoke(host, pas);
                        }

                        var encodedObject = PointerOrLocalIdEncoder.Instance.Encode(result);

                        var resultWrapper = new Dictionary<string, object>()
                        {
                            { "results" , encodedObject }
                        };
                        if (context is StorageRpcContext storageRpcContext)
                        {
                            storageRpcContext.Response.MetaText = Json.Encode(resultWrapper);
                        }

                    };

                    var hostName = rpcHost.ReflectRpcHostName();
                    var methodName = rpcAttribute.Name ?? method.Name;
                    var rpcMethodName = $"{hostName}_{methodName}";
                    if (!method.IsStatic)
                    {
                        var hostId = rpcHost.ReflectRpcHostIdPropertyValue();
                        rpcMethodName = $"{hostName}_{hostId}_{methodName}";
                    }
                    var mixedResult = (rpcMethodName, rpcFunction);

                    tupple.Add(mixedResult);
                }
            }
            return tupple;
        }

        public static object[] BindParamters(MethodInfo memberInfo, RpcContext context)
        {
            List<object> result = new List<object>();
            ParameterInfo[] rpcParameters = memberInfo.GetParameters();
            if (context is StorageRpcContext storageRpcContext)
            {
                var pObjs = storageRpcContext.Request.Body["args"] as List<object>;
                for (int i = 0; i < rpcParameters.Length - 1; i++)
                {
                    var pInfo = rpcParameters[i];
                    var pObj = AVDecoder.Instance.Decode(pObjs[i]);
                    var pValue = pObj;
                    result.Add(pValue);
                }
                result.Add(context);
            }
            return result.ToArray();
        }

        public static string ReflectRpcHostName(this IRpc rpcHost)
        {
            var hostType = rpcHost.GetType();
            var hostAttribute = hostType.GetCustomAttribute<RpcHostAttribute>();
            if (hostAttribute == null)
            {
                if (rpcHost is AVObject avobj)
                {
                    return avobj.ClassName;
                }
            }
            var hostName = string.IsNullOrEmpty(hostAttribute.Name) ? hostType.Name : hostAttribute.Name;
            return hostName;
        }

        public static string ReflectRpcHostIdPropertyValue(this IRpc rpcHost)
        {
            PropertyInfo[] props = rpcHost.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr is RpcHostIdAttribute idAttr)
                    {
                        return prop.GetValue(rpcHost).ToString();
                    }
                }
            }
            if (rpcHost is AVObject avobj)
            {
                return avobj.ObjectId;
            }
            return "";
        }
    }

    public delegate Task RpcFunctionDelegate(RpcContext context);

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class RpcHostAttribute : Attribute
    {
        public string Name { get; set; }
        public RpcHostAttribute(string name = null)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class RpcHostIdAttribute : Attribute
    {
        public RpcHostIdAttribute()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class RpcAttribute : Attribute
    {
        public string Name { get; set; }
        public RpcAttribute(string name = null)
        {
            Name = name;
        }
    }
}
