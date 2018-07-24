using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanCloud.Engine
{
    /// <summary>
    /// AVObject extensions.
    /// </summary>
    public static class AVObjectExtensions
    {
        /// <summary>
        /// Gets the updated keys.
        /// </summary>
        /// <returns>The updated keys.</returns>
        /// <param name="obj">Object.</param>
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
