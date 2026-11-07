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
    public class MatchController : ApiController
    {
        private Dictionary<string, LobbyClient> Lobby =
            (Dictionary<string, LobbyClient>) HttpContext.Current.Cache[Constants.LobbyKey];

        private Dictionary<string, Match> Matches =
            (Dictionary<string, Match>) HttpContext.Current.Cache[Constants.MatchesKey];

        private Dictionary<string, List<Move>> MatchMoves =
            (Dictionary<string, List<Move>>) HttpContext.Current.Cache[Constants.MatchMovesKey];

        [HttpGet]
        public Match StartMatch(string clientId)
        {
            if(!Lobby.ContainsKey(clientId))
                Lobby.Add(clientId,new LobbyClient() {ClientId =clientId, ClientName = GetClientName()});

            var client = Lobby[clientId];

            var match =
                Matches.FirstOrDefault(
                    item =>
                    item.Value.Player1ClientId != clientId && String.IsNullOrEmpty(item.Value.Player2ClientId) &&
                    String.IsNullOrEmpty(item.Value.WinnerClientId))
                       .Value;
            if (match != null)
            {
                match.Player2ClientId = clientId;
                match.Player2ClientName = client.ClientName;
            }
            else
            {
                match = new Match()
                    {
                        MatchId = Guid.NewGuid().ToString(),
                        Player1ClientId = clientId,
                        Player1ClientName = client.ClientName,
                        InitalMana = Constants.DefaultInitialMana,
                        ManaIncrement = Constants.DefaultManaIncrement
                    };

                List<Grid> layouts = (List<Grid>) HttpContext.Current.Cache[Constants.LayoutsKey];
                List<Grid> pieceLists = (List<Grid>) HttpContext.Current.Cache[Constants.PieceListsKey];
                var random = new Random();
                var player1Index = random.Next(layouts.Count);
                var player2Index = 0;
                do
                {
                    player2Index = random.Next(layouts.Count);
                } while (player1Index == player2Index);

                match.Player1Map = player1Index;
                match.Player2Map = player2Index;

                match.Player1Layout = layouts[player1Index];
                match.Player2Layout = layouts[player2Index];
                match.Player1Pieces = pieceLists[player1Index];
                match.Player2Pieces = pieceLists[player2Index];
                Matches.Add(match.MatchId, match);
                MatchMoves.Add(match.MatchId, new List<Move>());
            }
            return match;
        }

        [HttpGet]
        public Match GetMatch(string matchId)
        {
            if (Matches.ContainsKey(matchId))
                return Matches[matchId];
            else
                return null;
        }

        [HttpGet]
        public void EndMatch(string matchId, string winnerClientId)
        {
            Matches[matchId].WinnerClientId = winnerClientId;
        }

        [HttpGet]
        public List<Move> GetMoves(string matchId, int offset = 0)
        {
            if (!MatchMoves.ContainsKey(matchId))
                MatchMoves.Add(matchId, new List<Move>());
            var moves = MatchMoves[matchId];
            var trimmedMoves = moves.Where((t, index) => index > offset - 1).ToList();
            return trimmedMoves;
        }

        [HttpPost]
        public void SaveMove(Move move)
        {
            if (!MatchMoves.ContainsKey(move.MatchId))
                MatchMoves.Add(move.MatchId, new List<Move>());
            MatchMoves[move.MatchId].Add(move);
        }

        private static string GetClientName()
        {
            var random = new Random();
            var nameIndex = random.Next(0, Constants.Names.Count);
            var suffix = random.Next(0, 1000);
            return String.Format("{0}", Constants.Names[nameIndex], suffix);
        }
    }
}
