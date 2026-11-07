using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DragonService.Model;
using Dragons.Shared;

namespace Dragons.Service.Controllers
{
    public class  LobbyController : ApiController
    {
        Dictionary<string, LobbyClient> Lobby = (Dictionary<string, LobbyClient>) HttpContext.Current.Cache[Constants.LobbyKey];

        [HttpGet]
        public void JoinLobby(string clientId, string clientName)
        {
            if (!Lobby.ContainsKey(clientId))
                Lobby.Add(clientId, new LobbyClient() {ClientId = clientId, ClientName = clientName});
            else
                Lobby[clientId].ClientName = clientName;
        }

        [HttpGet]
        public void ExitLobby(string clientId)
        {
            Lobby.Remove(clientId);
        }

        [HttpGet]
        public List<LobbyClient> GetLobby()
        {
            return Lobby.Values.ToList();
        }
    }
}
