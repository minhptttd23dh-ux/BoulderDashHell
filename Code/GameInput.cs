using Godot;

namespace BoulderDash
{
    public partial class Game
    {
        public override void _UnhandledKeyInput(InputEvent @event)
        {
            if (@event is InputEventKey key && key.Pressed && !key.Echo)
            {
                if (key.Keycode == Key.R)
                {
                    if (_state == State.AllClear)
                    {
                        _score = 0;
                        LoadLevel(0);
                    }
                    else
                    {
                        LoadLevel(_levelIndex);
                    }
                }
                else if (key.Keycode == Key.H)
                {
                    _hintHidden = !_hintHidden;
                }
                else if (key.Keycode == Key.F11)
                {
                    bool full = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
                    DisplayServer.WindowSetMode(full ? DisplayServer.WindowMode.Windowed : DisplayServer.WindowMode.Fullscreen);
                }
                else if (key.Keycode == Key.M || key.Keycode == Key.Tab)
                {
                    _showFullMap = !_showFullMap;
                    QueueRedraw();
                }
                else if ((key.Keycode == Key.Enter || key.Keycode == Key.Space) && _state == State.LevelComplete)
                {
                    LoadLevel(_levelIndex + 1);
                }
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mb && mb.Pressed)
            {
                if (mb.ButtonIndex == MouseButton.Left)
                {
                    _hintHidden = true;
                }
            }
        }

        private Vector2I ReadDirection()
        {
            if (Input.IsKeyPressed(Key.Left) || Input.IsKeyPressed(Key.A)) return Vector2I.Left;
            if (Input.IsKeyPressed(Key.Right) || Input.IsKeyPressed(Key.D)) return Vector2I.Right;
            if (Input.IsKeyPressed(Key.Up) || Input.IsKeyPressed(Key.W)) return Vector2I.Up;
            if (Input.IsKeyPressed(Key.Down) || Input.IsKeyPressed(Key.S)) return Vector2I.Down;
            return Vector2I.Zero;
        }

    }
}
