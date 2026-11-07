using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragons.Shared
{
    public class Constants
    {
        public static readonly List<string> Names = new List<string>() { "Gandolf", "Lavarus", "Ezekiel", "Magnus", "Mortius", "Cyrus", "Belzof", "Palpatine", "Morgoth" };

        public const string ServerBaseAddress = "http://dlalande01.newyork.arkadium.com/";
//        public const string ServerBaseAddress = "http://localhost:53339/";
        public const string MediaType = "application/json";
        
        public const string LobbyKey = "Lobby";
        public const string MatchesKey = "Matches";
        public const string MatchMovesKey = "MatchMoves";
        public const string LayoutsKey = "Layouts";
        public const string PieceListsKey = "PieceLists";
        
        public const string InitialManaKey = "InitialMana";
        public const string ManaIncrementKey = "ManaIncrement";
        public const string GridSizeKey = "GridSize";

        public const int DefaultInitialMana = 10;
        public const int DefaultManaIncrement = 10;
        public const int DefaultGridSize = 14;
    }
}
