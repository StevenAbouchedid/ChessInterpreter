using System;

namespace ChessInterpreter
{
    public enum PieceType
    {
        None,           //0
        Pawn,           //1
        Rook,           //2
        Knight,         //3
        Bishop,         //4
        Queen,          //5
        King            //6
    }

    public enum PieceColor
    {
        None,          //0
        White,         //1
        Black          //2
    }

    class Board
    {
        public BoardSquare[,] Squares { get; set; }

        public Board()
        {
            Squares = new BoardSquare[8, 8];
        }

        public void Print()
        {
            for (int i = 0; i < Squares.GetLength(0); i++)
            {
                for (int j = 0; j < Squares.GetLength(1); j++)
                {
                    Squares[i, j].Print();
                }
            }
        }

        public static void StandardAlgebraicNotationParser(string move)
        {
            // There are 6 sections in the algebraic chess notation
            // {Piece}{from}{takes}{to}{promotion}{check}

            string piece = "";
            string from = "";
            string takes = "";
            string to = "";
            string promotion = "";
            string check = "";
            bool QCastle = false;
            bool KCastle = false;

            if (move == "O-O")
            {
                KCastle = true;
            }
            else if (move == "O-O-O")
            {
                QCastle = true;
            }
            else
            {
                // Check for Piece name
                if ("KQRBN".Contains(move[0]))
                {
                    piece = move[0].ToString();
                    move = move.Substring(1);
                }

                // Check for takes
                if (move.Contains("x"))
                {
                    takes = "x";
                    move = move.Replace("x", "");
                }

                // Check for promotion
                if (move.Contains("="))
                {
                    promotion = "=";
                    move = move.Replace("=", "");
                }

                // Check for check or checkmate
                if (move.Contains("+") || move.Contains("#"))
                {
                    check = move.Substring(move.Length - 1);
                    move = move.Substring(0, move.Length - 1);
                }

                // Check for from
                if (move.Length == 3)
                {
                    from = move.Substring(0, 1);
                    move = move.Substring(1);
                }

                // Check for to
                if (move.Length == 2)
                {
                    to = move;
                }
            }
        }

        public string GetPieceCharacter(ChessPiece chessPiece)
        {
            string pieceCharacter = "";

            switch (chessPiece.Type)
            {
                case PieceType.None:
                    switch (chessPiece.Color)
                    {
                        case PieceColor.White:
                            pieceCharacter = " ";
                            break;
                        case PieceColor.Black:
                            pieceCharacter = " ";
                            break;
                    }
                    break;
                case PieceType.Pawn:
                    switch (chessPiece.Color)
                    {
                        case PieceColor.White:
                            pieceCharacter = "♟";
                            break;
                        case PieceColor.Black:
                            pieceCharacter = "♙";
                            break;
                    }
                    break;
                case PieceType.Rook:
                    switch (chessPiece.Color)
                    {
                        case PieceColor.White:
                            pieceCharacter = "♜";
                            break;
                        case PieceColor.Black:
                            pieceCharacter = "♖";
                            break;
                    }
                    break;
                case PieceType.Knight:
                    switch (chessPiece.Color)
                    {
                        case PieceColor.White:
                            pieceCharacter = "♞";
                            break;
                        case PieceColor.Black:
                            pieceCharacter = "♘";
                            break;
                    }
                    break;
                case PieceType.Bishop:
                    switch (chessPiece.Color)
                    {
                        case PieceColor.White:
                            pieceCharacter = "♝";
                            break;
                        case PieceColor.Black:
                            pieceCharacter = "♗";
                            break;
                    }
                    break;
                case PieceType.Queen:
                    switch (chessPiece.Color)
                    {
                        case PieceColor.White:
                            pieceCharacter = "♛";
                            break;
                        case PieceColor.Black:
                            pieceCharacter = "♕";
                            break;
                    }
                    break;
                case PieceType.King:
                    switch (chessPiece.Color)
                    {
                        case PieceColor.White:
                            pieceCharacter = "♚";
                            break;
                        case PieceColor.Black:
                            pieceCharacter = "♔";
                            break;
                    }
                    break;
            }

            if (chessPiece.Color == PieceColor.Black)
            {
                pieceCharacter = pieceCharacter.ToLower();
            }

            return pieceCharacter;
        }

        public void MovePiece(BoardSquare from, BoardSquare to)
        {
            to.Piece = from.Piece;
            from.Piece = new ChessPiece();
        }

        public void InitializeEmptyBoard()
        {
            for (int i = 0; i < Squares.GetLength(0); i++)
            {
                for (int j = 0; j < Squares.GetLength(1); j++)
                {
                    Squares[i, j] = new BoardSquare($"{i}{j}", new ChessPiece());
                }
            }
        }

