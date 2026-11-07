using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dragons.Shared;
using Newtonsoft.Json;

namespace Dragons.Services
{
    public static class MatchService
    {
        public static async Task<Match> StartMatch()
        {
            var client = new HttpClient { BaseAddress = new Uri(Constants.ServerBaseAddress) };
            var request = String.Format("api/match/StartMatch?clientId={0}", LobbyService.ClientId);
            var content = await client.GetStringAsync(request);
            return JsonConvert.DeserializeObject<Match>(content);
        }

        public static async Task<Match> GetMatch(string matchId)
        {
            var client = new HttpClient { BaseAddress = new Uri(Constants.ServerBaseAddress) };
            var request = String.Format("api/match/GetMatch?matchId={0}", matchId);
            var content = await client.GetStringAsync(request);
            if (!String.IsNullOrEmpty(content))
                return JsonConvert.DeserializeObject<Match>(content);
            else
                return null;
        }

        public static async Task EndMatch(string matchId, string winnerClientId)
        {
            var client = new HttpClient { BaseAddress = new Uri(Constants.ServerBaseAddress) };
            var request = String.Format("api/match/EndMatch?matchId={0}&winnerClientId={1}", matchId, winnerClientId);
            var response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
        }
        public static async Task<List<Move>> GetMoves(string matchId, int offset = 0)
        {
            var client = new HttpClient {BaseAddress = new Uri(Constants.ServerBaseAddress)};
            var request = String.Format("api/match/GetMoves?matchId={0}&offset={1}", matchId, offset);
            var content = await client.GetStringAsync(request);
            return JsonConvert.DeserializeObject<List<Move>>(content);
        }

        public static async Task SaveMove(Move move)
        {
            var content = new StringContent(JsonConvert.SerializeObject(move), Encoding.UTF8, Constants.MediaType);
            var client = new HttpClient {BaseAddress = new Uri(Constants.ServerBaseAddress)};
            var message = new HttpRequestMessage(HttpMethod.Post, "api/match/SaveMove") {Content = content};
            var response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();
        }
    }
}
