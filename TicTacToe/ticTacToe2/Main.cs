using System;

namespace ticTacToe2 {
    class Program {
        static void Main(string[] args) {
            var algo = new Algo();
            Graphics.PrintWelcomeAndGetParams();
            var input = "";
            do {
                Console.WriteLine("Do u want to start ('yes/'no')? ");
                input = Console.ReadLine();
                bool user_start = (input == "yes") ? true : false;
                algo.startGame(user_start, ref input);
            } while (input != "quit");
        }
    }
}
