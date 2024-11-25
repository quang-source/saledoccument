using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesContractApplication.API;
using SalesContractApplication.Models;
using OfficeOpenXml;
using System.Security.Policy;
using SalesContractApplication.Helper;
using System.Reflection;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using Newtonsoft.Json.Linq;

namespace SalesContractApplication.Controllers
{
    public class SalesDocumentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APIServices _apiServices;
        private readonly string _apiLink;

        public SalesDocumentController(IConfiguration configuration, APIServices apiServices)
        {
            _configuration = configuration;
            _apiServices = apiServices;
            _apiLink = configuration.GetValue<string>("AppSettings:API_Link");
        }

        [HttpGet]
        public async Task<IActionResult> CreateEdit(int? id)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var model = new SalesDocumentViewModel();
            bool isEdit = (id != null && id > 0);
            ViewBag.IsEdit = isEdit;

            if (isEdit)
            {
                var response = await GetSalesDocumentViewModel(id ?? -11, token);
                if (response.Success)
                {
                    var responseModel = JsonConvert.DeserializeObject<SalesDocumentViewModel>(response.Data);
                    model.SalesDocument = responseModel.SalesDocument;
                    model.SalesDocumentLines = responseModel.SalesDocumentLines;
                }
                else
                {
                    id = null;
                    return RedirectToAction("CreateEdit", "SalesDocument", new { id });
                }
            }

            return View(await InitForm(model, token));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEdit(SalesDocumentViewModel model, int? id)
        {
            ViewBag.PostErrors = new List<string>();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            bool isEdit = id != null && id > 0;
            ViewBag.IsEdit = isEdit;
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                return View(await InitForm(model, token));
            }

            model.SalesDocument.CustomerId = (int)model.SalesDocument.CustomerId;

            if (isEdit)
            {
                var apiResponse = await GetSalesDocumentViewModel(id ?? -113, token);
                bool documentDone = false;
                bool linesDone = false;

                if (apiResponse.Success)
                {
                    var currentData = JsonConvert.DeserializeObject<SalesDocumentViewModel>(apiResponse.Data);

                    // Check document
                    var documentUpdates = GetEditedFields(currentData?.SalesDocument, model.SalesDocument);
                    if (documentUpdates.Count > 0)
                    {
                        string url = $"{_apiLink}/edit-sales-document";
                        var parameter = new
                        {
                            document_id = id,
                            data = documentUpdates
                        };
                        var jsonDataDocument = JsonConvert.SerializeObject(parameter);
                        var responseDocument = await _apiServices.SendPostRequest(url, jsonDataDocument, token);

                        if (responseDocument.Success)
                        {
                            documentDone = true;
                        }
                        else
                        {
                            ViewBag.PostErrors.Add(responseDocument.Error);
                        }
                    }
                    else
                    {
                        documentDone = true;
                    }

                    // Check lines
                    if (model.SalesDocumentLines.Count > 0)
                    {
                        var linesUpdates = new List<object>();
                        var responseCreate = new ApiResponseModel { Success = true };
                        var responseEdit = new ApiResponseModel { Success = true };

                        // Edit existing lines
                        var editLines = model.SalesDocumentLines.Where(line => line.SalesDocumentLineId != null && line.SalesDocumentLineId > 0).ToList();
                        if (editLines.Count > 0)
                        {
                            var parameters = new List<object>();
                            foreach (var line in editLines)
                            {
                                SalesDocumentLineModel currentLine = currentData.SalesDocumentLines.FirstOrDefault(l => l.SalesDocumentLineId == line.SalesDocumentLineId);

                                var lineUpdates = GetEditedFields(currentLine, line);
                                if (lineUpdates.Count > 0)
                                {
                                    var parameter = new
                                    {
                                        line_id = line.SalesDocumentLineId,
                                        data = lineUpdates
                                    };
                                    parameters.Add(parameter);
                                }
                            }

                            if (parameters.Count > 0)
                            {
                                var url = $"{_apiLink}/edit-sales-document-lines";
                                var jsonData = JsonConvert.SerializeObject(parameters);
                                responseEdit = await _apiServices.SendPostRequest(url, jsonData, token);
                            }

                            if (!responseEdit.Success)
                            {
                                ViewBag.PostErrors.Add(responseEdit.Error);
                            }
                        }

                        // Create new lines
                        if (responseEdit.Success)
                        {
                            var newLines = model.SalesDocumentLines.Where(line => line.SalesDocumentLineId == null || line.SalesDocumentLineId == 0).ToList();
                            if (newLines.Count > 0)
                            {
                                var url = $"{_apiLink}/create-sales-document-lines";

                                foreach (var line in newLines)
                                {
                                    line.SalesDocumentId = id;
                                }

                                var jsonData = JsonConvert.SerializeObject(newLines);
                                responseCreate = await _apiServices.SendPostRequest(url, jsonData, token);

                                if (!responseCreate.Success)
                                {
                                    ViewBag.PostErrors.Add(responseCreate.Error);
                                }
                            }
                        }

                        if (responseCreate.Success && responseEdit.Success)
                        {
                            linesDone = true;
                        }
                    }
                    else
                    {
                        linesDone = true;
                    }

                    if (documentDone && linesDone)
                    {
                        TempData["Success"] = "Sales document saved successfully!";
                        return Redirect($"/SalesDocument/CreateEdit/{id}");
                    }
                }
                else
                {
                    return NotFound("Sales document no longer exists");
                }
            }
            else
            {
                string url = $"{_apiLink}/create-sales-document";

                model.SalesDocument.Lines = model.SalesDocumentLines;

                string jsonData = JsonConvert.SerializeObject(model.SalesDocument);

                var apiResponse = await _apiServices.SendPostRequest(url, jsonData, token);

                if (apiResponse.Success)
                {
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(apiResponse.Data.ToString());
                    return Redirect($"/SalesDocument/CreateEdit/{data?.sales_document_id}");
                }

                ViewBag.PostErrors.Add(apiResponse.Error);
            }

