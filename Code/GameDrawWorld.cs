using Godot;

namespace BoulderDash
{
    public partial class Game
    {
        private void DrawBackground()
        {
            if (_textures.TryGetValue("background", out Texture2D bg))
            {
                float px = Mathf.Clamp(_origin.X * 0.02f, -36f, 36f);
                float py = Mathf.Clamp((_origin.Y - HudHeight) * 0.02f, -20f, 20f);
                DrawTextureRect(bg, new Rect2(-40 + px, HudHeight - 24 + py, ViewW + 80, ViewH + 48), false);
            }
            else
            {
                const int bands = 16;
                float bandH = ViewH / (float)bands;
                for (int i = 0; i < bands; i++)
                {
                    float f = i / (float)(bands - 1);
                    Color c = new Color(0.04f + 0.28f * f * f, 0.0f + 0.05f * f, 0.02f + 0.01f * f);
                    DrawRect(new Rect2(0, HudHeight + i * bandH, ViewW, bandH + 1), c);
                }
            }
        }

        private void DrawTowerFloor()
        {
            float pad = _ts;
            Color tint = new Color(1f, 1f, 1f);
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_grid[x, y] == Tile.Void)
                    {
                        continue;
                    }
                    Vector2 pos = _origin + new Vector2(x, y) * _ts;
                    if (pos.X < -pad || pos.X > ViewW || pos.Y < HudHeight - pad * 2 || pos.Y > HudHeight + ViewH)
                    {
                        continue;
                    }
                    Rect2 r = new Rect2(pos, new Vector2(_ts, _ts));
                    if (!TryDrawTexture("floor", r, tint))
                    {
                        bool alt = ((x + y) & 1) == 0;
                        DrawRect(r, alt ? new Color(0.12f, 0.07f, 0.15f) : new Color(0.10f, 0.06f, 0.13f));
                        DrawRect(r, new Color(0.06f, 0.03f, 0.08f), false, 1f);
                    }
                    if (_grid[x, y] == Tile.Wall && (y == 0 || _grid[x, y - 1] == Tile.Void))
                    {
                        DrawMerlons(r);
                    }
                }
            }
        }

        private void DrawMerlons(Rect2 r)
        {
            float s = r.Size.X;
            float mw = s * 0.34f;
            float mh = s * 0.26f;
            for (int i = 0; i < 2; i++)
            {
                float fx = 0.06f + i * 0.54f;
                Rect2 m = new Rect2(r.Position.X + s * fx, r.Position.Y - mh, mw, mh);
                if (_textures.TryGetValue("wall", out Texture2D wt))
                {
                    Rect2 src = new Rect2(wt.GetWidth() * fx, 0, wt.GetWidth() * 0.34f, wt.GetHeight() * 0.26f);
                    DrawTextureRectRegion(wt, m, src);
                }
                else
                {
                    DrawRect(m, new Color(0.2f, 0.12f, 0.3f));
                }
                DrawRect(m, new Color(0.07f, 0.04f, 0.1f), false, 2f);
            }
        }

        private void DrawMiniMap()
        {
            float mp = Mathf.Min(8f, Mathf.Floor(Mathf.Min(300f / _width, 200f / _height)));
            if (mp < 2f)
            {
                mp = 2f;
            }
            float mapW = _width * mp;
            float mapH = _height * mp;
            Vector2 topLeft = new Vector2(ViewW - mapW - 14, HudHeight + ViewH - mapH - 14);

            DrawRect(new Rect2(topLeft - new Vector2(4, 4), new Vector2(mapW + 8, mapH + 8)), new Color(0, 0, 0, 0.7f));
            DrawRect(new Rect2(topLeft - new Vector2(4, 4), new Vector2(mapW + 8, mapH + 8)), new Color(0.75f, 0.15f, 0.08f), false, 2f);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Tile t = _grid[x, y];
                    if (t == Tile.Empty || t == Tile.Void)
                    {
                        continue;
                    }
                    Color c = Colors.White;
                    switch (t)
                    {
                        case Tile.Dirt: c = new Color(0.4f, 0.14f, 0.1f); break;
                        case Tile.Wall: c = new Color(0.45f, 0.25f, 0.55f); break;
                        case Tile.Boulder: c = new Color(0.55f, 0.52f, 0.5f); break;
                        case Tile.Diamond: c = new Color(1f, 0.55f, 0.15f); break;
                        case Tile.Key: c = new Color(1f, 0.9f, 0.3f); break;
                        case Tile.Door: c = new Color(0.9f, 0.7f, 0.2f); break;
                        case Tile.Exit: c = GateOpen() ? new Color(0.9f, 0.3f, 1f) : new Color(0.5f, 0.1f, 0.1f); break;
                    }
                    DrawRect(new Rect2(topLeft + new Vector2(x * mp, y * mp), new Vector2(mp, mp)), c);
                }
            }

            Rect2 visible = new Rect2(-_origin.X / _ts, -(_origin.Y - HudHeight) / _ts, ViewW / _ts, ViewH / _ts);
            visible = visible.Intersection(new Rect2(0, 0, _width, _height));
            if (visible.Size.X > 0 && visible.Size.Y > 0)
            {
                DrawRect(new Rect2(topLeft + visible.Position * mp, visible.Size * mp), new Color(1f, 1f, 1f, 0.8f), false, 1f);
            }

            foreach (Enemy e in _enemies)
            {
                DrawRect(new Rect2(topLeft + new Vector2(e.X * mp, e.Y * mp), new Vector2(mp, mp)), new Color(1f, 0.1f, 0.1f));
            }

            // Player marker blinks
            if (Mathf.Sin(_time * 8f) > -0.3f)
            {
                DrawRect(new Rect2(topLeft + _playerVisual * mp - new Vector2(1, 1), new Vector2(mp + 2, mp + 2)), Colors.White);
            }
        }

    }
}
