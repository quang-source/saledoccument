using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using SalesContractApplication.API;
using SalesContractApplication.Models;

namespace SalesContractApplication.Controllers
{
    public class SalesHistoryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APIServices _apiServices;
        private readonly string? _apiLink;

        public SalesHistoryController(IConfiguration configuration, APIServices apiServices)
        {

            _configuration = configuration;
            _apiServices = apiServices;
            _apiLink = configuration.GetValue<string>("AppSettings:API_Link");
        }

        public async Task<IActionResult> Index()
        {
            var model = new SalesHistoryViewModel();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Get Customers
            string url = $"{_apiLink}/get-customers";
            var apiResponse = await _apiServices.SendPostRequest(url, "{\"include_MRM\": true, \"include_divisions\": false}", token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());
                if (data != null)
                {
                    model.Customers = data.Select(item => new SelectListItem
                    {
                        Text = $"{item.customer_id.ToString().Split('.')[0]} - {item.customer_code} - {item.customer_name}",
                        Value = item.customer_id.ToString().Split('.')[0]
                    }).ToList();
                }
            }

            // Get Brands
            url = $"{_apiLink}/get-brands";
            apiResponse = await _apiServices.SendGetRequest(url, "", token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());
                if (data != null)
                {
                    data = data.OrderBy(item =>
                    {
                        string brandCode = item.brand_code.ToString();
                        if (!string.IsNullOrEmpty(brandCode))
                            return char.IsLetter(brandCode[0]) ? 0 : 1;
                        else return 0;
                    })
                    .ThenBy(item => item.brand_code.ToString()) // Sort remaining items alphabetically
                    .ToList();

                    model.BrandNames = data.Select(item => new SelectListItem
                    {
                        Text = item.brand_name.ToString(),
                        Value = item.brand_name.ToString()
                    }).DistinctBy(item => item.Value).ToList();
                    model.BrandCodes = data.Select(item => new SelectListItem
                    {
                        Text = item.brand_code.ToString(),
                        Value = item.brand_code.ToString()
                    }).DistinctBy(item => item.Value).ToList();
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDivisions(int customerID)
        {
            string url = $"{_apiLink}/get-divisions-by-customer-id";

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(new { Error = "Unauthorized!" });
            }

            var requestData = "{\"customer_id\":" + customerID + "}";
            var apiResponse = await _apiServices.SendPostRequest(url, requestData, token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());
                var result = data.Select(item => new SelectListItem
                {
                    Text = item.division_code.ToString(),
                    Value = item.division_code.ToString()
                }).ToList();

                return Json(result);
            }

            return Json(new { Error = $"Failed to get divisions of customer {customerID}!" });
        }

        [HttpPost]
        public async Task<IActionResult> GetOldSales(bool filterByDwno, string strings)
        {
            string url = $"{_apiLink}/get-sales-history-old";

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(new { Error = "Unauthorized!" });
            }

            var stringsList = JsonConvert.DeserializeObject<List<string>>(strings);
            var parameter = new
            {
                filter_by_dwno = filterByDwno,
                strings = stringsList
            };

            var jsonData = JsonConvert.SerializeObject(parameter);
            var apiResponse = await _apiServices.SendPostRequest(url, jsonData, token);

            if (apiResponse.Success)
            {
                var model = new SalesHistoryViewModel();
                model.OldSalesList = JsonConvert.DeserializeObject<List<OldSalesModel>>(apiResponse.Data.ToString());

                return PartialView("_partial/sales_history_table", model);
            }

            return Json(new { Error = $"Failed to get sales history: " + apiResponse.Error });
        }

        [HttpPost]
        public async Task<IActionResult> GetInvoiceData(string columnToFind, string columnToFind2, string customerFilter, string dateFilter)
        {
            string url = $"{_apiLink}/get-invoice-data";

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(new { Error = "Unauthorized!" });
            }

            var parameter = new
            {
                column_to_find = columnToFind,
                column_to_find_2 = columnToFind2,
                customer_filter = customerFilter,
                date_filter = dateFilter
            };

            var jsonData = JsonConvert.SerializeObject(parameter);
            var apiResponse = await _apiServices.SendPostRequest(url, jsonData, token);

            if (apiResponse.Success)
            {
                var model = new SalesHistoryViewModel();
                model.InvoiceDataList = JsonConvert.DeserializeObject<List<InvoiceDataModel>>(apiResponse.Data.ToString());

                return PartialView("_partial/invoice_data_table", model);
            }

            return Json(new { Error = $"Failed to get invoice data: " + apiResponse.Error });
        }

        [HttpPost]
        public JsonResult ExportToExcel(string headers, string data)
        {
            var headerList = JsonConvert.DeserializeObject<List<string>>(headers);
            var dataList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(data);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Invoices data");

                // Add headers to the first row
                for (int i = 0; i < headerList.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headerList[i];
                }

                // Add invoice data starting from the second row
                for (int row = 0; row < dataList.Count; row++)
                {
                    var rowData = dataList[row];
                    int col = 1;
                    foreach (var header in headerList)
                    {
                        var cellValue = rowData.ContainsKey(header) ? rowData[header] : "";

                        if (decimal.TryParse(cellValue, out decimal numericValue))
                        {
                            worksheet.Cells[row + 2, col++].Value = numericValue;
                        }
                        else
                        {
                            worksheet.Cells[row + 2, col++].Value = cellValue;
                        }
                    }
                }

                var excelBytes = package.GetAsByteArray();

                return Json(new { success = true, fileData = Convert.ToBase64String(excelBytes) });
            }
        }
    }
}