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

        public static void StandardAlgebraicNotationParser(string move, PieceColor color)
        {
            // There are 6 sections in the algebraic chess notation
            // {Piece}{from}{takes}{to}{promotion}{check}

            string piece = "";
            string from = "";
            string takes = "";
            string to = "";
            string promotion = "";
            string check = "";
            string checkmate = "";
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
                
                // Convert piece name to PieceType
                PieceType pieceType;
                if (piece == "K")
                {
                    pieceType = PieceType.King;
                }
                else if (piece == "Q")
                {
                    pieceType = PieceType.Queen;
                }
                else if (piece == "R")
                {
                    pieceType = PieceType.Rook;
                }
                else if (piece == "B")
                {
                    pieceType = PieceType.Bishop;
                }
                else if (piece == "N")
                {
                    pieceType = PieceType.Knight;
                }
                else
                {
                    pieceType = PieceType.Pawn;
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
                if (move.Contains("+"))
                {
                    check = move.Substring(move.Length - 1);
                    move = move.Substring(0, move.Length - 1);
                }

                if (move.Contains("#"))
                {
                    checkmate = move.Substring(move.Length - 1);
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

                StandardAlgebraicNotationInterpreter(new PieceMove(
                    from, 
                    to, 
                    takes != "" ? true : false, 
                    promotion, 
                    check != "" ? true : false, 
                    checkmate != "" ? true : false, 
                    QCastle, 
                    KCastle,
                    new ChessPiece(pieceType, color)
                    ));
            }
        }

        public static void StandardAlgebraicNotationInterpreter(PieceMove move)
        {
            string fromSquare;
            string toSquare;

            fromSquare = move.From;
            toSquare = move.To;


            switch (toSquare.Length)
            {
                case 0:
                    toSquare = toSquare + "1";
                    break;
                case 1:
                    toSquare = toSquare + "1";
                    break;

            }


            if (move.QCastle)
            {
                fromSquare = "O-O";
                toSquare = "O-O";
            }
            else if (move.KCastle)
            {
                fromSquare = "O-O-O";
                toSquare = "O-O-O";
            }
            else
            {
                CalculateMoves(toSquare, fromSquare, move.chessPiece);
            }

        }

        public static PieceMove[] CalculateMoves(string toSquare, string fromSquare, ChessPiece chessPiece) 
        {
            PieceMove[] moves = new PieceMove[0];

            switch (chessPiece.Type)
            {
                case PieceType.Pawn:
                    moves = CalculatePawnMoves(toSquare, fromSquare, chessPiece);
                    break;
                case PieceType.Rook:
                    moves = CalculateRookMoves(toSquare, fromSquare, chessPiece);
                    break;
                case PieceType.Knight:
                    moves = CalculateKnightMoves(toSquare, fromSquare, chessPiece);
                    break;
                case PieceType.Bishop:
                    moves = CalculateBishopMoves(toSquare, fromSquare, chessPiece);
                    break;
                case PieceType.Queen:
                    moves = CalculateQueenMoves(toSquare, fromSquare, chessPiece);
                    break;
                case PieceType.King:
                    moves = CalculateKingMoves(toSquare, fromSquare, chessPiece);
                    break;
            }

            return moves;
        }

        private static PieceMove[] CalculatePawnMoves(string toSquare, string fromSquare, ChessPiece chessPiece)
        {
            PieceMove[] moves = new PieceMove[0];

            int toSquareX = toSquare[0] - 97;
            int toSquareY = toSquare[1] - 48;

            //Calculate the moves for white pawns vs black pawns.
            if (chessPiece.Color == PieceColor.White)
            {
                if (toSquareY == 4)
                {
                    moves = new PieceMove[4];
                    moves[0] = new PieceMove(toSquare[0]+"2", toSquare, false, "", false, false, false, false, chessPiece);
                    moves[1] = new PieceMove(Convert.ToChar(toSquare[0] - 1)+"3", toSquare, false, "", false, false, false, false, chessPiece);
                    moves[2] = new PieceMove(Convert.ToChar(toSquare[0] + 1)+"3", toSquare, false, "", false, false, false, false, chessPiece);
                    moves[3] = new PieceMove(toSquare[0]+"3", toSquare, false, "", false, false, false, false, chessPiece);

                }
                else
                {
                    moves = new PieceMove[3];
                    moves[0] = new PieceMove(toSquare[0]+(toSquareY-1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                    moves[1] = new PieceMove(Convert.ToChar(toSquare[0] - 1)+(toSquareY-1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                    moves[2] = new PieceMove(Convert.ToChar(toSquare[0] + 1)+(toSquareY-1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                }
            }
            else
            {
                if (toSquareY == 4)
                {
                    moves = new PieceMove[4];
                    moves[0] = new PieceMove(toSquare[0]+"6", toSquare, false, "", false, false, false, false, chessPiece);
                    moves[1] = new PieceMove(Convert.ToChar(toSquare[0] - 1)+"6", toSquare, false, "", false, false, false, false, chessPiece);
                    moves[2] = new PieceMove(Convert.ToChar(toSquare[0] + 1)+"6", toSquare, false, "", false, false, false, false, chessPiece);
                    moves[3] = new PieceMove(toSquare[0]+"7", toSquare, false, "", false, false, false, false, chessPiece);
                }
                else
                {
                    moves = new PieceMove[3];
                    moves[0] = new PieceMove(toSquare[0]+(toSquareY+1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                    moves[1] = new PieceMove(Convert.ToChar(toSquare[0] - 1)+(toSquareY+1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                    moves[2] = new PieceMove(Convert.ToChar(toSquare[0] + 1)+(toSquareY+1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                }
            }

            //remove moves that are out of bounds
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i].From[0] < 97 || moves[i].From[0] > 104 || moves[i].From[1] < 49 || moves[i].From[1] > 56)
                {
                    moves[i] = null;
                }
            }

            //remove moves that arent a part of the from square
            if (fromSquare != "")
            {
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i] != null)
                    {
                        switch (moves[i].From.Length)
                        {
                            case 1:
                                if (moves[i].From[0] != fromSquare[0] && moves[i].From[1] != fromSquare[1])
                                {
                                    moves[i] = null;
                                }
                                break;
                            case 2:
                                if (moves[i].From != fromSquare)
                                {
                                    moves[i] = null;
                                }
                                break;
                        }
                    }
                }
            }
            
            Console.WriteLine("To: "+toSquare);
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i] != null)
                {
                    Console.WriteLine("From: " + moves[i].From);
                }
            }

            return moves;
        }
        private static PieceMove[] CalculateRookMoves(string toSquare, string fromSquare, ChessPiece chessPiece)
        {
            PieceMove[] moves = new PieceMove[0];

            int toSquareX = toSquare[0] - 97;
            int toSquareY = toSquare[1] - 48;

            //Calculate the moves 
            moves = new PieceMove[28];

            //up
            for (int i = 0; i < 7; i++)
            {
                moves[i] = new PieceMove(toSquare[0] + (toSquareY + i + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //down
            for (int i = 0; i < 7; i++)
            {
                moves[i + 7] = new PieceMove(toSquare[0] + (toSquareY - i - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //left
            for (int i = 0; i < 7; i++)
            {
                moves[i + 14] = new PieceMove(Convert.ToChar(toSquare[0] - i - 1) + toSquareY.ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //right
            for (int i = 0; i < 7; i++)
            {
                moves[i + 21] = new PieceMove(Convert.ToChar(toSquare[0] + i + 1) + toSquareY.ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //remove moves that are out of bounds
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i].From[0] < 97 || moves[i].From[0] > 104 || moves[i].From[1] < 49 || moves[i].From[1] > 56)
                {
                    moves[i] = null;
                }
            }

            //remove moves that arent a part of the from square
            if (fromSquare != "")
            {
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i] != null)
                    {
                        switch (moves[i].From.Length)
                        {
                            case 1:
                                if (moves[i].From[0] != fromSquare[0] && moves[i].From[1] != fromSquare[1])
                                {
                                    moves[i] = null;
                                }
                                break;
                            case 2:
                                if (moves[i].From != fromSquare)
                                {
                                    moves[i] = null;
                                }
                                break;
                        }
                    }
                }
            }
            
            Console.WriteLine("To: "+toSquare);
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i] != null)
                {
                    Console.WriteLine("From: " + moves[i].From);
                }
            }

            return moves;
        }
        private static PieceMove[] CalculateKnightMoves(string toSquare, string fromSquare, ChessPiece chessPiece)
        {
            PieceMove[] moves = new PieceMove[0];

            int toSquareX = toSquare[0] - 97;
            int toSquareY = toSquare[1] - 48;

            //Calculate the moves 
            moves = new PieceMove[8];

            moves[0] = new PieceMove(Convert.ToChar(toSquare[0] - 1) + (toSquareY + 2).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            moves[1] = new PieceMove(Convert.ToChar(toSquare[0] + 1) + (toSquareY + 2).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            moves[2] = new PieceMove(Convert.ToChar(toSquare[0] - 1) + (toSquareY - 2).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            moves[3] = new PieceMove(Convert.ToChar(toSquare[0] + 1) + (toSquareY - 2).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            moves[4] = new PieceMove(Convert.ToChar(toSquare[0] - 2) + (toSquareY + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            moves[5] = new PieceMove(Convert.ToChar(toSquare[0] + 2) + (toSquareY + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            moves[6] = new PieceMove(Convert.ToChar(toSquare[0] - 2) + (toSquareY - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            moves[7] = new PieceMove(Convert.ToChar(toSquare[0] + 2) + (toSquareY - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
        
            //remove moves that are out of bounds
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i].From[0] < 97 || moves[i].From[0] > 104 || moves[i].From[1] < 49 || moves[i].From[1] > 56)
                {
                    moves[i] = null;
                }
            }

            //remove moves that arent a part of the from square
            if (fromSquare != "")
            {
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i] != null)
                    {
                        switch (moves[i].From.Length)
                        {
                            case 1:
                                if (moves[i].From[0] != fromSquare[0] && moves[i].From[1] != fromSquare[1])
                                {
                                    moves[i] = null;
                                }
                                break;
                            case 2:
                                if (moves[i].From != fromSquare)
                                {
                                    moves[i] = null;
                                }
                                break;
                        }
                    }
                }
            }
            
            Console.WriteLine("To: "+toSquare);
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i] != null)
                {
                    Console.WriteLine("From: " + moves[i].From);
                }
            }

            return moves;
        }
        private static PieceMove[] CalculateBishopMoves(string toSquare, string fromSquare, ChessPiece chessPiece)
        {
            PieceMove[] moves = new PieceMove[0];

            int toSquareX = toSquare[0] - 97;
            int toSquareY = toSquare[1] - 48;

            //Calculate the moves 
            moves = new PieceMove[28];

            //up right
            for (int i = 0; i < 7; i++)
            {
                moves[i] = new PieceMove(Convert.ToChar(toSquare[0] + i + 1) + (toSquareY + i + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //down right
            for (int i = 0; i < 7; i++)
            {
                moves[i + 7] = new PieceMove(Convert.ToChar(toSquare[0] + i + 1) + (toSquareY - i - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //down left
            for (int i = 0; i < 7; i++)
            {
                moves[i + 14] = new PieceMove(Convert.ToChar(toSquare[0] - i - 1) + (toSquareY - i - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //up left
            for (int i = 0; i < 7; i++)
            {
                moves[i + 21] = new PieceMove(Convert.ToChar(toSquare[0] - i - 1) + (toSquareY + i + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //remove moves that are out of bounds
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i].From[0] < 97 || moves[i].From[0] > 104 || moves[i].From[1] < 49 || moves[i].From[1] > 56)
                {
                    moves[i] = null;
                }
            }

            //remove moves that arent a part of the from square
            if (fromSquare != "")
            {
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i] != null)
                    {
                        switch (moves[i].From.Length)
                        {
                            case 1:
                                if (moves[i].From[0] != fromSquare[0] && moves[i].From[1] != fromSquare[1])
                                {
                                    moves[i] = null;
                                }
                                break;
                            case 2:
                                if (moves[i].From != fromSquare)
                                {
                                    moves[i] = null;
                                }
                                break;
                        }
                    }
                }
            }
            
            Console.WriteLine("To: "+toSquare);
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i] != null)
                {
                    Console.WriteLine("From: " + moves[i].From);
                }
            }

            return moves;
        }
        private static PieceMove[] CalculateKingMoves(string toSquare, string fromSquare, ChessPiece chessPiece)
        {
            PieceMove[] moves = new PieceMove[0];

            int toSquareX = toSquare[0] - 97;
            int toSquareY = toSquare[1] - 48;

            //Calculate the moves 
            moves = new PieceMove[8];

            //up
            moves[0] = new PieceMove(Convert.ToChar(toSquare[0]) + (toSquareY + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

            //up right
            moves[1] = new PieceMove(Convert.ToChar(toSquare[0] + 1) + (toSquareY + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

            //right
            moves[2] = new PieceMove(Convert.ToChar(toSquare[0] + 1) + (toSquareY).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

            //down right
            moves[3] = new PieceMove(Convert.ToChar(toSquare[0] + 1) + (toSquareY - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

            //down
            moves[4] = new PieceMove(Convert.ToChar(toSquare[0]) + (toSquareY - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

            //down left
            moves[5] = new PieceMove(Convert.ToChar(toSquare[0] - 1) + (toSquareY - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

            //left
            moves[6] = new PieceMove(Convert.ToChar(toSquare[0] - 1) + (toSquareY).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

            //up left
            moves[7] = new PieceMove(Convert.ToChar(toSquare[0] - 1) + (toSquareY + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

            //remove moves that are out of bounds
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i].From[0] < 97 || moves[i].From[0] > 104 || moves[i].From[1] < 49 || moves[i].From[1] > 56)
                {
                    moves[i] = null;
                }
            }

            //remove moves that arent a part of the from square
            if (fromSquare != "")
            {
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i] != null)
                    {
                        switch (moves[i].From.Length)
                        {
                            case 1:
                                if (moves[i].From[0] != fromSquare[0] && moves[i].From[1] != fromSquare[1])
                                {
                                    moves[i] = null;
                                }
                                break;
                            case 2:
                                if (moves[i].From != fromSquare)
                                {
                                    moves[i] = null;
                                }
                                break;
                        }
                    }
                }
            }
            
            Console.WriteLine("To: "+toSquare);
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i] != null)
                {
                    Console.WriteLine("From: " + moves[i].From);
                }
            }

            return moves;
        }
        private static PieceMove[] CalculateQueenMoves(string toSquare, string fromSquare, ChessPiece chessPiece)
        {
            PieceMove[] moves = new PieceMove[0];

            int toSquareX = toSquare[0] - 97;
            int toSquareY = toSquare[1] - 48;

            //Calculate the moves 
            moves = new PieceMove[56];

            //up
            for (int i = 0; i < 7; i++)
            {
                moves[i] = new PieceMove(toSquare[0] + (toSquareY + i + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //down
            for (int i = 0; i < 7; i++)
            {
                moves[i + 7] = new PieceMove(toSquare[0] + (toSquareY - i - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //left
            for (int i = 0; i < 7; i++)
            {
                moves[i + 14] = new PieceMove(Convert.ToChar(toSquare[0] - i - 1) + toSquareY.ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //right
            for (int i = 0; i < 7; i++)
            {
                moves[i + 21] = new PieceMove(Convert.ToChar(toSquare[0] + i + 1) + toSquareY.ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

                        //up right
            for (int i = 0; i < 7; i++)
            {
                moves[i + 28] = new PieceMove(Convert.ToChar(toSquare[0] + i + 1) + (toSquareY + i + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //down right
            for (int i = 0; i < 7; i++)
            {
                moves[i + 35] = new PieceMove(Convert.ToChar(toSquare[0] + i + 1) + (toSquareY - i - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //down left
            for (int i = 0; i < 7; i++)
            {
                moves[i + 42] = new PieceMove(Convert.ToChar(toSquare[0] - i - 1) + (toSquareY - i - 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

            //up left
            for (int i = 0; i < 7; i++)
            {
                moves[i + 49] = new PieceMove(Convert.ToChar(toSquare[0] - i - 1) + (toSquareY + i + 1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
            }

                        //remove moves that are out of bounds
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i].From[0] < 97 || moves[i].From[0] > 104 || moves[i].From[1] < 49 || moves[i].From[1] > 56)
                {
                    moves[i] = null;
                }
            }

            //remove moves that arent a part of the from square
            if (fromSquare != "")
            {
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i] != null)
                    {
                        switch (moves[i].From.Length)
                        {
                            case 1:
                                if (moves[i].From[0] != fromSquare[0] && moves[i].From[1] != fromSquare[1])
                                {
                                    moves[i] = null;
                                }
                                break;
                            case 2:
                                if (moves[i].From != fromSquare)
                                {
                                    moves[i] = null;
                                }
                                break;
                        }
                    }
                }
            }
            
            Console.WriteLine("To: "+toSquare);
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i] != null)
                {
                    Console.WriteLine("From: " + moves[i].From);
                }
            }

            return moves;
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

    class PieceMove
    {
        public string From { get; set; }
        public string To { get; set; }
        public bool Takes { get; set; }
        public string Promotion { get; set; }
        public bool Check { get; set; }
        public bool Checkmate { get; set; }
        public bool QCastle { get; set; }
        public bool KCastle { get; set; }
        public ChessPiece chessPiece { get; set; }

        public PieceMove()
        {
            From = "";
            To = "";
            Takes = false;
            Promotion = "";
            Check = false;
            Checkmate = false;
            QCastle = false;
            KCastle = false;
        }

        public PieceMove(string from, string to, bool takes, string promotion, bool check, bool checkmate, bool qCastle, bool kCastle, ChessPiece chessPiece)
        {
            From = from;
            To = to;
            Takes = takes;
            Promotion = promotion;
            Check = check;
            Checkmate = checkmate;
            QCastle = qCastle;
            KCastle = kCastle;
            this.chessPiece = chessPiece;
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
