# Handoff: Sudoku — "Zen" Redesign (warm terracotta)

## Overview
A calm, minimal, mobile-first visual redesign of the XenobiaSoft Sudoku React app. It keeps your
existing architecture, routes, data model, and features intact — this is a **skin + interaction
polish**, not a re-architecture. It covers every screen currently in `Sudoku.React` plus two
additions we discussed: a **light/dark theme toggle** and a **Hint** control (you mentioned hints
are planned).

The signature look: warm off-white paper, terracotta accent, `Newsreader` serif for display text,
`Hanken Grotesk` for UI, generous rounding, soft shadows, quick/subtle motion.

## About the Design Files
The files in this bundle (`Sudoku.dc.html`, `Sudoku Redesign.dc.html`, `support.js`) are
**design references built in HTML** — an interactive prototype showing intended look and behavior.
They are **not** production code to copy. The task is to **recreate this design inside the existing
`Sudoku.React` codebase** (React 19 + TypeScript + Vite + CSS Modules), reusing your components,
routes, hooks (`useGameService`, `usePlayerService`), and `apiClient`. Port the *styling and layout*
into each component's `.module.css` and adjust JSX only where a screen gains/loses an element
(e.g. the Hint button, the theme toggle).

- `Sudoku.dc.html` — the full app: all screens, fully playable, with a `palette` (warm/cool) and
  `theme` (light/dark) switch. Open it in a browser to click through everything.
- `Sudoku Redesign.dc.html` — a side-by-side "warm vs cool" comparison canvas (reference only).
- `support.js` — runtime needed for the two `.dc.html` files to run locally. Not part of the app.

To run the prototype: serve this folder over any static server and open `Sudoku.dc.html`.

## Fidelity
**High-fidelity.** Final colors, typography, spacing, radii, shadows, and interactions. Recreate
pixel-close using your existing CSS-Module patterns. Exact values are in **Design Tokens** below.

---

## Design Tokens

