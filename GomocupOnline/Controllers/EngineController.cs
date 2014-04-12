using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GomocupOnline.Controllers
{
    public class EngineController : Controller
    {
        //
        // GET: /Engine/
        public ActionResult Download(string engine)
        {
            TournamentController.SecurityCheckPath(engine);

            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "AI");

            string[] possibleformat = new string[]
            {
                engine + ".zip",
                "pbrain-" + engine + ".zip",
                 engine + ".exe",
                "pbrain-" + engine + ".exe",
            };

            foreach (string item in possibleformat)
            {
                string fullpath = Path.Combine(path, item);
                if (!System.IO.File.Exists(fullpath))
                    continue;

                //counter
                string counterFile = fullpath + ".counter";
                int downloadedCount = 0;
                if (System.IO.File.Exists(counterFile))
                {
                    string strCounter = System.IO.File.ReadAllText(counterFile);
                    int.TryParse(strCounter, out downloadedCount);
                }
                downloadedCount++;
                System.IO.File.WriteAllText(counterFile, downloadedCount.ToString());

                return DownloadBinary(fullpath);
            }

            return new HttpStatusCodeResult(404);
        }

        public FileResult DownloadBinary(string path)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = Path.GetFileName(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
	}
}