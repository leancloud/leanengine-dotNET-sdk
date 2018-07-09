using System;
namespace LeanCloud.Engine
{
    /// <summary>
    /// Engine hook attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class EngineObjectHookAttribute : Attribute
    {
        public EngineObjectHookAttribute(string className, EngineHookType hookType)
        {
            this.ClassName = className;
            this.HookType = hookType;
        }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        public string ClassName { get; private set; }

        /// <summary>
        /// Gets the type of the hook.
        /// </summary>
        /// <value>The type of the hook.</value>
        public EngineHookType HookType { get; private set; }
    }
}
