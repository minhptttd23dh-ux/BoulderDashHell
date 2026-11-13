using Godot;
using System.Collections.Generic;

namespace BoulderDash
{
    public enum Tile { Empty, Dirt, Wall, Boulder, Diamond, Exit, Key, Door, Void }
    public partial class Game : Node2D
    {
        public const int HudHeight = 60;
        public const int ViewW = 1280;
        public const int ViewH = 720 - HudHeight;
        public const float ZoomTile = 72f;

        private const int Gravity = 1;

        [Export] public float EnemyScale = 1.5f;
        [Export] public float MusicVolumeDb = -8f;
        [Export] public float PlayerStepTime = 0.13f;
        [Export] public float PhysicsStepTime = 0.15f;
        [Export] public float EnemyStepTime = 0.35f; 
        [Export] public float WobbleStart = 1.0f;
        [Export] public float WobbleDuration = 0.8f; 
        private enum State { Playing, Dead, LevelComplete, AllClear }

        private class Enemy
        {
            public int X;
            public int Y;
            public int Dir;
            public Vector2 Visual; 
        }

        private static readonly Vector2I[] Dirs = { Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left };

        private Tile[,] _grid;
        private bool[,] _falling;
        private bool[,] _moved;
        private bool[,] _wobbling;
        private int _width;
        private int _height;
        private Vector2I _player;

        private Vector2 _playerVisual;
        private Vector2[,] _offset; 

        private int _kills;
        private int _killsNeeded;
        private float _driftSeconds; 
        private float _driftTimer;

        private readonly List<Enemy> _enemies = new List<Enemy>();
        private string _title = "";
        private string _hint = "";
        private float _enemyTimer;
        private float _levelClock;
        private float _idle; 
        private int _keys; 
        private bool _hintHidden;
        private bool _tutorial;

        private int _levelIndex;
        private int _diamonds;
        private int _needed;
        private int _score;
        private int _scoreAtLevelStart;
        private float _timeLeft;
        private float _playerTimer;
        private float _physicsTimer;
        private State _state = State.Playing;
        private float _stateTime;
        private string _endLine = "";

        private static readonly string[] DeathLines =
        {
            "Your journey ends here, little goat.",
            "The rings will wait a little longer.",
            "Your penance is not yet paid.",
            "Hell keeps one more goat.",
            "The flock will not find you here.",
        };

        private bool _showFullMap;
        private float _ts;
        private Vector2 _origin;
        private float _time;

        private RandomNumberGenerator _rng;

        private Font _font;
        private Font _titleFont;

        private const int DiamondFrames = 8;
        private Texture2D _diamondSheet;

        private const int EnemyFrames = 4;
        private Texture2D _enemySheet;

        private const int BoulderFrames = 5;
        private Texture2D _boulderSheet;

        private readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

        public override void _Ready()
        {
            foreach (string name in new[] { "dirt", "wall", "boulder", "diamond", "exit", "exit_closed", "player", "skeleton", "key", "door", "floor", "background" })
            {
                string plain = name.Replace("_", "");
                foreach (string file in new[] { Capitalise(name), Capitalise(plain), name, plain })
                {
                    string path = "res://Art/" + file + ".png";
                    if (ResourceLoader.Exists(path))
                    {
                        _textures[name] = GD.Load<Texture2D>(path);
                        break;
                    }
                }
            }

            _diamondSheet = FindSheet("diamond");

            _enemySheet = FindSheet("enemy");

            _boulderSheet = FindSheet("boulder");

            _font = LoadFont("body");
            _titleFont = LoadFont("title");
            if (_titleFont != _font)
            {
                _titleFont.Fallbacks = new Godot.Collections.Array<Font> { _font };
            }

            _rng = new RandomNumberGenerator();
            _rng.Randomize();
            StartMusic();
            LoadLevel(0);
        }
        private void StartMusic()
        {
            const string path = "res://Audio/hellawaitsdemo.mp3";
            if (!ResourceLoader.Exists(path))
            {
                return;
            }

            AudioStream music = GD.Load<AudioStream>(path);
            if (music is AudioStreamMP3 mp3)
            {
                mp3.Loop = true;
            }
            AudioStreamPlayer player = new AudioStreamPlayer();
            player.Stream = music;
            player.VolumeDb = MusicVolumeDb;
            AddChild(player);
            player.Play();
        }

        private static Texture2D FindSheet(string name)
        {
            string cap = Capitalise(name);
            foreach (string file in new[] { name + "_anim", cap + "_anim", cap + "Animation", name + "Animation", name + "_animation", cap + "_animation" })
            {
                string path = "res://Art/" + file + ".png";
                if (ResourceLoader.Exists(path))
                {
                    return GD.Load<Texture2D>(path);
                }
            }
            return null;
        }

