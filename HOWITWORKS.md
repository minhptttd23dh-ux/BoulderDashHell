# How the code is organised

All code is in `Code/`. `Game` is one `partial class` split into files by topic.

| File | What it does |
|---|---|
| `Game.cs` | Fields, setup, level loading, random gate placement |
| `GameInput.cs` | Keyboard and mouse |
| `GameLoop.cs` | Per frame loop, end screen text, smooth movement |
| `GamePlayer.cs` | Player moves, pushing boulders, keys, doors, gate |
| `GamePhysics.cs` | Falling, rolling, wobbling and crushing |
| `GameWorld.cs` | Gate rule, drifting rings, demon movement |
| `GameDraw*.cs` | Camera, HUD, background, tiles, minimap |
| `Levels.cs` | Level settings (title, hint, goals, timer) |
| `Levels/*.tscn` | Level maps painted in a `TileMapLayer`, with `Levels/tiles.tres` |

## Design choices

- **The grid holds the rules.** A 2D array of tile types is the game state. Falling, rolling and crushing all work on it.
- **Logic and drawing are separate.** The grid changes in steps. Smooth movement is only a drawn offset that slides to the real position.
- **Levels are painted in the editor.** Each level is a scene with a `TileMapLayer`. On load, `LoadLevel` reads every cell's `kind` label into the grid and then frees the scene. Level settings stay in `Levels.cs`.
- **Shapes first, art later.** Tiles are drawn as simple shapes until a PNG with the right name is put in `Art/`. The names are listed in `README.md`.
- **Drawing with `_Draw()`.** A level has hundreds of identical tiles, so one draw call avoids hundreds of nodes and keeps camera and fades in one place.

## Limits

- The `TileMapLayer` is only used to author and load levels. Drawing and movement still use the grid and `_Draw()`, so smooth sliding is not affected.
- Difficulty and balance need more playtesting.
- Not built yet: shooting demon, magic wall.
