using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoC
{
    public class Container
    {
        Dictionary<Type, object> Instances { get; } = new Dictionary<Type, object>();

        Dictionary<Type, (Type, InstanceOption)> Map { get; } =
            new Dictionary<Type, (Type, InstanceOption)>();

        public Container Register<T, TS>(TS instance = null, InstanceOption option = InstanceOption.Default) where TS : class, T
        {
            var type = typeof(T);
            if (instance == null)
            {
                Map.Add(type, (typeof(TS), option));
            }
            else
            {
                Instances.Add(type, instance);
            }

            return this;
        }

        // TODO: rethink instance passing;
        public Container Register<T>(T instance = null, InstanceOption option = InstanceOption.Default) where T : class
        {
            return Register<T, T>(instance, option);
        }

        public T Resolve<T>() => (T)Resolve(typeof(T), CreateScope());

        public object Resolve(Type t) => Resolve(t, CreateScope());

        protected virtual object Resolve(Type t, Dictionary<Type, object> scope)
        {

            if (Instances.TryGetValue(t, out object o) || scope.TryGetValue(t, out o))
            {
                return o;
            }

            var (type, option) = Map[t];

            var ci = type.GetTypeInfo().DeclaredConstructors.First(i => i.IsPublic);

            object instance = Activator.CreateInstance(type,
                ci.GetParameters().Select(i => Resolve(i.ParameterType, scope)).ToArray());

            switch (option)
            {
                case InstanceOption i when (i == InstanceOption.Singleton):
                    Instances.Add(t, instance);
                    break;
                case InstanceOption i when (i == InstanceOption.InScope):
                    scope.Add(t, instance);
                    break;
            }

            return instance;
        }

        Dictionary<Type, object> CreateScope() => new Dictionary<Type, object>();
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