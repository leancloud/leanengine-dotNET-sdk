using System;
using System.Threading.Tasks;

namespace LeanCloud.Engine
{
    public class EngineUserVerifyHookContext
    {
        public AVUser TheUser { get; set; }

        public string Field { get; set; }
    }

    public interface IEngineUserVerifyHookHandler
    {
        Task ExecuteAsync(EngineUserVerifyHookContext context);
    }

    /// <summary>
    /// Engine object hook handler.
    /// </summary>
    public abstract class EngineUserVerifyHandler : IEngineUserVerifyHookHandler
    {
        public abstract Task ExecuteAsync(EngineUserVerifyHookContext context);
    }

    public class StandardUserVerifyHandler : EngineUserVerifyHandler
    {
        public StandardUserVerifyHandler(EngineUserHookDelegate funcDelegate)
        {
            FuncDel = funcDelegate;
        }

        public EngineUserHookDelegate FuncDel { get; set; }

        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="context">Context.</param>
        public override Task ExecuteAsync(EngineUserVerifyHookContext context)
        {
            return FuncDel.Invoke(context);
        }
    }
}
