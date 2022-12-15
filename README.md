# 354-Final-Project

## How to Run
1. Clone the repository on your machine
2. Download Visual Studio. Make sure to include the .NET compiler with your installation.
3. Open the .sln file with Visual Studio
4. Run the program

## Milestone 1: Create a parser for Algebraic Chess Notation - 12/10
Communicating chess moves could be very simple. Simply state the square the piece is moving from, and the square the piece is moving to. However, The Standard Chess Notation (aka Algebraic Chess Notation) includes contextual information about the move, such as whether or not a piece was taken, what the piece was that took the move, and many other things relating to the game state. 

For this stage of the project, all we need to do is parse the different sections of the notation. A single move in algebraic chess notation contains multiple sections.

They are generally structured as such:

{piece}{from}{takes}{to}{promotion}{check}

### Description of each section of the ACN

The piece is one of 5 characters (K, Q, N, B, R) representing each of the pieces with the exception of the pawn, which is indicated by an omission of this section.

The from is an optional metric specified using the rank, column of both when multiple of the same piece can perform the same move. 

Takes indicates whether or not a piece is being taken by this move with an 'x' symbol.

To is the square the piece is moving to.

Promotion is indicated with an '=' followed by the piece being promoted to. 

Finally, Check is used to indicate whether or not a move puts the opponent in check, checkmate, or stalemate. with a '+' for a check, and a '#' for a checkmate or stalemate. 

There is also the unique symbol for castling. This symbol is a O-O for king-side castling and O-O-O for queen side castling. 

### Description of Submission
The process for parsing is rather simple. We simply have to identify each of the sections of a chess move in the standard notation, and consider any contingincies such as whether or not a certain section is present in this chess move. We dont have to check the validity of the move, as we dont have any context of the chess game in this stage of the interpreter. This makes our job easy, as we only need to gather the information given to us for now. 

## Milestone 2: Create an interpreter for Algebraic Chess Notation - 12/13
The interpreter takes a lot more work than the parser. The interpreter involves taking the information gathered from the parser, and generating the possible solutions to the missing information. At the current stage of the program, the piece moves lack context of the board, while the ACN move syntax involves only specifying information when ambiguous. For instance, the move Qe8 does not specify at all how the queen gets to that position because that context isnt necessary with knowledge of the board state. If there are multiple queens that CAN make that move, then which queen is specified. 

The interpreter in this program does not have context, so it will generate all possible moves that could have preceded the known move in the syntax. It does this by using rules of the game, including how the pieces move, and how many squares are on the board. The next milestone will involve putting all of this together and making a functioning chess program. 

