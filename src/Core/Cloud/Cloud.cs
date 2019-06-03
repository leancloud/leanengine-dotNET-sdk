using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeanCloud.Core.Internal;

namespace LeanCloud.Engine
{
    /// <summary>
    /// Engine hook delegate.
    /// </summary>
    public delegate Task EngineHookDelegate(EngineObjectHookContext context);

    /// <summary>
    /// Engine hook delegate synchronous.
    /// </summary>
    public delegate void EngineHookDelegateSynchronous(EngineObjectHookContext context);

    /// <summary>
    /// Engine object hook delegate.
    /// </summary>
    public delegate Task EngineObjectHookDelegate(AVObject theObject);

    /// <summary>
    /// Engine object hook deltegate synchronous.
    /// </summary>
    public delegate void EngineObjectHookDeltegateSynchronous(AVObject theObject);

    /// <summary>
    /// Engine object with user hook delegate.
    /// </summary>
    public delegate Task EngineObjectWithUserHookDelegate(AVObject theObject, AVUser by);

    /// <summary>
    /// Engine object with user hook delegate synchronous.
    /// </summary>
    public delegate void EngineObjectWithUserHookDelegateSynchronous(AVObject theObject, AVUser by);

    /// <summary>
    /// Engine object hook delegate.
    /// </summary>
    public delegate Task EngineObjectHookDelegate<TAVObject>(TAVObject theObject);

    /// <summary>
    /// Engine object hook delegate synchronous.
    /// </summary>
    public delegate void EngineObjectHookDelegateSynchronous<TAVObject>(TAVObject theObject);

    /// <summary>
    /// Engine object with user hook delegate.
    /// </summary>
    public delegate Task EngineObjectWithUserHookDelegate<TAVObject>(TAVObject theObject, AVUser by);

    /// <summary>
    /// Engine object with user hook delegate synchronous.
    /// </summary>
    public delegate void EngineObjectWithUserHookDelegateSynchronous<TAVObject>(TAVObject theObject, AVUser by);

    /// <summary>
    /// Engine function delegate.
    /// </summary>
    public delegate Task EngineFunctionDelegate(EngineFunctionContext context);

    /// <summary>
    /// Engine user hook delegate.
    /// </summary>
    public delegate Task EngineUserHookDelegate(EngineUserVerifyHookContext context);

    /// <summary>
    /// Engine user hook delegate synchronous.
    /// </summary>
    public delegate void EngineUserHookDelegateSynchronous(EngineUserVerifyHookContext context);

    /// <summary>
    /// Engine user action hook delegate.
    /// </summary>
    public delegate Task EngineUserActionHookDelegate(EngineUserActionHookContext context);

    /// <summary>
    /// Engine user action hook delegate synchronous.
    /// </summary>
    public delegate Task EngineUserActionHookDelegateSynchronous(EngineUserActionHookContext context);

    /// <summary>
    /// Cloud.
    /// </summary>
    public class Cloud
    {
        #region static properties and methods
        /// <summary>
        /// Gets or sets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static Cloud Singleton { get; set; }
        #endregion
        /// <summary>
        /// Gets the app identifier.
        /// </summary>
        /// <value>The app identifier.</value>
        public string AppId
        {
            get => GetLeanEnv(LeanEnvKey.LEANCLOUD_APP_ID);
        }

        /// <summary>
        /// Gets the app key.
        /// </summary>
        /// <value>The app key.</value>
        public string AppKey
        {
            get => GetLeanEnv(LeanEnvKey.LEANCLOUD_APP_KEY);
        }

        /// <summary>
        /// Gets the master key.
        /// </summary>
        /// <value>The master key.</value>
        public string MasterKey
        {
            get => GetLeanEnv(LeanEnvKey.LEANCLOUD_APP_MASTER_KEY);
        }

        /// <summary>
        /// Lean env key.
        /// </summary>
        public enum LeanEnvKey
        {
            LEANCLOUD_APP_ID,
            LEANCLOUD_APP_KEY,
            LEANCLOUD_APP_MASTER_KEY,
            LEANCLOUD_APP_HOOK_KEY,
            LEANCLOUD_API_SERVER,
            LEANCLOUD_APP_PROD,
            LEANCLOUD_APP_ENV,
            LEANCLOUD_REGION,
            LEANCLOUD_APP_INSTANCE,
            LEANCLOUD_APP_DOMAIN,
            LEANCLOUD_APP_PORT,
            LEANCLOUD_APP_REPO_PATH,
        }

