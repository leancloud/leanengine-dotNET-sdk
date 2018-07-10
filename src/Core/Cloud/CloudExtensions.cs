using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using LeanCloud.Core.Internal;

namespace LeanCloud.Engine
{
    public static class ICloudExtensions
    {
        public static Cloud UseHook(this Cloud cloud, string className, EngineHookType hookType, EngineHookDelegate hookDelegate)
        {
            switch (hookType)
            {
                case EngineHookType.BeforeSave:
                    cloud.BeforeSave(className, hookDelegate);
                    break;
                case EngineHookType.AfterSave:
                    cloud.AfterSave(className, hookDelegate);
                    break;
                case EngineHookType.BeforeUpdate:
                    cloud.BeforeUpdate(className, hookDelegate);
                    break;
                case EngineHookType.AfterUpdate:
                    cloud.AfterUpdate(className, hookDelegate);
                    break;
                case EngineHookType.BeforeDelete:
                    cloud.BeforeDelete(className, hookDelegate);
                    break;
                case EngineHookType.AfterDelete:
                    cloud.AfterDelete(className, hookDelegate);
                    break;
                default:
                    break;
            }
            return cloud;
        }

        public static Cloud BeforeSave(this Cloud cloud, string className, EngineHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.BeforeSave, hookDelegate);
            return cloud;
        }

        public static Cloud AfterSave(this Cloud cloud, string className, EngineHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.AfterSave, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeUpdate(this Cloud cloud, string className, EngineHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.BeforeUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud AfterUpdate(this Cloud cloud, string className, EngineHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.AfterUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeDelete(this Cloud cloud, string className, EngineHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.BeforeDelete, hookDelegate);
            return cloud;
        }

        public static Cloud AfterDelete(this Cloud cloud, string className, EngineHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.AfterDelete, hookDelegate);
            return cloud;
        }

        #region seperate TheObject with context in delegate

        public static Cloud UseHook(this Cloud cloud, string className, EngineHookType hookType, EngineObjectHookDelegate hookDelegate)
        {
            cloud.UseHook(className, hookType, (EngineObjectHookContext context) =>
            {
                return hookDelegate(context.TheObject);
            });
            return cloud;
        }

        public static Cloud BeforeSave(this Cloud cloud, string className, EngineObjectHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.BeforeSave, hookDelegate);
            return cloud;
        }

        public static Cloud AfterSave(this Cloud cloud, string className, EngineObjectHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.AfterSave, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeUpdate(this Cloud cloud, string className, EngineObjectHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.BeforeUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud AfterUpdate(this Cloud cloud, string className, EngineObjectHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.AfterUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeDelete(this Cloud cloud, string className, EngineObjectHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.BeforeDelete, hookDelegate);
            return cloud;
        }

        public static Cloud AfterDelete(this Cloud cloud, string className, EngineObjectHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.AfterDelete, hookDelegate);
            return cloud;
        }

        #endregion

        #region passing current user
        public static Cloud UseHook(this Cloud cloud, string className, EngineHookType hookType, EngineObjectWithUserHookDelegate hookDelegate)
        {
            cloud.UseHook(className, hookType, (EngineObjectHookContext context) =>
            {
                return hookDelegate(context.TheObject, context.By);
            });
            return cloud;
        }

        public static Cloud BeforeSave(this Cloud cloud, string className, EngineObjectWithUserHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.BeforeSave, hookDelegate);
            return cloud;
        }

