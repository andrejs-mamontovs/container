using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoC
{
    // Test
    public class Container
    {
        readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();
        readonly Dictionary<Type, Tuple<Type, InstanceOption>> map = new Dictionary<Type, Tuple<Type, InstanceOption>>();

        public Container Register<T, TS>(TS instance = null, InstanceOption option = InstanceOption.Default) where TS : class, T
        {
            if (instance == null)
            {
                map.Add(typeof(T), Tuple.Create(typeof(TS), option));
            }
            else
            {
                instances.Add(typeof(T), instance);
            }

            return this;
        }

        // TODO: rethink instance passing;
        public Container Register<T>(T instance = null, InstanceOption option = InstanceOption.Default) where T : class
        {
            return Register<T, T>(instance, option);
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object Resolve(Type t) => Resolve(t, new Dictionary<Type, object>());

        protected virtual object Resolve(Type t, Dictionary<Type, object> scope)
        {
            object o;

            if (instances.TryGetValue(t, out o) || scope.TryGetValue(t, out o))
            {
                return o;
            }

            Tuple<Type, InstanceOption> settings = map[t];

            Type type = (settings).Item1;

            ConstructorInfo constructor = type.GetTypeInfo().DeclaredConstructors.First(i => i.IsPublic);

            object instance = Activator.CreateInstance(type,
                constructor.GetParameters().Select(i => Resolve(i.ParameterType, scope)).ToArray());

            if (settings.Item2 == InstanceOption.Singleton)
            {
                instances.Add(t, instance);
            }

            if (settings.Item2 == InstanceOption.InScope)
            {
                scope.Add(t, instance);
            }

            return instance;
        }
    }

    public enum InstanceOption
    {
        Default,
        Singleton,
        InScope
    }

    public static class ContainerExtensions
    {
        public static Container AddSingleton<T>(this Container container) where T : class
        {
            return container?.Register<T, T>(null, InstanceOption.Singleton);
        }

        public static Container AddSingleton<T, TS>(this Container container) where TS : class, T
        {
            return container?.Register<T, TS>(null, InstanceOption.Singleton);
        }

        public static Container AddScoped<T>(this Container container) where T : class
        {
            return container?.Register<T, T>(null, InstanceOption.InScope);
        }
        public static Container AddScoped<T, TS>(this Container container) where TS : class, T
        {
            return container?.Register<T, TS>(null, InstanceOption.InScope);
        }

    }
}