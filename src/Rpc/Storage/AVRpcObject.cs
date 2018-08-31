using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LeanCloud.Storage.Internal;

namespace LeanCloud.Engine.Rpc
{
    public interface INotifyRpcSetProperty
    {
        event OnRpcSetPropertyEventHandler RpcSetProperty;
    }

    public class RpcSetPropertyEventArgs : EventArgs
    {
        public RpcSetPropertyEventArgs(string propertyName, object toSetValue)
        {
            PropertyName = propertyName;
            ToSetValue = toSetValue;
        }

        public string PropertyName { get; set; }
        public object ToSetValue { get; set; }
    }

    public delegate void OnRpcSetPropertyEventHandler(object sender, RpcSetPropertyEventArgs args);

    public class AVRpcObject : AVObject, INotifyRpcSetProperty
    {
        private SynchronizedEventHandler<RpcSetPropertyEventArgs> rpcSettingProperty =
            new SynchronizedEventHandler<RpcSetPropertyEventArgs>();

        public event OnRpcSetPropertyEventHandler RpcSetProperty
        {
            add
            {
                rpcSettingProperty.Add(value);
            }
            remove
            {
                rpcSettingProperty.Remove(value);
            }
        }

        protected virtual void OnRpcSetProperty(string propertyName, object toSetValue)
        {
            rpcSettingProperty.Invoke(this, new RpcSetPropertyEventArgs(propertyName, toSetValue));
        }

        [Rpc]
        public void RpcSetingProperty(string propertyName, object toSetValue, RpcContext context)
        {
            OnRpcSetProperty(propertyName, toSetValue);
        }
    }
}
