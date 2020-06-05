using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VideoTextApp.Models;
using VideoTextApp.Views;
using AnalyzeVideos;
using System.Net;

namespace VideoTextApp.Controllers
{
    public class HomeController : Controller
    {
        public IWebHostEnvironment HostingEnvironment { get; }

        private readonly ILogger<HomeController> _logger;

        

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            return RedirectToAction("GetVideo", "Home");
        }

        [HttpGet]
        public ActionResult GetVideo()
        {
            return View();
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> GetVideo(IFormFile uploadFile)
        {
            if(ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    string extension = Path.GetExtension(uploadFile.FileName);
                    string newfileName = "InputVideo" + extension;

                    string uploadsFolder = @"";
                    string filePath = Path.Combine(uploadsFolder, newfileName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    uploadFile.CopyTo(fs);
                    fs.Close();

                    AnalyzeVideos.Program ap = new AnalyzeVideos.Program() ;
                    

                    ViewData["message"] = $"{uploadFile.Length} bytes uploaded successfully!";

                    await AnalyzeVideos.Program.Main();
                    
                    string textToDisplay = "";
                    textToDisplay = AnalyzeVideos.Program.FetchData();
                    ViewData["DisplayText"] = textToDisplay;

                    return View("GetVideo");
                }

            }

            return View("GetVideo");
        }

    }
}
