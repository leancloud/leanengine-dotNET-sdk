using System;
using LeanCloud;

namespace AspNetWebApi.Default.Models
{
    public class Todo
    {
        public Todo()
        {
        }

        public string Title { get; set; }

        public string Content { get; set; }

        public AVUser Owner { get; set; }
    }
}
