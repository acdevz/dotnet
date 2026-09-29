namespace SudokuSolver;

public interface IBoard
{
    int this[int row, int col] { get; set; }
    int Size { get; }
    int GridSize { get; }
}