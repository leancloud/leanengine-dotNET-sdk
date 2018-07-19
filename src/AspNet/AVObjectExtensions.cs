using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanCloud.Engine
{
    public static class AVObjectExtensions
    {
        public static IEnumerable<string> GetUpdatedKeys(this AVObject obj)
        {
            var updatedKeys = new List<string>();
            if (obj.ContainsKey("_updatedKeys"))
            {
                if (obj["_updatedKeys"] is List<object> upKys)
                {
                    updatedKeys = upKys.Select(k => k.ToString()).ToList();
                }
            }
            return updatedKeys;
        }
    }
}
