using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Mir.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewData["Teams1"] = GetTeamList();
            return View();
        }

        private static string GetTeamName(string TeamID)
        {
            string teamName = "";

            List<string[]> teamsCSV = ReadFile();

            foreach (string[] teamData in teamsCSV)
            {
                if (teamData[0] == TeamID)
                {
                    return teamData[1];
                }
            }

            //teamName = from x in teamsCSV
            //           where x.Team_Id = TeamID
            //           select Team_Name;

 
            return teamName;
        }

        private static List<SelectListItem> GetTeamList()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://pwp.apphb.com/api/values/getteamlist");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var teams=reader.ReadToEnd();
            var teamslist = teams.Split(',');

            

            var numbers = (from p in teamslist.Take(teamslist.Count())
                           select new SelectListItem
                           {
                               Text = GetTeamName(p.ToString().TrimStart().TrimEnd().Replace("[","").Replace("\"","").Replace("]","")),
                               Value = p.ToString().TrimStart().TrimEnd().Replace("[", "").Replace("\"", "").Replace("]", "")
                           });
            return numbers.ToList();
        }

        private static List<SelectListItem> GetOpposingTeamList(string team)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://pwp.apphb.com/api/values/getopposingteamlist?team=" + team.Replace("\"", ""));
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var teams = reader.ReadToEnd();
            var teamslist = teams.Split(',');


            var numbers = (from p in teamslist.Take(teamslist.Count())
                           select new SelectListItem
                           {
                               Text = GetTeamName(p.ToString().TrimStart().TrimEnd().Replace("[", "").Replace("\"", "").Replace("]", "")),
                               Value = p.ToString().TrimStart().TrimEnd().Replace("[", "").Replace("\"", "").Replace("]", "")
                           });
            return numbers.ToList();
        }

        private static List<string> GetPreds(string team1, string team2)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://pwp.apphb.com/api/values/?year=2017&team1=" + team1.Replace("\"", "") + "&team2=" + team2.Replace("\"", ""));
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var preds = reader.ReadToEnd().Replace("[","").Replace("]","");
            var predslist = preds.Split(',');


            string[] returnpreds = new string[3];
            //for (int i=0; i<=2;i++)
            //{
            //    returnpreds[i] = predslist[i];
            //}
            if (predslist[0] == "1.0")
            {
                returnpreds[0] = GetTeamName(team1.Replace("\"", "")) + " is the winner!";
            }
            else
            {
                returnpreds[0] = GetTeamName(team2.Replace("\"", "")) + " is the winner!";
            }

            return returnpreds.ToList();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetListViaJson()
        {
            return Json(GetTeamList(), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetOpposingListViaJson(string team)
        {
            return Json(GetOpposingTeamList(team), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPredsViaJson(string team1, string team2)
        {
            return Json(GetPreds(team1, team2), JsonRequestBehavior.AllowGet);
        }

        private static List<string[]> ReadFile()
        {
            string sFileContents = "";

            var url = "http://pwp.apphb.com//teams.csv";
            //var url = "http://localhost:60029//teams.csv";

            WebClient wc = new WebClient();

            using (StreamReader oStreamReader = new StreamReader(wc.OpenRead(url)))
            {
                sFileContents = oStreamReader.ReadToEnd();
            }

            List<string[]> oCsvList = new List<string[]>();

            string[] sFileLines = sFileContents.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string sFileLine in sFileLines)
            {
                oCsvList.Add(sFileLine.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            }

            return oCsvList;
        }
    }
}