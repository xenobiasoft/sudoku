import { useState, useEffect, useCallback, useRef, type KeyboardEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import type { GameModel } from '../types';
import { apiClient } from '../api/apiClient';
import { validateCells, isSolved } from '../utils/gameUtils';
import Layout from '../components/Layout';
import GameBoard from '../components/GameBoard';
import GameControls from '../components/GameControls';
import GameStats from '../components/GameStats';
import VictoryDisplay from '../components/VictoryDisplay';
import styles from './GamePage.module.css';

export default function GamePage() {
  const { puzzleId } = useParams<{ puzzleId: string }>();
  const navigate = useNavigate();

  const [game, setGame] = useState<GameModel | null>(null);
  const [selectedCell, setSelectedCell] = useState<{ row: number; column: number } | null>(null);
  const [pencilMode, setPencilMode] = useState(false);
  const [elapsedSeconds, setElapsedSeconds] = useState(0);
  const [solved, setSolved] = useState(false);

  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const startTimeRef = useRef<number>(Date.now());
  const elapsedRef = useRef(0);
  const gameRef = useRef<GameModel | null>(null);
  const solvedRef = useRef(false);

  const alias = localStorage.getItem('playerAlias') ?? '';

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

  const pauseGame = useCallback(async () => {
    const g = gameRef.current;
    if (!g || solvedRef.current) return;
    stopTimer();
    try {
      await apiClient.updateStatus(alias, g.id, 'Paused');
    } catch {
      // ignore
    }
  }, [alias, stopTimer]);

  useEffect(() => {
    if (!puzzleId) return;
    const load = async () => {
      try {
        const g = await apiClient.getGame(alias, puzzleId);
        gameRef.current = g;
        setGame(g);
        const duration = g.statistics?.playDuration ?? '00:00:00';
        startTimer(duration);
        await apiClient.updateStatus(alias, g.id, 'InProgress');
      } catch (e) {
        console.error('Failed to load game', e);
        navigate('/');
      }
    };
    load();
    return () => stopTimer();
  }, [puzzleId, alias, startTimer, stopTimer, navigate]);

  useEffect(() => {
    const handleVisibilityChange = () => {
      if (document.hidden) pauseGame();
    };
    const handleBeforeUnload = () => pauseGame();

    document.addEventListener('visibilitychange', handleVisibilityChange);
    window.addEventListener('beforeunload', handleBeforeUnload);
    return () => {
      document.removeEventListener('visibilitychange', handleVisibilityChange);
      window.removeEventListener('beforeunload', handleBeforeUnload);
    };
  }, [pauseGame]);

  const handleCellAction = async (_cells: unknown, action: () => Promise<GameModel>) => {
    if (!game) return;
    try {
      const updated = await action();
      gameRef.current = updated;
      setGame(updated);
      if (isSolved(updated.cells)) {
        solvedRef.current = true;
        stopTimer();
        setSolved(true);
        try {
          await apiClient.deleteGame(alias, game.id);
        } catch {
          // ignore
        }
      }
    } catch (e) {
      console.error('Action failed', e);
    }
  };

  const handleNumberInput = async (value: number) => {
    if (!game || !selectedCell) return;
    const cell = game.cells.find(c => c.row === selectedCell.row && c.column === selectedCell.column);
    if (!cell || cell.isFixed) return;

    if (pencilMode) {
      await handleCellAction(game.cells, async () => {
        if (cell.possibleValues.includes(value)) {
          return apiClient.removePossibleValue(alias, game.id, selectedCell.row, selectedCell.column, value);
        } else {
          return apiClient.addPossibleValue(alias, game.id, selectedCell.row, selectedCell.column, value);
        }
      });
    } else {
      await handleCellAction(game.cells, () =>
        apiClient.makeMove(alias, game.id, selectedCell.row, selectedCell.column, value, formatDuration(elapsedRef.current))
      );
    }
  };

  const handleErase = async () => {
    if (!game || !selectedCell) return;
    const cell = game.cells.find(c => c.row === selectedCell.row && c.column === selectedCell.column);
    if (!cell || cell.isFixed) return;

    if (pencilMode && cell.possibleValues.length > 0) {
      await handleCellAction(game.cells, () =>
        apiClient.clearPossibleValues(alias, game.id, selectedCell.row, selectedCell.column)
      );
    } else if (!pencilMode && cell.hasValue) {
      await handleCellAction(game.cells, () =>
        apiClient.makeMove(alias, game.id, selectedCell.row, selectedCell.column, null, formatDuration(elapsedRef.current))
      );
    }
  };

  const handleUndo = async () => {
    if (!game) return;
    await handleCellAction(game.cells, () => apiClient.undoMove(alias, game.id));
  };

  const handleReset = async () => {
    if (!game) return;
    await handleCellAction(game.cells, () => apiClient.resetGame(alias, game.id));
  };

  const handleHome = async () => {
    await pauseGame();
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

  if (!game) {
    return (
      <Layout>
        <div style={{ textAlign: 'center', marginTop: '4rem' }}>Loading puzzle...</div>
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
        {solved && <VictoryDisplay onClose={handleVictoryClose} />}
      </div>
    </Layout>
  );
}
