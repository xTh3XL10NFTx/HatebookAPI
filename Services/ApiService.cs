//using HatebookUX.Models;
//using System.Net.Http;
//using System.Threading.Tasks;

//namespace HatebookUX.Services
//{
//    public class ApiService
//    {

//        private readonly HttpClient httpClient;

//        public ApiService()
//        {
//            httpClient = new HttpClient();
//        }

//        public async Task<string> CallApiEndpoint(string endpointUrl, Account account)
//        {
//            // Assuming you need to pass the username and password as part of the API request
//            // You can modify this code based on the API requirements
//            var requestContent = new FormUrlEncodedContent(new[]
//            {
//                new KeyValuePair<string, string>("Email", account.Email),
//                new KeyValuePair<string, string>("Password", account.Password)
//            });

//            HttpResponseMessage response = await httpClient.PostAsync(endpointUrl, requestContent);

//            if (response.IsSuccessStatusCode)
//            {
//                string responseData = await response.Content.ReadAsStringAsync();
//                return responseData;
//            }

//            return null; // or throw an exception, depending on your requirements
//        }
//    }
//}
