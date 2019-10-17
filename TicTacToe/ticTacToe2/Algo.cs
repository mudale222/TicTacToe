using System;
using System.Diagnostics;
using static ticTacToe2.Uti;

namespace ticTacToe2 {
    class Algo {
        public enum Score {
            Win = 100,
            Draw = 0,
            Loose = -100,
            TooDeep = -1
        }
        public struct Move {
            public int x;
            public int y;
            public Score score;
            public Move(int x, int y, Score score) {
                this.x = x; this.y = y; this.score = score;
            }
        }

        static bool ENDGAME = false;
        public Algo() {
            MakeNewBoard();
        }

        private void MakeNewBoard() {
            BOARD = new int[BOARD_SIZE][];
            for (var i = 0; i < BOARD.Length; i++)
                BOARD[i] = new int[BOARD_SIZE];
            for (var i = 0; i < BOARD_SIZE; i++) {
                for (var j = 0; j < BOARD_SIZE; j++) {
                    BOARD[i][j] = 0;
                }
            }
        }

        public void startGame(bool user_first, ref string input) {
            MakeNewBoard();
            ENDGAME = false;
            MAXTIMEPERCELL = MAXTIME / (BOARD.Length * BOARD.Length);
            if (user_first) {
                USERSYMBOL = EX; COMPUTERSYMBOL = IGOL;
                while (!ENDGAME) {
                    handleUser(ref input);
                    if (!ENDGAME)
                        handleComputer();
                }
            }
            else {
                USERSYMBOL = IGOL; COMPUTERSYMBOL = EX;
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
                    if (BOARD[i][j] == EMPTYSYMBOL)
                        return false;
            return true;
        }

        private void EndGame(int XorOorDraw) {
            Graphics.PrintBoard();
            if (XorOorDraw == USERSYMBOL)
                Console.WriteLine("OH U BRAVE HERO!! U WON!!!! CONGRAT!!!");
            else if (XorOorDraw == COMPUTERSYMBOL)
                Console.WriteLine("OH U POOR BASTARD!!! BETTER LUCK NEXT TIME!!!");
            else if (XorOorDraw == 3)
                Console.WriteLine("JESUS! WHO COULD ANTICIPATE IT???!! DRAW!");
            Graphics.PrintBoard();
            ENDGAME = true;
            MakeNewBoard();
        }

        public void handleUser(ref string input) {
            Graphics.PrintBoard();
            Console.WriteLine("Please enter location for move (for example: 1 3 = x-1 y-3): ");
            var user_input = Console.ReadLine();
            if (user_input.Length == 3 &&
                int.TryParse(user_input[0].ToString(), out int y) && int.TryParse(user_input[2].ToString(), out int x)
               && (x >= 0 && y >= 0 && x < BOARD_SIZE && y < BOARD_SIZE) && BOARD[x][y] == EMPTYSYMBOL)
                BOARD[x][y] = USERSYMBOL;
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
                    EndGame(3);
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
            var res = CheckEndGame();//reurn Score.loose (-100) if computer win
            if (res != -3) {
                if (res < 0)
                    EndGame(COMPUTERSYMBOL); //get parameter who win
                else if (res > 0)
                    EndGame(USERSYMBOL);
                else
                    EndGame(3);
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
            clock.Restart();
            var move = MiniMax(BOARD, depth, COMPUTERSYMBOL, 0, 0);
            clock.Reset();
            Console.WriteLine("\rMOVE:   X - " + move.y + "    Y - " + move.x + "   Score: " + move.score + "    MinMaxCounter: " + MINMAXCOUNTER + "  Biggest depth: " + BIGGESTDEPTH);
            BOARD[move.x][move.y] = COMPUTERSYMBOL;
        }

        static Stopwatch clock = new Stopwatch();
        private Move MiniMax(int[][] bOARD, int depth, int whosTurn, int oroginalI, int originalJ) {
            MINMAXCOUNTER++;
            if (MINMAXCOUNTER % 100000 == 0)
                Console.Write("\rCalculation: {0}", MINMAXCOUNTER.ToString("N0"));
            if (depth > BIGGESTDEPTH)
                BIGGESTDEPTH++;
            //if (depth >= MAXDEPTH)
            //  if (whosTurn==COMPUTERSYMBOL)
            //   return new Move(-5, -5, Score.TooDeep);

            var checkEndGameResult = CheckEndGame();//user view, computer win = loose(-100)
            if (checkEndGameResult != -3) {
                if (checkEndGameResult == (int)Score.Win)
                    return new Move(-2, -2, Score.Loose);
                else if (checkEndGameResult == (int)Score.Loose)
                    return new Move(-2, -2, Score.Win);
                else if (checkEndGameResult == (int)Score.Draw)
                    return new Move(-2, -2, Score.Draw);
            }

            if (whosTurn == COMPUTERSYMBOL) {
                var best_move = new Move();
                best_move.score = Score.Loose;
                for (var i = 0; i < BOARD.Length; i++) {
                    for (var j = 0; j < BOARD[i].Length; j++) {
                        if (BOARD[i][j] == EMPTYSYMBOL) {
                            BOARD[i][j] = COMPUTERSYMBOL;
                            var temp_move = new Move(-5, -5, Score.Loose);
                            if (depth == 0)
                                temp_move = MiniMax(BOARD, depth + 1, USERSYMBOL, i, j);
                            else
                                temp_move = MiniMax(BOARD, depth + 1, USERSYMBOL, oroginalI, originalJ);
                            if ((int)temp_move.score >= (int)best_move.score) {
                                best_move.score = temp_move.score;
                                best_move.x = i; best_move.y = j;
                            }
                            BOARD[i][j] = 0;
                            if (best_move.score == Score.Win)
                                return best_move;
                            if (clock.Elapsed.TotalSeconds >= ((oroginalI + 1) + (originalJ * BOARD.Length)) * MAXTIMEPERCELL)
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
                        if (BOARD[i][j] == EMPTYSYMBOL) {
                            BOARD[i][j] = USERSYMBOL;
                            var temp_move = new Move(-5, -5, Score.Loose);
                            if (depth == 0)
                                temp_move = MiniMax(BOARD, depth + 1, COMPUTERSYMBOL, i, j);
                            else
                                temp_move = MiniMax(BOARD, depth + 1, COMPUTERSYMBOL, oroginalI, originalJ);

                            if (temp_move.score < best_move.score) {
                                best_move.score = temp_move.score;
                                best_move.x = i; best_move.y = j;
                            }
                            BOARD[i][j] = 0;
                            if (best_move.score == Score.Loose)
                                return best_move;
                            if (clock.Elapsed.TotalSeconds >= ((oroginalI + 1) + (originalJ * BOARD.Length)) * MAXTIMEPERCELL)
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
                    if (BOARD[i][j] == EMPTYSYMBOL) {
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

        private bool CheckEndHorizontal(int XO, int length) {
            for (var i = 0; i < BOARD.Length; i++) {
                if (CheckLineForStraight(BOARD[i], XO, length)) {
                    return true;
                }
            }
            return false;
        }

        private bool CheckEndDiagonal(int XO, int length) {
            //right diagonal
            //BOARD.Length - length to jump over all not possible diagonal
            for (var i = 0; i <= BOARD.Length - length; i++) {
                for (var j = 0; j <= BOARD[i].Length - length; j++) {
                    var line = new int[BOARD_SIZE];
                    for (var q = 0; q < line.Length; q++)
                        line[q] = -1;
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
            //if (j < (BOARD_SIZE - (BOARD_SIZE - length)) - 1), this is the reflection of the right diagonal reduction of possibilites. Catch only the corner.
            for (var i = 0; i <= BOARD.Length - length; i++) {
                for (var j = 0; j < BOARD[i].Length; j++) {
                    if (j < (BOARD_SIZE - (BOARD_SIZE - length)) - 1)
                        continue;
                    var line = new int[BOARD_SIZE];
                    for (var q = 0; q < line.Length; q++)
                        line[q] = -1;
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

        public bool CheckHorizontal(ref int x, ref int y, int XO, int length) {
            for (var i = 0; i < BOARD.Length; i++) {
                if (CheckLineForStraightNextTurn(BOARD[i], XO, length, ref y)) {
                    x = i;
                    return true;
                }
            }
            return false;
        }

        private bool CheckLineForStraightNextTurn(int[] line, int XO, int length, ref int stepsX) {
            for (var i = 0; i < line.Length; i++) {
                if (line[i] == EMPTYSYMBOL) {
                    line[i] = XO;
                    var counter = 0;
                    for (var j = 0; j < line.Length; j++) {
                        if (line[j] == XO)
                            counter++;
                        else
                            counter = 0;
                        if (counter == length) {
                            line[i] = 0;
                            stepsX = i;
                            return true;
                        }
                    }
                    line[i] = EMPTYSYMBOL;
                }
            }
            return false;
        }

        private bool CheckLineForStraight(int[] line, int XO, int length) {
            var total = 0;
            var biggest_total = 0;
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

        private bool CheckEndVertical(int XO, int length) {
            var line = new int[BOARD_SIZE];
            for (var q = 0; q < line.Length; q++)
                line[q] = -1;
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

        public bool CheckVertical(ref int x, ref int y, int XO, int length) {
            var line = new int[BOARD_SIZE];
            for (var q = 0; q < line.Length; q++)
                line[q] = -1;
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

        public bool CheckDiagonal(ref int x, ref int y, int XO, int length) {
            //right diagonal
            for (var i = 0; i <= BOARD.Length - length; i++) {
                for (var j = 0; j <= BOARD[i].Length - length; j++) {
                    var counter = 0;
                    var line = new int[BOARD_SIZE];
                    for (var q = 0; q < line.Length; q++)
                        line[q] = -1;
                    for (int k = i, l = j; k < BOARD.Length && l < BOARD[k].Length; k++, l++) {
                        line[counter] = BOARD[k][l];
                        counter++;
                    }
                    if (CheckLineForStraightNextTurn(line, XO, length, ref x)) {
                        var steps = x;
                        x = i + steps;
                        y = j + steps;
                        return true;
                    }
                }
            }
            //left diagonal
            for (var i = 0; i <= BOARD.Length - length; i++) {
                for (var j = 0; j < BOARD[i].Length; j++) {
                    if (j < (BOARD_SIZE - (BOARD_SIZE - length)) - 1)
                        continue;
                    var counter = 0;
                    var line = new int[BOARD_SIZE];
                    for (var q = 0; q < line.Length; q++)
                        line[q] = -1;
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

