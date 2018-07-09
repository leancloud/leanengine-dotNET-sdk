using System;
using System.Threading.Tasks;

namespace LeanCloud.Engine.AspNetDemo
{
    public static class SampleHookMiddleware
    {
        public static Cloud SetHooks(this Cloud cloud)
        {
            cloud.BeforeSave("Todo", SampleHooks.Singleton.CheckTitle);
            cloud.UseClassHook<TodoHook>();
            return cloud;
        }
    }

    // case 1. use Cloud.BeforeSave
    public class SampleHooks
    {
        public static SampleHooks Singleton = new SampleHooks();

        public Task CheckTitle(EngineObjectHookContext context)
        {
            var title = context.TheObject.Get<string>("title");
            // reset value for title
            if (title.Length > 20) context.TheObject["title"] = title.Substring(0, 20);
            // returning any value will be ok.
            return Task.FromResult(true);
        }
    }

    // case 2. use Cloud.UseClassHook
    public class TodoHook
    {
        [EngineObjectHook("Todo", EngineHookType.AfterSave)]
        public Task CheckTitle(EngineObjectHookContext context)
        {
            var title = context.TheObject.Get<string>("title");
            // reset value for title
            if (title.Length > 20) context.TheObject["title"] = title.Substring(0, 20);
            // returning any value will be ok.
            return Task.FromResult(true);
        }
    }
}
