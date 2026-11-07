using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragons.Shared
{
    public class Grid : List<List<Cell>>
    {
        public Grid(int size)
        {
            for (var x = 0; x < size; x++)
            {
                this.Add(new List<Cell>());
                for (var y = 0; y < size; y++)
                    this[x].Add(new Cell() { Coordinate = new Coordinate() { x = x, y = y }, PieceKey = "Ground", Value = 0, LastMoves = new Stack<string>()});
            }

        }

        public Grid()
            : base()
        { }
    }
}
