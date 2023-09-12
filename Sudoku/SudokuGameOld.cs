namespace XenobiaSoft.Sudoku;

public class SudokuGameOld
{
	private const int Columns = 9;
	private const int Rows = 9;

	private bool _BruteForceStop;
	private readonly Stack<int[,]> _ActualStack;
	private readonly Stack<string[,]> _PossibleStack;
	private int[,] _ActualBackup;

	public SudokuGameOld()
	{
		Level = Level.Easy;
		Actual = new int[Columns + 1, Rows + 1];
		Possible = new string[Columns + 1, Rows + 1];
		_ActualStack = new Stack<int[,]>();
		_PossibleStack = new Stack<string[,]>();
		_ActualBackup = new int[Columns + 1, Rows + 1];
	}

	public bool SolvePuzzle()
	{
		var exitLoop = false;
		try
		{
			var changes = false;

			while (!changes)
			{
				while (!changes)
				{
					while (!changes)
					{
						while (!changes)
						{
							while (!changes)
							{
								while (!changes)
								{
									while (!changes)
									{
										while (!changes)
										{
											while (!changes)
											{
												while (!changes)
												{
													changes = CheckColumnsAndRows();

													if (Actual.IsPuzzleSolved())
													{
														exitLoop = true;
														break;
													}

												}

												if (exitLoop)
												{
													break;
												}

												changes = LookForLoneRangersInMiniGrids();

												if (Actual.IsPuzzleSolved())
												{
													exitLoop = true;
													break;
												}

											}

											if (exitLoop)
											{
												break;
											}

											changes = LookForLoneRangersInRows();

											if (Actual.IsPuzzleSolved())
											{
												exitLoop = true;
												break;
											}

										}

										if (exitLoop)
										{
											break;
										}

										changes = LookForLoneRangersInColumns();

										if (Actual.IsPuzzleSolved())
										{
											exitLoop = true;
											break;
										}

									}

									if (exitLoop)
									{
										break;
									}

									changes = LookForTwinsInMiniGrids();

									if (Actual.IsPuzzleSolved())
									{
										exitLoop = true;
										break;
									}

								}

								if (exitLoop)
								{
									break;
								}

								changes = LookForTwinsInRows();

								if (Actual.IsPuzzleSolved())
								{
									exitLoop = true;
									break;
								}

							}

							if (exitLoop)
							{
								break;
							}

							changes = LookForTwinsInColumns();

							if (Actual.IsPuzzleSolved())
							{
								exitLoop = true;
								break;
							}

						}

						if (exitLoop)
						{
							break;
						}

						changes = LookForTripletsInMiniGrids();

						if (Actual.IsPuzzleSolved())
						{
							exitLoop = true;
							break;
						}

					}

					if (exitLoop)
					{
						break;
					}

					changes = LookForTripletsInRows();

					if (Actual.IsPuzzleSolved())
					{
						exitLoop = true;
						break;
					}

				}

				if (exitLoop)
				{
					break;
				}

				changes = LookForTripletsInColumns();

				if (Actual.IsPuzzleSolved())
				{
					break;
				}

			}
		}
		catch (Exception)
		{
			throw new Exception("Invalid Move");
		}

		return Actual.IsPuzzleSolved();
	}

	private bool CheckColumnsAndRows()
	{
		var changes = false;

		for (var row = 1; row <= Rows; row++)
		{
			for (var col = 1; col <= Columns; col++)
			{
				if (Actual[col, row] == 0)
				{
					try
					{
						Possible[col, row] = CalculatePossibleValues(col, row);
					}
					catch (Exception)
					{
						throw new Exception("Invalid Move");
					}

					if (Possible[col, row].Length == 1)
					{
						Actual[col, row] = Convert.ToInt32(Possible[col, row]);
						changes = true;

						TotalScore += 1;
					}
				}
			}
		}

		return changes;
	}

	private string CalculatePossibleValues(int col, int row)
	{
		var possibleValues = Possible[col, row] == string.Empty ? "123456789" : Possible[col, row];

		for (var r = 1; r <= Rows; r++)
		{
			if (Actual[col, r] != 0)
			{
				possibleValues = possibleValues.Replace(Actual[col, r].ToString(), string.Empty);
			}
		}

		for (var c = 1; c <= Columns; c++)
		{
			if (Actual[c, row] != 0)
			{
				possibleValues = possibleValues.Replace(Actual[c, row].ToString(), string.Empty);
			}
		}

		var startC = col - (col - 1) % 3;
		var startR = row - (row - 1) % 3;

		for (var rr = startR; rr <= startR + 2; rr++)
		{
			for (var cc = startC; cc <= startC + 2; cc++)
			{
				if (Actual[cc, rr] != 0)
				{
					possibleValues = possibleValues.Replace(Actual[cc, rr].ToString(), string.Empty);
				}
			}
		}

		if (string.IsNullOrEmpty(possibleValues))
		{
			throw new Exception("Invalid Move");
		}

		return possibleValues;
	}

