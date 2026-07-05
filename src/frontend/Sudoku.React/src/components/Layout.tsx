import { type ReactNode } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useTheme } from '../hooks/useTheme';
import styles from './Layout.module.css';

interface LayoutProps {
  children: ReactNode;
  /** Hide the header entirely (create-profile, loading, victory). */
  hideHeader?: boolean;
  /** Contextual title shown in the header center slot. */
  title?: string;
  /** Override the back affordance action (e.g. pause-then-home on the game screen). */
  onBack?: () => void;
  /** Label for the back affordance (default "Home"). */
  backLabel?: string;
}

export default function Layout({ children, hideHeader = false, title, onBack, backLabel = 'Home' }: LayoutProps) {
  const location = useLocation();
  const navigate = useNavigate();
  const { theme, toggleTheme } = useTheme();

  const isHome = location.pathname === '/';
  const handleBack = onBack ?? (() => navigate('/'));

  return (
    <div className={styles.page}>
      <div className={styles.column}>
        {!hideHeader && (
          <header className={styles.header}>
            <div className={styles.left}>
              {isHome ? (
                <span className={styles.wordmark}>Sudoku</span>
              ) : (
                <button type="button" className={styles.back} onClick={handleBack}>
                  <span aria-hidden="true">‹</span> {backLabel}
                </button>
              )}
            </div>
            <div className={styles.center}>{title}</div>
            <div className={styles.right}>
              <button
                type="button"
                className={styles.themeToggle}
                onClick={toggleTheme}
                aria-label={theme === 'light' ? 'Switch to dark theme' : 'Switch to light theme'}
              >
                <i className={theme === 'light' ? 'fa-solid fa-moon' : 'fa-solid fa-sun'} />
              </button>
            </div>
          </header>
        )}
        <main className={styles.main}>{children}</main>
      </div>
    </div>
  );
}
