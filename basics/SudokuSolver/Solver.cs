using System.Globalization;

namespace SudokuSolver;

public class Solver
{
    private readonly IBoard _board;

    public Solver(IBoard board)
    {
        _board = board;
    }

    public bool IsValid()
    {
        int size = _board.Size;
        var usedSet = new HashSet<int>();
        for (int row = 0; row < size; row++)
        {
            usedSet.Clear();
            for (int col = 0; col < size; col++)
            {
                int num = _board[row, col];
                if (num == 0) continue;
                if (usedSet.Contains(num)) return false;
                usedSet.Add(num);
            }
        }

        for (int col = 0; col < size; col++)
        {
            usedSet.Clear();
            for (int row = 0; row < size; row++)
            {
                int num = _board[row, col];
                if (num == 0) continue;
                if (usedSet.Contains(num)) return false;
                usedSet.Add(num);
            }
        }
        int gridSize = _board.GridSize;

        for (int grid = 0; grid < size; grid++)
        {
            int startRow = (grid / gridSize) * gridSize;
            int startCol = (grid % gridSize) * gridSize;
            usedSet.Clear();
            for (int cell = 0; cell < size; cell++)
            {
                int num = _board[startRow + cell / gridSize, startCol + cell % gridSize];
                if (num == 0) continue;
                if (usedSet.Contains(num)) return false;
                usedSet.Add(num);
            }
        }
        return true;
    }

    public bool Solve()
    {
        if (!IsValid())
            return false;
        int size = _board.Size;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                int num = _board[row, col];
                if (num != 0) continue;
                for (int i = 1; i <= size; i++)
                {
                    _board[row, col] = i;
                    if (Solve()) return true;
                    _board[row, col] = 0;
                }
                return false;
            }
        }
        return true;
    }
}
