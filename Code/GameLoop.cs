using Godot;

namespace BoulderDash
{
    public partial class Game
    {
        public override void _Process(double delta)
        {
            float dt = (float)delta;

            UpdateVisuals(dt);

            if (_state != State.Playing)
            {
                return;
            }

            _levelClock += dt;
            _idle += dt;
            _timeLeft -= dt;
            if (_timeLeft <= 0)
            {
                _timeLeft = 0;
                _state = State.Dead;
            }

            _playerTimer += dt;
            if (_playerTimer >= PlayerStepTime)
            {
                Vector2I dir = ReadDirection();
                if (dir != Vector2I.Zero)
                {
                    Vector2I before = _player;
                    TryMovePlayer(dir);
                    if (_player != before)
                    {
                        _idle = 0;
                    }
                    _playerTimer = 0;
                }
            }

            _physicsTimer += dt;
            if (_physicsTimer >= PhysicsStepTime)
            {
                _physicsTimer = 0;
                PhysicsStep();
            }

            if (_driftSeconds > 0)
            {
                _driftTimer += dt;
                if (_driftTimer >= _driftSeconds)
                {
                    _driftTimer = 0;
                    DriftSoul();
                }
            }

            _enemyTimer += dt;
            if (_enemyTimer >= EnemyStepTime)
            {
                _enemyTimer = 0;
                EnemyStep();
            }
        }

        private void PickEndLine()
        {
            switch (_state)
            {
                case State.Dead:
                    _endLine = _timeLeft <= 0f ? "Time ran out. The gates stay shut." : DeathLines[_rng.RandiRange(0, DeathLines.Length - 1)];
                    break;
                case State.LevelComplete:
                    _endLine = "The gate opens. Heaven draws nearer.";
                    break;
                case State.AllClear:
                    _endLine = "The last ring is claimed. Your penance is paid.";
                    break;
            }
        }

        private void UpdateVisuals(float dt)
        {
            _time += dt;

            if (_state == State.Playing)
            {
                _stateTime = 0f;
            }
            else
            {
                if (_stateTime <= 0f)
                {
                    PickEndLine();
                }
                _stateTime += dt;
            }

            Vector2 target = new Vector2(_player.X, _player.Y);
            if (_playerVisual != target)
            {
                _playerVisual = _playerVisual.MoveToward(target, dt * 1.3f / PlayerStepTime);
            }

            foreach (Enemy e in _enemies)
            {
                Vector2 et = new Vector2(e.X, e.Y);
                if (e.Visual != et)
                {
                    e.Visual = e.Visual.MoveToward(et, dt * 1.3f / EnemyStepTime);
                }
            }

            float slide = dt / PhysicsStepTime;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_offset[x, y] != Vector2.Zero)
                    {
                        _offset[x, y] = _offset[x, y].MoveToward(Vector2.Zero, slide);
                    }
                }
            }

            QueueRedraw();
        }

    }
}
