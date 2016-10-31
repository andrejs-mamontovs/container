using System;
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

            Assert.True(true);
        }
    }

    public class ServiceMock {

    }
}
