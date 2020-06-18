using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NiboWebApp.Models;

namespace NiboWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(List<IFormFile> files)
        {
            foreach (var file in files)
            {

                var result = new StringBuilder();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        result.AppendLine(reader.ReadLine());
                }

                var ofxText = result.ToString();

                ProcessOfxText(ofxText);
            }

            return Ok();
        }

        private void ProcessOfxText(string ofxText)
        {
            ofxText = ofxText.Replace("\r", "").Replace("\n", "");
            var strList = ofxText.Split("<STMTTRN>").ToList();
            strList.RemoveAt(0);

            foreach (var str in strList)
            {
                var pos = str.IndexOf("</STMTTRN>");
                var s = str.Substring(0, pos);

                var trn = new STMTTRN();

                trn.TRNTYPE = GetContent(s, "TRNTYPE");
                var strDtPosted = GetContent(s, "DTPOSTED");
                strDtPosted = strDtPosted.Substring(0, strDtPosted.IndexOf("["));

                var newDate = DateTime.ParseExact(strDtPosted,
                                  "yyyyMMddHHmmss",
                                   CultureInfo.InvariantCulture);

                trn.DTPOSTED = newDate;



                var format = new NumberFormatInfo
                {
                    NegativeSign = "-",
                    NumberDecimalSeparator = "."
                };

                trn.TRNAMT = Convert.ToDecimal(GetContent(s, "TRNAMT"), format);

                trn.MEMO = GetContent(s, "MEMO");

                Console.WriteLine($"{trn.TRNTYPE} || {trn.DTPOSTED} || {trn.TRNAMT} || {trn.MEMO}");
            }

        }

        private static string GetContent(string text, string elSearch)
        {
            elSearch = $"<{elSearch}>";
            var posIniCont = text.IndexOf(elSearch) + elSearch.Length;
            var posFimCont = text.IndexOf("<", posIniCont);

            if (posFimCont == -1) posFimCont = text.Length;

            return text.Substring(posIniCont, posFimCont - posIniCont).Trim();
        }

        private class STMTTRN
        {
            public string TRNTYPE { get; set; }
            public DateTime DTPOSTED { get; set; }
            public decimal TRNAMT { get; set; }
            public string MEMO { get; set; }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
