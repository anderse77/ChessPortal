using ChessPortal.DataInterfaces;
using ChessPortal.Models.Chess.ChessProblems;
using ChessPortal.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Services
{
    public class ChessProblemService : IChessProblemService
    {
        public HttpClient HttpClient { get; set; }
        public ChessProblemSettings Settings { get; set; }

        public ChessProblemService(IOptions<ChessProblemSettings> settings)
        {
            HttpClient = new HttpClient();
            Settings = settings.Value;
        }

        public async Task<ChessProblemResponse> GetChessProblemAsync(ChessProblemRequest request)
        {
            HttpResponseMessage response;
            try
            {
                response = await HttpClient.PostAsync(
                new Uri(Settings.ChessProblemUrl),
                new StringContent(
                    JsonConvert.SerializeObject(request,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }).ToString(),
                    Encoding.UTF8,
                    "application/json"));
            }
            catch
            {
                throw new Exception("The api seems to be down at the moment. Please try again later");
            }
            
            return JsonConvert.DeserializeObject<ChessProblemResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
