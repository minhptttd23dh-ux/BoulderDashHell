using Godot;

namespace BoulderDash
{
    public partial class Game
    {
        private bool TryDrawTexture(string name, Rect2 r, Color? modulate = null)
        {
            if (_textures.TryGetValue(name, out Texture2D tex))
            {
                DrawTextureRect(tex, r, false, modulate);
                return true;
            }
            return false;
        }

        private void DrawTile(Tile t, Rect2 r, int gx, int gy)
        {
            float s = r.Size.X;
            Vector2 c = r.Position + r.Size / 2f;

            switch (t)
            {
                case Tile.Dirt:
                    if (!TryDrawTexture("dirt", r))
                    {
                        DrawRect(r, new Color(0.3f, 0.1f, 0.08f));
                        DrawRect(r.Grow(-s * 0.08f), new Color(0.23f, 0.07f, 0.06f));
                        int h = (gx * 7 + gy * 13) % 5;
                        if (h == 0)
                        {
                            DrawLine(r.Position + new Vector2(s * 0.2f, s * 0.3f), r.Position + new Vector2(s * 0.45f, s * 0.5f), new Color(1f, 0.4f, 0.1f, 0.8f), 2f);
                            DrawLine(r.Position + new Vector2(s * 0.45f, s * 0.5f), r.Position + new Vector2(s * 0.4f, s * 0.75f), new Color(1f, 0.4f, 0.1f, 0.8f), 2f);
                        }
                        else if (h == 2)
                        {
                            DrawCircle(r.Position + new Vector2(s * 0.7f, s * 0.65f), s * 0.05f, new Color(1f, 0.35f, 0.1f, 0.8f));
                        }
                    }
                    break;

                case Tile.Wall:
                    if (!TryDrawTexture("wall", r))
                    {
                        DrawRect(r, new Color(0.1f, 0.05f, 0.14f));
                        DrawRect(r.Grow(-s * 0.06f), new Color(0.2f, 0.1f, 0.27f));
                        DrawRect(new Rect2(r.Position + new Vector2(s * 0.12f, s * 0.12f), new Vector2(s * 0.3f, s * 0.1f)), new Color(0.4f, 0.22f, 0.5f));
                    }
                    break;

                case Tile.Boulder:
                    if (_boulderSheet != null)
                    {
                        int frame = _falling[gx, gy] ? 1 + (int)(_time * 12f + gx) % (BoulderFrames - 1) : 0;
                        float frameW = _boulderSheet.GetWidth() / (float)BoulderFrames;
                        float h = s * _boulderSheet.GetHeight() / frameW;
                        DrawTextureRectRegion(_boulderSheet, new Rect2(r.Position.X, r.End.Y - h, s, h), new Rect2(frame * frameW, 0, frameW, _boulderSheet.GetHeight()));
                    }
                    else if (!TryDrawTexture("boulder", r))
                    {
                        DrawCircle(c, s * 0.47f, new Color(0.9f, 0.25f, 0.05f, 0.35f));
                        DrawCircle(c, s * 0.4f, new Color(0.22f, 0.2f, 0.2f));
                        DrawCircle(c + new Vector2(-s * 0.1f, -s * 0.1f), s * 0.13f, new Color(0.38f, 0.34f, 0.33f));
                        DrawLine(c + new Vector2(s * 0.05f, s * 0.05f), c + new Vector2(s * 0.22f, s * 0.2f), new Color(1f, 0.4f, 0.1f, 0.9f), 2f);
                    }
                    break;

                case Tile.Diamond:
                    if (_diamondSheet != null)
                    {
                        int frame = (int)((_time * 10f + gx * 3 + gy * 5) % DiamondFrames);
                        float frameW = _diamondSheet.GetWidth() / (float)DiamondFrames;
                        DrawTextureRectRegion(_diamondSheet, r.Grow(-s * 0.04f), new Rect2(frame * frameW, 0, frameW, _diamondSheet.GetHeight()));
                    }
                    else if (!TryDrawTexture("diamond", r))
                    {
                        float pulse = 0.5f + 0.5f * Mathf.Sin(_time * 4f + gx + gy);
                        DrawCircle(c, s * (0.36f + 0.04f * pulse), new Color(1f, 0.5f, 0.1f, 0.25f));
                        Vector2[] outer =
                        {
                            c + new Vector2(0, -s * 0.38f), c + new Vector2(s * 0.3f, 0),
                            c + new Vector2(0, s * 0.38f), c + new Vector2(-s * 0.3f, 0),
                        };
                        DrawColoredPolygon(outer, new Color(1f, 0.35f, 0.1f));
                        Vector2[] inner =
                        {
                            c + new Vector2(0, -s * 0.2f), c + new Vector2(s * 0.14f, 0),
                            c + new Vector2(0, s * 0.2f), c + new Vector2(-s * 0.14f, 0),
                        };
                        DrawColoredPolygon(inner, new Color(1f, 0.85f, 0.4f));
                    }
                    break;

                case Tile.Key:
                    if (!TryDrawTexture("key", r))
                    {
                        Color gold = new Color(1f, 0.85f, 0.25f);
                        DrawCircle(c + new Vector2(-s * 0.12f, -s * 0.1f), s * 0.14f, gold);
                        DrawCircle(c + new Vector2(-s * 0.12f, -s * 0.1f), s * 0.06f, new Color(0.1f, 0.05f, 0.05f));
                        DrawLine(c + new Vector2(-s * 0.02f, 0), c + new Vector2(s * 0.28f, s * 0.3f), gold, 4f);
                        DrawLine(c + new Vector2(s * 0.14f, s * 0.16f), c + new Vector2(s * 0.24f, s * 0.06f), gold, 4f);
                    }
                    break;

                case Tile.Door:
                    if (!TryDrawTexture("door", r))
                    {
                        DrawRect(r, new Color(0.15f, 0.1f, 0.05f));
                        DrawRect(r.Grow(-s * 0.08f), new Color(0.35f, 0.25f, 0.1f));
                        for (int i = 1; i <= 3; i++)
                        {
                            DrawLine(r.Position + new Vector2(s * 0.25f * i, s * 0.08f), r.Position + new Vector2(s * 0.25f * i, s * 0.92f), new Color(0.15f, 0.1f, 0.05f), 3f);
                        }
                        DrawCircle(c, s * 0.1f, new Color(1f, 0.85f, 0.25f));
                    }
                    break;

                case Tile.Exit:
                    bool open = GateOpen();
                    if (open)
                    {
                        if (!TryDrawTexture("exit", r))
                        {
                            float pulse = 0.5f + 0.5f * Mathf.Sin(_time * 4f);
                            DrawCircle(c, s * 0.46f, new Color(0.6f, 0.1f, 0.8f));
                            DrawCircle(c, s * (0.28f + 0.06f * pulse), new Color(1f, 0.75f, 1f));
                            DrawArc(c, s * 0.46f, 0, Mathf.Tau, 32, new Color(1f, 0.5f, 1f), 3f);
                        }
                    }
                    else
                    {
                        if (!TryDrawTexture("exit_closed", r) && !TryDrawTexture("exit", r, new Color(0.35f, 0.35f, 0.35f)))
                        {
                            DrawCircle(c, s * 0.46f, new Color(0.18f, 0.04f, 0.04f));
                            DrawArc(c, s * 0.46f, 0, Mathf.Tau, 32, new Color(0.5f, 0.1f, 0.1f), 3f);
                            DrawLine(c + new Vector2(-s * 0.2f, -s * 0.2f), c + new Vector2(s * 0.2f, s * 0.2f), new Color(0.5f, 0.1f, 0.1f), 3f);
                            DrawLine(c + new Vector2(s * 0.2f, -s * 0.2f), c + new Vector2(-s * 0.2f, s * 0.2f), new Color(0.5f, 0.1f, 0.1f), 3f);
                        }
                    }
                    break;
            }
        }

