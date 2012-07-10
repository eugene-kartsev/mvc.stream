using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Routing;

namespace Mvc.Stream.Tests.Mocks
{
    public class MockHttpRequest : HttpRequestBase
    {
        private readonly NameValueCollection _headers = new NameValueCollection();
        public override NameValueCollection Headers
        {
            get { return _headers; }
        }

        private readonly HttpFileCollectionBase _files;
        public MockHttpRequest(MockHttpFilesCollection filesMock)
        {
            _files = filesMock;
        }

        public override HttpFileCollectionBase Files
        {
            get { return _files; }
        }

        private string _applicationPath;
        public override string ApplicationPath
        {
            get
            {
                return _applicationPath;
            }
        }

        public string TestInput;
        public override System.IO.Stream InputStream
        {
            get
            {
                if (TestInput != null)
                {
                    var stream = new MemoryStream();
                    var chars = TestInput.ToCharArray();
                    foreach (var c in chars)
                    {
                        stream.WriteByte(Convert.ToByte(c));
                    }
                    return stream;
                }
                return new MemoryStream();
            }
        }

        public string TestHttpMethod;
        public override string HttpMethod
        {
            get
            {
                return TestHttpMethod;
            }
        }

        public MockHttpRequest SetHeader(string header, string val)
        {
            _headers[header] = val;
            return this;
        }

        public MockHttpRequest SetApplicationPath(string path)
        {
            _applicationPath = path;
            return this;
        }

        private MockRequestContext _context = new MockRequestContext();
        public override RequestContext RequestContext
        {
            get
            {
                return _context;
            }
        }
    }
}