	private bool LookForLoneRangersInMiniGrids()
	{
		var changes = false;
		var cPos = 0;
		var rPos = 0;

		for (var n = 1; n <= 9; n++)
		{
			for (var r = 1; r <= Rows; r += 3)
			{
				for (var c = 1; c <= Columns; c += 3)
				{
					var nextMiniGrid = false;
					var occurrence = 0;

					for (var rr = 0; rr <= 2; rr++)
					{
						for (var cc = 0; cc <= 2; cc++)
						{
							if (Actual[c + cc, r + rr] == 0 && Possible[c + cc, r + rr].Contains(n.ToString()))
							{
								occurrence += 1;
								cPos = c + cc;
								rPos = r + rr;
								if (occurrence > 1)
								{
									nextMiniGrid = true;
									break;
								}
							}
						}

						if (nextMiniGrid)
						{
							break;
						}
					}

					if (!nextMiniGrid && occurrence == 1)
					{
						Actual[cPos, rPos] = n;
						changes = true;

						TotalScore += 2;
					}
				}
			}
		}

		return changes;
	}

	private bool LookForLoneRangersInRows()
	{
		var changes = false;
		var cPos = 0;
		var rPos = 0;

		for (var r = 1; r <= Rows; r++)
		{
			for (var n = 1; n <= 9; n++)
			{
				var occurrence = 0;

				for (var c = 1; c <= Columns; c++)
				{
					if (Actual[c, r] == 0 && Possible[c, r].Contains(n.ToString()))
					{
						occurrence += 1;

						if (occurrence > 1)
						{
							break;
						}

						cPos = c;
						rPos = r;
					}
				}

				if (occurrence == 1)
				{
					Actual[cPos, rPos] = n;
					changes = true;
					TotalScore += 2;
				}
			}
		}

		return changes;
	}

	private bool LookForLoneRangersInColumns()
	{
		var changes = false;
		var colPos = 0;
		var rowPos = 0;

		for (var col = 1; col <= Columns; col++)
		{
			for (var number = 1; number <= 9; number++)
			{
				var occurrence = 0;

				for (var row = 1; row <= Rows; row++)
				{
					if (Actual[col, row] == 0 && Possible[col, row].Contains(number.ToString()))
					{
						occurrence += 1;

						if (occurrence > 1)
						{
							break;
						}

						colPos = col;
						rowPos = row;
					}
				}

				if (occurrence == 1)
				{
					Actual[colPos, rowPos] = number;
					changes = true;
					TotalScore += 2;
				}
			}
		}
		return changes;
	}

