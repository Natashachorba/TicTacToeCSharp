using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public class Board
    {
        private GridEntry[] _grid;
        private int _score;
        private bool _xIsCurrentPlayer;//0 for O, 1 for X
        public int RecursiveScore { get; private set; }
        public bool GameOver { get; private set; }

        public Board()
        {
            _grid = Enumerable.Repeat(GridEntry.Empty, 9).ToArray();
            _xIsCurrentPlayer = true;
            ComputeScore();
        }

        public Board(GridEntry[] values, bool turnForX)
        {
            _xIsCurrentPlayer = turnForX;
            _grid = values;
            ComputeScore();
        }

        #region printing
        private char GetGridCharValue(GridEntry entry)
        {
            char value = ' ';
            if (entry == GridEntry.PlayerO) value = 'O';
            else if (entry == GridEntry.PlayerX) value = 'X';
            return value;
        }

        /*public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GridEntry v = _grid[i * 3 + j];
                    char c = '-';
                    if (v == GridEntry.PlayerX)
                        c = 'X';
                    else if (v == GridEntry.PlayerO)
                        c = 'O';
                    sb.Append(c);
                }
                sb.Append('\n');
            }
            sb.AppendFormat("score={0},ret={1},{2}", _score, RecursiveScore, _xIsCurrentPlayer);
            return sb.ToString();
        }*/

        public override string ToString()
        {
            var printableBoard = new StringBuilder();
            printableBoard.Append("   |   |   \n");
            printableBoard.Append($" {GetGridCharValue(_grid[0])} | {GetGridCharValue(_grid[1])} | {GetGridCharValue(_grid[2])}\n");
            printableBoard.Append("___|___|___\n");
            printableBoard.Append("   |   |   \n");
            printableBoard.Append($" {GetGridCharValue(_grid[3])} | {GetGridCharValue(_grid[4])} | {GetGridCharValue(_grid[5])}\n");
            printableBoard.Append("___|___|___\n");
            printableBoard.Append("   |   |   \n");
            printableBoard.Append($" {GetGridCharValue(_grid[6])} | {GetGridCharValue(_grid[7])} | {GetGridCharValue(_grid[8])}\n");
            printableBoard.Append("   |   |   \n");
            //printableBoard.AppendFormat("score={0},ret={1},{2}", _score, RecursiveScore, _xIsCurrentPlayer);
            return printableBoard.ToString();
        }
        #endregion printing

        public Board GetChildFromMoveAtPosition(int column, int row)
        {
            if (column>2 || column<0 || row > 2 || row<0) {
                throw new Exception($"Invalid Index! [{column},{row} is outside the board!");
            }
            int flatPosition = row * 3 + column;

            if (_grid[flatPosition] != GridEntry.Empty)
            {
                throw new Exception($"Invalid Index! [{column},{row}] is taken by {_grid[flatPosition]}!");
            }

            var newGridValues = (GridEntry[])_grid.Clone();
            newGridValues[flatPosition] = _xIsCurrentPlayer? GridEntry.PlayerX : GridEntry.PlayerO;
            return new Board(newGridValues, !_xIsCurrentPlayer);
        }

        public bool IsEndStateNode()
        {
            if (GameOver) return true;
            foreach(GridEntry entry in _grid)
            {
                if (entry == GridEntry.Empty) return false;
            }
            return true;
        }

        #region minMax
        private int GetScoreForOneLine(GridEntry[] values)
        {
            var countX = 0;var countO = 0;
            foreach(GridEntry v in values)
            {
                if (v == GridEntry.PlayerX) countX++;
                else if (v == GridEntry.PlayerO) countO++;
            }

            if (countO == 3 || countX == 3)
            {
                GameOver = true;
            }

            int advantage = 1;
            
            if (countO == 0)
            {
                if (_xIsCurrentPlayer) advantage = 3;
                return (int)Math.Pow(10, countX) * advantage;
            }
            else if (countX == 0)
            {
                if (!_xIsCurrentPlayer) advantage = 3;
                return -(int)Math.Pow(10, countO) * advantage;
            }
            return 0;
        }

        void ComputeScore()
        {
            var currScore = 0;
            int[,] lines = {{ 0, 1, 2 },
                            { 3, 4, 5 },
                            { 6, 7, 8 },
                            { 0, 3, 6 },
                            { 1, 4, 7 },
                            { 2, 5, 8 },
                            { 0, 4, 8 },
                            { 2, 4, 6 }};
            for (int i = lines.GetLowerBound(0); i<= lines.GetUpperBound(0); i++)
            {
                currScore += GetScoreForOneLine(new GridEntry[] { _grid[lines[i,0]], _grid[lines[i, 1]], _grid[lines[i, 2]] });
            }

            _score = currScore;
        }

        public IEnumerable<Board> GetChildren()
        {
            for(int i = 0; i < _grid.Length; i++)
            {
                if(_grid[i] == GridEntry.Empty)
                {
                    var newGridValues = (GridEntry[])_grid.Clone();
                    newGridValues[i] = _xIsCurrentPlayer ? GridEntry.PlayerX : GridEntry.PlayerO;
                    yield return new Board(newGridValues, !_xIsCurrentPlayer);
                }
            }
        }

        public int ABPruning(int depth, int alpha, int beta, out Board bestScoredChild)
        {
            bestScoredChild = null;
            if (depth == 0 || IsEndStateNode())
            {
                RecursiveScore = _score;
                return _score;
            }
            foreach(Board currChild in GetChildren())
            {
                Board dummy;
                int score = currChild.ABPruning(depth - 1, alpha, beta, out dummy);
                if (_xIsCurrentPlayer)
                {
                    if (score > alpha)
                    {
                        alpha = score;
                        bestScoredChild = currChild;
                    }
                }
                else
                {
                    if (score < beta)
                    {
                        beta = score;
                        bestScoredChild = currChild;
                    }
                }
                if(alpha >= beta) break;
            }
            RecursiveScore = _xIsCurrentPlayer ? alpha : beta;
            return RecursiveScore;
        }

        public Board FindNextMove(int depth)
        {
            Board idealNextBoard = null;
            ABPruning(depth, int.MinValue + 1, int.MaxValue - 1, out idealNextBoard);
            return idealNextBoard;
        }
        #endregion minMax
    }
}
