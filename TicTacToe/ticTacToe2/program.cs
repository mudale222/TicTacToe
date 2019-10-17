using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ticTacToe2 {
    class Program {
        static int BOARD_SIZE = 4;
        static string[][] BOARD;
        static int STRAIGHT = 3;
        static string USERSYMBOL = "";
        static string COMPUTERSYMBOL = "";
        static int MINMAXCOUNTER = 0;
        static int BIGGESTDEPTH = 0;
        static int MAXDEPTH = 11;
        public enum Score {
            Win = 100,
            Draw = 0,
            Loose = -100,
            TooDeep = 666
        }
        public struct Move {
            public int x;
            public int y;
            public Score score;
            public Move(int x, int y, Score score) {
                this.x = x; this.y = y; this.score = score;
            }
        }


        static void Main(string[] args) {
            var algo = new Algo();
            Graphic.PrintWelcomeAndGetParams();
            var input = "";
            do {
                Console.WriteLine("Do u want to start ('yes/'no')? ");
                input = Console.ReadLine();
                bool user_start = (input == "yes") ? true : false;
                algo.startGame(user_start, ref input);
            } while (input != "quit");
            //   Graphic.PrintBoard();
        }

        class Graphic {
            public static void PrintBoard() {
                for (var i = 0; i < BOARD_SIZE; i++) {
                    Console.WriteLine();
                    for (var j = 0; j < BOARD_SIZE; j++) {
                        if (j < BOARD_SIZE - 1)
                            Console.Write(" " + BOARD[i][j] + " |");
                        else
                            Console.Write(" " + BOARD[i][j] + " ");

                    }
                    Console.WriteLine();
                    if (i < BOARD_SIZE - 1) {
                        for (var k = 0; k < BOARD_SIZE * 4 - 1; k++)
                            Console.Write((char)95);
                        Console.WriteLine();
                    }
                }
                Console.WriteLine();
            }

            public static void PrintWelcomeAndGetParams() {
                Console.WriteLine("WELCOME TO TicTacToe!!!");//\nTo quit the game please enter 'quit'.");
                Console.WriteLine("This game use MinMax algorithm to solve!!");
                Console.WriteLine("Please enter the board size (3=3*3,4=4*4):");
                BOARD_SIZE = int.Parse(Console.ReadLine());
                Console.WriteLine("Please enter the STRAIGHT (retzef) needed to win:");
                STRAIGHT = int.Parse(Console.ReadLine());
                Console.WriteLine("Please enter the MAX DEPTH for minMax algo (big board with big MAX DEPTH can take very long time (even years!!!): ");
                MAXDEPTH = int.Parse(Console.ReadLine());
            }
        }

        class Algo {
            static bool ENDGAME = false;
            public Algo() {
                MakeNewBoard();
            }

            private void MakeNewBoard() {
                BOARD = new string[BOARD_SIZE][];
                for (var i = 0; i < BOARD.Length; i++)
                    BOARD[i] = new string[BOARD_SIZE];
                for (var i = 0; i < BOARD_SIZE; i++) {
                    for (var j = 0; j < BOARD_SIZE; j++) {
                        BOARD[i][j] = " ";
                    }
                }
            }

            public void startGame(bool user_first, ref string input) {
                MakeNewBoard();
                ENDGAME = false;
                if (user_first) {
                    USERSYMBOL = "X"; COMPUTERSYMBOL = "O";
                    while (!ENDGAME) {
                        handleUser(ref input);
                        if (!ENDGAME)
                            handleComputer();
                    }
                }
                else {
                    USERSYMBOL = "O"; COMPUTERSYMBOL = "X";
                    while (!ENDGAME) {
                        handleComputer();
                        if (!ENDGAME)
                            handleUser(ref input);
                    }
                }
            }

            private bool BoardFull() {
                for (var i = 0; i < BOARD.Length; i++)
                    for (var j = 0; j < BOARD[i].Length; j++)
                        if (BOARD[i][j] == " ")
                            return false;
                return true;
            }

            private void EndGame(string XorOorDraw) {
                Graphic.PrintBoard();
                if (XorOorDraw == USERSYMBOL)
                    Console.WriteLine("OH U BRAVE HERO!! U WON!!!! CONGRAT!!!");
                else if (XorOorDraw == COMPUTERSYMBOL)
                    Console.WriteLine("OH U POOR BASTARD!!! BETTER LUCK NEXT TIME!!!");
                else if (XorOorDraw == "draw")
                    Console.WriteLine("JESUS! WHO COULD ANTICIPATE IT???!! DRAW!");
                Graphic.PrintBoard();
                ENDGAME = true;
                MakeNewBoard();
            }

            public void handleUser(ref string input) {
                Graphic.PrintBoard();
                Console.WriteLine("Please enter location for move (for example: 1 3 = x-1 y-3): ");
                var user_input = Console.ReadLine();
                if (user_input.Length == 3 &&
                    int.TryParse(user_input[0].ToString(), out int y) && int.TryParse(user_input[2].ToString(), out int x)
                   && (x >= 0 && y >= 0 && x < BOARD_SIZE && y < BOARD_SIZE) && BOARD[x][y] == " ")
                    BOARD[x][y] = USERSYMBOL.ToString();
                else {
                    if (user_input == "quit") {
                        input = user_input;
                        ENDGAME = true;
                        return;
                    }
                    else {
                        Console.WriteLine("Worng input!!! try again...");
                        handleUser(ref input);
                    }
                }
                // CheckEndGame();
                var res = CheckEndGame();
                if (res != -3) {
                    if (/*res > 0*/ res == (int)Score.Win)
                        EndGame(USERSYMBOL);
                    else if (/*res < 0*/ res == (int)Score.Loose)
                        EndGame(COMPUTERSYMBOL);
                    else
                        EndGame("draw");
                }
            }

            public void handleComputer() {
                int x = -1, y = -1;
                if (CanWin(ref x, ref y)) {
                    BOARD[x][y] = COMPUTERSYMBOL;
                    Console.WriteLine("Computer -   X:  " + y + "    Y:  " + x);
                }
                else if (AvoidLoose(ref x, ref y)) {
                    BOARD[x][y] = COMPUTERSYMBOL;
                    Console.WriteLine("Computer -   X:  " + y + "    Y:  " + x);
                }
                else
                    TryWin();
                var res = CheckEndGame();
                if (res != -3) {
                    if (res > 0)
                        EndGame(COMPUTERSYMBOL);
                    else if (res < 0)
                        EndGame(USERSYMBOL);
                    else
                        EndGame("draw");
                }
            }

            private void TryWin() {
                if (OnlyOneLeft(out int x, out int y)) {
                    BOARD[x][y] = COMPUTERSYMBOL;
                    return;
                }
                var depth = 0;
                MINMAXCOUNTER = 0;
                BIGGESTDEPTH = 0;
                var move = MiniMax(BOARD, depth, COMPUTERSYMBOL);
                Console.WriteLine("\rMOVE:   X - " + move.x + "    Y - " + move.y + "   Score: " + move.score + "    MinMaxCounter: " + MINMAXCOUNTER + "  Biggest depth: " + BIGGESTDEPTH);
                BOARD[move.x][move.y] = COMPUTERSYMBOL;
            }

            private Move MiniMax(string[][] bOARD, int depth, string whosTurn) {
                MINMAXCOUNTER++;
                if (MINMAXCOUNTER % 100000 == 0)
                    Console.Write("\rCalculation: {0}", MINMAXCOUNTER.ToString("N0"));
                if (depth > BIGGESTDEPTH)
                    BIGGESTDEPTH++;
                if (depth >= MAXDEPTH)
                    //  if (whosTurn==COMPUTERSYMBOL)
                    return new Move(-5, -5, Score.Draw);

                var checkEndGameResult = CheckEndGame();
                if (checkEndGameResult != -3) {
                    if (checkEndGameResult == (int)Score.Win)
                        return new Move(-2, -2, Score.Win);
                    else if (checkEndGameResult == (int)Score.Loose)
                        return new Move(-2, -2, Score.Loose);
                    else if (checkEndGameResult == (int)Score.Draw)
                        return new Move(-2, -2, Score.Draw);
                }

                if (whosTurn == COMPUTERSYMBOL) {
                    var best_move = new Move();
                    best_move.score = Score.Loose;
                    for (var i = 0; i < BOARD.Length; i++) {
                        for (var j = 0; j < BOARD[i].Length; j++) {
                            if (BOARD[i][j] == " ") {
                                BOARD[i][j] = COMPUTERSYMBOL;
                                var temp_move = MiniMax(BOARD, depth + 1, USERSYMBOL);
                                if ((int)temp_move.score >= (int)best_move.score) {
                                    best_move.score = temp_move.score;
                                    best_move.x = i; best_move.y = j;
                                }
                                BOARD[i][j] = " ";
                                if (best_move.score == Score.Win)
                                    return best_move;
                            }
                        }
                    }
                    return best_move;
                }

                else {
                    var best_move = new Move();
                    best_move.score = Score.Win;
                    for (var i = 0; i < BOARD.Length; i++) {
                        for (var j = 0; j < BOARD[i].Length; j++) {
                            if (BOARD[i][j] == " ") {
                                BOARD[i][j] = USERSYMBOL;
                                var temp_move = MiniMax(BOARD, depth + 1, COMPUTERSYMBOL);
                                if (temp_move.score < best_move.score) {
                                    best_move.score = temp_move.score;
                                    best_move.x = i; best_move.y = j;
                                }
                                BOARD[i][j] = " ";
                                if (best_move.score == Score.Loose)
                                    return best_move;
                            }
                        }
                    }
                    return best_move;
                }
            }

            private bool OnlyOneLeft(out int x, out int y) {
                var counter = 0;
                x = -1; y = -1;
                for (var i = 0; i < BOARD.Length; i++) {
                    for (var j = 0; j < BOARD[i].Length; j++) {
                        if (BOARD[i][j] == " ") {
                            x = i; y = j;
                            counter++;
                        }
                    }
                }
                if (counter == 1)
                    return true;
                return false;
            }

            private int CheckEndGame() {
                int computerTurns = 0, userTurns = 0;
                for (var i = 0; i < BOARD.Length; i++)
                    for (var j = 0; j < BOARD.Length; j++)
                        if (BOARD[i][j] == COMPUTERSYMBOL)
                            computerTurns++;
                        else if (BOARD[i][j] == USERSYMBOL)
                            userTurns++;

                if (computerTurns >= STRAIGHT)
                    if (CheckEndHorizontal(COMPUTERSYMBOL, STRAIGHT) || CheckEndVertical(COMPUTERSYMBOL, STRAIGHT) || CheckEndDiagonal(COMPUTERSYMBOL, STRAIGHT)) {
                        /// EndGame(USERSYMBOL);
                        return (int)Score.Loose;
                    }
                if (userTurns >= STRAIGHT)
                    if (CheckEndHorizontal(USERSYMBOL, STRAIGHT) || CheckEndVertical(USERSYMBOL, STRAIGHT) || CheckEndDiagonal(USERSYMBOL, STRAIGHT)) {
                        //EndGame(COMPUTERSYMBOL);
                        return (int)Score.Win;
                    }
                if (BoardFull()) {
                    //EndGame("draw");
                    return (int)Score.Draw;
                }
                return -3;
            }

            private bool AvoidLoose(ref int x, ref int y) {
                if (CheckHorizontal(ref x, ref y, USERSYMBOL, STRAIGHT) || CheckVertical(ref x, ref y, USERSYMBOL, STRAIGHT) || CheckDiagonal(ref x, ref y, USERSYMBOL, STRAIGHT))
                    return true;
                return false;
            }

            public bool CanWin(ref int x, ref int y) {
                if (CheckHorizontal(ref x, ref y, COMPUTERSYMBOL, STRAIGHT) || CheckVertical(ref x, ref y, COMPUTERSYMBOL, STRAIGHT) || CheckDiagonal(ref x, ref y, COMPUTERSYMBOL, STRAIGHT))
                    return true;
                return false;
            }


            private bool CheckEndHorizontal(string XO, int length) {
                for (var i = 0; i < BOARD.Length; i++) {
                    if (CheckLineForStraight(BOARD[i], XO, length)) {
                        return true;
                    }
                }
                return false;
            }

            private bool CheckEndDiagonal(string XO, int length) {
                //right diagonal
                //BOARD.Length - length to jump over all not possible diagonal
                for (var i = 0; i <= BOARD.Length - length; i++) {
                    for (var j = 0; j <= BOARD[i].Length - length; j++) {
                        var line = new string[BOARD_SIZE];
                        var counter = 0;
                        for (int k = i, l = j; k < BOARD.Length && l < BOARD[k].Length; k++, l++) {
                            line[counter] = BOARD[k][l];
                            counter++;

                        }
                        if (CheckLineForStraight(line, XO, length))
                            return true;
                    }
                }
                //left diagonal
                // Math.Abs((i + 1) - BOARD_SIZE)... this is the reflection of the right diagonal reduction of possibilites. Catch only the corner.
                for (var i = 0; i <= BOARD.Length - length; i++) {
                    for (var j = 0; j < BOARD[i].Length; j++) {
                        if (j < (BOARD_SIZE - (BOARD_SIZE - length)) - 1)
                            continue;
                        var line = new string[BOARD_SIZE];
                        var counter = 0;
                        for (int k = i, l = j; k < BOARD.Length && l >= 0; k++, l--) {
                            line[counter] = BOARD[k][l];
                            counter++;
                        }
                        if (CheckLineForStraight(line, XO, length))
                            return true;
                    }
                }
                return false;
            }

            public bool CheckHorizontal(ref int x, ref int y, string XO, int length) {
                for (var i = 0; i < BOARD.Length; i++) {
                    if (CheckLineForStraightNextTurn(BOARD[i], XO, length, ref y)) {
                        x = i;
                        return true;
                    }
                }
                return false;
            }

            private bool CheckLineForStraightNextTurn(string[] line, string XO, int length, ref int stepsX) {
                for (var i = 0; i < line.Length; i++) {
                    if (line[i] == " ") {
                        line[i] = XO;
                        var counter = 0;
                        for (var j = 0; j < line.Length; j++) {
                            if (line[j] == XO)
                                counter++;
                            else
                                counter = 0;
                            if (counter == length) {
                                line[i] = " ";
                                stepsX = i;
                                return true;
                            }
                        }
                        line[i] = " ";
                    }
                }
                return false;
            }

            private bool CheckLineForStraight(string[] line, string XO, int length) {
                var total = 0;
                var biggest_total = 0;
                if (line.Length < length)
                    throw new NotImplementedException();//////////////////////////////////////////////////////////////////////////
                for (var i = 0; i < line.Length; i++) {
                    if (line[i] == XO) {
                        total++;
                        if (total > biggest_total)
                            biggest_total = total;
                    }
                    else
                        total = 0;

                }
                if (biggest_total == length)
                    return true;
                return false;
            }

            private bool CheckEndVertical(string XO, int length) {
                string[] line = new string[BOARD_SIZE];
                for (var i = 0; i < BOARD.Length; i++) {
                    var counter = 0;
                    for (var j = 0; j < BOARD[i].Length; j++) {
                        line[counter] = BOARD[j][i];
                        counter++;
                    }
                    if (CheckLineForStraight(line, XO, length)) {
                        return true;
                    }
                }
                return false;
            }

            public bool CheckVertical(ref int x, ref int y, string XO, int length) {
                string[] line = new string[BOARD_SIZE];
                for (var i = 0; i < BOARD.Length; i++) {
                    var counter = 0;
                    for (var j = 0; j < BOARD[i].Length; j++) {
                        line[counter] = BOARD[j][i];
                        counter++;
                    }
                    if (CheckLineForStraightNextTurn(line, XO, length, ref x)) {
                        y = i;
                        return true;
                    }
                }
                return false;
            }

            public bool CheckDiagonal(ref int x, ref int y, string XO, int length) {
                //right diagonal
                for (var i = 0; i < BOARD.Length; i++) {
                    for (var j = 0; j < BOARD[i].Length; j++) {
                        var counter = 0;
                        string[] line = new string[BOARD_SIZE];
                        for (int k = i, l = j; k < BOARD.Length && l < BOARD[k].Length; k++, l++) {
                            line[counter] = BOARD[k][l];
                            counter++;
                        }
                        if (CheckLineForStraightNextTurn(line, XO, length, ref x)) {
                            var steps = x;
                            x = i + steps;
                            y = j + steps;
                            //y = i + x;
                            return true;
                        }
                    }
                }
                //left diagonal
                for (var i = 0; i < BOARD.Length; i++) {
                    for (var j = 0; j < BOARD[i].Length; j++) {
                        var counter = 0;
                        string[] line = new string[BOARD_SIZE];
                        for (int k = i, l = j; k < BOARD.Length && l >= 0; k++, l--) {
                            line[counter] = BOARD[k][l];
                            counter++;
                        }

                        if (CheckLineForStraightNextTurn(line, XO, length, ref x)) {
                            var steps = x;
                            x = i + steps;
                            y = j - steps;
                            return true;
                        }
                    }
                }

                return false;
            }

        }
    }
}
