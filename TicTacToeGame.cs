using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    public class TicTacToeGame
    {
        public Board CurrentState { get; private set; }
        private Board _initialBoard;

        public TicTacToeGame()
        {
            _initialBoard = new Board();
            CurrentState = _initialBoard;
        }

        public Board GetInitialBoard()
        {
            return _initialBoard;
        }

        public void AIMove(int depth)
        {
            Board next = CurrentState.FindNextMove(depth);
            if (next != null) CurrentState = next;
        }

        public void GetNextMoveFromUser()
        {
            if (CurrentState.IsEndStateNode()) return;

            while (true)
            {
                try
                {
                    Console.WriteLine($"Current Board is\n{CurrentState}\n Please type in column:[1-3]");
                    int column = int.Parse(Console.ReadLine());
                    Console.WriteLine("Please type in row:[1-3]");
                    int row = int.Parse(Console.ReadLine());
                    Console.WriteLine($"column={column},row={row}");
                    CurrentState = CurrentState.GetChildFromMoveAtPosition(column-1, row-1);
                    Console.WriteLine(CurrentState);
                    return;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
