using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.States;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Represents an active game session
/// </summary>
public class GameSession(GameStateMemory gameState, IGameTimer timer) : GameSessionBase
{
    private IGameSessionState _state = new NewGameSessionState(gameState, timer);
    private EventHandler? _moveRecordedHandler;

    public override bool IsNull => false;
    public override string Alias => _state.Alias;
    public override string PuzzleId => _state.PuzzleId;
    public override Cell[] Board => _state.Board;
    public override int InvalidMoves => _state.InvalidMoves;
    public override int TotalMoves => _state.TotalMoves;
    public override TimeSpan PlayDuration => _state.PlayDuration;
    public override IGameTimer Timer => _state.Timer;

    public override event EventHandler? OnMoveRecorded
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

    public override void RecordMove(bool isValid)
    {
        _state.RecordMove(isValid);
    }

    public override void ReloadBoard(GameStateMemory gameState)
    {
        _state.ReloadBoard(gameState);
    }

    public override void Start()
    {
        _state.Start();
        TransitionToState(new ActiveGameSessionState(CreateGameStateMemory(), Timer));
    }

    public override void Pause()
    {
        _state.Pause();
    }

    public override void Resume()
    {
        _state.Resume();
    }

    public override void End()
    {
        _state.End();
        TransitionToState(new CompletedGameSessionState(CreateGameStateMemory(), Timer));
    }

    private void TransitionToState(IGameSessionState newState)
    {
        TransferEventHandlers(newState);
        _state = newState;
    }

    private void TransferEventHandlers(IGameSessionState newState)
    {
        if (_moveRecordedHandler != null)
        {
            newState.OnMoveRecorded += _moveRecordedHandler;
        }
    }

    private GameStateMemory CreateGameStateMemory()
    {
        return new GameStateMemory(PuzzleId, Board)
        {
            Alias = Alias,
            InvalidMoves = InvalidMoves,
            TotalMoves = TotalMoves,
            PlayDuration = PlayDuration
        };
    }
}