        /// <summary>
        /// Gets the lean env.
        /// </summary>
        /// <returns>The lean env.</returns>
        /// <param name="key">Key.</param>
        public string GetLeanEnv(LeanEnvKey key)
        {
            return Environment.GetEnvironmentVariable(key.ToString());
        }

        private bool CompareLeanEnv(string target, LeanEnvKey key)
        {
            var leanEnv = GetLeanEnv(key);
            return !string.IsNullOrEmpty(leanEnv) && target.Equals(leanEnv);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:LeanCloud.Engine.Cloud"/> is production.
        /// </summary>
        /// <value><c>true</c> if is production; otherwise, <c>false</c>.</value>
        public bool IsProduction
        {
            get
            {
                return CompareLeanEnv("production", LeanEnvKey.LEANCLOUD_APP_ENV);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:LeanCloud.Engine.EngineAspNetMiddleware"/> is development.
        /// </summary>
        /// <value><c>true</c> if is development; otherwise, <c>false</c>.</value>
        public bool IsDevelopment
        {
            get
            {
                var isDev = CompareLeanEnv("development", LeanEnvKey.LEANCLOUD_APP_ENV);
                return !(IsStaging || IsProduction) || isDev;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:LeanCloud.Engine.EngineAspNetMiddleware"/> is staging.
        /// </summary>
        /// <value><c>true</c> if is staging; otherwise, <c>false</c>.</value>
        public bool IsStaging
        {
            get
            {
                return CompareLeanEnv("staging", LeanEnvKey.LEANCLOUD_APP_ENV);
            }
        }

        public string GetLeanCacheRedisConnectionString(string instanceName)
        {
            return Environment.GetEnvironmentVariable($"REDIS_URL_{instanceName}");
        }

        public void PrintEnvironmentVariables()
        {
            foreach (LeanEnvKey key in (LeanEnvKey[])Enum.GetValues(typeof(LeanEnvKey)))
            {
                Console.WriteLine($"{key}:{GetLeanEnv(key)}");
            }
            Console.WriteLine($"dev|stg|prod:{IsDevelopment}|{IsStaging}|{IsProduction}");
        }

        public virtual void Start()
        {
            if (Singleton != null) throw new NotSupportedException("An running instance has been found in the environment. Only one Cloud instance can be used at LeanEngine.");
            Singleton = this;

            PrintEnvironmentVariables();

            if (IsProduction || IsStaging)
            {
                AVClient.Configuration configuration = new AVClient.Configuration()
                {
                    ApplicationId = AppId,
                    ApplicationKey = AppKey,
                    MasterKey = MasterKey
                };
                AVClient.Initialize(configuration);
            }
        }
         
        internal static IDictionary<string, IEngineObjectHookHandler> Hooks { get; set; } = new Dictionary<string, IEngineObjectHookHandler>();

        internal static IDictionary<string, IEngineFunctionHandler> Funcs = new Dictionary<string, IEngineFunctionHandler>();

        internal static IDictionary<string, IEngineUserVerifyHookHandler> VerifyHooks { get; set; } = new Dictionary<string, IEngineUserVerifyHookHandler>();

        internal static IDictionary<string, IEngineUserActionHookHandler> UserActionHooks { get; set; } = new Dictionary<string, IEngineUserActionHookHandler>();

        internal static string[] classNameMapFunctionMetaData = new string[]
        {
            "placeholder",//default
            "__before_save_for_{0}",
            "__after_save_for_{0}",
            "__before_update_for_{0}",
            "__after_update_for_{0}",
            "__before_delete_for_{0}",
            "__after_delete_for_{0}",
        };

        internal static string[] hookNameMapType = new string[]
        {
            "placeholder",//default
            "beforeSave",
            "afterSave",
            "beforeUpdate",
            "afterUpdate",
            "beforeDelete",
            "afterDelete",
        };

        internal static string[] vefifyHookMetaData = new string[]
        {
            "__on_verified_{0}"
        };

        internal static string[] userActionHookMetaData = new string[]
        {
            "__on_{0}__User"
        };

        internal static string FunctionMetaName(string className, EngineHookType hookType)
        {
            return string.Format(classNameMapFunctionMetaData[(int)hookType], className);
        }

        internal static string FunctionMetaName(string funtionName)
        {
            return funtionName;
        }

        public static EngineHookType ClassHookMapType(string hookName)
        {
            var index = Array.IndexOf(hookNameMapType, hookName);
            if (Array.IndexOf(hookNameMapType, hookName) > -1)
            {
                return (EngineHookType)Enum.ToObject(typeof(EngineHookType), index);
            }
            throw new ArgumentNullException(hookName);
        }

        /// <summary>
        /// Queries the cloud functions meta data async.
        /// </summary>
        /// <returns>The cloud functions meta data async.</returns>
        public virtual Task<IEnumerable<string>> QueryCloudFunctionsMetaDataAsync()
        {
            var hookMetaNames = Hooks.Count != 0 ? Hooks.Keys as IEnumerable<string> : new string[] { } as IEnumerable<string>;

            var funcNames = Funcs.Count != 0 ? Funcs.Keys as IEnumerable<string> : new string[] { } as IEnumerable<string>;

            var verifyNames = VerifyHooks.Count != 0 ? VerifyHooks.Keys as IEnumerable<string> : new string[] { } as IEnumerable<string>;

            var actionNames = UserActionHooks.Count != 0 ? UserActionHooks.Keys as IEnumerable<string> : new string[] { } as IEnumerable<string>;

            return Task.FromResult(hookMetaNames.Concat(funcNames).Concat(verifyNames).Concat(actionNames));
        }

        /// <summary>
        /// Register the specified className, hookType and hookHandler.
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="className">Class name.</param>
        /// <param name="hookType">Hook type.</param>
        /// <param name="hookHandler">Hook handler.</param>
        public Cloud Register(string className, EngineHookType hookType, IEngineObjectHookHandler hookHandler)
        {
            var functionName = FunctionMetaName(className, hookType);
            Hooks[functionName] = hookHandler;
            return this;
        }

        /// <summary>
        /// Register the specified functionName and functionHandler.
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="functionName">Function name.</param>
        /// <param name="functionHandler">Function handler.</param>
        public Cloud Register(string functionName, IEngineFunctionHandler functionHandler)
        {
            Funcs[functionName] = functionHandler;
            return this;
        }

        /// <summary>
        /// Register the specified verifiedField and userVerifyHookHandler.
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="verifiedField">Verified field.</param>
        /// <param name="userVerifyHookHandler">User verify hook handler.</param>
        public Cloud Register(string verifiedField, IEngineUserVerifyHookHandler userVerifyHookHandler)
        {
            var hookName = string.Format(vefifyHookMetaData[0], verifiedField);
            VerifyHooks[hookName] = userVerifyHookHandler;
            return this;
        }

        /// <summary>
        /// Register the specified action and userActionHookHandler.
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="action">Action.</param>
        /// <param name="userActionHookHandler">User action hook handler.</param>
        public Cloud Register(string action, IEngineUserActionHookHandler userActionHookHandler)
        {
            var hookName = string.Format(userActionHookMetaData[0], action);
            UserActionHooks[hookName] = userActionHookHandler;
            return this;
        }

        /// <summary>
        /// Invoke the specified className, hookType and context.
        /// </summary>
        /// <returns>The invoke.</returns>
        /// <param name="className">Class name.</param>
        /// <param name="hookType">Hook type.</param>
        /// <param name="context">Context.</param>
        public Task Invoke(string className, EngineHookType hookType, EngineObjectHookContext context)
        {
            var functionName = FunctionMetaName(className, hookType);
            if (Hooks.ContainsKey(functionName))
            {
                var hook = Hooks[functionName];
                return hook.ExecuteAsync(context);
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// Invokes the class hook.
        /// </summary>
        /// <returns>The class hook.</returns>
        /// <param name="className">Class name.</param>
        /// <param name="hookName">Hook name.</param>
        /// <param name="context">Context.</param>
        public async Task<AVObject> InvokeClassHook(string className, string hookName, EngineObjectHookContext context)
        {
            var hookType = ClassHookMapType(hookName);
            await Invoke(className, hookType, context);
            if (hookType == EngineHookType.BeforeSave)
            {
                context.TheObject.DisableBeforeSave();
            }
            else if (hookType == EngineHookType.AfterSave)
            {
                context.TheObject.DisableAfterSave();
            }
            return context.TheObject;
        }

        /// <summary>
        /// Invoke the specified functionName, funcOrRpc and context.
        /// </summary>
        /// <returns>The invoke.</returns>
        /// <param name="functionName">Function name.</param>
        /// <param name="funcOrRpc">Func or rpc.</param>
        /// <param name="context">Context.</param>
        public Task Invoke(string functionName, string funcOrRpc, EngineFunctionContext context)
        {
            if (Funcs.ContainsKey(functionName))
            {
                var function = Funcs[functionName];
                return function.ExecuteAsync(context);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Invoke the specified verifiedField and context.
        /// </summary>
        /// <returns>The invoke.</returns>
        /// <param name="verifiedField">Verified field.</param>
        /// <param name="context">Context.</param>
        public Task Invoke(string verifiedField, EngineUserVerifyHookContext context)
        {
            var hookName = string.Format(vefifyHookMetaData[0], verifiedField);
            if (VerifyHooks.ContainsKey(hookName))
            {
                var function = VerifyHooks[hookName];
                return function.ExecuteAsync(context);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Invoke the specified action and context.
        /// </summary>
        /// <returns>The invoke.</returns>
        /// <param name="action">Action.</param>
        /// <param name="context">Context.</param>
        public Task Invoke(string action, EngineUserActionHookContext context)
        {
            var hookName = string.Format(userActionHookMetaData[0], action == "onLogin" ? "login" : action);
            if (UserActionHooks.ContainsKey(hookName))
            {
                var function = UserActionHooks[hookName];
                return function.ExecuteAsync(context);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Befores the save.
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="className">Class name.</param>
        /// <param name="handler">Handler.</param>
        public Cloud BeforeSave(string className, EngineHookDelegate handler)
        {
            return Register(className, EngineHookType.BeforeSave, new StandardEngineObjectHookHandler(handler));
        }

        /// <summary>
        /// Afters the save.
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="className">Class name.</param>
        /// <param name="handler">Handler.</param>
        public Cloud AfterSave(string className, EngineHookDelegate handler)
        {
            return Register(className, EngineHookType.AfterSave, new StandardEngineObjectHookHandler(handler));
        }

        /// <summary>
        /// Befores the update.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="className">Class name.</param>
        /// <param name="handler">Handler.</param>
        public Cloud BeforeUpdate(string className, EngineHookDelegate handler)
        {
            return Register(className, EngineHookType.BeforeUpdate, new StandardEngineObjectHookHandler(handler));
        }

        /// <summary>
        /// Afters the update.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="className">Class name.</param>
        /// <param name="handler">Handler.</param>
        public Cloud AfterUpdate(string className, EngineHookDelegate handler)
        {
            return Register(className, EngineHookType.AfterUpdate, new StandardEngineObjectHookHandler(handler));
        }

        /// <summary>
        /// Befores the delete.
        /// </summary>
        /// <returns>The delete.</returns>
        /// <param name="className">Class name.</param>
        /// <param name="handler">Handler.</param>
        public Cloud BeforeDelete(string className, EngineHookDelegate handler)
        {
            return Register(className, EngineHookType.BeforeDelete, new StandardEngineObjectHookHandler(handler));
        }

        /// <summary>
        /// Afters the delete.
        /// </summary>
        /// <returns>The delete.</returns>
        /// <param name="className">Class name.</param>
        /// <param name="handler">Handler.</param>
        public Cloud AfterDelete(string className, EngineHookDelegate handler)
        {
            return Register(className, EngineHookType.AfterDelete, new StandardEngineObjectHookHandler(handler));
        }

        /// <summary>
        /// Define the specified functionName and function.
        /// </summary>
        /// <returns>The define.</returns>
        /// <param name="functionName">Function name.</param>
        /// <param name="function">Function.</param>
        public Cloud Define(string functionName, EngineFunctionDelegate function)
        {
            return Register(functionName, new StandardEngineFunctionHandler(function));
        }

        /// <summary>
        /// On the verified.
        /// </summary>
        /// <returns>The verified.</returns>
        /// <param name="field">Field.</param>
        /// <param name="verifyHook">Verify hook.</param>
        public Cloud OnVerified(string field, EngineUserHookDelegate verifyHook)
        {
            return Register(field, new StandardUserVerifyHandler(verifyHook));
        }

        /// <summary>
        /// Ons the user action.
        /// </summary>
        /// <returns>The user action.</returns>
        /// <param name="action">Action.</param>
        /// <param name="actionHook">Action hook.</param>
        public Cloud OnUserAction(string action, EngineUserActionHookDelegate actionHook)
        {
            return Register(action, new StandardUserActionHandler(actionHook));
        }
    }
}
