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
using System.IO;

namespace TPBApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
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
            var tpbApiurl = _configuration.GetValue<string>("TPBAPIUrl");
            var response = client.GetAsync(tpbApiurl +"/q.php?q=" + query).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonobject = JsonConvert.DeserializeObject<List<TPBJSON>>(json);
                return jsonobject;
            }

            return null;
        }


        [HttpPost]
        public Response<string> DownloadTorrent([FromQuery]int id, bool filmeSerie, string SerieName) 
        {
            var tpbUrl = _configuration.GetValue<string>("TPBUrl");
            //Parseando a page do TPB e pegando o link magnet
            var html = tpbUrl + id;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var xPath = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"details\"]/div[3]/div[1]/a[1]");
            var magnetLink = xPath.Select(x => x.Attributes["href"].Value).FirstOrDefault();

            //Baixando o torrent
            if(magnetLink != null)
            {

                var FilmesPath = _configuration.GetValue<string>("FilmesPath");
                var SeriesPath = _configuration.GetValue<string>("SeriesPath");
                DownloaderTorrent helper = new DownloaderTorrent();
                var path = filmeSerie == true ? FilmesPath : SeriesPath+"\\"+SerieName;
                var response = helper.TorrentDownload(magnetLink, path);
                if(response.Result.Message == "OK")
                {
                    return new Response<string> { Message= response.Result.Message.ToString() };
                }
                else
                {
                    return new Response<string>() { Message = response.Result.Message.ToString(), Data = response.Result.Data.ToString() };
                }

            }
            return new Response<string> { Message = "Magnet Link inválido" };
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
            var PlexIPAddress = _configuration.GetValue<string>("PlexIPAddress");
            var PlexServerToken = _configuration.GetValue<string>("PlexServerToken");

            var response = client.GetAsync("http://"+ PlexIPAddress + ":32400/library/sections/all/refresh?X-Plex-Token="+ PlexServerToken).Result;
        }

        [HttpPost]
        public void PauseDownload(int index)
        {
            DownloaderTorrent downloader = new DownloaderTorrent();
            downloader.pauseDownload(index);
        }

        [HttpPost]
        public void RetomarDownload(int index)
        {
            DownloaderTorrent downloader = new DownloaderTorrent();
            downloader.retomarDownload(index);
        }

        [HttpGet]
        public List<string> getSeriesStored()
        {
            var seriesPath = _configuration.GetValue<string>("SeriesPath");

            if (Directory.Exists(seriesPath))
            {
                string[] subpastas = Directory.GetDirectories(seriesPath);
                return subpastas.Select(x => x.Replace(seriesPath,"").Replace("\\","")).ToList();
            }
            return null;
        }
    }
}