using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragons.Shared
{

    public class Move
    {
        public string MatchId; 
        public Player Mover;
        public string MoveKey;
        public List<Coordinate> Coordinates;
        public int Cost;

    }
}
