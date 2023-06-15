using HatebookUX.Models;
//using HatebookUX.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HatebookUX.Controllers
{
    public class UserAccountController : Controller
    {
        //private readonly ApiService apiService;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private readonly string _accountEndpoint;
        private readonly string _otherEndpoint;
        private async Task<bool> IsValidEmail(string email)
        {
            

            return false;
        }

        private bool IsValidPassword(string password)
        {
            // Add your password validation logic here
            // Example: return password.Length >= 8;
            return true; // Replace with your validation logic
        }

        public UserAccountController(IConfiguration configuration )
        {
            //apiService = new ApiService();
            _configuration = configuration;
            _baseUrl = _configuration["ApiEndpoints:BaseUrl"];
            _accountEndpoint = _configuration["ApiEndpoints:AccountEndpoint"];
            _otherEndpoint = _configuration["ApiEndpoints:OtherEndpoint"];
        }

        public async Task<IActionResult> Index(Account account)
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(account.Email) || string.IsNullOrEmpty(account.Password))
            {
                errors.Add("Please enter both email and password.");
            }
            //else if (!IsValidEmail(account.Email) || !IsValidPassword(account.Password))
            //{
            //    errors.Add("Invalid email or password.");
            //}

            if (errors.Count > 0)
            {
                ViewData["Errors"] = errors;
                return View("Index", account);
            }

            Account obj = new Account()
            {
                Email = account.Email,
                Password = account.Password
            };


            if (account.Email != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUrl + _accountEndpoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage getData = await client.PostAsJsonAsync<Account>("LogIn", obj);

                    if (getData.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Login successful! Welcome.";

                        var responseContent = await getData.Content.ReadAsStringAsync();
                        var token = JObject.Parse(responseContent)["token"].ToString();

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                    }
                }
            }


           

            return View();
        }

      


        public async Task<IActionResult> Users()
        {
            IList<UserEntity> user = new List<UserEntity>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl + _accountEndpoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Retrieve the token from the HttpContext session
                string token = HttpContext.Session.GetString("AuthToken");

                // Add the bearer token to the request's Authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage getData = await client.GetAsync("get");

                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<IList<UserEntity>>(results);
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                }

                ViewData.Model = user;
            }

            return View();
        }
    }
}
