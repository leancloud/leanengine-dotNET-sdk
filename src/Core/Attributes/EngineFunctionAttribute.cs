using System;
namespace LeanCloud.Engine
{
    /// <summary>
    /// Engine function attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class EngineFunctionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Engine.EngineFunctionAttribute"/> class.
        /// </summary>
        /// <param name="functianName">Functian name.</param>
        public EngineFunctionAttribute(string functianName)
        {
            this.FunctionName = functianName;
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <value>The name of the function.</value>
        public string FunctionName { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public sealed class EngineFunctionParameterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LeanCloud.Engine.EngineFunctionAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">Functian name.</param>
        public EngineFunctionParameterAttribute(string parameterName)
        {
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <value>The name of the function.</value>
        public string ParameterName { get; private set; }
    }
}
