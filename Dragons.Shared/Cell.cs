using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragons.Shared
{
    public class Cell
    {
        public Coordinate Coordinate;
        public string PieceKey;
        public Stack<string> LastMoves;
        public int Value;
        public int Direction;
    }
}
