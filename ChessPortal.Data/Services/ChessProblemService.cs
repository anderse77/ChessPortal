using ChessPortal.Data.Settings;
using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Data.Services
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

        public async Task<ChessProblemResponseDto> GetChessProblemAsync(ChessProblemRequestDto request)
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
            
            return JsonConvert.DeserializeObject<ChessProblemResponseDto>(await response.Content.ReadAsStringAsync());
        }
    }
}
