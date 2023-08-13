using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TPBApi.Models;
using TPBApi.Classes;
using System.Runtime.ConstrainedExecution;
using System.Net;
using Newtonsoft.Json;
using HtmlAgilityPack;

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

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
        public void DownloadTorrent([FromQuery]int id) 
        {
            //Parseando a page do TPB
            var html = @"https://thepiratebay10.org/torrent/"+ id;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            var xPath = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"details\"]/div[3]/div[1]/a[1]");
            var cu = xPath.Select(x => x.Attributes["href"].Value).FirstOrDefault();

        }
    }
}