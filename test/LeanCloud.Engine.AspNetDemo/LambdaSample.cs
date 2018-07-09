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
            cloud.UseClassHook("Todo", EngineHookType.BeforeSave, (context) =>
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
            cloud.UseClassHook("Todo", EngineHookType.BeforeSave, (AVObject todoObj) =>
            {
                return Task.FromResult(todoObj);
            });

            cloud.BeforeSave("Todo", (AVObject todoObj) =>
            {
                return Task.FromResult(todoObj);
            });

            // case 3. use TheObject and user
            cloud.UseClassHook("Todo", EngineHookType.BeforeSave, (AVObject todoObj, AVUser by) =>
            {
                return Task.FromResult(todoObj);
            });

            cloud.BeforeSave("Todo", (AVObject todoObj, AVUser by) =>
            {
                return Task.FromResult(todoObj);
            });

            // case 3. use sub-class
            AVObject.RegisterSubclass<Todo>();

            cloud.UseClassHook<Todo>(EngineHookType.BeforeSave, (Todo theTodoObj) =>
            {
                // theTodoObj is an Todo instance.
                return Task.FromResult(theTodoObj);
            });

            cloud.UseClassHook<Todo>(EngineHookType.BeforeSave, (Todo theTodoObj, AVUser by) =>
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
        }

        public class Todo : AVObject
        {
            public Todo()
            {
            }
        }
    }
}
