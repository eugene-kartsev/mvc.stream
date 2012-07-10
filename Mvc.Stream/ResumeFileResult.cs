using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Mvc.Stream.Internal;

namespace Mvc.Stream
{
    /// <summary>
    /// class Extends standard FilePathResult - it adds possibility to suspend and resume downloading
    /// </summary>
    public class ResumeFileResult : FilePathResult
    {
        /// <summary>
        /// Default exceptiion logger.
        /// Log4net wasn't used because of additional dependency.
        /// </summary>
        public static Action<Exception> LogException;

        private readonly Regex _rangePattern = new Regex("bytes=(\\d*)-(\\d*)");
        private readonly string _ifNoneMatch;
        private readonly string _ifModifiedSince;
        private readonly string _ifMatch;
        private readonly string _ifUnmodifiedSince;
        private readonly string _ifRange;
        private readonly string _etag;
        private readonly Range _range;
        private readonly FileInfo _file;
        private readonly string _lastModified;
        private readonly bool _rangeRequest;
        private readonly string _downloadFileName;

        public ResumeFileResult(string fileName,
                                string contentType,
                                HttpRequestBase request)
            : this(fileName, contentType, request, null)
        { }

        public ResumeFileResult(string fileName,
                                string contentType,
                                HttpRequestBase request,
                                string downloadFileName)
            : this(fileName,
                   contentType,
                   request.Headers[HttpHeaders.IfNoneMatch],
                   request.Headers[HttpHeaders.IfModifiedSince],
                   request.Headers[HttpHeaders.IfMatch],
                   request.Headers[HttpHeaders.IfUnmodifiedSince],
                   request.Headers[HttpHeaders.IfRange],
                   request.Headers[HttpHeaders.Range],
                   downloadFileName)
        {}


        public ResumeFileResult(string fileName,
                                string contentType,
                                string ifNoneMatch,
                                string ifModifiedSince,
                                string ifMatch,
                                string ifUnmodifiedSince,
                                string ifRange,
                                string range,
                                string downloadFileName)
            : base(fileName, contentType)
        {
            _file = new FileInfo(fileName);
            _lastModified = Util.FormatDate(_file.LastWriteTime);
            _rangeRequest = range != null;
            _range = Range(range);
            _etag = Etag();
            _ifNoneMatch = ifNoneMatch;
            _ifModifiedSince = ifModifiedSince;
            _ifMatch = ifMatch;
            _ifUnmodifiedSince = ifUnmodifiedSince;
            _ifRange = ifRange;
            _downloadFileName = downloadFileName;
        }

        /// <summary>
        /// Checks headers in request, adds appropriate headers to response
        /// </summary>
        /// <param name="response"></param>
        protected override void WriteFile(HttpResponseBase response)
        {
            response.AppendHeader(HttpHeaders.Etag, _etag);
            response.AppendHeader(HttpHeaders.LastModified, _lastModified);
            response.AppendHeader(HttpHeaders.Expires, Util.FormatDate(DateTime.Now));

            if (IsNotModified())
            {
                response.StatusCode = (int)HttpStatusCode.NotModified;
            }
            else if (IsPreconditionFailed())
            {
                response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
            }
            else if (IsRangeNotSatisfiable())
            {
                response.AppendHeader(HttpHeaders.ContentRange, "bytes */" + _file.Length);
                response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
            }
            else
            {
                TransmitFile(response);
            }
        }

        /// <summary>
        /// Calculates total length of bytes to write to Response
        /// </summary>
        /// <returns></returns>
        protected long ContentLength()
        {
            return _range.End - _range.Start + 1;
        }

        /// <summary>
        /// Analyzes If-Range header and returns:
        ///     true - if partial content must be sent
        ///     false - if whole file must be sent
        /// spec: http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.27
        /// </summary>
        /// <returns></returns>
        protected bool SendRange()
        {
            return _rangeRequest && _ifRange == null
                    || _rangeRequest && _ifRange == _etag;
        }

        /// <summary>
        /// Writes file to response, adds correct headers based on Request headers and file attributes
        /// </summary>
        /// <param name="response"></param>
        protected virtual void TransmitFile(HttpResponseBase response)
        {
            var contentLength = ContentLength();
            response.StatusCode = SendRange() ? (int)HttpStatusCode.PartialContent : (int)HttpStatusCode.OK;

            response.AppendHeader(HttpHeaders.ContentLength, contentLength.ToString(CultureInfo.InvariantCulture));
            response.AppendHeader(HttpHeaders.AcceptRanges, "bytes");
            response.AppendHeader(HttpHeaders.ContentRange, string.Format("bytes {0}-{1}/{2}", _range.Start, _range.End, _file.Length));

            if(!string.IsNullOrWhiteSpace(_downloadFileName))
            {
                response.AddHeader("Content-Disposition", string.Format("attachment;filename=\"{0}\"", _downloadFileName));
            }

            try
            {
                response.TransmitFile(FileName, _range.Start, contentLength);
            }
            catch (Exception ex)
            {
                if(LogException != null)
                {
                    LogException(ex);
                }
            }
        }

