using System;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            bool stop = false;
            while (!stop)
            {
                var userFirst = false;
                var game = new TicTacToeGame();
                Console.WriteLine("user against computer, would you like to go first? [y/n]");
                if (Console.ReadLine().StartsWith("y", StringComparison.InvariantCultureIgnoreCase)) userFirst = true;
                int depth = 8;
                Console.WriteLine("Please select a difficulty from 1-8, with 8 being the hardest");
                int.TryParse(Console.ReadLine(), out depth);
                Console.WriteLine("You have chosen to go {0} against a level {1} AI", userFirst ? "first" : "second", depth);
                while (!game.CurrentState.IsEndStateNode())
                {
                    if (userFirst)
                    {
                        game.GetNextMoveFromUser();
                        game.AIMove(depth);
                    }
                    else
                    {
                        game.AIMove(depth);
                        game.GetNextMoveFromUser();
                    }
                }

                Console.WriteLine("The final state of the game is:");
                Console.WriteLine(game.CurrentState);
                if (game.CurrentState.RecursiveScore > 200) Console.WriteLine("Player X has won.");
                else if (game.CurrentState.RecursiveScore < -200) Console.WriteLine("Player O has won.");
                else { Console.WriteLine("It's a tie!"); }
                Console.WriteLine("Would you like to try again? [y/n]");
                if (!Console.ReadLine().StartsWith("y", StringComparison.InvariantCultureIgnoreCase)) stop = true;

            }
            Console.WriteLine("See you next time.");
        }
    }
}
