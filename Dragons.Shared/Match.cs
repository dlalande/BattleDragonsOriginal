using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragons.Shared
{
    public class Match
    {
        public string MatchId;
        public int InitalMana;
        public int ManaIncrement;
        public string Player1ClientId;
        public string Player1ClientName;
        public string Player2ClientId;
        public string Player2ClientName;
        public string WinnerClientId;
        public Grid Player1Layout;
        public Grid Player2Layout;
        public Grid Player1Pieces;
        public Grid Player2Pieces;
        public int Player1Map;
        public int Player2Map;
    }
}
