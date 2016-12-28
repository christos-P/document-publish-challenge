using System.Web.Mvc;
using DocumentPublishChallenge.Web.Models;

namespace DocumentPublishChallenge.Web.Controllers
{
    public class DocumentsController : Controller
    {
        [HttpGet]
        public ActionResult MyDocuments() => View(new MyDocumentsViewModel());

        [HttpGet]
        public ActionResult Upload() => View();
    }
}