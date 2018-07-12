using System;
using System.Collections.Generic;
using AspNetWebApi.Default.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetWebApi.Default.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        // POST api/todo
        [HttpPost]
        public void Post([FromBody]Todo todo)
        {
            Console.WriteLine(todo.Title);
        }
    }
}
