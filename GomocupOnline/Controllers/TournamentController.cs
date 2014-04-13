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

        public ActionResult Schedule()
        {
            return View();
        }

        public ActionResult Openings()
        {
            return View();
        }

        public ActionResult OpeningsImg()
        {
            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "openings.png");
            Response.ContentType = "image/png";

            return DownloadBinary(path);
        }

        public FileResult DownloadBinary(string path)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = Path.GetFileName(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult OpeningsDownload()
        {
            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "openings.txt");

            return DownloadBinary(path);
        }

        public ActionResult Results()
        {
            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "Tournaments");

            if (!Directory.Exists(path))
                return View(new TournamentModel[0]);

            string[] dirs = Directory.GetDirectories(path);

            TournamentModel[] model = dirs.Select(d => new TournamentModel(d)).ToArray();
            return View(model);
        }

        public ActionResult MatchesByEngine(string tournament, string engine)
        {
            if (tournament == null || tournament.Contains("."))
                throw new ArgumentException();

            GomokuMatchInfoModel[] matches = GetMatchesModelByTournament(tournament);

            matches = matches
                .Where(m => m.Player1 == engine || m.Player2 == engine)
                .ToArray();

            TournamentMatch model = new TournamentMatch()
            {
                Matches = matches,
                Tournament = tournament,
            };

            return View("Matches", model);
        }

        public ActionResult Matches(string tournament)
        {
            if (tournament == null || tournament.Contains("."))
                throw new ArgumentException();

            GomokuMatchInfoModel[] matches = GetMatchesModelByTournament(tournament);

            TournamentMatch model = new TournamentMatch()
            {
                Matches = matches,
                Tournament = tournament,
            };

            return View(model);
        }

        private static GomokuMatchModel[] GetMatchesByTournament(string tournament)
        {
            SecurityCheckPath(tournament);

            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "Tournaments");
            path = Path.Combine(path, tournament);

            string[] files = Directory.GetFiles(path, "*.psq");

            GomokuMatchModel[] model = new GomokuMatchModel[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                model[i] = new GomokuMatchModel(files[i]);
            }
            return model;
        }

        private static GomokuMatchInfoModel[] GetMatchesModelByTournament(string tournament)
        {
            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "Tournaments");
            path = Path.Combine(path, tournament);

            string[] files = Directory.GetFiles(path, "*.psq");

            GomokuMatchInfoModel[] model = new GomokuMatchInfoModel[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                GomokuMatchModel matchModel = new GomokuMatchModel(file);


                model[i] = new GomokuMatchInfoModel()
                {
                    FileName = tournament + "\\" + Path.GetFileName(file),
                    LastChange = new FileInfo(file).LastWriteTime,
                    Player1 = matchModel.Player1,
                    Player2 = matchModel.Player2,
                    Moves = matchModel.Moves.Length,
                    Result = matchModel.GetMatchResult(),
                };
            }
            return model;
        }

        public ActionResult Compare(string engine, string tournamentMatch)
        {
            SecurityCheckPath(tournamentMatch);

            string tournament = Path.GetDirectoryName(tournamentMatch);

            string path = GetMatchPath(tournamentMatch);
            GomokuMatchModel reference = new GomokuMatchModel(path);
            reference.FileName = tournamentMatch.Replace("\\", "\\\\");


            GomokuMatchModel[] sameOpening = GetMatchesWithSameOpening(reference, tournament, engine);

            GomokuCompareModel model = new GomokuCompareModel()
            {
                Matches = sameOpening,
                Reference = reference,
                Player = engine,
            };

            return View(model);
        }

        private GomokuMatchModel[] GetMatchesWithSameOpening(GomokuMatchModel reference, string tournament, string engine)
        {
            int minMoves = 3;

            if (reference.Moves.Length < minMoves)
                return new GomokuMatchModel[0];


            GomokuMatchModel[] all = GetMatchesByTournament(tournament);

            List<GomokuMatchModel> selection = new List<GomokuMatchModel>();

            foreach (GomokuMatchModel item in all)
            {
                if (item.Moves.Length < minMoves)
                    continue;

                if (reference.FileName.EndsWith(item.FileName))
                    continue;//self

                int maxMoves = Math.Min(item.Moves.Length, reference.Moves.Length);

                //if( reference.Player1 == player)
                //{
                //    if (item.Player1 == reference.Player1)
                //        continue;//self
                //}
                //else if (reference.Player2 == player)
                //{
                //    if (item.Player2 == reference.Player1)
                //        continue;//self
                //}

                bool founded = true;

                for (int i = 0; i < maxMoves; i++)
                {
                    if ((item.Moves[i].X != reference.Moves[i].X) || (item.Moves[i].Y != reference.Moves[i].Y))
                    {
                        if (i % 2 == 1 && engine == reference.Player2 && i > minMoves)
                        {
                            founded = true;
                            break;
                        }

                        if (i % 2 == 0 && engine == reference.Player1 && i > minMoves)
                        {
                            founded = true;
                            break;
                        }
                        founded = false;
                        break;
                    }
                }
                if (founded)
                {
                    selection.Add(item);
                }
            }

            return selection.ToArray();
        }

        public ActionResult Match(string tournamentMatch)
        {
            SecurityCheckPath(tournamentMatch);

            string path = GetMatchPath(tournamentMatch);
            GomokuMatchModel model = new GomokuMatchModel(path);
            model.FileName = tournamentMatch.Replace("\\", "\\\\");
            return View(model);
        }

        public static void SecurityCheckPath(string tournamentMatch)
        {
            if (tournamentMatch == null || tournamentMatch.Contains("..") || tournamentMatch.Contains(":"))
                throw new ArgumentException();
        }

        public JsonResult MatchJSON(string tournamentMatch)
        {
            SecurityCheckPath(tournamentMatch);

            string path = GetMatchPath(tournamentMatch);
            GomokuMatchModel model = new GomokuMatchModel(path);
            model.FileName = tournamentMatch.Replace("\\", "\\\"");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public static string GetMatchPath(string tournamentMatch)
        {
            SecurityCheckPath(tournamentMatch);

            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            path = Path.Combine(path, "Tournaments");
            path += "\\" + tournamentMatch;
            return path;
        }

    }
}