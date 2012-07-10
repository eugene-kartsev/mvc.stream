﻿using System.Web;

namespace Mvc.Stream.Tests.Mocks
{
    public class MockResumeFileResult : ResumeFileResult
    {
        public MockResumeFileResult(string fileName, string contentType, HttpRequestBase request)
            : base(fileName, contentType, request)
        {
        }

        public MockResumeFileResult(string fileName, string contentType, HttpRequestBase request, string downloadFileName)
            : base(fileName, contentType, request, downloadFileName)
        {
        }

        public new bool IsNotModified()
        {
            return base.IsNotModified();
        }

        public new bool IsPreconditionFailed()
        {
            return base.IsPreconditionFailed();
        }

        public new bool IsRangeNotSatisfiable()
        {
            return base.IsRangeNotSatisfiable();
        }

        public new bool SendRange()
        {
            return base.SendRange();
        }

        public void WriteFileTest(HttpResponseBase response)
        {
            base.WriteFile(response);
        }

        public void TransmitTest(HttpResponseBase response)
        {
            base.TransmitFile(response);
        }
    }
}
