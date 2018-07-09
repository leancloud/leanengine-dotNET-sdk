using System;
using System.Threading.Tasks;

namespace LeanCloud.Engine
{
    public class EngineUserActionHookContext
    {
        public AVUser TheUser { get; set; }

        public string Action { get; set; }
    }

    public interface IEngineUserActionHookHandler
    {
        Task ExecuteAsync(EngineUserActionHookContext context);
    }

    /// <summary>
    /// Engine object hook handler.
    /// </summary>
    public abstract class EngineUserActionHandler : IEngineUserActionHookHandler
    {
        public abstract Task ExecuteAsync(EngineUserActionHookContext context);
    }

    public class StandardUserActionHandler : EngineUserActionHandler
    {
        public StandardUserActionHandler(EngineUserActionHookDelegate funcDelegate)
        {
            FuncDel = funcDelegate;
        }

        public EngineUserActionHookDelegate FuncDel { get; set; }

        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="context">Context.</param>
        public override Task ExecuteAsync(EngineUserActionHookContext context)
        {
            return FuncDel.Invoke(context);
        }
    }
}
