using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeanCloud.Engine.AspNetDemo
{
    public static class LambdaSample
    {
        public static void UseLambda(this Cloud cloud)
        {
            // case 1. use EngineObjectHookContext
            cloud.UseHook("Todo", EngineHookType.BeforeSave, (context) =>
            {
                var todo = context.TheObject;
                var by = context.By;
                return Task.FromResult(context.TheObject);
            });

            cloud.BeforeSave("Todo", (context) =>
            {
                return Task.FromResult(context.TheObject);
            });

            // case 2. use TheObject
            cloud.UseHook("Todo", EngineHookType.BeforeSave, (AVObject todoObj) =>
            {
                return Task.FromResult(todoObj);
            });

            cloud.BeforeSave("Todo", (AVObject todoObj) =>
            {
                return Task.FromResult(todoObj);
            });

            // case 3. use TheObject and user
            cloud.UseHook("Todo", EngineHookType.BeforeSave, (AVObject todoObj, AVUser by) =>
            {
                return Task.FromResult(todoObj);
            });

            cloud.BeforeSave("Todo", (AVObject todoObj, AVUser by) =>
            {
                return Task.FromResult(todoObj);
            });

            // case 3. use sub-class
            AVObject.RegisterSubclass<Todo>();

            cloud.UseHook<Todo>(EngineHookType.BeforeSave, (Todo theTodoObj) =>
            {
                // theTodoObj is an Todo instance.
                return Task.FromResult(theTodoObj);
            });

            cloud.UseHook<Todo>(EngineHookType.BeforeSave, (Todo theTodoObj, AVUser by) =>
            {
                return Task.FromResult(theTodoObj);
            });

            cloud.BeforeSave<Todo>((todo) =>
            {
                // todo is an Todo instance.
                return Task.FromResult(todo);
            });

            cloud.BeforeSave<Todo>((todo, by) =>
            {
                // todo is an Todo instance.
                return Task.FromResult(todo);
            });

            cloud.BeforeUpdate("Todo", review => 
            {
                var updatedKeys = review.GetUpdatedKeys();
                if (updatedKeys.Contains("comment"))
                {
                    var comment = review.Get<string>("comment");
                    if (comment.Length > 140) throw new EngineException(400, "comment 长度不得超过 140 字符");
                }
                return Task.FromResult(true);
            });
        }

        public class Todo : AVObject
        {
            public Todo()
            {
            }
        }
    }
}
