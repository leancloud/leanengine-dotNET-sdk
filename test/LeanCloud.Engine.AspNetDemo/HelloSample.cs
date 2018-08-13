using System;
using LeanCloud.Engine;

namespace AspNetHosting.Default
{
    public class HelloSample
    {
        [EngineFunction("Hello")]
        public static string Hello([EngineFunctionParameter("text")]string text)
        {
            return $"Hello, {text}";
        }
    }
}
