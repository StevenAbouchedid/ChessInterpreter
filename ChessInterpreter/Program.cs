using System;

namespace ChessInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            //Milestone 1
            PieceMove pm = Board.StandardAlgebraicNotationParser("Nh8", PieceColor.White);

            // Milestone 2
            Board.StandardAlgebraicNotationInterpreter(pm);
        }
    }
}
