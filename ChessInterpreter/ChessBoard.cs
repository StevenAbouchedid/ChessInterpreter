using System;
using System.Collections.Generic;
using System.Linq;

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

    class Game
    {
        public PieceColor playerTurn;

        public Board board;

        public bool check;

        public bool checkmate;

        public bool isGameFinished;

        public Game()
        {
            //Initialize new game values
            playerTurn = PieceColor.White;
            check = false;
            checkmate = false;
            isGameFinished = false;

            //Initialize new game board
            board = new Board();
            board.InitializeDefaultBoard();
        }

        // This function will take as input a move in algebraic chess notation and compute the move and play it on the chess board. 
        public void PlayMove(string ChessNotationString)
        {
            try
            {
                //Parse Move
                PieceMove parsedMove = Board.StandardAlgebraicNotationParser(ChessNotationString, playerTurn);
                
                //Interpret move
                PieceMove ContextParsedMove = ParsedMoveContextInterpreter(parsedMove);
                
                //Compile Move
                MakeMoveInGame(ContextParsedMove);

                //Switch Turns
                if (playerTurn == PieceColor.White)
                    playerTurn = PieceColor.Black;
                else
                    playerTurn = PieceColor.White;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Invalid move, please try again");
                return;
            }
        }

        // Interprets a parsed move with the context of the current board state and plays the move on the board.
        private PieceMove ParsedMoveContextInterpreter(PieceMove pieceMove)
        {
            //Uses Interpreter to get an array of legal moves based on the parsed move
            PieceMove[] LegalMoves = Board.StandardAlgebraicNotationInterpreter(pieceMove);

            //New list to check for legal moves with context
            List<PieceMove> LegalMovesWithContext = new List<PieceMove>();

            //Filters for context legal moves and adds them to list
            for (int i = 0; i < LegalMoves.Length; ++i)
                if (CheckIfLegalMove(LegalMoves[i]))
                    LegalMovesWithContext.Add(LegalMoves[i]);

            for (int i = 0; i < LegalMovesWithContext.Count; ++i)
            {
                Console.Write("Legal Moves Based on Context: ");
                Console.WriteLine(LegalMovesWithContext.ElementAt(i));
            }

            if (LegalMovesWithContext.Count > 1)
            {
                Console.WriteLine("Error, too many legal moves");
                return null;
            }
            else if (LegalMovesWithContext.Count < 1)
            {
                Console.WriteLine("Error, no legal move entered");
                return null;
            }
            else
                return LegalMovesWithContext.ElementAt(0);
        }

        public bool CheckIfLegalMove(PieceMove pieceMove)
        {
            //Check for Null
            if (pieceMove == null)
                return false;

            //Check for empty move squares
            if (pieceMove.From == "" || pieceMove.To == "")
                return false;

            //Get Context about piece squares from board
            BoardSquare departureSquare = board.GetSquareByName(pieceMove.From);
            BoardSquare destinationSquare = board.GetSquareByName(pieceMove.To);

            //Check for Null
            if (destinationSquare == null || departureSquare == null)
                return false;

            if (departureSquare.Piece.Type == PieceType.None)
                return false;

            //Make sure the original square has the actual piece being moved
            if (departureSquare.Piece.Type != pieceMove.chessPiece.Type || departureSquare.Piece.Color != pieceMove.chessPiece.Color)
                return false;

            //Move cannot be made if destination is friendly piece
            if (destinationSquare.Piece.Color == playerTurn)
                return false;

            // Need to consider checks
            // The move needs to remove check if the player is in check
            if (check)
                if (!MoveRemovesCheck(board, pieceMove))
                    return false;
            
            // If all conditions are satisfied, then the move is legal
            return true;

        }

        //Checks if the board is still in a checked state after a move
        public bool MoveRemovesCheck(Board currentBoard, PieceMove move)
        {
            //Make move on temp Board Object
            Board newBoard = MakeMove(currentBoard, move);

            //Check if the move gets the turn player out of check
            return !newBoard.CheckForChecks(playerTurn);
        }

        public void MakeMoveInGame(PieceMove move)
        {
            try
            {
                board.MovePieceWithBoardSquareCoordinates(board.GetSquareByName(move.From), board.GetSquareByName(move.To));
            }
            catch (Exception e)
            {
                Console.WriteLine("Move not made, please try again");
            }
        }
        public Board MakeMove(Board chessBoard, PieceMove move)
        {
            Board changedChessBoard = chessBoard;
            changedChessBoard.MovePieceWithBoardSquareCoordinates(changedChessBoard.GetSquareByName(move.From), changedChessBoard.GetSquareByName(move.To));
            return changedChessBoard;
        } 

    }
    
    class Board
    {
        public BoardSquare[,] Squares { get; set; }

        public Board()
        {
            Squares = new BoardSquare[8, 8];
            for (int i = 0; i < Squares.GetLength(0); ++i)
            {
                for (int j = 0; j < Squares.GetLength(1); ++j)
                {
                    Squares[i, j] = new BoardSquare();
                }
            }

            InitializeSquareNames();
        }

        public BoardSquare GetSquareByName(string squareName)
        {
            foreach (BoardSquare square in Squares)
            {
                if (square.Position == squareName)
                {
                    return square;
                }
            }

            return null;
        }

        public static PieceMove StandardAlgebraicNotationParser(string move, PieceColor color)
        {
            // There are 6 sections in the algebraic chess notation
            // {Piece}{from}{takes}{to}{promotion}{check}

            PieceMove pieceMove = null;
            PieceType pieceType = PieceType.None;

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


            }

            pieceMove = new PieceMove(
                from, 
                to, 
                takes != "" ? true : false, 
                promotion, 
                check != "" ? true : false, 
                checkmate != "" ? true : false, 
                QCastle, 
                KCastle,
                new ChessPiece(pieceType, color)
                );

            return pieceMove;
        }

        public static PieceMove[] StandardAlgebraicNotationInterpreter(PieceMove move)
        {
            PieceMove[] moves = new PieceMove[1];
            
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
                return moves;
            }
            else if (move.KCastle)
            {
                fromSquare = "O-O-O";
                toSquare = "O-O-O";
                return moves;
            }
            else
            {
                return CalculateMoves(toSquare, fromSquare, move.chessPiece);
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

                    moves[1].PawnAttack = true;
                    moves[2].PawnAttack = true;

                }
                else
                {
                    moves = new PieceMove[3];
                    moves[0] = new PieceMove(toSquare[0]+(toSquareY-1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                    moves[1] = new PieceMove(Convert.ToChar(toSquare[0] - 1)+(toSquareY-1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                    moves[2] = new PieceMove(Convert.ToChar(toSquare[0] + 1)+(toSquareY-1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

                    moves[1].PawnAttack = true;
                    moves[2].PawnAttack = true;
                }
            }
            else
            {
                if (toSquareY == 5)
                {
                    moves = new PieceMove[4];
                    moves[0] = new PieceMove(toSquare[0]+"6", toSquare, false, "", false, false, false, false, chessPiece);
                    moves[1] = new PieceMove(Convert.ToChar(toSquare[0] - 1)+"6", toSquare, false, "", false, false, false, false, chessPiece);
                    moves[2] = new PieceMove(Convert.ToChar(toSquare[0] + 1)+"6", toSquare, false, "", false, false, false, false, chessPiece);
                    moves[3] = new PieceMove(toSquare[0]+"7", toSquare, false, "", false, false, false, false, chessPiece);

                    moves[1].PawnAttack = true;
                    moves[2].PawnAttack = true;
                }
                else
                {
                    moves = new PieceMove[3];
                    moves[0] = new PieceMove(toSquare[0]+(toSquareY+1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                    moves[1] = new PieceMove(Convert.ToChar(toSquare[0] - 1)+(toSquareY+1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);
                    moves[2] = new PieceMove(Convert.ToChar(toSquare[0] + 1)+(toSquareY+1).ToString(), toSquare, false, "", false, false, false, false, chessPiece);

                    moves[1].PawnAttack = true;
                    moves[2].PawnAttack = true;
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
                        switch (fromSquare.Length)
                        {
                            case 1:
                                if (moves[i].From[0] != fromSquare[0] && moves[i].From[1] != fromSquare[0])
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
        public bool CheckForChecks(PieceColor color)
        {
            BoardSquare CurrentKing = FindChessPieceOnBoard(new ChessPiece(PieceType.King, color))[0];

            for (int i = 0; i < Squares.GetLength(0); ++i)
            {
                for (int j = 0; j < Squares.GetLength(1); ++j)
                {
                    if (Squares[i, j].Piece.Color != color)
                    {
                        if (Squares[i, j].Piece.Type != PieceType.King && Squares[i, j].Piece.Type != PieceType.None)
                        {
                            if (CheckIfPieceIsAttacked(CurrentKing, Squares[i, j]))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        //Function to check if a square is in an array of squares
        //Used to check if a piece is being attacked
        //For Rooks, Bishops, and Queens, will also filter out moves if a piece is in the way. 
        public bool CheckIfPieceIsAttacked(BoardSquare pieceBeingAttacked, BoardSquare attackingPiece)
        {
            if (attackingPiece.Piece.Type == PieceType.Pawn || attackingPiece.Piece.Type == PieceType.Knight)
            {
                //Gets a list of squares that the pieces attack
                string[] AttackingSquares = attackingPiece.GetAttackingPositions();

                //Checks if the coordinates of the piece being attacked is included in any of those coordinates
                for (int i = 0; i < AttackingSquares.Length; ++i)
                {
                    if (pieceBeingAttacked.Position == AttackingSquares[i])
                    {
                        return true;
                    }
                }
            }
            else //For Bishops, Rooks, and Queens, need to consider pieces blocking the view of the attacking piece. 
            {
                //Gets a list of squares that the pieces attack
                string[] AttackingSquares = attackingPiece.GetAttackingPositions();

                //Checks if the coordinates of the piece being attacked is included in any of those coordinates
                for (int i = 0; i < AttackingSquares.Length; ++i)
                {
                    if (pieceBeingAttacked.Position == AttackingSquares[i])
                    {
                        string[] InbetweenSquares = GetInbetweenSquares(pieceBeingAttacked.Position, attackingPiece.Position);

                        for (int j = 0; j < InbetweenSquares.Length; ++i)
                        {
                            //Check if inbetween Squares are empty
                            if (GetSquareByName(InbetweenSquares[j]).Piece.Type != PieceType.None) 
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
                
            }

            return false;
        }

        //This function uses the Bresenham's line algorithm to calculate the intermediate coordinates
        private string[] GetInbetweenSquares(string square1, string square2)
        {
            //Convert the square strings into coordinates
            (int x1, int y1) = SquareNameToCoordinates(square1);
            (int x2, int y2) = SquareNameToCoordinates(square2);

            // Initialize a list to store the coordinates
            var coordinates = new List<(int x, int y)>();

            // Handle the case where the points are the same
            if (x1 == x2 && y1 == y2)
            {
                coordinates.Add((x1, y1));
            }

            // Handle the case where the points have the same x-coordinate
            else if (x1 == x2)
            {
                int start = Math.Min(y1, y2);
                int end = Math.Max(y1, y2);
                for (int y = start; y <= end; y++)
                {
                    coordinates.Add((x1, y));
                }
            }

            // Handle the case where the points have the same y-coordinate
            else if (y1 == y2)
            {
                int start = Math.Min(x1, x2);
                int end = Math.Max(x1, x2);
                for (int x = start; x <= end; x++)
                {
                    coordinates.Add((x, y1));
                }
            }

            else // Handle the case where the points have different x- and y-coordinates
            {
                int dx = Math.Abs(x2 - x1);
                int dy = Math.Abs(y2 - y1);
                int sx = x1 < x2 ? 1 : -1;
                int sy = y1 < y2 ? 1 : -1;
                int err = dx - dy;

                while (true)
                {
                    coordinates.Add((x1, y1));
                    if (x1 == x2 && y1 == y2) break;
                    int e2 = err * 2;
                    if (e2 > -dy)
                    {
                        err -= dy;
                        x1 += sx;
                    }
                    if (e2 < dx)
                    {
                        err += dx;
                        y1 += sy;
                    }
                }
            }

            string[] InbetweenSquares = new string[coordinates.Count];

            for (int i = 0; i < coordinates.Count; ++i)
            {
                (int x, int y) = coordinates.ElementAt(i);

                InbetweenSquares[0] = CoordinatesToSquareName(x, y);
            }

            return InbetweenSquares;

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
        public BoardSquare[] FindChessPieceOnBoard(ChessPiece piece)
        {
            BoardSquare[] foundPieces = new BoardSquare[8];
            int pieceCount = 0;

            for (int i = 0; i < Squares.GetLength(0); ++i)
            {
                for (int j = 0; j < Squares.GetLength(1); ++j)
                {
                    if (Squares[i, j].Piece == piece)
                    {
                        foundPieces[pieceCount] = Squares[i, j];
                        pieceCount++;
                    }
                }    
            }

            return foundPieces;
        }
        public void MovePieceWithBoardSquareCoordinates(BoardSquare from, BoardSquare to)
        {
            to.Piece = from.Piece;
            from.Piece = new ChessPiece();
        }

        //Board InitializationFunctions
        private void InitializeSquareNames()
        {
            for (int i = 0; i < Squares.GetLength(0); i++)
            {
                for (int j = 0; j < Squares.GetLength(1); j++)
                {
                    Squares[i, j].Position = CoordinatesToSquareName(j,i);
                }
            }
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

            InitializeSquareNames();
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

        //Position Name Conversion Functions - Functions to convert 'a1' to coordinates and back
        public (int x, int y) SquareNameToCoordinates(string squareName)
        {
            // Convert the square name to lowercase for easier processing
            squareName = squareName.ToLower();

            // Check that the square name is in the correct format (e.g. "a1", "h8")
            if (squareName.Length != 2 || !Char.IsLetter(squareName[0]) || !Char.IsDigit(squareName[1]))
            {
                throw new ArgumentException("Invalid square name");
            }

            // Convert the letter to an x-coordinate (a = 0, b = 1, etc.)
            int x = squareName[0] - 'a';

            // Convert the digit to a y-coordinate (1 = 7, 2 = 6, etc.)
            int y = 7 - (squareName[1] - '1');

            return (x, y);
        }
        public string CoordinatesToSquareName(int x, int y)
        {
            // Check that the coordinates are within the bounds of the chess board
            if (x < 0 || x > 7 || y < 0 || y > 7)
            {
                throw new ArgumentOutOfRangeException("Invalid coordinates");
            }

            // Convert the x-coordinate to a letter (0 = a, 1 = b, etc.)
            char letter = (char)('a' + x);

            // Convert the y-coordinate to a digit (7 = 1, 6 = 2, etc.)
            char digit = (char)('1' + (7 - y));

            return letter.ToString() + digit.ToString();
        }
        
        //Print Functions
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
        public void Print()
        {
            for (int i = 0; i < Squares.GetLength(0); i++)
            {
                for (int j = 0; j < Squares.GetLength(1); j++)
                {
                    Console.Write($"Square {i},{j}");
                    Squares[i, j].Print();
                }
            }
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
        public bool PawnAttack { get; set; }
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

        public void printMove()
        {
            Console.WriteLine($"From: {From}, To: {To}, Takes: {Takes}, Promotion: {Promotion}, Check: {Check}, Checkmate: {Checkmate}, QCastle: {QCastle}, KCastle: {KCastle}");
        }
    }

    class BoardSquare
    {
        public string Position { get; set; }
        public ChessPiece Piece { get; set; }

        public BoardSquare()
        {
            Position = "a1";
            Piece = new ChessPiece(PieceType.None, PieceColor.None);
        }
        
        public BoardSquare(string position, ChessPiece piece)
        {
            Position = position;
            Piece = piece;
        }

        //Functions to return an array of positions that can be ATTACKED by the current piece. 
        public string[] GetAttackingPositions()
        {
            string[] positions = new string[0];

            switch (Piece.Type)
            {
                case PieceType.Pawn:
                    break;
                case PieceType.Bishop:
                    break;
                case PieceType.Knight:
                    break;
                case PieceType.Rook:
                    break;
                case PieceType.Queen:
                    break;
                case PieceType.King:
                    break;

            }

            return positions;
        }
        public string[] GetPawnAttackingPositions()
        {
            string[] positions = new string[2];

            if (Piece.Color == PieceColor.White)
            {
                positions[0] = ConvertPositionString(Position, 1, 1);
                positions[0] = ConvertPositionString(Position, -1, 1);
            }
            else
            {
                positions[0] = ConvertPositionString(Position, 1, -1);
                positions[0] = ConvertPositionString(Position, -1, -1);
            }

            return CleanPositionArrayOfInvalidPositions(positions);
        }
        public string[] GetBishopAttackingPositions()
        {
            string[] positions = new string[96];

            for (int i = -4; i < 16; ++i)
            {
                positions[2*i + 8] = ConvertPositionString(Position, i, i);
            }

            for (int i = -4; i < 16; ++i)
            {
                positions[2 * i + 16] = ConvertPositionString(Position, i, -i);
            }

            for (int i = -4; i < 16; ++i)
            {
                positions[2 * i + 32] = ConvertPositionString(Position, -i, i);
            }

            for (int i = -4; i < 16; ++i)
            {
                positions[2 * i + 48] = ConvertPositionString(Position, -i, -i);
            }

            return CleanPositionArrayOfInvalidPositions(positions);
        }
        public string[] GetRookAttackingPositions()
        {
            string[] positions = new string[96];

            for (int i = -7; i < 7; ++i)
            {
                positions[i+7] = ConvertPositionString(Position, i, 0);
            }
            for (int i = -7; i <7; ++i)
            {
                positions[i+21] = ConvertPositionString(Position, 0, i);
            }

            return CleanPositionArrayOfInvalidPositions(positions);
        }
        public string[] GetKnightAttackingPositions()
        {
            string[] positions = new string[8];

            positions[0] = ConvertPositionString(Position, 2, 1);
            positions[1] = ConvertPositionString(Position, -2, 1);
            positions[2] = ConvertPositionString(Position, 2, -1);
            positions[3] = ConvertPositionString(Position, -2, -1);
            positions[4] = ConvertPositionString(Position, 1, 2);
            positions[5] = ConvertPositionString(Position, -1, 2);
            positions[6] = ConvertPositionString(Position, 1, -2);
            positions[7] = ConvertPositionString(Position,- 1, -2);

            return CleanPositionArrayOfInvalidPositions(positions);
        }
        public string[] GetKingAttackingPositions()
        {
            string[] positions = new string[8];

            positions[0] = ConvertPositionString(Position, 0, 1);
            positions[1] = ConvertPositionString(Position, 1, 0);
            positions[2] = ConvertPositionString(Position, 1, 1);

            positions[3] = ConvertPositionString(Position, 0, -1);
            positions[4] = ConvertPositionString(Position, -1, 0);
            positions[5] = ConvertPositionString(Position, -1, -1);

            positions[6] = ConvertPositionString(Position, 1, -1);
            positions[7] = ConvertPositionString(Position, -1, 1);

            return CleanPositionArrayOfInvalidPositions(positions);
        }
        public string[] GetQueenAttackingPositions()
        {
            return MergeArrays(GetBishopAttackingPositions(), GetRookAttackingPositions());
        }
        
        //Position Array Helper Functions
        public string[] CleanPositionArrayOfInvalidPositions(string[] positions)
        {
            int nullCounter = 0;
            
            for (int i = 0; i < positions.Length; ++i)
            {
                if (!CheckIfRealPosition(positions[i]))
                {
                    positions[i] = null;
                    nullCounter++;
                }
            }

            string[] newPositions = new string[positions.Length - nullCounter];
            int arrayCursor = 0;

            for (int i = 0; i < positions.Length; ++i)
            {
                if (positions[i] != null)
                {
                    newPositions[arrayCursor] = positions[i];
                    arrayCursor++;
                }
            }

            return RemoveSelfPosition(RemoveDuplicates(newPositions), this.Position);
        }
        public string[] MergeArrays(string[] arr1, string[] arr2)
        {
            string[] arr3 = new string[arr1.Length + arr2.Length];
            arr1.CopyTo(arr3, 0);
            arr2.CopyTo(arr3, arr1.Length);
            return arr3;
        }
        public string[] RemoveDuplicates(string[] arr)
        {
            System.Collections.ArrayList newList = new System.Collections.ArrayList();

            foreach (string str in arr)
                if (!newList.Contains(str))
                    newList.Add(str);
            return (string[])newList.ToArray(typeof(string));
        }
        public string[] RemoveSelfPosition(string[] arr, string selfPosition)
        {
            bool found = false;

            for (int i = 0; i < arr.Length; ++i)
            {
                if (arr[i] == selfPosition)
                {
                    arr[i] = null;
                    found = true;
                }
            }

            if (found)
            {
                string[] arr2 = new string[arr.Length - 1];
                int j = 0;

                for (int i = 0; i < arr.Length; ++i)
                {
                    if (arr[i] != null)
                    {
                        arr[i] = arr2[j];
                        j++;
                    }
                }

                return arr2;
            }

            return arr;
        }
       
        //Position String Functions
        public static string ConvertPositionString(string position, int deltaX, int deltaY)
        {
            string newPosition = "";

            try
            {
                newPosition += Convert.ToChar(Convert.ToInt32(position[0]) + deltaX);

                newPosition += Convert.ToString(Int32.Parse((position[1]) + "") + deltaY);

                return newPosition;
            }
            catch (Exception e)
            {
                return "00";
            }
        }
        public static bool CheckIfRealPosition(string position)
        {
            if (position != null)
                if ("abcdefgh".Contains(Char.ToLower(position[0])))
                    if ("12345678".Contains(position[1]))
                        if (position.Length == 2)
                            return true;
            return false;
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
