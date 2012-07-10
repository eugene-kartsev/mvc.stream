using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace Mvc.Stream.Tests.Mocks
{
    public class MockHttpResponse : HttpResponseBase
    {
        private NameValueCollection _headers = new NameValueCollection();
        public override NameValueCollection Headers
        {
            get { return _headers; }
        }

        public override void AppendHeader(string name, string value)
        {
            AddHeader(name, value);
        }

        public override void AddHeader(string name, string value)
        {
            _headers.Add(name, value);
        }

        public bool FileTransmitted { get; set; }
        public override bool BufferOutput { get; set; }

        private MemoryStream _stream;
        public override System.IO.Stream OutputStream
        {
            get
            {
                return _stream ?? (_stream = new MemoryStream());
            }
        }

        public void ClearTestResponse()
        {
            _stream = new MemoryStream();
            Headers.Clear();
            StatusCode = 0;
        }

        public override void Flush()
        { }

        public override bool IsClientConnected
        {
            get { return true; }
        }

        public override string ContentType
        {
            get;
            set;
        }

        public override int StatusCode
        {
            get;
            set;
        }

        public override void Write(string s)
        {
        }

        public bool IsClosed;
        public override void Close()
        {
            IsClosed = true;
        }

        public override void TransmitFile(string filename)
        {
            FileTransmitted = true;
            var fi = new FileInfo(filename);
            using (var read = fi.OpenRead())
            {
                for (var i = 0; i < fi.Length; i++)
                {
                    OutputStream.WriteByte((byte)read.ReadByte());
                }
            }
        }

        public override void TransmitFile(string filename, long offset, long length)
        {
            var fi = new FileInfo(filename);
            using (var read = fi.OpenRead())
            {
                read.Seek(offset, SeekOrigin.Begin);
                for (var i = 0; i < length; i++)
                {
                    OutputStream.WriteByte((byte)read.ReadByte());
                }
            }
        }
    }
}
