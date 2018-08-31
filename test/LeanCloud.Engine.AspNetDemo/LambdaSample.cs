using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeanCloud.Core.Internal;

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

            EngineHookDelegateSynchronous afterPostHook = (post) =>
            {

            };

            cloud.AfterSave("Post", (EngineObjectHookDeltegateSynchronous)(async post =>
            {
                // 直接修改并保存对象不会再次触发 after update hook 函数
                post["foo"] = "bar";
                await post.SaveAsync();
                // 如果有 FetchAsync 操作，则需要在新获得的对象上调用相关的 disable 方法
                // 来确保不会再次触发 Hook 函数
                await post.FetchAsync();
                post.DisableAfterHook();
                post["foo"] = "bar";

                // 如果是其他方式构建对象，则需要在新构建的对象上调用相关的 disable 方法
                // 来确保不会再次触发 Hook 函数
                post = AVObject.CreateWithoutData<AVObject>(post.ObjectId);
                post.DisableAfterHook();
                await post.SaveAsync();
            }));
        }

        public class Todo : AVObject
        {
            public Todo()
            {
            }
        }
    }
}
