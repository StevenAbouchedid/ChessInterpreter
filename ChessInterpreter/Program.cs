using System;

namespace ChessInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            ////Milestone 1
            //PieceMove pm = Board.StandardAlgebraicNotationParser("Nh8", PieceColor.White);

            //// Milestone 2
            //Board.StandardAlgebraicNotationInterpreter(pm);

            Game game = new Game();
            game.board.PrettyPrintChessBoard();
            while (true)
            {
                game.PlayMove(Console.ReadLine());
                game.board.PrettyPrintChessBoard();
            }
        }
    }
}
