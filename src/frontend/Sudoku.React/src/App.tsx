import { BrowserRouter, Routes, Route } from 'react-router-dom';
import HomePage from './pages/HomePage';
import NewGamePage from './pages/NewGamePage';
import GamePage from './pages/GamePage';
import CreateProfilePage from './pages/CreateProfilePage';
import ProfilePage from './pages/ProfilePage';
import SelectDifficultyPage from './pages/SelectDifficultyPage';
import GameListPage from './pages/GameListPage';
import StatsPage from './pages/StatsPage';
import { ThemeProvider } from './hooks/ThemeProvider';
import './App.css';

export default function App() {
  return (
    <ThemeProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/new/:difficulty" element={<NewGamePage />} />
          <Route path="/game/:puzzleId" element={<GamePage />} />
          <Route path="/create-profile" element={<CreateProfilePage />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/select-difficulty" element={<SelectDifficultyPage />} />
          <Route path="/games" element={<GameListPage />} />
          <Route path="/stats" element={<StatsPage />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}