        public void InitializeDefaultBoard()
        {
            for (int i = 0; i < Squares.GetLength(0); i++)
            {
                for (int j = 0; j < Squares.GetLength(1); j++)
                {
                    Squares[i, j] = new BoardSquare($"{i}{j}", new ChessPiece());
                }
            }

            for (int i = 0; i < Squares.GetLength(0); i++)
            {
                Squares[1, i].Piece = new ChessPiece(PieceType.Pawn, PieceColor.Black);
                Squares[6, i].Piece = new ChessPiece(PieceType.Pawn, PieceColor.White);
            }

            for (int i = 0; i < 6; i++)
            {
                Squares[2, i].Piece = new ChessPiece(PieceType.None, PieceColor.None);
                Squares[3, i].Piece = new ChessPiece(PieceType.None, PieceColor.None);
                Squares[4, i].Piece = new ChessPiece(PieceType.None, PieceColor.None);
                Squares[5, i].Piece = new ChessPiece(PieceType.None, PieceColor.None);
            }

            Squares[0, 0].Piece = new ChessPiece(PieceType.Rook, PieceColor.Black);
            Squares[0, 1].Piece = new ChessPiece(PieceType.Knight, PieceColor.Black);
            Squares[0, 2].Piece = new ChessPiece(PieceType.Bishop, PieceColor.Black);
            Squares[0, 3].Piece = new ChessPiece(PieceType.Queen, PieceColor.Black);
            Squares[0, 4].Piece = new ChessPiece(PieceType.King, PieceColor.Black);
            Squares[0, 5].Piece = new ChessPiece(PieceType.Bishop, PieceColor.Black);
            Squares[0, 6].Piece = new ChessPiece(PieceType.Knight, PieceColor.Black);
            Squares[0, 7].Piece = new ChessPiece(PieceType.Rook, PieceColor.Black);

            Squares[7, 0].Piece = new ChessPiece(PieceType.Rook, PieceColor.White);
            Squares[7, 1].Piece = new ChessPiece(PieceType.Knight, PieceColor.White);
            Squares[7, 2].Piece = new ChessPiece(PieceType.Bishop, PieceColor.White);
            Squares[7, 3].Piece = new ChessPiece(PieceType.Queen, PieceColor.White);
            Squares[7, 4].Piece = new ChessPiece(PieceType.King, PieceColor.White);
            Squares[7, 5].Piece = new ChessPiece(PieceType.Bishop, PieceColor.White);
            Squares[7, 6].Piece = new ChessPiece(PieceType.Knight, PieceColor.White);
            Squares[7, 7].Piece = new ChessPiece(PieceType.Rook, PieceColor.White);
        }

        public void InitializeBoardFromFEN(string fen)
        {
            string[] fenParts = fen.Split(' ');

            string[] fenRows = fenParts[0].Split('/');

            for (int i = 0; i < fenRows.Length; i++)
            {
                int j = 0;
                foreach (char c in fenRows[i])
                {
                    if (char.IsDigit(c))
                    {
                        for (int k = 0; k < int.Parse(c.ToString()); k++)
                        {
                            Squares[i, j] = new BoardSquare($"{i}{j}", new ChessPiece());
                            j++;
                        }
                    }
                    else
                    {
                        Squares[i, j] = new BoardSquare($"{i}{j}", new ChessPiece());
                        j++;
                    }
                }
            }
        }

        public void PrettyPrintChessBoard()
        {
            for (int i = 0; i < Squares.GetLength(0); i++)
            {
                Console.Write($"{8 - i} ");
                for (int j = 0; j < Squares.GetLength(1); j++)
                {
                    if (Squares[i, j].Piece.Type == PieceType.None)
                    {
                        Console.Write("   ");
                    }
                    else
                    {
                        string type = Squares[i, j].Piece.Type.ToString();
                        string color = Squares[i, j].Piece.Color.ToString();
                        if (Squares[i, j].Piece.Type == PieceType.Knight)
                            type = "N";
                        Console.Write($"{color[0]}{type[0]} ");
                        //Console.Write($" {GetPieceCharacter(Squares[i, j].Piece)} ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("   a  b  c  d  e  f  g  h ");
        }
    }


    class BoardSquare
    {
        public string Position { get; set; }
        public ChessPiece Piece { get; set; }

        public BoardSquare(string position, ChessPiece piece)
        {
            Position = position;
            Piece = piece;
        }

        public void Print()
        {
            Console.WriteLine($"Position: {Position}, Piece: {Piece}");
        }
    }

    class ChessPiece
    {
        public PieceType Type { get; set; }
        public PieceColor Color { get; set; }

        public ChessPiece()
        {
            Type = PieceType.None;
            Color = PieceColor.None;
        }

        public ChessPiece(PieceType type, PieceColor color)
        {
            Type = type;
            Color = color;
        }

        public override string ToString()
        {
            return $"{Color} {Type}";
        }
    }
}
