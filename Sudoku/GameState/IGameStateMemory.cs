namespace XenobiaSoft.Sudoku.GameState;

public interface IGameStateMemory
{
	void Clear();
	bool IsEmpty();
	void Save(GameStateMemento gameState);
	GameStateMemento Undo();
}