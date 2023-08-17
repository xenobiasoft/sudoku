namespace XenobiaSoft.Sudoku;

public class SudokuGame
{
	public void Initialize()
	{
		Board = new int[9, 9];

		int[,] initialBoard = {
			{5, 3, 0, 0, 7, 0, 0, 0, 0},
			{6, 0, 0, 1, 9, 5, 0, 0, 0},
			{0, 9, 8, 0, 0, 0, 0, 6, 0},
			{8, 0, 0, 0, 6, 0, 0, 0, 3},
			{4, 0, 0, 8, 0, 3, 0, 0, 1},
			{7, 0, 0, 0, 2, 0, 0, 0, 6},
			{0, 6, 0, 0, 0, 0, 2, 8, 0},
			{0, 0, 0, 4, 1, 9, 0, 0, 5},
			{0, 0, 0, 0, 8, 0, 0, 7, 9}
		};

		Array.Copy(initialBoard, Board, initialBoard.Length);
	}

	public int[,] Board { get; private set; }
	public int Rows => Board.GetLength(0);
	public int Columns => Board.GetLength(1);

	public bool IsValid()
	{
		var rows = new bool[9, 9];
		var columns = new bool[9, 9];
		var subGrids = new bool[9, 9];

		for (var row = 0; row < Board.GetLength(0); row++)
		{
			for (var col = 0; col < Board.GetLength(1); col++)
			{
				if (Board[row, col] != 0)
				{
					var value = Board[row, col] - 1;

					if (rows[row, value] || columns[col, value] || subGrids[row / 3 * 3 + col / 3, value])
					{
						return false;
					}

					rows[row, value] = true;
					columns[col, value] = true;
					subGrids[row / 3 * 3 + col / 3, value] = true;
				}
			}
		}

		return true;
	}
}