### Color — authoritative values are `oklch()` (works in all current browsers)
The prototype themes the whole app by setting CSS custom properties on a root wrapper; every element
reads `var(--token)`. Recommend the same: define these on a `[data-theme]` wrapper (e.g. on
`Layout`'s `.page`) and switch `data-theme` between `light`/`dark`. A `data-palette` can hold
warm/cool if you want to keep both.

**WARM — Light (default / primary direction)**
```
--bg:         oklch(0.982 0.008 78)    /* app background, warm paper       */
--surface:    oklch(0.996 0.005 78)    /* cards, board, inputs             */
--surface-2:  oklch(0.955 0.013 70)    /* number pad, control buttons      */
--ink:        oklch(0.31 0.03 48)      /* primary text + given digits      */
--ink-soft:   oklch(0.53 0.03 52)      /* secondary text, captions         */
--line:       oklch(0.90 0.013 66)     /* hairlines, thin grid lines       */
--line-bold:  oklch(0.52 0.04 48)      /* 3x3 box borders, board outline   */
--accent:     oklch(0.60 0.11 45)      /* terracotta: primary actions      */
--accent-soft:oklch(0.93 0.04 58)      /* selected-cell fill               */
--accent-ink: oklch(0.99 0.006 78)     /* text on accent                   */
--given:      oklch(0.31 0.03 48)      /* fixed clue digit (= --ink)       */
--entry:      oklch(0.56 0.12 46)      /* player-entered digit             */
--highlight:  oklch(0.958 0.016 72)    /* row/col/box highlight fill       */
--same:       oklch(0.91 0.05 58)      /* same-number highlight fill       */
--error:      oklch(0.55 0.15 27)      /* invalid digit / destructive      */
--error-soft: oklch(0.93 0.055 32)     /* invalid-cell fill                */
--shadow:     rgba(70,45,25,0.10)      /* card/board shadow color          */
```
Hint digits reuse `--accent` at font-weight 700 (locked, distinct from givens/entries).

**WARM — Dark**
```
--bg:         oklch(0.21 0.016 55)
--surface:    oklch(0.25 0.018 52)
--surface-2:  oklch(0.29 0.022 52)
--ink:        oklch(0.94 0.012 72)
--ink-soft:   oklch(0.72 0.025 62)
--line:       oklch(0.35 0.02 55)
--line-bold:  oklch(0.60 0.035 55)
--accent:     oklch(0.73 0.11 58)
--accent-soft:oklch(0.34 0.05 50)
--accent-ink: oklch(0.20 0.02 50)
--given:      oklch(0.94 0.012 72)
--entry:      oklch(0.82 0.11 58)
--highlight:  oklch(0.29 0.028 55)
--same:       oklch(0.37 0.06 55)
--error:      oklch(0.71 0.14 32)
--error-soft: oklch(0.37 0.075 32)
--shadow:     rgba(0,0,0,0.35)
```

**COOL — alternate palette** (we explored it; warm was chosen. Include only if you want the toggle.)
```
Light:  --bg oklch(0.981 0.006 220) · --surface oklch(0.995 0.004 220) · --surface-2 oklch(0.955 0.009 220)
        --ink oklch(0.32 0.03 245) · --ink-soft oklch(0.56 0.025 240) · --line oklch(0.90 0.012 230)
        --line-bold oklch(0.55 0.03 240) · --accent oklch(0.60 0.10 215) · --accent-soft oklch(0.93 0.035 215)
        --accent-ink oklch(0.99 0.005 220) · --entry oklch(0.55 0.11 220) · --highlight oklch(0.955 0.014 220)
        --same oklch(0.90 0.045 215) · --error oklch(0.56 0.15 25) · --error-soft oklch(0.93 0.05 25)
        --shadow rgba(30,45,70,0.10)
Dark:   --bg oklch(0.21 0.016 245) · --surface oklch(0.25 0.018 245) · --surface-2 oklch(0.29 0.02 245)
        --ink oklch(0.93 0.012 225) · --ink-soft oklch(0.70 0.02 230) · --line oklch(0.35 0.02 245)
        --line-bold oklch(0.60 0.03 235) · --accent oklch(0.72 0.10 205) · --accent-soft oklch(0.34 0.045 220)
        --accent-ink oklch(0.20 0.02 245) · --entry oklch(0.80 0.11 205) · --highlight oklch(0.29 0.025 240)
        --same oklch(0.37 0.055 210) · --error oklch(0.70 0.14 28) · --error-soft oklch(0.37 0.07 28)
        --shadow rgba(0,0,0,0.35)
```

### Typography
- **Display / serif:** `Newsreader` (Google Fonts), weights 400/500, plus *italic* 400/500 for
  taglines. Used for the wordmark, page titles, big numbers, and the "Solved" headline.
- **UI / sans:** `Hanken Grotesk` (Google Fonts), weights 400/500/600/700. Everything else.
- Load both: `https://fonts.googleapis.com/css2?family=Hanken+Grotesk:wght@400;500;600;700&family=Newsreader:ital,opsz,wght@0,6..72,400;0,6..72,500;0,6..72,600;1,6..72,400;1,6..72,500&display=swap`
- Digits everywhere use `font-variant-numeric: tabular-nums` for time/counters.

Type scale (px unless noted):
| Role | Font | Size | Weight |
|---|---|---|---|
| App wordmark / big title | Newsreader | clamp(40,13vw,54) title; 21 header wordmark | 500 |
| Page title (Select difficulty, Saved games, Profile) | Newsreader | clamp(28,8–9vw,40) | 500 |
| Tagline / italic subtitle | Newsreader italic | 16–19 | 400 |
| "Solved" headline | Newsreader | clamp(50,16vw,68) | 500 |
| Card title | Hanken | 17 | 600 |
| Card subtitle / captions | Hanken | 13 | 400 |
| Body / helper | Hanken | 12.5–14 | 400 |
| Board digit | Hanken | clamp(17,5.6vw,27) | 500 (entry) / 600 (given) / 700 (hint) |
| Pencil note | Hanken | clamp(7,2.1vw,10) | 500 |
| Number-pad digit | Hanken | clamp(21,6.2vw,28) | 500 |
| Control-button label | Hanken | 11 | 400 |
| Eyebrow (uppercase) | Hanken | 11–12, letter-spacing .1–.18em | — |

### Spacing / radii / shadow / motion
- App column: `max-width: 460px`, centered, `min-height: 100%`.
- Content padding: `clamp(18px, 4.5vw, 28px)`. Header padding: `16px clamp(18px,4.5vw,28px)`.
- Board max-width 440px; number pad / controls max-width 440px.
- Radii: cards/inputs/pad/controls **12–15px**; stats card **14px**; board **12px**; theme toggle
  **50%** (36–40px circle); small icon buttons 9–10px.
- Card gap in lists: 11–12px.
- Shadows: cards `0 2px 12px var(--shadow)`; primary/accent `0 6px 18–20px var(--shadow)`;
  board `0 10px 34px var(--shadow)`.
- Transitions: theme cross-fade `background-color .35s ease, color .35s ease`; cell fills
  `background-color .16s ease, color .16s ease`; card hover `transform .18s` (lift `translateY(-2px)`);
  buttons `.15s`.
- Keyframes:
  - `sd-fadeup` — `opacity 0→1, translateY(10px)→0`, ~.4–.5s ease (screen enters).
  - `sd-pop` — `scale .6→1.14→1`, .2s ease (digit lands in a cell).
  - `sd-load` — `opacity .25↔1, translateY 0↔-5px`, 1.1s ease-in-out infinite, staggered .09s per tile.
  - `sd-modal` — `opacity 0→1, translateY(14px)+scale(.97)→none`, .5s (victory).

### Icons
The prototype uses **typographic glyphs** to stay minimal, but your app already uses Font Awesome —
keep FA and map 1:1:
`↶ Undo → fa-undo` · `✕ Erase → fa-eraser` · `✎ Pencil → fa-pencil` · `✦ Hint → fa-lightbulb` ·
`⟲ Reset → fa-trash-can` · `☾/☼ Theme → fa-moon / fa-sun` · `⌄/⌃ stats chevron → fa-chevron-down/up` ·
`→ ‹ › ` arrows for cards/back. Keep sizes ~17–20px; labels 11px beneath.

---

## Screens / Views
Order below maps to your routes/pages. App is a single centered ≤460px column on all screens.

### 0. Shell / Header → `components/Layout.tsx` + `Layout.module.css`
- Replaces the navy→purple gradient bar with a **quiet header**: `display:flex; align-items:center;
  justify-content:space-between`, bottom hairline `1px solid var(--line)`.
- Left slot: on **Home** = serif wordmark "Sudoku" (Newsreader 21/500, `--ink`); on inner pages =
  back affordance "‹ Back" / "‹ Home" (Hanken 14, `--ink-soft`). Center slot: contextual title
  (Newsreader 16). Right slot: **theme toggle** — 36px circle, `--surface-2` bg, `--ink-soft`, ☾/☼.
- Header hidden on: create-profile, loading, victory.
- (If you keep the logo: drop `logo.png` into the left slot instead of the wordmark, ~28px tall.)

### 1. Create Profile → `pages/CreateProfilePage.tsx` + `.module.css`
- **Purpose:** first-run alias creation. Same validation you already have (2–50 chars,
  `^[a-z0-9_-]+$`, 409 = taken).
- **Layout:** vertically centered column, `sd-fadeup` in.
- Serif "Sudoku" clamp(40,13vw,54)/500; italic tagline "a quiet place to think" 17 `--ink-soft`.
- Section heading "Choose your alias" 19/600 `--ink`; helper 14 `--ink-soft`.
- Input: full width, padding 14×16, radius 12, `1px solid var(--line)`, bg `--surface`, 16px,
  placeholder "e.g. quiet-fox". Error line 13 `--error` (reserve ~18px height). Rule hint 12
  `--ink-soft` "2–50 characters · letters, numbers, dashes, underscores".
- Primary button "Begin": padding 14×22, radius 12, bg `--accent`, `--accent-ink` 600/15,
  shadow `0 6px 18px --shadow`, left-aligned.

### 2. Home → `pages/HomePage.tsx` + `.module.css`
- **Purpose:** hub. Cards gate on profile existing exactly like today.
- Greeting block: italic "welcome back," 16 `--ink-soft` + alias in Newsreader clamp(34,10vw,44)/500.
- Three stacked cards (gap 11), each `display:flex; justify-content:space-between; align-items:center`,
  radius 15, padding 17–19×20, `1px solid var(--line)`, shadow `0 2px 12px --shadow`, hover lift.
  **Critical:** the inner text wrapper must be `flex:1; min-width:0;` so titles don't wrap.
  - **New game** (primary): bg `--accent`, title `--accent-ink` 17/600 "New game", subtitle
    "Start a fresh puzzle" (accent-ink @ .8 opacity), arrow `→` accent-ink. → `/select-difficulty`.
  - **Saved games**: title `--ink`, subtitle = "{n} in progress" / "None yet", arrow `→` `--accent`.
    → `/games`.
  - **Profile**: subtitle "Manage your alias", → `/profile`.
- (Prototype merges your original "Create/Manage Profile" into the Profile card and makes
  New Game the hero. Keep the profile-gating/disabled behavior from your current code.)

### 3. Select Difficulty → `pages/SelectDifficultyPage.tsx` + `.module.css`
- Title "Select difficulty" Newsreader clamp(30,9vw,40); italic sub "how much quiet do you want?".
- Three cards (Easy/Medium/Hard) same card style as Home (non-primary): title 17/600, subtitle
  ("A gentle warm-up" / "A steady challenge" / "For a clear mind"), right side dots `·` / `··` / `···`
  in `--accent`. Click → `/new/{difficulty}` (unchanged).

### 4. Loading / New Game → `pages/NewGamePage.tsx` + `.module.css`
- Centered; italic "setting out a {difficulty} puzzle…" 19 `--ink-soft`.
- Row of nine 26×32 tiles (radius 6, bg `--surface-2`, digit `--accent` 600/15) animating
  `sd-load` staggered .09s. Replaces the old plain 1–9 loader. Keep your create→redirect logic.

### 5. Game → `pages/GamePage.tsx` (+ `GameBoard`, `CellInput`, `GameControls`, `GameStats`)
- **Stats card** (`GameStats`): radius 14, bg `--surface`, `1px solid var(--line)`, padding 12×16.
  Collapsed header row = "Time" (14/600) · elapsed (tabular 15) · chevron ⌄. Expanded (click to
  toggle) adds rows: "Total moves", "Invalid moves", "Hints used" — label 13 `--ink-soft`, value 13
  `--ink` tabular. Values come from `game.statistics` + elapsed + local hint counter.
- **Board** (`GameBoard`/`CellInput`): CSS `grid` 9×9, `aspect-ratio:1/1`, max 440, container
  `2px solid var(--line-bold)`, radius 12, overflow hidden, shadow `0 10px 34px --shadow`.
  Per cell (`display:flex; center`; digit font clamp(17,5.6vw,27), tabular):
  - Box separators: `border-right/bottom: 2px solid var(--line-bold)` on columns/rows index 2 & 5;
    otherwise `1px solid var(--line)`; none on last col/row (container supplies outer edge).
  - **given** = `--given` 600 · **player entry** = `--entry` 500 · **hint** = `--accent` 700
    (locked) · **invalid** = `--error` 600.
  - Fill precedence (background): selected `--accent-soft` **+** `box-shadow: inset 0 0 0 2px
    var(--accent)` > invalid `--error-soft` > same-number `--same` > peer (row/col/box) `--highlight`
    > transparent. Fill transition `.16s`.
  - Pencil marks: 3×3 mini-grid inside the cell, font clamp(7,2.1vw,10), `--ink-soft`, showing which
    of 1–9 are in `possibleValues`.
  - Keep keyboard nav (arrows, 1–9, Delete/Backspace) from your `handleKeyDown`.
- **Number pad** (`GameControls`, top block): single flex **row of 9**, gap 6, max 440; each button
  `flex:1; aspect-ratio:1/1.15`, radius 12, bg `--surface-2`, `1px solid var(--line)`; big digit
  clamp(21,6.2vw,28)/500 + small "remaining" count (10px `--ink-soft` = 9 − placed); when remaining
  ≤0 → `opacity .3; pointer-events:none`.
- **Controls row** (below pad): flex, gap 8, max 440; five `flex:1` column buttons (icon + 11px
  label), radius 12, bg `--surface-2`, `1px solid var(--line)`:
  **Undo** (disabled `opacity .4` when no history) · **Erase** · **Pencil** (active = bg `--accent`,
  `--accent-ink`; toggles `pencilMode`) · **Hint** (label "Hint · {n}", disabled at 0) · **Reset**.
  Home lives in the header, not this row.

### 6. Saved Games → `pages/GameListPage.tsx` + `components/GameThumbnail.tsx`
- Title "Saved games" Newsreader clamp(28,8vw,36). Empty state: centered italic "nothing here yet"
  + primary "Start a game".
- Each row: card radius 15, padding 14, `1px solid var(--line)`, shadow `0 2px 12px --shadow`,
  `display:flex; align-items:center; gap:14`:
  - **Thumbnail** left: 88×88, `display:grid` 9×9, `1.5px solid var(--line-bold)`, radius 6,
    overflow hidden; cells 7px, given `--ink-soft` 600 / entry `--accent` 500, `.5px solid var(--line)`
    hairlines. Click → resume (`navigate('/game/{id}')`).
  - **Info** (must be `flex:1; min-width:0`): difficulty 16/600 `--ink`; meta 13 `--ink-soft` tabular
    = "mm:ss · N moves".
  - **Delete** ✕: 36px, `--ink-soft`, hover bg `--error-soft` / color `--error`. Calls your
    `deleteGame`.

### 7. Profile → `pages/ProfilePage.tsx` + `.module.css`
- Title "Your profile" Newsreader clamp(28,8vw,36).
- **Alias card** (radius 15, padding 20, bg `--surface`, border `--line`): uppercase eyebrow "Alias"
  12/.1em `--ink-soft`; value 20/500 `--ink` with "Edit" link (`--accent`). Edit mode swaps to input
  + "Save"(accent) / "Cancel"(surface-2) — reuse your alias validation & 409 handling.
- **Member since card**: eyebrow + `toLocaleDateString`.
- Warning copy 12.5 `--ink-soft`. **Delete profile** = text link `--error`; on click reveal a
  confirm block (bg `--error-soft`, "Yes, delete" `--error` / "Cancel"). Wire to your delete flow.

### 8. Victory → `components/VictoryDisplay.tsx` + `.module.css`
- Centered, `sd-modal` in. Eyebrow = difficulty (uppercase .18em `--accent`); "Solved" Newsreader
  clamp(50,16vw,68)/500; italic "a clear, quiet finish" 18 `--ink-soft`.
- Stat tiles (gap 22): Time / Moves / Invalid / Hints — number Newsreader 28, caption 11 uppercase
  `--ink-soft`.
- Buttons: primary "New puzzle" (→ difficulty) + ghost "Home" (bg `--surface-2`, border `--line`).
  Replaces the emoji "🎉 Puzzle Solved!" modal; keep your `onClose`/navigation.

---

## Interactions & Behavior
- **Navigation** unchanged from `App.tsx` routes. Header left slot drives Back/Home; Home cards and
  Victory buttons drive forward nav. Leaving a game pauses+saves (your existing `pauseGameStatus`).
- **Cell selection** highlights its row, column, and 3×3 box (`--highlight`), all same-value cells
  (`--same`), and rings the selected cell (`--accent` inset). Invalid cells (from `validateCells`)
  show `--error-soft` + `--error` digit.
- **Number entry** animates `sd-pop`; entering a correct value auto-clears that value from
  peers' pencil marks. **Pencil mode** toggles writing to `possibleValues` instead.
- **Hint (new):** reveals the correct value for the selected empty cell (or the next empty cell if
  none selected), locks it (`--accent`, weight 700, not editable/erasable), decrements the counter,
  clears resolved pencil marks, and is Undo-able. Default **3 per game**; **Reset** restores to 3.
  When wired to a backend, swap the local reveal for your hint endpoint but keep the count/lock/stat
  behavior. Counts toward completion.
- **Reset** clears all non-given cells + pencil marks and resets hints. **Undo** pops the last board
  change (prototype keeps a 50-deep local stack; your `undoMove` service can back this).
- **Theme toggle (new):** flips `light`/`dark` with the `.35s` color cross-fade; persist choice
  (e.g. `localStorage 'sudoku-theme'`) and read on load.
- **Motion:** screens enter with `sd-fadeup`; cards lift `translateY(-2px)` on hover; loading tiles
  breathe; victory uses `sd-modal`. All quick and subtle.

## State Management
Reuse `useGameService` / `usePlayerService` / `apiClient` as-is. Redesign adds only:
- `theme: 'light' | 'dark'` — app-level (Context or a small hook), persisted.
- `pencilMode: boolean` — already in `GamePage`.
- `statsExpanded: boolean` — already in `GameStats`.
- **Hint state:** `hintsLeft: number` (start 3; persist per game alongside statistics), plus a
  per-cell `isHint` flag so hinted cells render/behave distinctly. `hintsUsed = 3 - hintsLeft`.
- Everything else (cells, `statistics.totalMoves/invalidMoves/playDuration`, moveHistory, profiles,
  saved games) is your existing model — no schema change required beyond hint tracking.

## Assets
- Fonts: Newsreader + Hanken Grotesk (Google Fonts link above). No other new assets.
- Icons: Font Awesome (already in your app) — mapping in **Icons** above.
- Optional: your existing `public/images/logo.png` if you keep a logo in the header.
- No hand-drawn SVGs; the board, thumbnails, and loader are pure CSS/DOM.

## Files
In this bundle:
- `Sudoku.dc.html` — full interactive prototype (all screens; warm/cool + light/dark switches).
- `Sudoku Redesign.dc.html` — warm-vs-cool comparison canvas (reference only).
- `support.js` — runtime for the two prototype files (not app code).

Target files to restyle in `src/frontend/Sudoku.React/` (map is per-screen above):
`components/Layout`, `components/GameBoard`, `components/CellInput`, `components/GameControls`,
`components/GameStats`, `components/GameThumbnail`, `components/VictoryDisplay`, and
`pages/HomePage`, `pages/CreateProfilePage`, `pages/ProfilePage`, `pages/SelectDifficultyPage`,
`pages/NewGamePage`, `pages/GamePage`, `pages/GameListPage` — each with its `.module.css`.
