using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SalesContractApplication.API;
using SalesContractApplication.Models;

namespace SalesContractApplication.Controllers
{
    public class WIPReportController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APIServices _apiServices;
        private readonly string? _apiLink;
        private readonly ILogger<HomeController> _logger;
        public WIPReportController(IConfiguration configuration, APIServices apiServices, ILogger<HomeController> logger)
        {

            _configuration = configuration;
            _apiServices = apiServices;
            _apiLink = configuration.GetValue<string>("AppSettings:API_Link");
            _logger = logger;
        }

        [HttpGet]
        public async Task<JsonResult> GetWipReport()
        {
            string? token = HttpContext.Session.GetString("Token");

            if (token == null)
            {
                return Json(null);
            }

            return Json(await GetWipReportHandle(token));
        }

        [HttpGet]
        public async Task<JsonResult> GetWipReportByCustOrder(string custOrder)
        {
            string? token = HttpContext.Session.GetString("Token");

            if (token == null)
            {
                return Json(null);
            }

            return Json(await GetWipReportHandle(token, custOrder));
        }

        private async Task<ApiResponseModel> GetWipReportHandle(string token, string custOrder = "")
        {
            string jsonData = "{}";
            if (!string.IsNullOrEmpty(custOrder))
            {
                var parameter = new
                {
                    customer_order_number = custOrder
                };
                jsonData = JsonConvert.SerializeObject(parameter);
            }
            string url = $"{_apiLink}/get-wip-report";
            return await _apiServices.SendPostRequest(url, jsonData, token);
        }

        [HttpPost]
        public async Task<JsonResult> GetWipReport_detail(string group, string customer, string orderType, string custOrder, string orderID, string LOP, string currentStep)
        {
            string? token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(null);
            }
            string url = $"{_apiLink}/get-wip-detail";

            var parameter = new
            {
                customer_group = group,
                customer_code = customer,
                order_type = orderType,
                customer_order_number = custOrder,
                order_id = orderID,
                lop = LOP,
                pro_step = currentStep
            };
            string jsonData = JsonConvert.SerializeObject(parameter);
            Models.ApiResponseModel response = await _apiServices.SendPostRequest(url, jsonData, token);
            return Json(response);
        }


        public async Task<IActionResult> Index()
        {
            string? token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}