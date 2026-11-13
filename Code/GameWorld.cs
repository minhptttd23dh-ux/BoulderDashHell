using Godot;
using System.Collections.Generic;

namespace BoulderDash
{
    public partial class Game
    {
        private bool GateOpen()
        {
            return _diamonds >= _needed && _kills >= _killsNeeded;
        }
        private void DriftSoul()
        {
            List<Vector2I> souls = new List<Vector2I>();
            List<Vector2I> spots = new List<Vector2I>();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    int dist = Mathf.Abs(x - _player.X) + Mathf.Abs(y - _player.Y);
                    if (dist < 4)
                    {
                        continue;
                    }

                    if (_grid[x, y] == Tile.Diamond && !_falling[x, y] && _offset[x, y] == Vector2.Zero)
                    {
                        souls.Add(new Vector2I(x, y));
                    }
                    else if (_grid[x, y] == Tile.Dirt)
                    {
                        int by = y + Gravity;
                        if (InBounds(x, by) && _grid[x, by] != Tile.Empty)
                        {
                            spots.Add(new Vector2I(x, y));
                        }
                    }
                }
            }

            if (souls.Count == 0 || spots.Count == 0)
            {
                return;
            }

            Vector2I from = souls[(int)(_rng.Randi() % (uint)souls.Count)];
            Vector2I to = spots[(int)(_rng.Randi() % (uint)spots.Count)];
            _grid[from.X, from.Y] = Tile.Empty;
            _grid[to.X, to.Y] = Tile.Diamond;
        }

        private Enemy EnemyAt(int x, int y)
        {
            foreach (Enemy e in _enemies)
            {
                if (e.X == x && e.Y == y)
                {
                    return e;
                }
            }
            return null;
        }

        private bool EnemyCanEnter(int x, int y)
        {
            return InBounds(x, y) && _grid[x, y] == Tile.Empty && EnemyAt(x, y) == null;
        }
        private void EnemyStep()
        {
            foreach (Enemy e in _enemies)
            {
                int left = (e.Dir + 3) % 4;
                Vector2I l = Dirs[left];
                Vector2I f = Dirs[e.Dir];

                if (EnemyCanEnter(e.X + l.X, e.Y + l.Y))
                {
                    e.Dir = left;
                    e.X += l.X;
                    e.Y += l.Y;
                }
                else if (EnemyCanEnter(e.X + f.X, e.Y + f.Y))
                {
                    e.X += f.X;
                    e.Y += f.Y;
                }
                else
                {
                    e.Dir = (e.Dir + 1) % 4;
                }

                if (PlayerAt(e.X, e.Y))
                {
                    _state = State.Dead;
                }
            }
        }

    }
}