        /// <summary>
        /// Range isn't satisfiable when:
        ///     Start point is greater than total size of the file
        ///     Start point is less than 0
        ///     End point is equal or greater than size of the file
        ///     Start point greater than End point
        /// spec: http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.4.17
        /// </summary>
        /// <returns></returns>
        protected bool IsRangeNotSatisfiable()
        {
            return _range.Start >= _file.Length
                    || _range.Start < 0
                    || _range.End >= _file.Length
                    || _range.Start > _range.End;
        }

        /// <summary>
        /// Precondition may be failed when:
        /// If-Match (http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.24)
        ///     header is empty and it not matches etag
        /// If-Unmodified-Since (http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.28)
        ///     header is not empty and it not matches File.LastWriteTime.
        ///     It may happend when file has been changed during the downloading.
        /// </summary>
        /// <returns></returns>
        protected bool IsPreconditionFailed()
        {
            if (_ifMatch != null)
            {
                return !IsMatch(_ifMatch, _etag);
            }

            return _ifUnmodifiedSince != null && _ifUnmodifiedSince != _lastModified;
        }

        /// <summary>
        /// The method returns true if either
        /// If-None-Match (http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.26) or
        /// or If-Modified-Since (http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.25)
        /// is valid
        /// </summary>
        /// <returns></returns>
        protected bool IsNotModified()
        {
            if (_ifNoneMatch != null)
            {
                return IsMatch(_ifNoneMatch, _etag);
            }

            return _ifModifiedSince != null && _ifModifiedSince == _lastModified;
        }

        /// <summary>
        /// Etag response header for current file
        /// </summary>
        /// <returns></returns>
        private string Etag()
        {
            return Util.Etag(_file);
        }

        private bool IsMatch(string values, string etag)
        {
            var matches = (values ?? string.Empty).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return matches.Any(s => s.Equals("*") || s.Equals(etag));
        }

        /// <summary>
        /// Calculates start and end points based on Range header
        /// Spec: http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35.1
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        private Range Range(string range)
        {
            var lastByte = _file.Length - 1;

            if (!string.IsNullOrWhiteSpace(range))
            {
                var matches = _rangePattern.Matches(range);
                if (matches.Count != 0)
                {
                    /* 
                     Examples of byte-ranges-specifier values (assuming an entity-body of length 10000):
                    - The first 500 bytes (byte offsets 0-499, inclusive):  bytes=0-499

                    - The second 500 bytes (byte offsets 500-999, inclusive): bytes=500-999

                    - The final 500 bytes (byte offsets 9500-9999, inclusive): bytes=-500
                    - Or bytes=9500-

                    - The first and last bytes only (bytes 0 and 9999):  bytes=0-0,-1
                     */

                    var start = matches[0].Groups[1].Value.ToLong(-1);
                    var end = matches[0].Groups[2].Value.ToLong(-1);

                    if (start != -1 || end != -1)
                    {
                        if (start == -1)
                        {
                            /* -300: Final 300 b */
                            start = _file.Length - end;
                            end = lastByte;
                        }
                        else if (end == -1)
                        {
                            /* 1000-: from 1000 byte to the end */
                            end = lastByte;
                        }

                        return new Range{Start=start, End=end};
                    }
                }
                return new Range{Start=-1, End=-1};
            }
            return new Range{Start=0, End=lastByte};
        }

        /// <summary>
        /// Utilities which are used to support ResumeFileResult functionality
        /// </summary>
        public static class Util
        {
            /// <summary>
            /// Etag response header.
            /// Spec: http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.19
            /// </summary>
            /// <returns></returns>
            public static string Etag(FileInfo file)
            {
                return Etag(file.FullName, FormatDate(file.LastWriteTime));
            }

            /// <summary>
            /// <see cref="Etag(System.IO.FileInfo)"/>
            /// </summary>
            /// <param name="fullName"></param>
            /// <param name="lastModified"></param>
            /// <returns></returns>
            public static string Etag(string fullName, string lastModified)
            {
                return "\"mvc-streaming-" + fullName.GetHashCode() + "-" + fullName.GetHashCode() + "\"";
            }

            /// <summary>
            /// <see cref="Etag(System.IO.FileInfo)"/>
            /// </summary>
            /// <param name="fullName"></param>
            /// <param name="lastWriteTime"></param>
            /// <returns></returns>
            public static string Etag(string fullName, DateTime lastWriteTime)
            {
                return Etag(fullName, FormatDate(lastWriteTime));
            }

            /// <summary>
            /// The format is an absolute date and time as defined by HTTP-date in section 3.3.1.
            /// It MUST be in RFC 1123 date format.
            /// </summary>
            /// <param name="date"></param>
            /// <returns></returns>
            public static string FormatDate(DateTime date)
            {
                return date.ToString("R");
            }
        }
    }
}
