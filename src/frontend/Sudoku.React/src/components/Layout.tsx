import { type ReactNode } from 'react';
import styles from './Layout.module.css';

interface LayoutProps {
  children: ReactNode;
}

export default function Layout({ children }: LayoutProps) {
  return (
    <div className={styles.page}>
      <header className={styles.siteHeader}>
        <img src="/images/logo.png" alt="XenobiaSoft Sudoku" />
      </header>
      <main>
        {children}
      </main>
    </div>
  );
}
