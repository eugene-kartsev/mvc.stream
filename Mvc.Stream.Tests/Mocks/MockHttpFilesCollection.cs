using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc.Stream.Tests.Mocks
{
    public class MockHttpFilesCollection : HttpFileCollectionBase
    {
        private readonly Dictionary<string, MockHttpPostedFileBase> _files
            = new Dictionary<string, MockHttpPostedFileBase>();

        public MockHttpFilesCollection(MockHttpPostedFileBase file)
        {
            if (file != null)
            {
                _files.Add(file.FileName, file);
            }
        }

        public override int Count { get { return _files.Count; } }

        public override HttpPostedFileBase this[int index]
        {
            get
            {
                return _files.Skip(index).Take(1).FirstOrDefault().Value;
            }
        }

        public override HttpPostedFileBase this[string name] { get { return _files[name]; } }

        public override string[] AllKeys
        {
            get
            {
                return _files.Select(x => x.Key).ToArray();
            }
        }
    }
}
