using System;
using LeanCloud;

namespace AspNetWebApi.Default.Models
{
    public class Todo
    {
        public Todo()
        {
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public AVUser Owner { get; set; }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static Todo FromJson(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Todo>(json);
        }
    }
}
