# Boulder Dash (placeholder prototype)

See also HOWITWORKS.md for how the code is organised.

Godot 4.7 + C#. Open the folder in Godot, press Build (hammer icon), then Play (F5).

## Controls

* Arrow keys or WASD: move and dig
* Push boulders sideways, collect enough diamonds to open the exit
* R: restart level, Enter: next level
* Click: hides the hint box (H brings it back). F11: fullscreen. The window is 1280x720 and can be resized
* M (or Tab): switch between the zoomed view (follows the player, minimap in the corner) and the full map

## Levels

Tutorials with on-screen hints: Welcome to Hell, Falling Rocks, Demons, Keys and Gates.
Then Ashfields, Walled Pits, and three non-rectangular levels (Crossroads, The Ring, Caverns), and The Abyss.
Each level has a Title, Hint, DiamondsNeeded and TimeSeconds in `Code/Levels.cs`.

## Rules implemented

* Dirt is dug away when you walk into it
* Boulders and diamonds fall, and roll off other boulders/diamonds
* Standing still under a resting rock for 1 second makes it wobble, and 0.8 seconds later it drops on you
(`WobbleStart` and `WobbleDuration` on the Main node)
* Rocks and rings roll off round objects to a free side (random if both are free)
* A falling boulder or diamond kills you
* Demons (`x` in a level) patrol empty tunnels by following the wall on their left, they kill you on touch,
and a falling rock turns a demon into a ring. Speed: `EnemyStepTime` on the Main node
* The gate opens when `DiamondsNeeded` rings are collected AND `KillsNeeded` demons are defeated (a rock must fall on them). Set either to 0 to ignore it
* `DriftSeconds` (per level): every N seconds one ring far from the player respawns buried somewhere else
* `RandomExit = true` (per level) puts the gate in a random reachable spot every time the level starts
* Levels do not have to be rectangles: leave cells unpainted (void) outside and put wall tiles around the playable area
* Keys (`k`) open doors (`D`): walk into a door while holding a key, each key opens one door
* Exit opens after the required diamonds, time bonus is added to the score

## Fonts

`Fonts/title.otf` (Dystopian Canticle, HUD and titles) and `Fonts/body.ttf` (Poppins, hint text). Replace the files to change the fonts, see `Fonts/FONTS.txt`.

## Where to work (so nobody edits the same file)

* Art: put PNGs in `Art/` named `dirt.png`, `wall.png`, `boulder.png`, `diamond.png` (the ring), `skeleton.png` (the demon), `exit.png`, `player.png`
(optional `exit\_closed.png`, `key.png`, `door.png` and `background.png`, 960x560). They replace the placeholder shapes automatically. Tiles are square, 64x64 looks best.
* Levels: paint the map in `Levels/level\_NN.tscn` (TileMapLayer, see below). Title, hint and goals are in `Code/Levels.cs`
* Rules and drawing: `Code/Game.cs`
* Theme: hell. Placeholders are red/black, the player is a white cube (the goat goes here)
* Levels can be any size (level 3 is 40x24), the zoomed view scrolls to follow the player

## Tower

* `Art/floor.png` is the stone floor inside the tower (shows behind every non-void tile). Wall tiles on the open top edge get battlements.
* Death messages are in `DeathLines`. End screens fade in.

## Editing levels (TileMapLayer)

1. Open `Levels/level\_NN.tscn`, select the `Tiles` node and paint with the TileMap panel at the bottom.
2. The tiles come from `Levels/tiles.tres`: dirt, wall, boulder, diamond, exit, key, door, skeleton (the demon), player (start) and empty (an open tunnel).
3. Leave a cell unpainted to make it void. Paint exactly one player tile and one exit tile.
4. For a new level, duplicate a scene as `level\_NN.tscn` with the next number and add its entry to `Code/Levels.cs`.
Each tile has a custom data label called `kind`. The code reads that label, so a new tile type needs a label and a case in `LoadLevel` in `Code/Game.cs`.
