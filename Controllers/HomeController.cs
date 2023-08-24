using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TPBApi.Models;
using TPBApi.Classes;
using TPBApi.Helpers;
using System.Runtime.ConstrainedExecution;
using System.Net;
using Newtonsoft.Json;
using HtmlAgilityPack;
using MonoTorrent;
using MonoTorrent.Client;
using System.Reflection;

namespace TPBApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Downloads()
        {
            return View();
        }
        public IActionResult _download()
        {
            return PartialView("_download");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public List<TPBJSON> QueryTPBApi([FromQuery]string query)
        {
            var retorno = new List<TPBJSON>();
            HttpClient client = new HttpClient();

            var response = client.GetAsync("https://apibay.org/q.php?q=" + query).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonobject = JsonConvert.DeserializeObject<List<TPBJSON>>(json);
                return jsonobject;
            }

            return null;
        }


        [HttpPost]
        public IActionResult DownloadTorrent([FromQuery]int id, bool filmeSerie) 
        {
            //Parseando a page do TPB e pegando o link magnet
            var html = @"https://thepiratebay10.org/torrent/"+ id;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var xPath = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"details\"]/div[3]/div[1]/a[1]");
            var magnetLink = xPath.Select(x => x.Attributes["href"].Value).FirstOrDefault();

            //Baixando o torrent
            if(magnetLink != null)
            {
                DownloaderTorrent helper = new DownloaderTorrent();
                var path = filmeSerie == true ? "D:\\Filmes & Series\\FIlmes" : "D:\\Filmes & Series\\Series";
                helper.TorrentDownload(magnetLink, path);
            }
            return RedirectToAction("Downloads");
        }

        [HttpGet]
        public List<DownloadStatus> GetDownloadStatuses()
        {
            DownloaderTorrent downloadStatus = new DownloaderTorrent();
            return downloadStatus.getDownloads();
        }

        [HttpGet]
        public void ScanPlexLibrary()
        {
            HttpClient client = new HttpClient();

            var response = client.GetAsync("http://192.168.0.5:32400/library/sections/all/refresh?X-Plex-Token=8-gocuzH-FQeHpq3YsEa").Result;
        }
    }
}