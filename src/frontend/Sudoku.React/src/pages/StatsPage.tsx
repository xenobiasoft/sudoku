import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { usePlayerService } from '../hooks/usePlayerService';
import { useStatsService } from '../hooks/useStatsService';
import { formatDuration } from '../utils/timeUtils';
import Layout from '../components/Layout';
import styles from './StatsPage.module.css';

export default function StatsPage() {
  const navigate = useNavigate();
  const { isNewPlayer, profileId } = usePlayerService();
  const { stats, isLoaded, isLoading, error, loadStats } = useStatsService();

  useEffect(() => {
    if (isNewPlayer) { navigate('/'); return; }
    if (profileId) loadStats(profileId);
  }, [isNewPlayer, profileId, loadStats, navigate]);

  const winRate = stats ? `${Math.round(stats.winRate * 100)}%` : '0%';
  const isEmpty = isLoaded && !error && stats?.gamesPlayed === 0;

  const nineByDifficulty = stats?.byDifficulty.filter(d => d.size === 9) ?? [];
  const sixteenByDifficulty = stats?.byDifficulty.filter(d => d.size === 16) ?? [];
  const hasSixteenCompletions = sixteenByDifficulty.some(d => d.gamesPlayed > 0);

  return (
    <Layout>
      <div className={styles.container}>
        <h1 className={styles.title}>Stats</h1>

        {isLoading && <p className={styles.status}>Loading…</p>}
        {error && <p className={styles.errorStatus}>Failed to load stats. Please try again.</p>}

        {isEmpty && (
          <div className={styles.emptyState}>
            <p className={styles.emptyText}>no games yet</p>
            <button type="button" className={styles.startButton} onClick={() => navigate('/select-difficulty')}>
              Start a game
            </button>
          </div>
        )}

        {stats && !error && !isEmpty && (
          <>
            <div className={styles.tiles}>
              <div className={styles.tile}>
                <div className={`${styles.tileValue} tnum`}>{stats.gamesPlayed}</div>
                <div className={styles.tileCaption}>Games played</div>
              </div>
              <div className={styles.tile}>
                <div className={`${styles.tileValue} tnum`}>{stats.gamesWon}</div>
                <div className={styles.tileCaption}>Games won</div>
              </div>
              <div className={styles.tile}>
                <div className={`${styles.tileValue} tnum`}>{winRate}</div>
                <div className={styles.tileCaption}>Win rate</div>
              </div>
            </div>

            <section className={styles.breakdown} aria-labelledby="by-difficulty-9">
              <h2 id="by-difficulty-9" className={styles.eyebrow}>Classic 9×9 — by difficulty</h2>

              <table className={styles.table}>
                <thead>
                  <tr>
                    <th scope="col">Difficulty</th>
                    <th scope="col">Played</th>
                    <th scope="col">Won</th>
                    <th scope="col">Avg</th>
                    <th scope="col">Best</th>
                  </tr>
                </thead>
                <tbody>
                  {nineByDifficulty.map(row => (
                    <tr key={row.difficulty}>
                      <th scope="row" className={styles.difficulty}>{row.difficulty}</th>
                      <td className="tnum">{row.gamesPlayed}</td>
                      <td className="tnum">{row.gamesWon}</td>
                      <td className="tnum">{formatDuration(row.averageSolveTime)}</td>
                      <td className="tnum">{formatDuration(row.bestSolveTime)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </section>

            {hasSixteenCompletions && (
              <section className={styles.breakdown} aria-labelledby="by-difficulty-16">
                <h2 id="by-difficulty-16" className={styles.eyebrow}>Giant 16×16 — by difficulty</h2>

                <table className={styles.table}>
                  <thead>
                    <tr>
                      <th scope="col">Difficulty</th>
                      <th scope="col">Played</th>
                      <th scope="col">Won</th>
                      <th scope="col">Avg</th>
                      <th scope="col">Best</th>
                    </tr>
                  </thead>
                  <tbody>
                    {sixteenByDifficulty.map(row => (
                      <tr key={row.difficulty}>
                        <th scope="row" className={styles.difficulty}>{row.difficulty}</th>
                        <td className="tnum">{row.gamesPlayed}</td>
                        <td className="tnum">{row.gamesWon}</td>
                        <td className="tnum">{formatDuration(row.averageSolveTime)}</td>
                        <td className="tnum">{formatDuration(row.bestSolveTime)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </section>
            )}
          </>
        )}
      </div>
    </Layout>
  );
}