            return View(await InitForm(model, token));
        }

        [HttpGet]
        public async Task<IActionResult> Duplicate(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var model = new SalesDocumentViewModel();
            ViewBag.IsEdit = false;

            var response = await GetSalesDocumentViewModel(id, token);
            if (response.Success)
            {
                var responseModel = JsonConvert.DeserializeObject<SalesDocumentViewModel>(response.Data);
                model.SalesDocument = responseModel.SalesDocument;
                model.SalesDocumentLines = responseModel.SalesDocumentLines;

                model.SalesDocument.SalesDocumentId = null;
                model.SalesDocument.PurchaseOrderNumber = "";

                foreach (var line in model.SalesDocumentLines)
                {
                    line.SalesDocumentId = null;
                    line.SalesDocumentLineId = null;
                }
            }
            else
            {
                int? iD = null;
                return RedirectToAction("CreateEdit", "SalesDocument", new { iD });
            }

            ViewBag.Duplicating = true;
            return View("CreateEdit", await InitForm(model, token));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duplicate(SalesDocumentViewModel model, int id)
        {
            ViewBag.PostErrors = new List<string>();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.IsEdit = false;

            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                return View(await InitForm(model, token));
            }

            string url = $"{_apiLink}/create-sales-document";

            model.SalesDocument.CustomerId = (int)model.SalesDocument.CustomerId;
            //model.SalesDocument.Version = (int.Parse(model.SalesDocument.Version ?? "0") + 1).ToString();
            model.SalesDocument.ParentId = id.ToString();

            model.SalesDocument.Lines = model.SalesDocumentLines;

            string jsonData = JsonConvert.SerializeObject(model.SalesDocument);

            var apiResponse = await _apiServices.SendPostRequest(url, jsonData, token);

            if (apiResponse.Success)
            {
                dynamic data = JsonConvert.DeserializeObject<dynamic>(apiResponse.Data.ToString());
                return Redirect($"/SalesDocument/CreateEdit/{data?.sales_document_id}");
            }

            ViewBag.PostErrors.Add(apiResponse.Error);
            ViewBag.Duplicating = true;
            return View("CreateEdit", await InitForm(model, token));
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses(float customerID)
        {
            string url = $"{_apiLink}/get-addresses";

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(new { Error = "Unauthorized!" });
            }

            var requestData = "{\"customer_id\":" + customerID + "}";
            var apiResponse = await _apiServices.SendGetRequest(url, requestData, token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());
                var invoiceToAddresses = data.GroupBy(item => item.invoice_to_customer_address_id)
                    .Select(group => new
                    {
                        Text = group.First().invoice_to_address.ToString(),
                        Value = group.First().invoice_to_customer_address_id.ToString()
                    }).ToList();
                var shipToAddresses = data.GroupBy(item => item.ship_to_customer_address_id)
                    .Select(group => new
                    {
                        Text = group.First().ship_to_address.ToString(),
                        Value = group.First().ship_to_customer_address_id.ToString()
                    }).ToList();

                return Json(new
                {
                    InvoiceToAddresses = invoiceToAddresses,
                    ShipToAddresses = shipToAddresses
                });
            }

            return Json(new { InvoiceToAddresses = new List<object>(), ShipToAddresses = new List<object>() });
        }

        [HttpGet]
        public async Task<IActionResult> GetDivision(float customerID)
        {
            string url = $"{_apiLink}/get-division";

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(new { Error = "Unauthorized!" });
            }

            var requestData = "{\"customer_id\":" + customerID + "}";
            var apiResponse = await _apiServices.SendGetRequest(url, requestData, token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());
                var result = data.GroupBy(item => item.invoice_to_address_id)
                    .Select(group => new
                    {
                        Text = group.First().invoice_to_address,
                        Value = group.First().invoice_to_address_id
                    }).ToList();

                return Json(result);
            }

            return Json(new { InvoiceToAddresses = new List<object>(), ShipToAddresses = new List<object>(), RequestData = requestData });
        }

        [HttpPost]
        public IActionResult UploadExcel(IFormFile file)
        {
            int rowCount = int.Parse(Request.Form["rowCount"].ToString());

            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var package = new ExcelPackage(file.OpenReadStream()))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        var salesDocumentLines = new List<SalesDocumentLineModel>();

                        if (worksheet != null)
                        {
                            // Row 1 is header, data start from row 2
                            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                            {
                                var line = new SalesDocumentLineModel
                                {
                                    SalesDocumentLineId = 0,
                                    CustomerReference = worksheet.Cells[row, 1].Text,
                                    ProductIDSize = worksheet.Cells[row, 2].Text,
                                    LOP = worksheet.Cells[row, 3].Text,
                                    Quantity = Convert.ToInt32(worksheet.Cells[row, 4].Value ?? 0),
                                    UOM = worksheet.Cells[row, 5].Text,
                                    UnitPrice = Convert.ToDecimal(worksheet.Cells[row, 6].Value ?? 0.00m),
                                    Currency = worksheet.Cells[row, 7].Text,
                                    OrderType = worksheet.Cells[row, 8].Text,
                                    Stamp = worksheet.Cells[row, 9].Text,
                                    Remark = worksheet.Cells[row, 10].Text,
                                };
                                salesDocumentLines.Add(line);
                            }

                            var model = new SalesDocumentViewModel();
                            model = InitForm(model).GetAwaiter().GetResult();
                            model.SalesDocumentLines = salesDocumentLines;
                            model.FirstLineIndex = rowCount;

                            return PartialView("_partial/sales_document_lines_table", model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "No file uploaded" });
        }

        [HttpPost]
        public IActionResult UpdateStatus(int documentId, int status)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(null);
            }
            string url = $"{_apiLink}/edit-sales-document";

            var data = new List<object>
            {
                new
                {
                    key = "status",
                    value = status
                }
            };

            var parameter = new
            {
                document_id = documentId,
                data = new List<object>
                {
                    new
                    {
                        key = "status",
                        value = status
                    }
                }
            };
            string jsonData = JsonConvert.SerializeObject(parameter);
            var response = _apiServices.SendPostRequest(url, jsonData, token).GetAwaiter().GetResult();
            return Json(response);
        }

        [HttpPost]
        public IActionResult RenderNewSalesDocumentLinesTableRow(int index, float customerId)
        {
            var model = new SalesDocumentViewModel();
            model.SalesDocument = new SalesDocumentModel
            {
                CustomerId = customerId
            };
            model = InitForm(model).GetAwaiter().GetResult();

            return PartialView("_partial/sales_document_lines_table_row", new
            {
                Line = new SalesDocumentLineModel(),
                Index = index,
                model.UOMs,
                model.Currencies,
                model.OrderTypes,
                model.InvoiceToAddresses,
                model.ShipToAddresses
            });
        }

        private async Task<SalesDocumentViewModel> InitForm(SalesDocumentViewModel model, string token = "")
        {
            // Ko cho thì tự lấy
            if (string.IsNullOrEmpty(token))
            {
                token = HttpContext.Session.GetString("Token");
            }

            if (model.SalesDocument == null)
            {
                model.SalesDocument = new SalesDocumentModel
                {
                    Lines = new List<SalesDocumentLineModel>()
                };
            }

            // Get Currencies
            string url = $"{_apiLink}/get-currencies";
            var apiResponse = await _apiServices.SendGetRequest(url, "", token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());
                if (data != null)
                {
                    model.Currencies = data.Select(item => new SelectListItem
                    {
                        Text = item.currency_code,
                        Value = item.currency_code,
                        Selected = item.is_accounting_currency
                    }).ToList();

                    var defaultCurrency = data.FirstOrDefault(item => item.is_accounting_currency) ?? data.First();
                    if (model.SalesDocument.ExchangeRate == -1f) model.SalesDocument.ExchangeRate = defaultCurrency.exchange_rate_to_vnd;
                }
            }

            // Get UOMs
            url = $"{_apiLink}/get-uoms";
            apiResponse = await _apiServices.SendGetRequest(url, "", token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());
                if (data != null)
                {
                    model.UOMs = data.Select(item => new SelectListItem
                    {
                        Text = item.unit_of_measure_code,
                        Value = item.unit_of_measure_code,
                        Selected = item.unit_of_measure_code == "pc"
                    }).ToList();
                }
            }

            // Get Customers
            url = $"{_apiLink}/get-customers";
            apiResponse = await _apiServices.SendPostRequest(url, "{}", token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());
                if (data != null)
                {
                    var strCustomerID = model.SalesDocument.CustomerId?.ToString("F2");
                    var strDivision = model.SalesDocument.Division?.ToString();

                    model.Customers = data.Select(item => new SelectListItem
                    {
                        Text = $"{item.customer_id.ToString().Split('.')[0]} ({item.customer_code}{(item.division_name.ToString().Equals("MAIN") ? "" : $" - {item.division_name}")})",
                        Value = item.customer_id,
                        Selected = item.customer_id.ToString().Equals(strCustomerID)
                    }).ToList();
                    model.Divisions = data.Select(item => new SelectListItem
                    {
                        Text = item.customer_id,
                        Value = item.division_code,
                        Selected = item.customer_id.ToString().Equals(strCustomerID)
                    }).ToList();
                }
            }

            // Get Addresses
            url = $"{_apiLink}/get-addresses";
            string customerIdRequest = "";
            if (model.SalesDocument.CustomerId != null && model.SalesDocument.CustomerId > 0)
            {
                customerIdRequest = "{\"customer_id\":" + model.SalesDocument.CustomerId + "}";
            }
            apiResponse = await _apiServices.SendGetRequest(url, customerIdRequest, token);
            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());
                if (data != null)
                {
                    model.InvoiceToAddresses = data.Select(item => new SelectListItem
                    {
                        Text = item.invoice_to_address.ToString(),
                        Value = item.invoice_to_customer_address_id.ToString()
                    }).ToList();
                    model.ShipToAddresses = data.Select(item => new SelectListItem
                    {
                        Text = item.ship_to_address.ToString(),
                        Value = item.ship_to_customer_address_id.ToString()
                    }).ToList();
                }
            }

            // Set selected for others


            return model;
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRate(string currencyCode)
        {
            string url = $"{_apiLink}/get-currencies";

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return Json(new { Error = "Unauthorized!" });
            }

            var apiResponse = await _apiServices.SendGetRequest(url, "", token);

            if (apiResponse.Success)
            {
                var data = JsonConvert.DeserializeObject<List<dynamic>>(apiResponse.Data.ToString());

                if (data != null)
                {
                    var currency = data.FirstOrDefault(item => item.currency_code.ToString() == currencyCode);
                    if (currency != null)
                    {
                        var exchangeRate = currency.exchange_rate_to_vnd.ToString();

                        return Json(new { exchangeRate = exchangeRate });
                    }
                }
                return Json(new { exchangeRate = 0, data = data, rawData = apiResponse.Data.ToString() });
            }

            return Json(new { exchangeRate = 0 });
        }

        private async Task<ApiResponseModel> GetSalesDocumentViewModel(int salesDocumentId, string token)
        {
            ApiResponseModel result = new();
            SalesDocumentViewModel model = new();
            string getDocumentUrl = $"{_apiLink}/get-sales-document";
            string getLinesUrl = $"{_apiLink}/get-sales-document-lines";
            var parameter = new
            {
                document_id = salesDocumentId,
            };
            string jsonData = JsonConvert.SerializeObject(parameter);

            // Get Document
            var responseDocument = await _apiServices.SendPostRequest(getDocumentUrl, jsonData, token);
            var responseLines = await _apiServices.SendPostRequest(getLinesUrl, jsonData, token);

            // Pretending api executed successfully without any error, ahihi
            if (responseDocument.Success && responseLines.Success)
            {
                model.SalesDocument = JsonConvert.DeserializeObject<SalesDocumentModel>(responseDocument.Data);
                model.SalesDocumentLines = JsonConvert.DeserializeObject<List<SalesDocumentLineModel>>(responseLines.Data);

                result.Success = true;
                result.Data = JsonConvert.SerializeObject(model);

                return result;
            }

            if (!responseDocument.Success)
            {
                return responseDocument;
            }

            return responseLines;
        }

        private List<object> GetEditedFields<T>(T currentModel, T newModel)
        {
            var updates = new List<object>();
            try
            {

                var properties = typeof(T).GetProperties();

                foreach (var property in properties)
                {
                    var currentValue = property.GetValue(currentModel);
                    var newValue = property.GetValue(newModel);

                    if (newValue != null && !newValue.Equals(currentValue))
                    {
                        var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
                        var key = jsonProperty != null ? jsonProperty.PropertyName : property.Name;

                        updates.Add(new
                        {
                            key,
                            value = newValue
                        });
                    }
                }


            }
            catch (Exception ex)
            {

                throw;
            }
            return updates;
        }
    }
}