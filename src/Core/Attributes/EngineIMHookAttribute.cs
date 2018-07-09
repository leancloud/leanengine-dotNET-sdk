using System;
namespace LeanCloud.Engine
{
    /// <summary>
    /// IMH ook attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class EngineIMHookAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Engine.IMHookAttribute"/> class.
        /// </summary>
        /// <param name="hookType">Hook type.</param>
        public EngineIMHookAttribute(IMHookType hookType)
        {
            this.HookType = hookType;
        }

        /// <summary>
        /// Gets the type of the hook.
        /// </summary>
        /// <value>The type of the hook.</value>
        public IMHookType HookType { get; private set; }
    }

    /// <summary>
    /// IMHook type.
    /// </summary>
    public enum IMHookType
    {
        /// <summary>
        /// The message received.
        /// </summary>
        MessageReceived = 1,
        /// <summary>
        /// The receivers offline.
        /// </summary>
        ReceiversOffline = 2,
        /// <summary>
        /// The message sent.
        /// </summary>
        MessageSent = 3,
        /// <summary>
        /// The conversation start.
        /// </summary>
        ConversationStart = 4,
        /// <summary>
        /// The conversation started.
        /// </summary>
        ConversationStarted = 5,
        /// <summary>
        /// The conversation add.
        /// </summary>
        ConversationAdd = 6,
        /// <summary>
        /// The conversation remove.
        /// </summary>
        ConversationRemove = 7,

        /// <summary>
        /// The conversation update.
        /// </summary>
        ConversationUpdate = 8,
    }
}
