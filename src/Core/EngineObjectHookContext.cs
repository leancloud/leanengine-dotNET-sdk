using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeanCloud.Engine
{
    /// <summary>
    /// Engine hook type.
    /// </summary>
    public enum EngineHookType
    {
        /// <summary>
        /// The before save.
        /// </summary>
        BeforeSave = 1,
        /// <summary>
        /// The after save.
        /// </summary>
        AfterSave = 2,
        /// <summary>
        /// The before update.
        /// </summary>
        BeforeUpdate = 3,
        /// <summary>
        /// The after update.
        /// </summary>
        AfterUpdate = 4,

        /// <summary>
        /// The before delete.
        /// </summary>
        BeforeDelete = 5,

        /// <summary>
        /// The after delete.
        /// </summary>
        AfterDelete = 6,
    }

    /// <summary>
    /// Engine object hook context.
    /// </summary>
    public class EngineObjectHookContext
    {
        public EngineObjectHookContext()
        {

        }

        //public string ClassName { get; set; }

        //public EngineHookType HookType { get; set; }

        //public AVRequest Request { get; set; }

        //public AVResponse Response { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>The object.</value>
        public AVObject TheObject { get; set; }

        /// <summary>
        /// Gets or sets the by.
        /// </summary>
        /// <value>The by.</value>
        public AVUser By { get; set; }

        /// <summary>
        /// Gets or sets the meta body.
        /// </summary>
        /// <value>The meta body.</value>
        public IDictionary<string, object> MetaBody { get; set; }

        /// <summary>
        /// Gets or sets the updated keys.
        /// </summary>
        /// <value>The updated keys.</value>
        public IEnumerable<string> UpdatedKeys { get; set; }
    }

    /// <summary>
    /// Engine object hook handler.
    /// </summary>
    public abstract class EngineObjectHookHandler : IEngineObjectHookHandler
    {
        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="context">Context.</param>
        public abstract Task ExecuteAsync(EngineObjectHookContext context);
    }

    /// <summary>
    /// Standard engine object hook handler.
    /// </summary>
    public class StandardEngineObjectHookHandler : EngineObjectHookHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Engine.StandardEngineObjectHookHandler"/> class.
        /// </summary>
        /// <param name="hookDelegate">Hook delegate.</param>
        public StandardEngineObjectHookHandler(EngineHookDelegate hookDelegate)
        {
            EngineHookDel = hookDelegate;
        }

        /// <summary>
        /// Gets or sets the engine hook del.
        /// </summary>
        /// <value>The engine hook del.</value>
        public EngineHookDelegate EngineHookDel { get; set; }

        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="context">Context.</param>
        public override Task ExecuteAsync(EngineObjectHookContext context)
        {
            return EngineHookDel(context);
        }
    }

    /// <summary>
    /// Engine object hook handler.
    /// </summary>
    public interface IEngineObjectHookHandler
    {
        Task ExecuteAsync(EngineObjectHookContext context);
    }
}
