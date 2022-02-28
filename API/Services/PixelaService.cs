using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using API.Interfaces;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class PixelaService : IPixelaService
    {
        private readonly IConfiguration _config;
        public PixelaService(IConfiguration config)
        {
            _config = config;
        }

        private const string BASE_URL_CREATE_USER = "https://pixe.la/v1/users";
        private const string BASE_URL_CREATE_GRAPH = "https://pixe.la/v1/users/userName/graphs";
        private const string BASE_URL_ADD_ACTIVITY = "https://pixe.la/v1/users/userName/graphs/graphName";

        public async Task AddActivity(string habitPairName, string userName)
        {
            var url = GetUserGraph(habitPairName, userName);
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-USER-TOKEN", _config["PixelaSecret"].ToString());
            var body = "{\"date\":" + "\"" + DateTime.Now.ToString("yyyyMMdd") + "\"" + ",\"quantity\":\"50\"}";
            var serializedBody = new StringContent(
                body,
                Encoding.UTF8,
                "application/x-www-form-urlencoded");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            using var httpResponseMessage = await httpClient.PostAsync(url, serializedBody);
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        public async Task AddHabitPair(string habitPairName)
        {
            HttpClient httpClient = new HttpClient();
            var body = "{\"token\":" + "\"" + _config["PixelaSecret"].ToString() + "\"" + ", \"username\":" + "\"" + habitPairName + "\"" + ", \"agreeTermsOfService\":\"yes\", \"notMinor\":\"yes\"}";
            var serializedBody = new StringContent(
                body,
                Encoding.UTF8,
                "application/x-www-form-urlencoded");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            using var httpResponseMessage = await httpClient.PostAsync(BASE_URL_CREATE_USER, serializedBody);
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        public async Task CreateUserGraph(string habitPairName, string userName)
        {
            var url = BASE_URL_CREATE_GRAPH;
            userName = userName.ToLower();
            url = url.Replace("userName", habitPairName);
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-USER-TOKEN", _config["PixelaSecret"].ToString());
            var body = "{\"id\":" + "\"" + userName + "\"" + ",\"name\":\"habit-graph\",\"unit\":" +
                "\"commit\",\"type\":\"int\",\"color\":\"shibafu\"}";
            var serializedBody = new StringContent(
                body,
                Encoding.UTF8,
                "application/x-www-form-urlencoded");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            using var httpResponseMessage = await httpClient.PostAsync(url, serializedBody);
            Console.WriteLine("I am here");
        }

        public string GetUserGraph(string habitPairName, string userName)
        {
            var url = BASE_URL_ADD_ACTIVITY;
            url = url.Replace("userName", habitPairName);
            url = url.Replace("graphName", "" + userName.ToLower());
            return url;
        }
    }
}