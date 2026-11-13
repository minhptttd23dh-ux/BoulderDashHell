using Godot;

namespace BoulderDash
{
    public partial class Game
    {
        private void ComputeView()
        {
            if (_showFullMap)
            {
                _ts = Mathf.Floor(Mathf.Min(ViewW / (float)_width, ViewH / (float)_height));
                _origin = new Vector2((ViewW - _ts * _width) / 2f, HudHeight + (ViewH - _ts * _height) / 2f);
            }
            else
            {
                _ts = ZoomTile;
                float levelW = _width * _ts;
                float levelH = _height * _ts;
                Vector2 pc = (_playerVisual + new Vector2(0.5f, 0.5f)) * _ts;
                float ox = levelW <= ViewW ? (ViewW - levelW) / 2f : Mathf.Clamp(ViewW / 2f - pc.X, ViewW - levelW, 0f);
                float oy = levelH <= ViewH ? (ViewH - levelH) / 2f : Mathf.Clamp(ViewH / 2f - pc.Y, ViewH - levelH, 0f);
                _origin = new Vector2(ox, HudHeight + oy);
            }
        }

        private static readonly Color HudColor = new Color(0.16f, 0.04f, 0.04f);
        private static readonly Color GoldText = new Color(1f, 0.82f, 0.45f);

        public override void _Draw()
        {
            if (_grid == null)
            {
                return;
            }

            ComputeView();
            Font font = _font;
            Font titleFont = _titleFont;

            DrawBackground();

            DrawTowerFloor();

            float pad = _ts;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_grid[x, y] == Tile.Void)
                    {
                        continue;
                    }
                    Vector2 pos = _origin + new Vector2(x, y) * _ts + _offset[x, y] * _ts;
                    if (_wobbling[x, y])
                    {
                        pos += new Vector2(Mathf.Sin(_time * 60f) * _ts * 0.06f, 0);
                    }
                    if (pos.X < -pad || pos.X > ViewW || pos.Y < HudHeight - pad || pos.Y > HudHeight + ViewH)
                    {
                        continue;
                    }
                    DrawTile(_grid[x, y], new Rect2(pos, new Vector2(_ts, _ts)), x, y);
                }
            }

            foreach (Enemy e in _enemies)
            {
                DrawEnemy(new Rect2(_origin + e.Visual * _ts, new Vector2(_ts, _ts)));
            }

            Rect2 playerRect = new Rect2(_origin + _playerVisual * _ts, new Vector2(_ts, _ts));
            DrawPlayer(playerRect);
            DrawRect(new Rect2(0, 0, ViewW, HudHeight), HudColor);
            DrawRect(new Rect2(0, HudHeight - 2, ViewW, 2), new Color(0.75f, 0.15f, 0.08f));
            string left = "Level " + (_levelIndex + 1) + "    Rings " + _diamonds + "/" + _needed
                         + (_killsNeeded > 0 ? "    Demons " + _kills + "/" + _killsNeeded : "");
            string right = "Score " + _score + "    Time " + Mathf.CeilToInt(_timeLeft)
                           + (_keys > 0 ? "    Keys " + _keys : "");
            DrawString(titleFont, new Vector2(16, 42), left, HorizontalAlignment.Left, -1, 34, GoldText);
            DrawString(titleFont, new Vector2(0, 42), right, HorizontalAlignment.Right, ViewW - 16, 34, GoldText);
            string keys = (_showFullMap ? "M: back to zoom" : "M: full map") + "   F11: fullscreen";
            Vector2 keysSize = font.GetStringSize(keys, HorizontalAlignment.Left, -1, 22);
            DrawRect(new Rect2(10, HudHeight + ViewH - keysSize.Y - 18, keysSize.X + 20, keysSize.Y + 12), new Color(0, 0, 0, 0.6f));
            DrawString(font, new Vector2(20, HudHeight + ViewH - 16 - font.GetDescent(22)), keys, HorizontalAlignment.Left, -1, 22, new Color(0.9f, 0.75f, 0.65f));

            if (!_showFullMap)
            {
                DrawMiniMap();
            }

            if (_hint != "" && !_hintHidden && _state == State.Playing && (_tutorial || _levelClock < 8f))
            {
                const int hintSize = 28;
                float textWidth = ViewW - 76;
                Vector2 textSize = font.GetMultilineStringSize(_hint, HorizontalAlignment.Left, textWidth, hintSize);
                float titleBlock = 64;
                float boxH = titleBlock + textSize.Y + 28;
                Rect2 box = new Rect2(20, HudHeight + 10, ViewW - 40, boxH);
                DrawRect(box, new Color(0, 0, 0, 0.8f));
                DrawRect(box, new Color(0.75f, 0.15f, 0.08f), false, 2f);
                DrawString(titleFont, new Vector2(38, box.Position.Y + 46), _title, HorizontalAlignment.Left, -1, 40, GoldText);
                DrawString(font, new Vector2(box.Position.X, box.Position.Y + 36), "Click to hide", HorizontalAlignment.Right, box.Size.X - 18, 22, new Color(0.7f, 0.6f, 0.55f));
                DrawMultilineString(font, new Vector2(38, box.Position.Y + titleBlock + font.GetAscent(hintSize)), _hint, HorizontalAlignment.Left, textWidth, hintSize, -1, new Color(0.95f, 0.9f, 0.85f));
            }

            if (_state != State.Playing)
            {
                float fade = Mathf.Clamp(_stateTime / 0.7f, 0f, 1f);
                float tfade = Mathf.Clamp((_stateTime - 0.4f) / 0.6f, 0f, 1f);
                string sub = "";
                switch (_state)
                {
                    case State.Dead: sub = "Press R to try again"; break;
                    case State.LevelComplete: sub = "Press Enter for the next level"; break;
                    case State.AllClear: sub = "Score " + _score + "    Press R to play again"; break;
                }
                float midY = HudHeight + ViewH / 2f;
                DrawRect(new Rect2(0, HudHeight, ViewW, ViewH), new Color(0, 0, 0, 0.35f * fade));
                DrawRect(new Rect2(0, midY - 90, ViewW, 180), new Color(0, 0, 0, 0.85f * fade));
                DrawRect(new Rect2(0, midY - 90, ViewW, 2), new Color(0.75f, 0.15f, 0.08f, fade));
                DrawRect(new Rect2(0, midY + 88, ViewW, 2), new Color(0.75f, 0.15f, 0.08f, fade));
                DrawString(titleFont, new Vector2(0, midY - 4), _endLine, HorizontalAlignment.Center, ViewW, 48, new Color(GoldText.R, GoldText.G, GoldText.B, tfade));
                DrawString(font, new Vector2(0, midY + 52), sub, HorizontalAlignment.Center, ViewW, 30, new Color(0.85f, 0.75f, 0.7f, tfade));
            }
        }

    }
}
