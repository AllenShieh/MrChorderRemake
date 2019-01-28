using System.Web;
using System.Web.Mvc;
using PDF;
using OnsetDetection;

namespace MrChorder.Controllers
{
    public class ChordController : Controller
    {
        private static string filename;
        private static string wavFile;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void GetFile()
        {
            // Get uploaded file.
            foreach (string eachfile in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[eachfile] as HttpPostedFileBase;
                string path = System.IO.Path.Combine(Server.MapPath("~/Upload"), System.IO.Path.GetFileName(file.FileName));
                filename = path;
                file.SaveAs(path);
            }
            wavFile = filename.Substring(filename.LastIndexOf('\\') + 1, filename.Length - filename.LastIndexOf('\\') - 5);
            // Delete previously generated file.
            string genPath = System.IO.Path.Combine(Server.MapPath("~/Generate"), System.IO.Path.GetFileName("AnalyseResult.pdf"));
            if (System.IO.File.Exists(genPath))
            {
                System.IO.File.Delete(genPath);
            }
            Response.Write("File upload successfully!");
            Response.End();
        }

        [HttpGet]
        public void SendFile()
        {
            // Process file.
            string[] nameElements = wavFile.Split('_');

            string resultFilePath = System.IO.Path.Combine(Server.MapPath("~/Generate/"), System.IO.Path.GetFileName("AnalyseResult.pdf"));
            string imgPath = System.IO.Path.Combine(Server.MapPath("~/Images/"));
            
            while (!System.IO.File.Exists(resultFilePath))
            {
                // TODO(allenxie): Fancy work on filename processing.
                /*
                OnsetDetector od = new OnsetDetector(filename);
                float[] notes = od.GenerateNotes();
                ToPDF.ScoreCreation(imgPath, resultFilePath, notes, notes.Length, (nameElements.Length >= 1) ? nameElements[0] : "UndefinedChordName", (nameElements.Length >= 2) ? nameElements[1] : "Anonymous", (nameElements.Length >= 3) ? nameElements[2] : "Unpredictable Nature");
                */
            }
            Response.Write("Return file successfully!");
            Response.End();
        }
    }
}