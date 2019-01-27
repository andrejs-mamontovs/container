using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoC
{
    public class Container
    {
        Dictionary<Type, object> Instances { get; } = new Dictionary<Type, object>();

        Dictionary<Type, (Type, ConstructorInfo, InstanceOption)> Map { get; } =
            new Dictionary<Type, (Type, ConstructorInfo, InstanceOption)>();

        public Container Register<T, TS>(TS instance) where TS : class, T
        {
            Instances.Add(typeof(T), instance ?? throw new NullReferenceException());
            return this;
        }

        public Container Register<T, TS>(InstanceOption option = InstanceOption.Default) where TS : class, T
        {
            var type = typeof(T);
            var constructor = GetConstructorInfo<TS>();
            Map.Add(typeof(T), (constructor.DeclaringType, constructor, option));
            return this;
        }

        public Container Register<T>(InstanceOption option = InstanceOption.Default) where T : class
        {
            return Register<T, T>(option);
        }

        public T Resolve<T>() => (T)Resolve(typeof(T), CreateScope());

        public object Resolve(Type t) => Resolve(t, CreateScope());

        protected virtual object Resolve(Type t, Dictionary<Type, object> scope)
        {

            if (Instances.TryGetValue(t, out object o) || scope.TryGetValue(t, out o))
            {
                return o;
            }

            var (type, constructor, option) = Map[t];
            var instance = Activator.CreateInstance(type,
                constructor.GetParameters().Select(i => Resolve(i.ParameterType, scope)).ToArray());

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

        ConstructorInfo GetConstructorInfo<T>() =>
            (typeof(T)).GetTypeInfo().DeclaredConstructors.First(i => i.IsPublic);
    }

    public enum InstanceOption
    {
        Default,
        Singleton,
        InScope
    }
}