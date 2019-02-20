using System.Web;
using System.Web.Mvc;
using PDF;
using OnsetDetection;
using Training;

namespace MrChorder.Controllers
{
    public class ChordController : Controller
    {
        private static string filename;
        private static string musicname;

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
                string path = System.IO.Path.Combine(Server.MapPath("~\\Upload\\"), System.IO.Path.GetFileName(file.FileName));
                filename = path;
                file.SaveAs(path);
            }
            musicname = filename.Substring(filename.LastIndexOf('\\') + 1, filename.Length - filename.LastIndexOf('\\') - 5);
            // Delete previously generated file.
            string genPath = System.IO.Path.Combine(Server.MapPath("~\\Generate\\"), System.IO.Path.GetFileName("AnalyseResult.pdf"));
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
            string resultFilePath = System.IO.Path.Combine(Server.MapPath("~\\Generate\\"), System.IO.Path.GetFileName("AnalyseResult.pdf"));
            
            while (!System.IO.File.Exists(resultFilePath))
            {
                // TODO(allenxie): Fancy work on filename processing.
                double[][] inputs = { };
                int[] outputs = { };
                LearningModel svm = new LearningModel(inputs, outputs); // Pre-trained
                OnsetDetector od = new OnsetDetector(filename, svm);
                double[][] music = od.GenerateNotes();
                for (int i = 0; i < music.Length; i++)
                {
                    music[i][0] = music[i][0] + 1; // Sync Training module and ToPDF module
                }
                ToPDF PDFGenerator = new ToPDF(resultFilePath, music, music.Length, musicname);
            }
            Response.Write("Return file successfully!");
            Response.End();
        }
    }
}