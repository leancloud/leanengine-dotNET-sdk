using System;
using System.Collections.Generic;

namespace LeanCloud.Engine.Rpc
{
    [AVClassName("StorageResponse")]
    public class StorageResponse : AVObject
    {
        public StorageResponse()
        {

        }

        [AVFieldName("i")]
        public int CommandId
        {
            get { return GetProperty<int>("CommandId"); }
            set { SetProperty<int>(value, "CommandId"); }
        }

        [AVFieldName("si")]
        public string ServerCommandId
        {
            get { return GetProperty<string>("ServerCommandId"); }
            set { SetProperty<string>(value, "ServerCommandId"); }
        }

        public string MetaText { get; set; }

        [AVFieldName("results")]
        public IDictionary<string, object> Results
        {
            get { return GetProperty<IDictionary<string, object>>("Body"); }
            set { SetProperty<IDictionary<string, object>>(value, "Body"); }
        }
    }
}
