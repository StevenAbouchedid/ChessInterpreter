using System;

namespace ChessInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            string fenBoardString = "r1b1k1nr/p2p1pNp/n2B4/1p1NP2P/6P1/3P1Q2/P1P1K3/q5b1";
            Board board = new Board();
            board.InitializeDefaultBoard();
            board.PrettyPrintChessBoard();
            Console.WriteLine();

            board.StandardAlgebraicNotationParser("Ke4");

            //while (true) {
            //    Console.WriteLine("Enter a move in algebraic chess notation: ");
            //    string move = Console.ReadLine();
            //    string from = move.Substring(0, 2);
            //    string to = move.Substring(2, 2);
            //    board.MovePieceWithAlgebraicChessNotation(from, to);
            //    board.PrettyPrintChessBoard();
            //    Console.WriteLine();
            //}

            // board.MovePiece(board.Squares[6, 4], board.Squares[4, 4]);
            // board.PrettyPrintChessBoard();

        }
    }
}