	private bool LookForTwinsInMiniGrids()
	{
		var changes = false;

		for (var row = 1; row <= Rows; row++)
		{
			for (var col = 1; col <= Columns; col++)
			{
				if (Actual[col, row] == 0 && Possible[col, row].Length == 2)
				{
					var startC = col - ((col - 1) % 3);
					var startR = row - ((row - 1) % 3);

					for (var rr = startR; rr <= startR + 2; rr++)
					{
						for (var cc = startC; cc <= startC + 2; cc++)
						{
							if (!(cc == col && rr == row) && Possible[cc, rr] == Possible[col, row])
							{
								for (var rrr = startR; rrr <= startR + 2; rrr++)
								{
									for (var ccc = startC; ccc <= startC + 2; ccc++)
									{
										if (Actual[ccc, rrr] == 0 && Possible[ccc, rrr] != Possible[col, row])
										{
											var originalPossible = Possible[ccc, rrr];

											Possible[ccc, rrr] = Possible[ccc, rrr].Replace(Possible[col, row][0].ToString(), string.Empty);
											Possible[ccc, rrr] = Possible[ccc, rrr].Replace(Possible[col, row][1].ToString(), string.Empty);

											if (originalPossible != Possible[ccc, rrr])
											{
												changes = true;
											}

											if (string.IsNullOrEmpty(Possible[ccc, rrr]))
											{
												throw new Exception("Invalid Move");
											}

											if (Possible[ccc, rrr].Length == 1)
											{
												Actual[ccc, rrr] = int.Parse(Possible[ccc, rrr]);

												TotalScore += 3;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		return changes;
	}

	private bool LookForTwinsInRows()
	{
		var changes = false;

		for (var row = 1; row <= Rows; row++)
		{
			for (var col = 1; col <= Columns; col++)
			{
				if (Actual[col, row] == 0 && Possible[col, row].Length == 2)
				{
					for (var cc = col + 1; cc <= 9; cc++)
					{
						if ((Possible[cc, row] == Possible[col, row]))
						{
							for (var ccc = 1; ccc <= 9; ccc++)
							{
								if ((Actual[ccc, row] == 0) && (ccc != col) && (ccc != cc))
								{
									var originalPossible = Possible[ccc, row];

									Possible[ccc, row] = Possible[ccc, row].Replace(Possible[col, row][0].ToString(), string.Empty);
									Possible[ccc, row] = Possible[ccc, row].Replace(Possible[col, row][1].ToString(), string.Empty);

									if (originalPossible != Possible[ccc, row])
									{
										changes = true;
									}

									if (string.IsNullOrEmpty(Possible[ccc, row]))
									{
										throw new Exception("Invalid Move");
									}

									if (Possible[ccc, row].Length == 1)
									{
										Actual[ccc, row] = int.Parse(Possible[ccc, row]);
										TotalScore += 3;
									}
								}
							}
						}
					}
				}
			}
		}

		return changes;
	}

	private bool LookForTwinsInColumns()
	{
		var changes = false;

		for (var col = 1; col <= Columns; col++)
		{
			for (var row = 1; row <= Rows; row++)
			{
				if (Actual[col, row] == 0 && Possible[col, row].Length == 2)
				{
					for (var rr = row + 1; rr <= 9; rr++)
					{
						if ((Possible[col, rr] == Possible[col, row]))
						{
							for (var rrr = 1; rrr <= 9; rrr++)
							{
								if ((Actual[col, rrr] == 0) && (rrr != row) && (rrr != rr))
								{
									var originalPossible = Possible[col, rrr];

									Possible[col, rrr] = Possible[col, rrr].Replace(Possible[col, row][0].ToString(), string.Empty);
									Possible[col, rrr] = Possible[col, rrr].Replace(Possible[col, row][1].ToString(), string.Empty);

									if (originalPossible != Possible[col, rrr])
									{
										changes = true;
									}

									if (string.IsNullOrEmpty(Possible[col, rrr]))
									{
										throw new Exception("Invalid Move");
									}

									if (Possible[col, rrr].Length == 1)
									{
										Actual[col, rrr] = int.Parse(Possible[col, rrr]);
										TotalScore += 3;
									}
								}
							}
						}
					}
				}
			}
		}

		return changes;
	}

	private bool LookForTripletsInMiniGrids()
	{
		var changes = false;

		for (var row = 1; row <= Rows; row++)
		{
			for (var col = 1; col <= Columns; col++)
			{
				if (Actual[col, row] == 0 && Possible[col, row].Length == 3)
				{
					var tripletsLocation = col.ToString() + row.ToString();

					var startC = col - ((col - 1) % 3);
					var startR = row - ((row - 1) % 3);

					for (var rr = startR; rr <= startR + 2; rr++)
					{
						for (var cc = startC; cc <= startC + 2; cc++)
						{
							if (!(cc == col && rr == row) &&
								(
									(Possible[cc, rr] == Possible[col, row]) ||
									(Possible[cc, rr].Length == 2 &&
									 Possible[col, row].Contains(Possible[cc, rr][0].ToString()) &&
									 Possible[col, row].Contains(Possible[cc, rr][1].ToString()))
								))
							{
								tripletsLocation += cc.ToString() + rr.ToString();
							}
						}
					}

					if (tripletsLocation.Length == 6)
					{
						for (var rrr = startR; rrr <= startR + 2; rrr++)
						{
							for (var ccc = startC; ccc <= startC + 2; ccc++)
							{
								if (Actual[ccc, rrr] == 0 &&
									ccc != Convert.ToInt32(tripletsLocation[0].ToString()) &&
									rrr != Convert.ToInt32(tripletsLocation[1].ToString()) &&
									ccc != Convert.ToInt32(tripletsLocation[2].ToString()) &&
									rrr != Convert.ToInt32(tripletsLocation[3].ToString()) &&
									ccc != Convert.ToInt32(tripletsLocation[4].ToString()) &&
									rrr != Convert.ToInt32(tripletsLocation[5].ToString()))
								{
									var originalPossible = Possible[ccc, rrr];

									Possible[ccc, rrr] = Possible[ccc, rrr].Replace(Possible[col, row][0].ToString(), string.Empty);
									Possible[ccc, rrr] = Possible[ccc, rrr].Replace(Possible[col, row][1].ToString(), string.Empty);
									Possible[ccc, rrr] = Possible[ccc, rrr].Replace(Possible[col, row][2].ToString(), string.Empty);

									if (originalPossible != Possible[ccc, rrr])
									{
										changes = true;
									}

									if (string.IsNullOrEmpty(Possible[ccc, rrr]))
									{
										throw new Exception("Invalid Move");
									}

									if (Possible[ccc, rrr].Length == 1)
									{
										Actual[ccc, rrr] = int.Parse(Possible[ccc, rrr]);
										TotalScore += 4;
									}
								}
							}
						}
					}
				}
			}
		}

		return changes;
	}

	private bool LookForTripletsInRows()
	{
		var changes = false;

		for (var row = 1; row <= Rows; row++)
		{
			for (var col = 1; col <= Columns; col++)
			{
				if (Actual[col, row] == 0 && Possible[col, row].Length == 3)
				{
					var tripletsLocation = col.ToString() + row.ToString();

					for (var cc = 1; cc <= Columns; cc++)
					{
						if (cc != col &&
							(
								(Possible[cc, row] == Possible[col, row]) ||
								(Possible[cc, row].Length == 2 &&
								 Possible[col, row].Contains(Possible[cc, row][0].ToString()) &&
								 Possible[col, row].Contains(Possible[cc, row][1].ToString()))
							))
						{
							tripletsLocation += cc.ToString() + row.ToString();
						}
					}

					if (tripletsLocation.Length == 6)
					{
						for (var ccc = 1; ccc <= Columns; ccc++)
						{
							if (Actual[ccc, row] == 0 &&
								ccc != Convert.ToInt32(tripletsLocation[0].ToString()) &&
								ccc != Convert.ToInt32(tripletsLocation[2].ToString()) &&
								ccc != Convert.ToInt32(tripletsLocation[4].ToString()))
							{
								var originalPossible = Possible[ccc, row];

								Possible[ccc, row] = Possible[ccc, row].Replace(Possible[col, row][0].ToString(), string.Empty);
								Possible[ccc, row] = Possible[ccc, row].Replace(Possible[col, row][1].ToString(), string.Empty);
								Possible[ccc, row] = Possible[ccc, row].Replace(Possible[col, row][2].ToString(), string.Empty);

								if (originalPossible != Possible[ccc, row])
								{
									changes = true;
								}

								if (string.IsNullOrEmpty(Possible[ccc, row]))
								{
									throw new Exception("Invalid Move");
								}

								if (Possible[ccc, row].Length == 1)
								{
									Actual[ccc, row] = int.Parse(Possible[ccc, row]);
									TotalScore += 4;
								}
							}
						}
					}
				}
			}
		}

		return changes;
	}

	private bool LookForTripletsInColumns()
	{
		var changes = false;

		for (var col = 1; col <= Columns; col++)
		{
			for (var row = 1; row <= Rows; row++)
			{
				if (Actual[col, row] == 0 && Possible[col, row].Length == 3)
				{
					var tripletsLocation = col.ToString() + row.ToString();

					for (var rr = 1; rr <= Rows; rr++)
					{
						if (rr != row &&
							(
								(Possible[col, rr] == Possible[col, row]) ||
								(Possible[col, rr].Length == 2 &&
								 Possible[col, row].Contains(Possible[col, rr][0].ToString()) &&
								 Possible[col, row].Contains(Possible[col, rr][1].ToString()))
							))
						{
							tripletsLocation += col.ToString() + rr.ToString();
						}
					}

					if (tripletsLocation.Length == 6)
					{
						for (var rrr = 1; rrr <= Rows; rrr++)
						{
							if (Actual[col, rrr] == 0 &&
								rrr != Convert.ToInt32(tripletsLocation[1].ToString()) &&
								rrr != Convert.ToInt32(tripletsLocation[3].ToString()) &&
								rrr != Convert.ToInt32(tripletsLocation[5].ToString()))
							{
								var originalPossible = Possible[col, rrr];

								Possible[col, rrr] = Possible[col, rrr].Replace(Possible[col, row][0].ToString(), string.Empty);
								Possible[col, rrr] = Possible[col, rrr].Replace(Possible[col, row][1].ToString(), string.Empty);
								Possible[col, rrr] = Possible[col, rrr].Replace(Possible[col, row][2].ToString(), string.Empty);

								if (originalPossible != Possible[col, rrr])
								{
									changes = true;
								}

								if (string.IsNullOrEmpty(Possible[col, rrr]))
								{
									throw new Exception("Invalid Move");
								}

								if (Possible[col, rrr].Length == 1)
								{
									Actual[col, rrr] = int.Parse(Possible[col, rrr]);
									TotalScore += 4;
								}
							}
						}
					}
				}
			}
		}

		return changes;
	}

	private Tuple<int, int> FindCellWithFewestPossibleValues()
	{
		var min = 10;
		var cell = new Tuple<int, int>(0, 0);

		for (var row = 1; row <= Rows; row++)
		{
			for (var col = 1; col <= Columns; col++)
			{
				if (Actual[col, row] == 0 && Possible[col, row].Length < min)
				{
					min = Possible[col, row].Length;

					cell = new Tuple<int, int>(row, col);
				}
			}
		}

		return cell;
	}

	private void SolvePuzzleByBruteForce()
	{
		TotalScore += 5;

		var cell = FindCellWithFewestPossibleValues();

		var row = cell.Item1;
		var col = cell.Item2;

		var possibleValues = Possible[col, row];

		possibleValues = RandomizeThePossibleValues(possibleValues);

		_ActualStack.Push((int[,])Actual.Clone());
		_PossibleStack.Push((string[,])Possible.Clone());

		for (var i = 0; i <= possibleValues.Length - 1; i++)
		{
			Actual[col, row] = Convert.ToInt32(possibleValues[i].ToString());

			try
			{
				if (SolvePuzzle())
				{
					_BruteForceStop = true;
					return;
				}

				SolvePuzzleByBruteForce();

				if (_BruteForceStop)
				{
					return;
				}
			}
			catch (Exception)
			{
				TotalScore += 5;
				Actual = _ActualStack.Pop();
				Possible = _PossibleStack.Pop();
			}
		}
	}

	public string GetPuzzle()
	{
		string puzzle;
		Started = true;

		do
		{
			puzzle = GenerateNewPuzzle();

		} while (!IsValidPuzzle(puzzle));

		return puzzle;
	}

	private bool IsValidPuzzle(string puzzle)
	{
		var isValidPuzzle = false;

		if (!string.IsNullOrEmpty(puzzle))
		{
			switch (Level)
			{
				case Level.Easy:
					if (TotalScore >= 42 & TotalScore <= 46)
					{
						isValidPuzzle = true;
					}
					break;
				case Level.Medium:
					if (TotalScore >= 49 & TotalScore <= 53)
					{
						isValidPuzzle = true;
					}
					break;
				case Level.Hard:
					if (TotalScore >= 56 & TotalScore <= 60)
					{
						isValidPuzzle = true;
					}
					break;
				case Level.ExtremelyHard:
					if (TotalScore >= 112 & TotalScore <= 116)
					{
						isValidPuzzle = true;
					}
					break;
			}
		}

		return isValidPuzzle;
	}

	private static string RandomizeThePossibleValues(string puzzle)
	{
		var charArray = puzzle.ToCharArray();

		for (var i = 0; i <= puzzle.Length - 1; i++)
		{
			var j = Convert.ToInt32((puzzle.Length - i + 1) * RandomNumber(1, 9) + 1) % puzzle.Length;
			(charArray[i], charArray[j]) = (charArray[j], charArray[i]);
		}

		return new string(charArray);
	}

	private static int RandomNumber(int? min = null, int? max = null)
	{
		var rnd = new Random((int)DateTime.Now.Ticks);

		if (max.HasValue)
		{
			return min.HasValue ? rnd.Next(min.Value, max.Value) : rnd.Next(max.Value);
		}

		return rnd.Next();
	}

	private string GenerateNewPuzzle()
	{
		var numberOfEmpty = 0;

		for (var row = 1; row <= Rows; row++)
		{
			for (var col = 1; col <= Columns; col++)
			{
				Actual[col, row] = 0;
				Possible[col, row] = string.Empty;
			}
		}

		_ActualStack.Clear();
		_PossibleStack.Clear();

		try
		{
			if (SolvePuzzle())
			{
				SolvePuzzleByBruteForce();
			}
		}
		catch (Exception ex)
		{
			return string.Empty;
		}

		_ActualBackup = (int[,])Actual.Clone();

		switch (Level)
		{
			case Level.Easy:
				numberOfEmpty = RandomNumber(40, 45);
				break;
			case Level.Medium:
				numberOfEmpty = RandomNumber(46, 49);
				break;
			case Level.Hard:
				numberOfEmpty = RandomNumber(50, 53);
				break;
			case Level.ExtremelyHard:
				numberOfEmpty = RandomNumber(54, 58);
				break;
		}

		_ActualStack.Clear();
		_PossibleStack.Clear();
		_BruteForceStop = false;

		CreateEmptyCells(numberOfEmpty);

		var puzzle = string.Empty;

		for (var row = 1; row <= Rows; row++)
		{
			for (var col = 1; col <= Columns; col++)
			{
				puzzle += Actual[col, row].ToString();
			}
		}

		var tries = 0;

		do
		{
			TotalScore = 0;

			try
			{
				if (SolvePuzzle())
				{
					if (Level == Level.ExtremelyHard)
					{
						SolvePuzzleByBruteForce();
						break;
					}

					puzzle = VacateAnotherPairOfCells(puzzle);
					tries += 1;
				}
				else
				{
					break;
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}

			if (tries > 50)
			{
				return string.Empty;
			}
		} while (true);

		return puzzle;
	}

	private void CreateEmptyCells(int numberOfEmpty)
	{
		var emptyCells = new string[numberOfEmpty];

		for (var i = 0; i <= (numberOfEmpty / 2); i++)
		{
			bool duplicate;

			do
			{
				duplicate = false;
				int col;
				int row;

				do
				{
					col = RandomNumber(1, 9);
					row = RandomNumber(1, 5);
				} while ((row > 5 & col > 5));

				for (var j = 0; j <= i; j++)
				{
					if (emptyCells[j] == col.ToString() + row.ToString())
					{
						duplicate = true;
						break;
					}
				}

				if (!duplicate)
				{
					emptyCells[i] = col.ToString() + row.ToString();
					Actual[col, row] = 0;
					Possible[col, row] = string.Empty;
					emptyCells[numberOfEmpty - 1 - i] = (10 - col).ToString() + (10 - row).ToString();
					Actual[10 - col, 10 - row] = 0;
					Possible[10 - col, 10 - row] = string.Empty;
				}
			} while (duplicate);
		}
	}

	private string VacateAnotherPairOfCells(string puzzle)
	{
		int c;
		int r;

		do
		{
			c = RandomNumber(1, 9);
			r = RandomNumber(1, 9);
		} while (puzzle[(c - 1) + (r - 1) * 9].ToString() != "0");

		puzzle = puzzle.Remove((c - 1) + (r - 1) * 9, 1);
		puzzle = puzzle.Insert((c - 1) + (r - 1) * 9, _ActualBackup[c, r].ToString());

		puzzle = puzzle.Remove((10 - c - 1) + (10 - r - 1) * 9, 1);
		puzzle = puzzle.Insert((10 - c - 1) + (10 - r - 1) * 9, _ActualBackup[10 - c, 10 - r].ToString());

		do
		{
			c = RandomNumber(1, 9);
			r = RandomNumber(1, 9);
		} while (puzzle[(c - 1) + (r - 1) * 9].ToString() == "0");

		puzzle = puzzle.Remove((c - 1) + (r - 1) * 9, 1);
		puzzle = puzzle.Insert((c - 1) + (r - 1) * 9, "0");
		puzzle = puzzle.Remove((10 - c - 1) + (10 - r - 1) * 9, 1);
		puzzle = puzzle.Insert((10 - c - 1) + (10 - r - 1) * 9, "0");

		short counter = 0;

		for (var row = 1; row <= Rows; row++)
		{
			for (var col = 1; col <= Columns; col++)
			{
				if (int.Parse(puzzle[counter].ToString()) != 0)
				{
					Actual[col, row] = Convert.ToInt32(puzzle[counter].ToString());
					Possible[col, row] = puzzle[counter].ToString();
				}
				else
				{
					Actual[col, row] = 0;
					Possible[col, row] = string.Empty;
				}
				counter += 1;
			}
		}

		return puzzle;
	}

	public int[,] Actual { get; private set; }

	public string[,] Possible { get; private set; }

	public bool Started { get; private set; }

	public Level Level { get; set; }

	public int TotalScore { get; private set; }
}

public static class TempExtensions
{
	public static bool IsPuzzleSolved(this int[,] puzzle)
	{
		return true;
	}
}