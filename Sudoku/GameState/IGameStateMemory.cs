namespace XenobiaSoft.Sudoku.GameState;

public interface IGameStateMemory
{
	void Clear();
	void Save(GameStateMemento gameState);
	GameStateMemento Undo();
}