        private static string Capitalise(string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        private Font LoadFont(string name)
        {
            foreach (string ext in new[] { ".ttf", ".otf", ".woff2", ".woff" })
            {
                string path = "res://Fonts/" + name + ext;
                if (ResourceLoader.Exists(path))
                {
                    Font f = GD.Load<Font>(path);
                    if (f != null)
                    {
                        return f;
                    }
                }
            }
            return ThemeDB.FallbackFont;
        }


        private void LoadLevel(int index)
        {
            _levelIndex = index;
            _scoreAtLevelStart = _score;
            Levels.LevelData data = Levels.All[index];
            string number = (index + 1).ToString("00");
            string levelPath = "res://Levels/level_" + number + ".tscn";
            if (!ResourceLoader.Exists(levelPath))
            {
                levelPath = "res://Levels/level" + number + ".tscn";
            }
            Node levelRoot = GD.Load<PackedScene>(levelPath).Instantiate();
            TileMapLayer layer = levelRoot.GetNode<TileMapLayer>("Tiles");
            Rect2I area = layer.GetUsedRect();
            _width = area.Size.X;
            _height = area.Size.Y;
            _grid = new Tile[_width, _height];
            _falling = new bool[_width, _height];
            _moved = new bool[_width, _height];
            _wobbling = new bool[_width, _height];
            _idle = 0;
            _keys = 0;
            _offset = new Vector2[_width, _height];
            _hintHidden = false;
            _tutorial = data.Tutorial;
            _needed = data.DiamondsNeeded;
            _killsNeeded = data.KillsNeeded;
            _kills = 0;
            _driftSeconds = data.DriftSeconds;
            _driftTimer = 0;
            _timeLeft = data.TimeSeconds;
            _title = data.Title ?? "";
            _hint = data.Hint ?? "";
            _enemyTimer = 0;
            _levelClock = 0;
            _enemies.Clear();
            _diamonds = 0;
            _playerTimer = 0;
            _physicsTimer = 0;
            _state = State.Playing;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    TileData cell = layer.GetCellTileData(area.Position + new Vector2I(x, y));
                    string kind = cell == null ? "void" : (string)cell.GetCustomData("kind");
                    switch (kind)
                    {
                        case "wall": _grid[x, y] = Tile.Wall; break;
                        case "dirt": _grid[x, y] = Tile.Dirt; break;
                        case "boulder": _grid[x, y] = Tile.Boulder; break;
                        case "diamond": _grid[x, y] = Tile.Diamond; break;
                        case "exit": _grid[x, y] = Tile.Exit; break;
                        case "key": _grid[x, y] = Tile.Key; break;
                        case "door": _grid[x, y] = Tile.Door; break;
                        case "void": _grid[x, y] = Tile.Void; break;
                        case "skeleton":
                            _grid[x, y] = Tile.Empty;
                            _enemies.Add(new Enemy { X = x, Y = y, Dir = 3, Visual = new Vector2(x, y) });
                            break;
                        case "player":
                            _grid[x, y] = Tile.Empty;
                            _player = new Vector2I(x, y);
                            break;
                        default: _grid[x, y] = Tile.Empty; break;
                    }
                }
            }
            levelRoot.Free();
            if (data.RandomExit)
            {
                PlaceRandomExit();
            }

            _playerVisual = new Vector2(_player.X, _player.Y);
            QueueRedraw();
        }

        private void PlaceRandomExit()
        {
            Vector2I oldExit = new Vector2I(-1, -1);
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_grid[x, y] == Tile.Exit)
                    {
                        oldExit = new Vector2I(x, y);
                    }
                }
            }
            if (oldExit.X < 0)
            {
                return;
            }

            bool[,] seen = new bool[_width, _height];
            Queue<Vector2I> queue = new Queue<Vector2I>();
            queue.Enqueue(_player);
            seen[_player.X, _player.Y] = true;
            while (queue.Count > 0)
            {
                Vector2I p = queue.Dequeue();
                foreach (Vector2I d in Dirs)
                {
                    Vector2I n = p + d;
                    if (InBounds(n.X, n.Y) && !seen[n.X, n.Y] && _grid[n.X, n.Y] != Tile.Wall && _grid[n.X, n.Y] != Tile.Void)
                    {
                        seen[n.X, n.Y] = true;
                        queue.Enqueue(n);
                    }
                }
            }

            List<Vector2I> spots = new List<Vector2I>();
            int minDist = (_width + _height) / 3;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    int dist = Mathf.Abs(x - _player.X) + Mathf.Abs(y - _player.Y);
                    if (seen[x, y] && dist >= minDist && (_grid[x, y] == Tile.Dirt || _grid[x, y] == Tile.Empty) && EnemyAt(x, y) == null)
                    {
                        spots.Add(new Vector2I(x, y));
                    }
                }
            }
            if (spots.Count == 0)
            {
                return;
            }

            Vector2I chosen = spots[(int)(_rng.Randi() % (uint)spots.Count)];
            _grid[oldExit.X, oldExit.Y] = Tile.Dirt;
            _grid[chosen.X, chosen.Y] = Tile.Exit;
        }
    }
}