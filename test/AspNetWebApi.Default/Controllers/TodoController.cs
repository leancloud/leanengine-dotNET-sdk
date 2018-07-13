using System;
using System.Collections.Generic;
using AspNetWebApi.Default.Models;
using LeanCloud.Engine;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace AspNetWebApi.Default.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly Func<string, LeanCache> _serviceAccessor;

        public TodoController(Func<string, LeanCache> serviceAccessor)
        {
            _serviceAccessor = serviceAccessor;
            var todoDbName = "dev";
            todoDb = GetConnectionMultiplexer(todoDbName);
        }

        public IConnectionMultiplexer GetConnectionMultiplexer(string leancacheInstanceName)
        {
            return _serviceAccessor(leancacheInstanceName).GetConnection();
        }

        private IConnectionMultiplexer todoDb;
        public IConnectionMultiplexer TodoDb
        {
            get => todoDb;
        }

        // GET api/todo/5
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            IDatabase db = TodoDb.GetDatabase();
            var json = db.StringGet(id.ToString());
            if (string.IsNullOrEmpty(json)) return Json("{}");
            var todo = Todo.FromJson(json);
            return Json(todo);
        }

        // POST api/todo
        [HttpPost]
        public JsonResult Post([FromBody]Todo todo)
        {
            IDatabase db = TodoDb.GetDatabase();
            db.StringSet("1", todo.ToJson());

            return Json(new { ID = 1 });
        }

        // GET: api/todo
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        // PUT api/todo/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Todo todo)
        {

        }

        // DELETE api/todo/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
