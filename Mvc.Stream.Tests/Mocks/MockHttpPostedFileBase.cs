using System.IO;
using System.Web;

namespace Mvc.Stream.Tests.Mocks
{
    public class MockHttpPostedFileBase : HttpPostedFileBase
    {
        private readonly int _contentLen;
        private readonly string _fileName;
        private readonly string _contentType;
        private readonly System.IO.Stream _stream;

        public MockHttpPostedFileBase(int contentLen, string fileName, string contentType, System.IO.Stream stream = null)
        {
            _contentLen = contentLen;
            _fileName = fileName;
            _contentType = contentType;
            _stream = stream;
        }

        public override int ContentLength { get { return _contentLen; } }

        public override string FileName { get { return _fileName; } }

        public override System.IO.Stream InputStream { get { return _stream; } }

        public override string ContentType { get { return _contentType; } }

        public override void SaveAs(string filename)
        {
            var fileInfo = new FileInfo(filename);
            var directory = new DirectoryInfo(Path.GetDirectoryName(fileInfo.FullName));

            if (!directory.Exists)
            {
                directory.Create();
            }

            using (var file = fileInfo.CreateText())
            {
                file.Write("test");
            }
        }
    }
}
