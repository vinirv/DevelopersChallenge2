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
using Common;
using Business;

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
            var ofxTexts = new List<string>();
            foreach (var file in files)
            {
                var ofxText = Functions.IFormFileToString(file);
                ofxTexts.Add(ofxText);
            }

            ViewBag.uniqueTransactions = new OfxProcessBusiness().GetUniqueTransactionsOfFiles(ofxTexts);

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
