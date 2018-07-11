using System;
using System.Linq;
using System.Threading.Tasks;
using LeanCloud.Core.Internal;

namespace LeanCloud.Engine.Sample.ConsoleAppHosting.NetCore
{
    public class TodoHook
    {
        [EngineObjectHook("Todo", EngineHookType.BeforeSave)]
        public Task CheckTitle(AVObject todo)
        {
            var title = todo.Get<string>("title");
            // reset value for title
            if (title.Length > 20) todo["title"] = title.Substring(0, 20);
            // returning any value will be ok.
            return Task.FromResult(todo);
        }

        [EngineObjectHook("Todo", EngineHookType.AfterSave)]
        public Task SetDueDate(AVObject todo)
        {
            todo.Set("due", DateTime.Now);
            return todo.SaveAsync();
        }

        [EngineObjectHook("Todo", EngineHookType.BeforeUpdate)]
        public Task CheckUpdatedKeys(EngineObjectHookContext context)
        {
            if (context.UpdatedKeys.Contains("title"))
            {
                return CheckTitle(context.TheObject);
            }
            return Task.FromResult(true);
        }
    }
}
