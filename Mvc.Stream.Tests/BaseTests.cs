﻿using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Mvc.Stream.Tests.Mocks;
using NUnit.Framework;

namespace Mvc.Stream.Tests
{
    [TestFixture]
    public abstract class BaseTests
    {
        protected string TestDirectoryPath()
        {
            return new DirectoryInfo(".\\Resources").FullName;
        }

        protected FileInfo TestFile(string fileName)
        {
            return new FileInfo(string.Format("{0}\\{1}", TestDirectoryPath(), fileName));
        }

        protected string FilePath(string fileName)
        {
            return TestFile(fileName).FullName;
        }

        protected BaseTests()
        {
            InitMocks();
        }

        [SetUp]
        public void BaseTestsSetup()
        {
            InitMocks();
        }

        protected void InitMocks()
        {
            var mockHttpContext = new Mock<HttpContextBase>();
            Context = mockHttpContext.Object;
            Session = new MockHttpSession();
            Request = new MockHttpRequest(new MockHttpFilesCollection(null));
            Response = new MockHttpResponse();

            mockHttpContext.Setup(ctx => ctx.Session).Returns(() => Session);
            mockHttpContext.Setup(ctx => ctx.Request).Returns(() => Request);
            mockHttpContext.Setup(ctx => ctx.Response).Returns(() => Response);
            mockHttpContext.Setup(ctx => ctx.Cache).Returns(() => HttpRuntime.Cache);
        }

        protected internal MockHttpResponse Response { get; set; }
        protected internal HttpContextBase Context { get; set; }
        protected internal MockHttpRequest Request { get; set; }
        protected internal MockHttpSession Session { get; private set; }

        protected ControllerContext ControllerContext<T>(T controller) where T : ControllerBase
        {
            return new ControllerContext(Context, new RouteData(), controller);
        }

        protected ModelBindingContext BindingContext<T>()
        {
            return new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(T))
            };
        }

        protected T MockObject<T>() where T : class
        {
            var mock = new Mock<T>();
            return mock.Object;
        }

        protected Mock<T> Mock<T>() where T : class
        {
            return new Mock<T>();
        }
    }
}
