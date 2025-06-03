using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.States;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class GameSession(GameStateMemory gameState, IGameTimer timer) : IGameSession
{
    private IGameSessionState _state = new NewGameSessionState(gameState, timer);
    private EventHandler? _moveRecordedHandler;

    public string Alias => _state.Alias;
    public string PuzzleId => _state.PuzzleId;
    public Cell[] Board => _state.Board;
    public int InvalidMoves => _state.InvalidMoves;
    public int TotalMoves => _state.TotalMoves;
    public TimeSpan PlayDuration => _state.PlayDuration;
    public IGameTimer Timer => _state.Timer;

    public event EventHandler? OnMoveRecorded
    {
        add
        {
            _moveRecordedHandler += value;
            _state.OnMoveRecorded += value;
        }
        remove
        {
            _moveRecordedHandler -= value;
            _state.OnMoveRecorded -= value;
        }
    }

    public void RecordMove(bool isValid)
    {
        _state.RecordMove(isValid);
    }

    public void ReloadBoard(GameStateMemory gameState)
    {
        _state.ReloadBoard(gameState);
    }

    public void Start()
    {
        _state.Start();
        var newState = new ActiveGameSessionState(new GameStateMemory(PuzzleId, Board)
        {
            Alias = Alias,
            InvalidMoves = InvalidMoves,
            TotalMoves = TotalMoves,
            PlayDuration = PlayDuration
        }, Timer);

        // Transfer event handlers
        if (_moveRecordedHandler != null)
        {
            newState.OnMoveRecorded += _moveRecordedHandler;
        }

        _state = newState;
    }

    public void Pause()
    {
        _state.Pause();
    }

    public void Resume()
    {
        _state.Resume();
    }

    public void End()
    {
        _state.End();
        var newState = new CompletedGameSessionState(new GameStateMemory(PuzzleId, Board)
        {
            Alias = Alias,
            InvalidMoves = InvalidMoves,
            TotalMoves = TotalMoves,
            PlayDuration = PlayDuration
        }, Timer);

        // Transfer event handlers
        if (_moveRecordedHandler != null)
        {
            newState.OnMoveRecorded += _moveRecordedHandler;
        }

        _state = newState;
    }
}