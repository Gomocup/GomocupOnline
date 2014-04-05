using GomocupOnline.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GomocupOnline.Controllers
{
    public class TournamentController : Controller
    {
        //
        // GET: /Tournament/
        public ActionResult Tournaments()
        {
            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "Tournaments");

            if( !Directory.Exists(path))
                return View(new TournamentModel[0]);

            string[] dirs = Directory.GetDirectories(path);

            TournamentModel[] model = dirs.Select(d => new TournamentModel(d)).ToArray();
            return View(model);
        }

        public ActionResult Matches(string tournament)
        {
            if (tournament == null || tournament.Contains("."))
                throw new ArgumentException();

            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "Tournaments");
            path = Path.Combine(path, tournament);

            string[] files = Directory.GetFiles(path, "*.psq");

            GomokuMatchInfoModel[] model = files.Select(f =>
                new GomokuMatchInfoModel()
                {
                    FileName = tournament + "\\" + Path.GetFileName(f),
                    LastChange = new FileInfo(f).LastWriteTime,
                }
                ).ToArray();

            return View(model);
        }

        public ActionResult Match(string tournamentMatch)
        {
            if (tournamentMatch == null || tournamentMatch.Contains(".."))
                throw new ArgumentException();

            string path = GetMatchPath(tournamentMatch);
            GomokuMatchModel model = new GomokuMatchModel(path);
            model.FileName = tournamentMatch.Replace("\\","\\\\");
            return View(model);
        }

        public JsonResult MatchJSON(string tournamentMatch)
        {
            if (tournamentMatch == null || tournamentMatch.Contains(".."))
                throw new ArgumentException();

            string path = GetMatchPath(tournamentMatch);
            GomokuMatchModel model = new GomokuMatchModel(path);
            model.FileName = tournamentMatch.Replace("\\", "\\\"");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public static string GetMatchPath(string tournamentMatch)
        {
            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "Tournaments");
            path += "\\" + tournamentMatch;
            return path;
        }

    }
}