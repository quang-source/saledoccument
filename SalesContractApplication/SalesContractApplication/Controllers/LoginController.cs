
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using SalesContractApplication.API;
using SalesContractApplication.Models;
using Newtonsoft.Json;

namespace SalesContractApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly APIServices _apiServices;
        private readonly string _apiLink;

        // public LoginController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        // {
        //     _apiServices = new APIServices(httpClientFactory.CreateClient());
        //     _apiLink = configuration.GetValue<string>("AppSettings:API_Link");
        // }
        public LoginController(IConfiguration configuration, APIServices apiServices)
        {
            _apiServices = apiServices; // Use dependency injection for APIServices
            _apiLink = configuration.GetValue<string>("AppSettings:API_Link");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");

            if (token != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string encryptedPw = EncryptionHelper.Encrypt(model.Password);
                var loginData = new { username = model.Username, password = encryptedPw };
                string jsonData = JsonConvert.SerializeObject(loginData);

                ApiResponseModel apiResponse = await _apiServices.SendPostRequest($"{_apiLink}/login", jsonData);

                if (apiResponse.Success)
                {
                    LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(apiResponse.Data);
                    HttpContext.Session.SetString("Token", loginResponse.Token);
                    HttpContext.Session.SetString("User", model.Username);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        // new Claim("Token", loginResponse.Token)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(960)
                    });

                    return RedirectToAction("Index", "Home"); // Redirect to the home page after successful login
                }
                
                ModelState.AddModelError("", apiResponse.Error);
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}