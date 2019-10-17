using System;
using static ticTacToe2.Uti;

namespace ticTacToe2 {
    public static class Graphics {
        public static void PrintBoard() {
            for (var i = 0; i < BOARD_SIZE; i++) {
                Console.WriteLine();
                for (var j = 0; j < BOARD_SIZE; j++) {
                    var cell = "";
                    if (BOARD[i][j] == EMPTYSYMBOL)
                        cell = " ";
                    else if (BOARD[i][j] == EX)
                        cell = "X";
                    else
                        cell = "O";
                    if (j < BOARD_SIZE - 1)
                        Console.Write(" " + cell + " |");
                    else
                        Console.Write(" " + cell + " ");
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
            Console.WriteLine("WELCOME TO TicTacToe!!!\nTo QUIT the gameat any time, please enter 'quit'.");
            Console.WriteLine("This game use basic MinMax algorithm to mimic basic a.i.");
            Console.WriteLine("Please enter the board size (3=3*3,4=4*4):");
            BOARD_SIZE = int.Parse(Console.ReadLine());
            Console.WriteLine("Please enter the STRAIGHT (retzef) needed to win:");
            STRAIGHT = int.Parse(Console.ReadLine());
            //Console.WriteLine("Please enter the MAX DEPTH for minMax algo (big board with big MAX DEPTH can take very long time (even years!!!): ");
            //MAXDEPTH = int.Parse(Console.ReadLine());
            Console.WriteLine("Please enter the MAX TIME (seconds) for move calculation (30-120 recomended for big boards): ");
            MAXTIME = double.Parse(Console.ReadLine());
        }
    }
}