        private void DrawEnemy(Rect2 r)
        {
            if (_enemySheet != null)
            {
                int frame = (int)(_time * 8f) % EnemyFrames;
                float frameW = _enemySheet.GetWidth() / (float)EnemyFrames;
                float w = r.Size.X * EnemyScale;
                float h = w * _enemySheet.GetHeight() / frameW;
                Vector2 centre = r.Position + r.Size / 2f;
                DrawTextureRectRegion(_enemySheet, new Rect2(centre - new Vector2(w, h) / 2f, new Vector2(w, h)), new Rect2(frame * frameW, 0, frameW, _enemySheet.GetHeight()));
                return;
            }

            if (TryDrawTexture("skeleton", r))
            {
                return;
            }

            float s = r.Size.X;
            Vector2 c = r.Position + r.Size / 2f;
            Color bone = new Color(0.92f, 0.9f, 0.8f);
            Color dark = new Color(0.1f, 0.05f, 0.05f);
            DrawCircle(c + new Vector2(0, -s * 0.12f), s * 0.24f, bone);
            DrawRect(new Rect2(c + new Vector2(-s * 0.12f, s * 0.04f), new Vector2(s * 0.24f, s * 0.1f)), bone);
            DrawCircle(c + new Vector2(-s * 0.09f, -s * 0.14f), s * 0.06f, dark);
            DrawCircle(c + new Vector2(s * 0.09f, -s * 0.14f), s * 0.06f, dark);
            DrawLine(c + new Vector2(0, s * 0.14f), c + new Vector2(0, s * 0.4f), bone, 3f);
            for (int i = 0; i < 3; i++)
            {
                float yy = s * (0.2f + 0.07f * i);
                DrawLine(c + new Vector2(-s * 0.16f, yy), c + new Vector2(s * 0.16f, yy), bone, 2f);
            }
        }

        private void DrawPlayer(Rect2 r)
        {
            if (!TryDrawTexture("player", r))
            {
                // Placeholder: the goat
                float s = r.Size.X;
                Rect2 cube = r.Grow(-s * 0.15f);
                DrawRect(cube, Colors.White);
                DrawRect(cube, new Color(0.6f, 0.6f, 0.65f), false, 2f);
            }
        }
    }
}
