using Xunit;
using IoC;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void InitContainerTest()
        {
            var container = new Container();

            Assert.Same(container,
                container.AddSingleton<ServiceMock>());
            Assert.NotNull(container.Resolve<ServiceMock>());
        }

        [Fact]
        public void NullSafeTest()
        {
            Container container = null;
            Assert.Null(container.AddSingleton<ServiceMock>());
            Assert.Null(container.AddScoped<ServiceMock>());
            Assert.Null(container.AddSingleton<IServiceMock, ServiceMock>());
            Assert.Null(container.AddScoped<IServiceMock, ServiceMock>());
        }

        [Fact]
        public void SingletonTest()
        {
            var container = CreateContainerIncludeSingleton();

            var expected =
                container.Resolve<IServiceMock>();
            Assert.Same(expected, container.Resolve<IServiceMock>());
            Assert.Same(expected, container.Resolve<IServiceMock>());
            Assert.Same(expected, container.Resolve<IServiceMock>());

            var scope = container.Resolve<ScopeMock>();
            Assert.Same(expected, scope.ServiceA);
            Assert.Same(expected, scope.ServiceB);
            Assert.Same(scope.ServiceA, scope.ServiceB);
        }

        [Fact]
        public void ScopedTest()
        {
            var container = CreateContainerIncludeScoped();
            var scopeA = container.Resolve<ScopeMock>();
            var scopeB = container.Resolve<ScopeMock>();
            Assert.Same(scopeA.ServiceA, scopeA.ServiceB);
            Assert.Same(scopeB.ServiceA, scopeB.ServiceB);

            Assert.NotSame(scopeA.ServiceA, scopeB.ServiceA);
            Assert.NotSame(scopeA.ServiceB, scopeB.ServiceB);
        }

        private Container CreateContainerIncludeSingleton()
        {
            return new Container().AddSingleton<IServiceMock, ServiceMock>().Register<ScopeMock>();
        }

        private Container CreateContainerIncludeScoped()
        {
            return new Container().AddScoped<IServiceMock, ServiceMock>().Register<ScopeMock>();
        }
    }

    public class ServiceMock : IServiceMock
    {
        public int Id
        {
            get; set;
        }
    }

    public class ScopeMock
    {

        public IServiceMock ServiceA { get; }
        public IServiceMock ServiceB { get; }
        public ScopeMock(IServiceMock serviceA, IServiceMock serviceB)
        {
            this.ServiceA = serviceA;
            this.ServiceB = serviceB;
        }
    }

    public interface IServiceMock
    {
        int Id { get; set; }
    }
}