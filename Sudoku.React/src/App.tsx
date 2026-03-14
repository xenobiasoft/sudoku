import { BrowserRouter, Routes, Route } from 'react-router-dom';
import HomePage from './pages/HomePage';
import NewGamePage from './pages/NewGamePage';
import GamePage from './pages/GamePage';
import './App.css';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/new/:difficulty" element={<NewGamePage />} />
        <Route path="/game/:puzzleId" element={<GamePage />} />
      </Routes>
    </BrowserRouter>
  );
}
