namespace IoC
{
    public static class ContainerExtensions
    {
        public static Container AddSingleton<T>(this Container container) where T : class =>
            container?.Register<T, T>(InstanceOption.Singleton);
        public static Container AddSingleton<T, TS>(this Container container) where TS : class, T =>
            container?.Register<T, TS>(InstanceOption.Singleton);
        public static Container AddScoped<T>(this Container container) where T : class =>
            container?.Register<T, T>(InstanceOption.InScope);
        public static Container AddScoped<T, TS>(this Container container) where TS : class, T =>
            container?.Register<T, TS>(InstanceOption.InScope);
    }
}