        public static Cloud AfterSave(this Cloud cloud, string className, EngineObjectWithUserHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.AfterSave, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeUpdate(this Cloud cloud, string className, EngineObjectWithUserHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.BeforeUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud AfterUpdate(this Cloud cloud, string className, EngineObjectWithUserHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.AfterUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeDelete(this Cloud cloud, string className, EngineObjectWithUserHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.BeforeDelete, hookDelegate);
            return cloud;
        }

        public static Cloud AfterDelete(this Cloud cloud, string className, EngineObjectWithUserHookDelegate hookDelegate)
        {
            cloud.UseHook(className, EngineHookType.AfterDelete, hookDelegate);
            return cloud;
        }
        #endregion

        #region generic sub-class hook
        public static Cloud UseHook<TAVObject>(this Cloud cloud, EngineHookType hookType, EngineObjectHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            var className = AVObject.GetSubClassName<TAVObject>();
            cloud.UseHook(className, hookType, (EngineObjectHookContext context) =>
             {
                 return hookDelegate(context.TheObject as TAVObject);
             });
            return cloud;
        }

        public static Cloud BeforeSave<TAVObject>(this Cloud cloud, EngineObjectHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.BeforeSave, hookDelegate);
            return cloud;
        }
        public static Cloud AfterSave<TAVObject>(this Cloud cloud, EngineObjectHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.AfterSave, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeUpdate<TAVObject>(this Cloud cloud, EngineObjectHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.BeforeUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud AfterUpdate<TAVObject>(this Cloud cloud, EngineObjectHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.AfterUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeDelete<TAVObject>(this Cloud cloud, EngineObjectHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.BeforeDelete, hookDelegate);
            return cloud;
        }

        public static Cloud AfterDelete<TAVObject>(this Cloud cloud, EngineObjectHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.AfterDelete, hookDelegate);
            return cloud;
        }
        #endregion

        #region generic sub-class hook with AVUser
        public static Cloud UseHook<TAVObject>(this Cloud cloud, EngineHookType hookType, EngineObjectWithUserHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            var className = AVObject.GetSubClassName<TAVObject>();
            cloud.UseHook(className, hookType, (EngineObjectHookContext context) =>
            {
                return hookDelegate(context.TheObject as TAVObject, context.By);
            });
            return cloud;
        }

        public static Cloud BeforeSave<TAVObject>(this Cloud cloud, EngineObjectWithUserHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.BeforeSave, hookDelegate);
            return cloud;
        }

        public static Cloud AfterSave<TAVObject>(this Cloud cloud, EngineObjectWithUserHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.AfterSave, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeUpdate<TAVObject>(this Cloud cloud, EngineObjectWithUserHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.BeforeUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud AfterUpdate<TAVObject>(this Cloud cloud, EngineObjectWithUserHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.AfterUpdate, hookDelegate);
            return cloud;
        }

        public static Cloud BeforeDelete<TAVObject>(this Cloud cloud, EngineObjectWithUserHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.BeforeDelete, hookDelegate);
            return cloud;
        }

        public static Cloud AfterDelete<TAVObject>(this Cloud cloud, EngineObjectWithUserHookDelegate<TAVObject> hookDelegate) where TAVObject : AVObject
        {
            cloud.UseHook(EngineHookType.AfterDelete, hookDelegate);
            return cloud;
        }
        #endregion

        public static Cloud UseHookClass<THookClass>(this Cloud cloud) where THookClass : class
        {
            cloud.UseHookClass(default(THookClass));
            return cloud;
        }

        public static Cloud UseHookClass<THookClass>(this Cloud cloud, THookClass hook = null) where THookClass : class
        {
            var tupple = ReflectHooks<THookClass>(hook);
            tupple.ForEach(t =>
            {
                cloud.UseHook(t.Item1, t.Item2, t.Item3);
            });
            return cloud;
        }

        public static List<(string, EngineHookType, EngineHookDelegate)> ReflectHooks<THookClass>(THookClass hook = null) where THookClass : class
        {
            var methods = hook != null ? hook.GetType().GetMethods() : typeof(THookClass).GetMethods();
            var tupple = new List<(string, EngineHookType, EngineHookDelegate)>();
            foreach (var method in methods)
            {
                var hookAttributes = method.GetCustomAttributes(typeof(EngineObjectHookAttribute), false);

                if (hookAttributes.Length == 1)
                {
                    var hookAttribute = (EngineObjectHookAttribute)hookAttributes[0];

                    var parametersArray = method.GetParameters();
                    EngineHookDelegate del = null;
                    if (parametersArray.Count() == 1)
                    {
                        var theParameterType = parametersArray[0].ParameterType;
                        if (theParameterType == typeof(AVObject))
                        {
                            var objectDel = method.IsStatic ? (EngineObjectHookDelegate)Delegate.CreateDelegate(typeof(EngineObjectHookDelegate), method) : (EngineObjectHookDelegate)Delegate.CreateDelegate(typeof(EngineObjectHookDelegate), hook ?? Activator.CreateInstance<THookClass>(), method);
                            del = (context) =>
                            {
                                return objectDel(context.TheObject);
                            };
                        }
                        else if (theParameterType == typeof(EngineObjectHookContext))
                        {
                            del = method.IsStatic ? (EngineHookDelegate)Delegate.CreateDelegate(typeof(EngineHookDelegate), method) : (EngineHookDelegate)Delegate.CreateDelegate(typeof(EngineHookDelegate), hook ?? Activator.CreateInstance<THookClass>(), method);
                        }
                    }
                    else if (parametersArray.Count() == 2)
                    {
                        var firstParameterType = parametersArray[0].ParameterType;
                        var secondParameterType = parametersArray[1].ParameterType;
                        if (firstParameterType == typeof(AVObject) && secondParameterType == typeof(AVUser))
                        {
                            var objectUserDel = method.IsStatic ? (EngineObjectWithUserHookDelegate)Delegate.CreateDelegate(typeof(EngineObjectWithUserHookDelegate), method) : (EngineObjectWithUserHookDelegate)Delegate.CreateDelegate(typeof(EngineObjectWithUserHookDelegate), hook ?? Activator.CreateInstance<THookClass>(), method);
                            del = (context) =>
                            {
                                return objectUserDel(context.TheObject, context.By);
                            };
                        }
                    }
                    else
                    {
                        throw new EngineException(-1, "parameters not supported. Please define your method like foo(AVObject obj) or foo(AVObject obj, AVUser user) or foo(EngineObjectHookContext context).");
                    }
                    var mixedResult = (hookAttribute.ClassName, hookAttribute.HookType, del);
                    tupple.Add(mixedResult);
                }
            }
            return tupple;
        }

        public static Cloud UseFunction<TClass>(this Cloud cloud) where TClass : class
        {
            cloud.UseFunction<TClass>(default(TClass));
            return cloud;
        }

        public static Cloud UseFunction<TClass>(this Cloud cloud, TClass tClass = null) where TClass : class
        {
            var tupple = ReflectFunctions<TClass>(tClass);
            tupple.ForEach(t =>
            {
                cloud.Define(t.Item1, t.Item2);
            });
            return cloud;
        }

        public static List<(string, EngineFunctionDelegate)> ReflectFunctions<TClass>(TClass tInstance = null) where TClass : class
        {
            var methods = tInstance != null ? tInstance.GetType().GetMethods() : typeof(TClass).GetMethods();
            var tupple = new List<(string, EngineFunctionDelegate)>();
            foreach (var method in methods)
            {
                var hookAttributes = method.GetCustomAttributes(typeof(EngineFunctionAttribute), false);

                if (hookAttributes.Length == 1)
                {
                    var functionAttribute = (EngineFunctionAttribute)hookAttributes[0];

                    EngineFunctionDelegate engineFunctionDelegate = (context) =>
                    {
                        var pas = BindParamters(method, context);
                        if (method.IsStatic)
                        {
                            context.Result = method.Invoke(null, pas);
                        }
                        else
                        {
                            context.Result = method.Invoke(tInstance ?? Activator.CreateInstance<TClass>(), pas);
                        }
                        return Task.FromResult(context);
                    };
                    var mixedResult = (functionAttribute.FunctionName, engineFunctionDelegate);

                    tupple.Add(mixedResult);
                }
            }
            return tupple;
        }

        public static object[] BindParamters(MethodInfo memberInfo, EngineFunctionContext context)
        {
            List<object> result = new List<object>();
            ParameterInfo[] engineParameters = memberInfo.GetParameters();

            foreach (var pi in engineParameters)
            {
                var piAttrs = pi.GetCustomAttributes(typeof(EngineFunctionParameterAttribute), false);
                if (piAttrs.Length == 1)
                {
                    var functionParameterAttribute = (EngineFunctionParameterAttribute)piAttrs[0];
                    if (context.FunctionParameters.ContainsKey(functionParameterAttribute.ParameterName))
                    {
                        result.Add(context.FunctionParameters[functionParameterAttribute.ParameterName]);
                    }
                }
            }
            return result.ToArray();
        }

        public static Cloud Define<T>(this Cloud cloud, string functionName, Func<T> function)
        {
            cloud.Define(functionName, (context) =>
            {
                var result = function();
                context.Result = result;
                return Task.FromResult(context);
            });
            return cloud;
        }

        public static Cloud Define<T, R>(this Cloud cloud, string functionName, Func<T, R> function)
        {
            cloud.Define(functionName, (context) =>
            {
                var result = function((T)context.FunctionParameters.Values.ToList()[0]);
                context.Result = result;
                return Task.FromResult(context);
            });
            return cloud;
        }

        public static Cloud Define<T1, T2, R>(this Cloud cloud, string functionName, Func<T1, T2, R> function)
        {
            cloud.Define(functionName, (context) =>
            {
                var pList = context.FunctionParameters.Values.ToList();
                var result = function((T1)pList[0], (T2)pList[1]);
                context.Result = result;
                return Task.FromResult(context);
            });
            return cloud;
        }

        public static Cloud OnVerifiedSMS(this Cloud cloud, Func<AVUser, Task> func)
        {
            cloud.OnVerified("sms", context =>
            {
                return func.Invoke(context.TheUser);
            });
            return cloud;
        }

        public static Cloud OnVerifiedEmail(this Cloud cloud, Func<AVUser, Task> func)
        {
            cloud.OnVerified("email", context =>
            {
                return func.Invoke(context.TheUser);
            });
            return cloud;
        }

        public static Cloud OnLogIn(this Cloud cloud, Func<AVUser, Task> func)
        {
            cloud.OnLogIn(context =>
            {
                return func.Invoke(context.TheUser);
            });
            return cloud;
        }

        //private static Delegate CreateDelegate(this MethodInfo methodInfo, object target)
        //{
        //    Func<Type[], Type> getType;
        //    var isAction = methodInfo.ReturnType.Equals((typeof(void)));
        //    var types = methodInfo.GetParameters().Select(p => p.ParameterType);

        //    if (isAction)
        //    {
        //        getType = Expression.GetActionType;
        //    }
        //    else
        //    {
        //        getType = Expression.GetFuncType;
        //        types = types.Concat(new[] { methodInfo.ReturnType });
        //    }

        //    if (methodInfo.IsStatic)
        //    {
        //        return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
        //    }

        //    return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
        //}
    }
}
