using System.Data;
using Xunit.Sdk;

namespace SudokuSolver.UnitTests;

public class SolverTests
{
    /*  ********************************** graveyard **********************************
        [Fact]
        public void Empty4x4Board()
        {
            int[,] empty = new int[4, 4];
            var solver = new Solver(empty);
            Assert.True(solver.IsValid(), "Empty Board Check.");
        }

        [Fact]
        public void NonSquareBoard()
        {
            int[,] empty = new int[4, 9];
            var solver = new Solver(empty);
            Assert.False(solver.IsValid(), "Non Square Board Check.");
        } 

    ******************************************************************************** */

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(4, true)]
    [InlineData(8, false)]
    [InlineData(9, true)]
    [InlineData(10, false)]
    [InlineData(16, true)]
    public void EmptyBoardSizes(int size, bool isValid)
    {
        if (!isValid)
        {
            Assert.Throws<ArgumentException>(nameof(size), () => new ArrayBoard(size));
        }
        else
        {
            _ = new ArrayBoard(size);
        }
    }

    [Theory]
    [MemberData(nameof(Boards))]
    public void CheckRules(IBoard board, bool isValid)
    {
        var solver = new Solver(board);
        Assert.Equal(isValid, solver.IsValid());
    }

    public static IEnumerable<object[]> Boards //  ~ TheoryData<IBoard, bool> collection
    {
        get
        {
            IBoard board = new ArrayBoard(4);
            board[1, 0] = 1;
            board[3, 0] = 1;
            yield return new object[] { board, false };

            board = new ArrayBoard(4);
            board[1, 0] = 1;
            board[1, 2] = 1;
            yield return new object[] { board, false };

            board = new ArrayBoard(4);
            board[1, 2] = 1;
            board[0, 3] = 1;
            yield return new object[] { board, false };

            board = new ArrayBoard(4);
            board[1, 0] = 1;
            board[2, 3] = 1;
            yield return new object[] { board, true };
        }
    }

    [Fact]
    public void CheckSolution()
    {
        var actualBoard = new ArrayBoard(9);
        actualBoard[0, 0] = 1;
        actualBoard[0, 4] = 3;
        actualBoard[0, 5] = 4;
        actualBoard[0, 8] = 8;

        actualBoard[1, 1] = 7;
        actualBoard[1, 3] = 6;
        actualBoard[1, 4] = 8;
        actualBoard[1, 7] = 3;

        actualBoard[2, 2] = 8;
        actualBoard[2, 3] = 2;
        actualBoard[2, 4] = 1;
        actualBoard[2, 6] = 7;
        actualBoard[2, 8] = 4;

        actualBoard[3, 1] = 5;
        actualBoard[3, 2] = 4;
        actualBoard[3, 4] = 9;
        actualBoard[3, 6] = 6;
        actualBoard[3, 7] = 8;

        actualBoard[4, 0] = 9;
        actualBoard[4, 1] = 1;
        actualBoard[4, 3] = 5;
        actualBoard[4, 5] = 8;
        actualBoard[4, 7] = 2;

        actualBoard[5, 1] = 8;
        actualBoard[5, 3] = 3;
        actualBoard[5, 8] = 5;

        actualBoard[6, 0] = 3;
        actualBoard[6, 2] = 5;
        actualBoard[6, 3] = 9;
        actualBoard[6, 5] = 6;
        actualBoard[6, 6] = 8;
        actualBoard[6, 7] = 7;
        actualBoard[6, 8] = 1;

        actualBoard[7, 2] = 6;
        actualBoard[7, 7] = 4;

        actualBoard[8, 2] = 1;
        actualBoard[8, 4] = 7;
        actualBoard[8, 6] = 2;

        var expectedBoard = new ArrayBoard(9);
        expectedBoard[0, 0] = 1;
        expectedBoard[0, 1] = 6;
        expectedBoard[0, 2] = 2;
        expectedBoard[0, 3] = 7;
        expectedBoard[0, 4] = 3;
        expectedBoard[0, 5] = 4;
        expectedBoard[0, 6] = 5;
        expectedBoard[0, 7] = 9;
        expectedBoard[0, 8] = 8;

        expectedBoard[1, 0] = 4;
        expectedBoard[1, 1] = 7;
        expectedBoard[1, 2] = 9;
        expectedBoard[1, 3] = 6;
        expectedBoard[1, 4] = 8;
        expectedBoard[1, 5] = 5;
        expectedBoard[1, 6] = 1;
        expectedBoard[1, 7] = 3;
        expectedBoard[1, 8] = 2;

        expectedBoard[2, 0] = 5;
        expectedBoard[2, 1] = 3;
        expectedBoard[2, 2] = 8;
        expectedBoard[2, 3] = 2;
        expectedBoard[2, 4] = 1;
        expectedBoard[2, 5] = 9;
        expectedBoard[2, 6] = 7;
        expectedBoard[2, 7] = 6;
        expectedBoard[2, 8] = 4;

        expectedBoard[3, 0] = 2;
        expectedBoard[3, 1] = 5;
        expectedBoard[3, 2] = 4;
        expectedBoard[3, 3] = 1;
        expectedBoard[3, 4] = 9;
        expectedBoard[3, 5] = 7;
        expectedBoard[3, 6] = 6;
        expectedBoard[3, 7] = 8;
        expectedBoard[3, 8] = 3;

        expectedBoard[4, 0] = 9;
        expectedBoard[4, 1] = 1;
        expectedBoard[4, 2] = 3;
        expectedBoard[4, 3] = 5;
        expectedBoard[4, 4] = 6;
        expectedBoard[4, 5] = 8;
        expectedBoard[4, 6] = 4;
        expectedBoard[4, 7] = 2;
        expectedBoard[4, 8] = 7;

        expectedBoard[5, 0] = 6;
        expectedBoard[5, 1] = 8;
        expectedBoard[5, 2] = 7;
        expectedBoard[5, 3] = 3;
        expectedBoard[5, 4] = 4;
        expectedBoard[5, 5] = 2;
        expectedBoard[5, 6] = 9;
        expectedBoard[5, 7] = 1;
        expectedBoard[5, 8] = 5;

        expectedBoard[6, 0] = 3;
        expectedBoard[6, 1] = 4;
        expectedBoard[6, 2] = 5;
        expectedBoard[6, 3] = 9;
        expectedBoard[6, 4] = 2;
        expectedBoard[6, 5] = 6;
        expectedBoard[6, 6] = 8;
        expectedBoard[6, 7] = 7;
        expectedBoard[6, 8] = 1;

        expectedBoard[7, 0] = 7;
        expectedBoard[7, 1] = 2;
        expectedBoard[7, 2] = 6;
        expectedBoard[7, 3] = 8;
        expectedBoard[7, 4] = 5;
        expectedBoard[7, 5] = 1;
        expectedBoard[7, 6] = 3;
        expectedBoard[7, 7] = 4;
        expectedBoard[7, 8] = 9;

        expectedBoard[8, 0] = 8;
        expectedBoard[8, 1] = 9;
        expectedBoard[8, 2] = 1;
        expectedBoard[8, 3] = 4;
        expectedBoard[8, 4] = 7;
        expectedBoard[8, 5] = 3;
        expectedBoard[8, 6] = 2;
        expectedBoard[8, 7] = 5;
        expectedBoard[8, 8] = 6;

        var solver = new Solver(actualBoard);

        Assert.True(solver.Solve());
        AssertBoardsComplete(actualBoard);
        AssertBoardsEqual(expectedBoard, actualBoard);
    }

    private void AssertBoardsEqual(IBoard expected, IBoard actual)
    {
        Assert.Equal(expected.Size, actual.Size);
        Assert.Equal(expected.GridSize, actual.GridSize);

        for (var row = 0; row < expected.Size; row++)
        {
            for (var col = 0; col < expected.Size; col++)
            {
                Assert.True(
                    expected[row, col] == actual[row, col],
                    $"Cell [{row}, {col}] was expected to be " +
                    $"{expected[row, col]}, but was {actual[row, col]}.");
            }
        }
    }

    private void AssertBoardsComplete(IBoard board)
    {
        for (var row = 0; row < board.Size; row++)
        {
            for (var col = 0; col < board.Size; col++)
            {
                Assert.InRange(board[row, col], 1, board.Size);
            }
        }
    }
}
