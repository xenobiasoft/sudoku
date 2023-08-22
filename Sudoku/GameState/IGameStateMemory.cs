namespace XenobiaSoft.Sudoku.GameState;

public interface IGameStateMemory
{
	void Save(GameStateMemento gameState);
	GameStateMemento Undo();
}