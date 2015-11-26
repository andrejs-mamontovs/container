﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoC {
    public class Container
    {
        readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();
        readonly Dictionary<Type, Tuple<Type, InstanceOption>> map = new Dictionary<Type, Tuple<Type, InstanceOption>>();

        public void Register<T, TS>(TS instance = null, InstanceOption option = InstanceOption.Default) where TS : class, T
        {
            if (instance == null)
            {
                map.Add(typeof(T), Tuple.Create(typeof(TS), option));
            }
            else
            {
                instances.Add(typeof(T), instance);
            }
        }

        public void Register<T>(T instance = null, InstanceOption option = InstanceOption.Default) where T : class
        {
            Register<T, T>(instance, option);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T), new Dictionary<Type, object>());
        }

        public object Resolve(Type t)
        {
            return Resolve(t, new Dictionary<Type, object>());
        }

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
}