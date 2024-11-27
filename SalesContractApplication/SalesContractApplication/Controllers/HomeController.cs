using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesContractApplication.API;
using SalesContractApplication.Models;

namespace SalesContractApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly APIServices _svc;
        private readonly IConfiguration _configuration;

        private readonly ILogger<HomeController> _logger;
        private readonly string? _apiLink;

        public HomeController(APIServices svc, IConfiguration configuration, ILogger<HomeController> logger)
        {
            _svc = svc;
            _configuration = configuration;
            _logger = logger;
            _apiLink = configuration.GetValue<string>("AppSettings:API_Link");
        }


        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");

            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> GetSaleDocummentlDetaiView(int id)
        {
            ViewBag.Id = id;
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }
            string url = $"{_apiLink}/get-sales-document-lines";
            var parameter = new
            {
                document_id = id
            };
            string jsonData = JsonConvert.SerializeObject(parameter);
            var response = await _svc.SendPostRequest(url, jsonData, token);

            var model = new SalesDocumentViewModel();
            var list = JsonConvert.DeserializeObject<List<SalesDocumentLineModel>>(response.Data);

            model.SalesDocumentLines = list == null ? new List<SalesDocumentLineModel>() : list;
            return list != null ? PartialView("_partial/sale_document_detail_table", model) : Content("<html><body>No matching records found</body></html>", "text/html");
        }

        [HttpPost]
        public async Task<JsonResult> GetSaleDocummentlList(string from_date, string to_date, string filters)
        {
            var token = HttpContext.Session.GetString("Token");
            _logger.LogInformation($"Request received with from_date={from_date}, to_date={to_date}, filters={filters}, token={token}");
            if (token == null)
            {
                return Json(null);
            }
            string url = $"{_apiLink}/get-sales-documents";
            var parameter = new
            {
                from_date,
                to_date,
                filter_text = filters
            };
            string jsonData = JsonConvert.SerializeObject(parameter);
            var response = await _svc.SendPostRequest(url, jsonData, token);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteSaleDocumment(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(null);
            }
            string url = $"{_apiLink}/cancel-sales-document";

            var parameter = new
            {
                document_id = id
            };
            string jsonData = JsonConvert.SerializeObject(parameter);
            var response = await _svc.SendPostRequest(url, jsonData, token);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteSaleDocumentLine(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(null);
            }
            string url = $"{_apiLink}/cancel-sales-document-lines";
            var parameters = new List<dynamic> { new
            {
                line_id = id,
            }};
            string jsonData = JsonConvert.SerializeObject(parameters);
            var response = await _svc.SendPostRequest(url, jsonData, token);
            return Json(response);
        }
    }
}