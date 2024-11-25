using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesContractApplication.API;
using SalesContractApplication.Models;

namespace SalesContractApplication.Controllers
{
    public class WIPReportController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APIServices _apiServices;
        private readonly string? _apiLink;

        public WIPReportController(IConfiguration configuration, APIServices apiServices)
        {

            _configuration = configuration;
            _apiServices = apiServices;
            _apiLink = configuration.GetValue<string>("AppSettings:API_Link");
        }

        public async Task<IActionResult> Index()
        {
            var model = new WIPReportViewModel();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Get data
            var url = $"{_apiLink}/get-wip-report";
            var apiResponse = await _apiServices.SendGetRequest(url, "", token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<WIPReportModel>>(apiResponse.Data.ToString());
                if (data != null)
                {
                    model.WIPReportList = data;
                }
            }

            return View(model);
        }
    }
}