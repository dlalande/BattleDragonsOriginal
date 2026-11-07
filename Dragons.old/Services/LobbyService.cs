using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dragons.Shared;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Dragons.Services
{
    public static class LobbyService
    {
        
        public static readonly string ClientId = GetPersistentGuid("ClientId");
        public static readonly string ClientName = GetClientName();

        public async static Task JoinLobby()
        {
            Debug.WriteLine("JoinLobby");
            var client = new HttpClient {BaseAddress = new Uri(Constants.ServerBaseAddress)};
            var request = String.Format("api/lobby/JoinLobby?clientId={0}&clientName={1}", ClientId, ClientName);
            var response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async static Task ExitLobby()
        {
            Debug.WriteLine("ExitLobby");
            var client = new HttpClient {BaseAddress = new Uri(Constants.ServerBaseAddress)};
            var request = String.Format("api/lobby/ExitLobby?clientId={0}", ClientId);
            var response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
        }

        private static string GetClientName()
        {
            var name = ApplicationData.Current.LocalSettings.Values["ClientName"];
            if (name == null)
            {
                var random = new Random();
                var nameIndex = random.Next(0, Constants.Names.Count);
                var suffix = random.Next(0, 1000);
                name = String.Format("{0}", Constants.Names[nameIndex], suffix);
                ApplicationData.Current.LocalSettings.Values.Add("ClientName", name);
            }
            return name.ToString();
        }

        private static string GetPersistentGuid(string key)
        {
            var guid = ApplicationData.Current.LocalSettings.Values[key];
            if (guid == null)
            {
                guid = Guid.NewGuid().ToString();
                ApplicationData.Current.LocalSettings.Values.Add(key, guid);
            }
            return guid.ToString();
        }
     

    }
}
