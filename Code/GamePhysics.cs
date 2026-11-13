using Godot;

namespace BoulderDash
{
    public partial class Game
    {
        private bool IsRound(Tile t)
        {
            return t == Tile.Boulder || t == Tile.Diamond;
        }

        private bool PlayerAt(int x, int y)
        {
            return _player.X == x && _player.Y == y;
        }

        private bool CanRollTo(int x, int y)
        {
            int by = y + Gravity;
            if (!InBounds(x, y) || !InBounds(x, by)) return false;
            if (_grid[x, y] != Tile.Empty || _grid[x, by] != Tile.Empty) return false;
            if (PlayerAt(x, y) || PlayerAt(x, by)) return false;
            if (EnemyAt(x, y) != null || EnemyAt(x, by) != null) return false;
            return true;
        }

        private void MoveObject(int fromX, int fromY, int toX, int toY)
        {
            _grid[toX, toY] = _grid[fromX, fromY];
            _grid[fromX, fromY] = Tile.Empty;
            _falling[toX, toY] = true;
            _falling[fromX, fromY] = false;
            _moved[toX, toY] = true;
            _offset[toX, toY] = new Vector2(fromX - toX, fromY - toY);
            _offset[fromX, fromY] = Vector2.Zero;
        }

        private void PhysicsStep()
        {
            System.Array.Clear(_moved, 0, _moved.Length);
            System.Array.Clear(_wobbling, 0, _wobbling.Length);
            for (int y = _height - 2; y >= 0; y--)
            {
                for (int x = 0; x < _width; x++)
                {
                    Tile t = _grid[x, y];
                    if (!IsRound(t) || _moved[x, y])
                    {
                        continue;
                    }

                    int by = y + Gravity;
                    Tile below = _grid[x, by];

                    if (PlayerAt(x, by))
                    {
                        if (_falling[x, y])
                        {
                            _state = State.Dead;
                        }
                        else if (_idle >= WobbleStart)
                        {
                            _wobbling[x, y] = true;
                            if (_idle >= WobbleStart + WobbleDuration)
                            {
                                _state = State.Dead;
                            }
                        }
                        _falling[x, y] = false;
                        continue;
                    }

                    Enemy victim = EnemyAt(x, by);
                    if (victim != null)
                    {
                        if (_falling[x, y])
                        {
                            // A falling rock turns the skeleton into a soul.
                            _enemies.Remove(victim);
                            _kills++;
                            _score += 25;
                            _grid[x, by] = Tile.Diamond;
                        }
                        _falling[x, y] = false;
                        continue;
                    }

                    if (below == Tile.Empty)
                    {
                        MoveObject(x, y, x, by);
                        continue;
                    }

                    _falling[x, y] = false;

                    // Roll off the top of other round objects.
                    if (IsRound(below))
                    {
                        // Roll to whichever side is free. If both sides are free, pick one at random;
                        // if one side is blocked, use the other.
                        bool canLeft = CanRollTo(x - 1, y);
                        bool canRight = CanRollTo(x + 1, y);
                        if (canLeft && canRight)
                        {
                            if (_rng.Randf() < 0.5f) canRight = false; else canLeft = false;
                        }
                        if (canLeft)
                        {
                            MoveObject(x, y, x - 1, y);
                            _falling[x - 1, y] = false;
                        }
                        else if (canRight)
                        {
                            MoveObject(x, y, x + 1, y);
                            _falling[x + 1, y] = false;
                        }
                    }
                }
            }
        }

    }
}
