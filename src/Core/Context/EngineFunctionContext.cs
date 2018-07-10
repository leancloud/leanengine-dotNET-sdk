using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeanCloud.Engine
{
    public class EngineFunctionContext
    {
        public string FunctionName { get; set; }

        public string CallType { get; set; }

        public IDictionary<string, object> FunctionParameters { get; set; }

        public object Result { get; set; }
    }

    public interface IEngineFunctionHandler
    {
        Task ExecuteAsync(EngineFunctionContext context);
    }

    /// <summary>
    /// Engine object hook handler.
    /// </summary>
    public abstract class EngineFunctionHandler : IEngineFunctionHandler
    {
        public abstract Task ExecuteAsync(EngineFunctionContext context);
    }

    public class StandardEngineFunctionHandler : EngineFunctionHandler
    {

        public StandardEngineFunctionHandler(EngineFunctionDelegate funcDelegate)
        {
            FuncDel = funcDelegate;
        }

        public EngineFunctionDelegate FuncDel { get; set; }

        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="context">Context.</param>
        public override Task ExecuteAsync(EngineFunctionContext context)
        {
            return FuncDel.Invoke(context);
        }
    }
}