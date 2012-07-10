// ReSharper disable InconsistentNaming

using System.Net.Mime;
using System.Web.Mvc;
using Mvc.Stream.Mime;

namespace Mvc.Stream.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Stream480p()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_480p_stereo.ogg");
            return new ResumeFileResult(path, new MimeMapper().GetMimeFromPath(path), Request);
        }

        public ActionResult Stream480pOld()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_480p_stereo.ogg");
            return File(path, new MimeMapper().GetMimeFromPath(path));
        }

        public ActionResult Stream720p()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_720p_stereo.ogg");
            return new ResumeFileResult(path, new MimeMapper().GetMimeFromPath(path), Request);
        }

        public ActionResult Stream720pOld()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_720p_stereo.ogg");
            return File(path, new MimeMapper().GetMimeFromPath(path));
        }

        public ActionResult Stream1080p()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_1080p_stereo.ogg");
            return new ResumeFileResult(path, new MimeMapper().GetMimeFromPath(path), Request);
        }

        public ActionResult Stream1080pOld()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_1080p_stereo.ogg");
            return File(path, new MimeMapper().GetMimeFromPath(path));
        }

        public ActionResult Stream480pDownload()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_480p_stereo.ogg");
            return new ResumeFileResult(path, MediaTypeNames.Application.Octet, Request, "big_buck_bunny_480p_stereo.ogg");
        }

        public ActionResult Stream480pOldDownload()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_480p_stereo.ogg");
            return File(path, MediaTypeNames.Application.Octet, "big_buck_bunny_480p_stereo.ogg");
        }

        public ActionResult Stream720pDownload()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_720p_stereo.ogg");
            return new ResumeFileResult(path, MediaTypeNames.Application.Octet, Request, "big_buck_bunny_720p_stereo.ogg");
        }

        public ActionResult Stream720pOldDownload()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_720p_stereo.ogg");
            return new ResumeFileResult(path, MediaTypeNames.Application.Octet, Request, "big_buck_bunny_720p_stereo.ogg");
        }

        public ActionResult Stream1080pDownload()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_1080p_stereo.ogg");
            return new ResumeFileResult(path, MediaTypeNames.Application.Octet, Request, "big_buck_bunny_1080p_stereo.ogg");
        }

        public ActionResult Stream1080pOldDownload()
        {
            var path = Server.MapPath("~/Content/big_buck_bunny_1080p_stereo.ogg");
            return File(path, MediaTypeNames.Application.Octet, "big_buck_bunny_1080p_stereo.ogg");
        }
    }
}
