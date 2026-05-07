import { useState, useEffect, useCallback, useRef, type KeyboardEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import type { GameModel } from '../types';
import { validateCells, isSolved } from '../utils/gameUtils';
import { usePlayerService } from '../hooks/usePlayerService';
import { useGameService } from '../hooks/useGameService';
import Layout from '../components/Layout';
import GameBoard from '../components/GameBoard';
import GameControls from '../components/GameControls';
import GameStats from '../components/GameStats';
import VictoryDisplay from '../components/VictoryDisplay';
import styles from './GamePage.module.css';

export default function GamePage() {
  const { puzzleId } = useParams<{ puzzleId: string }>();
  const navigate = useNavigate();
  const { profileId, isInitialized, isNewPlayer, isLoading: playerLoading, error: playerError } = usePlayerService();
  const {
    currentGame,
    isGameLoading,
    gameError,
    getGame,
    pauseGame,
    resumeGame,
    makeMove,
    undoMove,
    resetGame,
    addPossibleValue,
    removePossibleValue,
    clearPossibleValues,
    deleteGame,
    clearCurrentGame,
  } = useGameService();

  const [selectedCell, setSelectedCell] = useState<{ row: number; column: number } | null>(null);
  const [pencilMode, setPencilMode] = useState(false);
  const [elapsedSeconds, setElapsedSeconds] = useState(0);
  const [solved, setSolved] = useState(false);

  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const startTimeRef = useRef<number>(Date.now());
  const elapsedRef = useRef(0);
  const solvedRef = useRef(false);

  // Use currentGame from useGameService instead of local state
  const game = currentGame;

  useEffect(() => {
    if (isNewPlayer) navigate('/');
  }, [isNewPlayer, navigate]);

  const startTimer = useCallback((baseDuration: string) => {
    const [h, m, s] = baseDuration.split(':').map(Number);
    const baseSeconds = (h || 0) * 3600 + (m || 0) * 60 + (s || 0);
    startTimeRef.current = Date.now();
    elapsedRef.current = baseSeconds;
    setElapsedSeconds(baseSeconds);

    if (timerRef.current) clearInterval(timerRef.current);
    timerRef.current = setInterval(() => {
      const delta = Math.floor((Date.now() - startTimeRef.current) / 1000);
      const total = baseSeconds + delta;
      elapsedRef.current = total;
      setElapsedSeconds(total);
    }, 1000);
  }, []);

  const stopTimer = useCallback(() => {
    if (timerRef.current) {
      clearInterval(timerRef.current);
      timerRef.current = null;
    }
  }, []);

  const formatDuration = (seconds: number): string => {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = seconds % 60;
    return [h, m, s].map(v => v.toString().padStart(2, '0')).join(':');
  };

  const pauseGameStatus = useCallback(async () => {
    if (!game || solvedRef.current || !profileId) return;
    stopTimer();
    try {
      await pauseGame(profileId, game.id);
    } catch {
      // ignore
    }
  }, [game, profileId, stopTimer, pauseGame]);

  useEffect(() => {
    if (!puzzleId || !profileId || !isInitialized) return;
    const load = async () => {
      try {
        const g = await getGame(profileId, puzzleId);
        const duration = g.statistics?.playDuration ?? '00:00:00';
        startTimer(duration);
        await resumeGame(profileId, g.id);
      } catch (e) {
        console.error('Failed to load game', e);
        navigate('/');
      }
    };
    load();
    return () => {
      stopTimer();
      clearCurrentGame();
    };
  }, [puzzleId, profileId, isInitialized, getGame, resumeGame, startTimer, stopTimer, navigate, clearCurrentGame]);

  useEffect(() => {
    const handleVisibilityChange = () => {
      if (document.hidden) pauseGameStatus();
    };
    const handleBeforeUnload = () => pauseGameStatus();

    document.addEventListener('visibilitychange', handleVisibilityChange);
    window.addEventListener('beforeunload', handleBeforeUnload);
    return () => {
      document.removeEventListener('visibilitychange', handleVisibilityChange);
      window.removeEventListener('beforeunload', handleBeforeUnload);
    };
  }, [pauseGameStatus]);

  const handleCellAction = async (_cells: unknown, action: () => Promise<GameModel>) => {
    if (!game) return;
    try {
      const updated = await action();
      if (isSolved(updated.cells)) {
        solvedRef.current = true;
        stopTimer();
        setSolved(true);
        if (profileId) {
          try {
            await deleteGame(profileId, game.id);
          } catch {
            // ignore
          }
        }
      }
    } catch (e) {
      console.error('Action failed', e);
    }
  };

  const handleNumberInput = async (value: number) => {
    if (!game || !selectedCell || !profileId) return;
    const cell = game.cells.find(c => c.row === selectedCell.row && c.column === selectedCell.column);
    if (!cell || cell.isFixed) return;

    if (pencilMode) {
      await handleCellAction(game.cells, async () => {
        if (cell.possibleValues.includes(value)) {
          return removePossibleValue(profileId, game.id, selectedCell.row, selectedCell.column, value);
        } else {
          return addPossibleValue(profileId, game.id, selectedCell.row, selectedCell.column, value);
        }
      });
    } else {
      await handleCellAction(game.cells, () =>
        makeMove(profileId, game.id, selectedCell.row, selectedCell.column, value, formatDuration(elapsedRef.current))
      );
    }
  };

  const handleErase = async () => {
    if (!game || !selectedCell || !profileId) return;
    const cell = game.cells.find(c => c.row === selectedCell.row && c.column === selectedCell.column);
    if (!cell || cell.isFixed) return;

    if (pencilMode && cell.possibleValues.length > 0) {
      await handleCellAction(game.cells, () =>
        clearPossibleValues(profileId, game.id, selectedCell.row, selectedCell.column)
      );
    } else if (!pencilMode && cell.hasValue) {
      await handleCellAction(game.cells, () =>
        makeMove(profileId, game.id, selectedCell.row, selectedCell.column, null, formatDuration(elapsedRef.current))
      );
    }
  };

  const handleUndo = async () => {
    if (!game || !profileId) return;
    await handleCellAction(game.cells, () => undoMove(profileId, game.id));
  };

  const handleReset = async () => {
    if (!game || !profileId) return;
    await handleCellAction(game.cells, () => resetGame(profileId, game.id));
  };

  const handleHome = async () => {
    await pauseGameStatus();
    navigate('/');
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLTableElement>) => {
    if (!selectedCell) return;
    const { row, column } = selectedCell;

    if (e.key >= '1' && e.key <= '9') {
      e.preventDefault();
      handleNumberInput(parseInt(e.key));
      return;
    }

    if (e.key === '0' || e.key === 'Delete' || e.key === 'Backspace') {
      e.preventDefault();
      handleErase();
      return;
    }

    let newRow = row;
    let newCol = column;
    if (e.key === 'ArrowUp') { e.preventDefault(); newRow = Math.max(0, row - 1); }
    else if (e.key === 'ArrowDown') { e.preventDefault(); newRow = Math.min(8, row + 1); }
    else if (e.key === 'ArrowLeft') { e.preventDefault(); newCol = Math.max(0, column - 1); }
    else if (e.key === 'ArrowRight') { e.preventDefault(); newCol = Math.min(8, column + 1); }

    if (newRow !== row || newCol !== column) {
      setSelectedCell({ row: newRow, column: newCol });
    }
  };

  const handleVictoryClose = () => {
    navigate('/');
  };

  if (playerLoading || !isInitialized) {
    return (
      <Layout>
        <div style={{ textAlign: 'center', marginTop: '4rem' }}>
          {playerLoading ? 'Initializing player...' : 'Loading...'}
        </div>
      </Layout>
    );
  }

  if (playerError) {
    return (
      <Layout>
        <div style={{ textAlign: 'center', marginTop: '4rem' }}>
          <div>Error: {playerError}</div>
          <button onClick={() => navigate('/')}>Go Home</button>
        </div>
      </Layout>
    );
  }

  if (gameError) {
    return (
      <Layout>
        <div style={{ textAlign: 'center', marginTop: '4rem' }}>
          <div>Error loading game: {gameError}</div>
          <button onClick={() => navigate('/')}>Go Home</button>
        </div>
      </Layout>
    );
  }

  if (solved) {
    return (
      <Layout>
        <VictoryDisplay onClose={handleVictoryClose} />
      </Layout>
    );
  }

  if (isGameLoading || !game) {
    return (
      <Layout>
        <div style={{ textAlign: 'center', marginTop: '4rem' }}>
          Loading puzzle...
        </div>
      </Layout>
    );
  }

  const invalidCells = validateCells(game.cells);

  return (
    <Layout>
      <div className={styles.gameView}>
        <GameStats
          statistics={game.statistics}
          elapsedSeconds={elapsedSeconds}
        />
        <GameBoard
          cells={game.cells}
          invalidCells={invalidCells}
          selectedCell={selectedCell}
          pencilMode={pencilMode}
          onCellSelect={(r, c) => setSelectedCell({ row: r, column: c })}
          onKeyDown={handleKeyDown}
        />
        <GameControls
          pencilMode={pencilMode}
          canUndo={(game.moveHistory?.length ?? 0) > 0}
          onNumberClick={handleNumberInput}
          onErase={handleErase}
          onHome={handleHome}
          onUndo={handleUndo}
          onReset={handleReset}
          onTogglePencil={() => setPencilMode(p => !p)}
        />
      </div>
    </Layout>
  );
}
