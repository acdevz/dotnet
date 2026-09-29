namespace SudokuSolver;

public class ArrayBoard : IBoard
{
    private readonly int[,] _boardArray;
    private readonly int _size;
    private readonly int _gridSize;

    public int this[int row, int col]
    {
        get => _boardArray[row, col];
        set => _boardArray[row, col] = value;
    }

    public int Size => _size;
    public int GridSize => _gridSize;

    public ArrayBoard(int size)
    {
        if (!IsValidSize(size, out _gridSize))
        {
            throw new ArgumentException($"Invalid size: {size}", nameof(size));
        }
        _boardArray = new int[size, size];
        _size = size;
    }

    private bool IsValidSize(int size, out int gridSize)
    {
        gridSize = (int)Math.Sqrt(size);
        return size >= 4 && gridSize * gridSize == size;
    }
}
