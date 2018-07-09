using System;
namespace LeanCloud.Engine
{
    public class EngineException : Exception
    {
        public EngineException()
        {

        }

        public EngineException(string message) : base(message)
        {

        }

        public int ErrorCode { get; set; }

        public int HttpStatusCode { get; set; }
    }
}
