using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PixelEngine
{
    public class Sprite
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private Color[] colorData = null;

        public Sprite(int w, int h)
        {
            Width = w;
            Height = h;

            colorData = new Color[Width * Height];
        }

        public Sprite(int w, int h, params Color[] data)
        {
            Width = w;
            Height = h;

            colorData = data;
        }

        public Point Size => new Point(Width, Height);
        public Point Center => Size / 2;

        public ref Color[] GetData() => ref colorData;

        private void LoadFromBitmapSlow(Bitmap bmp, params Color[] ignoreColors)
        {
            List<Color> ignoreC = new List<Color>(ignoreColors);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    System.Drawing.Color c = bmp.GetPixel(x, y);
                    Color color = new Color(c.R, c.G, c.B, c.A);

                    if (ignoreC.Contains(color))
                        this[x, y] = Color.Empty;
                    else
                        this[x, y] = new Color(c.R, c.G, c.B, c.A);
                }
            }
        }

        private void LoadFromBitmap(Bitmap bmp, params Color[] ignoreColors)
        {
            List<Color> ignoreC = new List<Color>(ignoreColors);
            Width = bmp.Width;
            Height = bmp.Height;

            colorData = new Color[Width * Height];

            unsafe
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

                byte* scan0 = (byte*)bmpData.Scan0;

                int depth = Image.GetPixelFormatSize(bmp.PixelFormat);

                int length = Width * Height * depth / 8;

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int i = ((y * Width) + x) * depth / 8;

                        System.Drawing.Color c = System.Drawing.Color.Empty;

                        switch (depth)
                        {
                            case 32:
                                {
                                    byte b = scan0[i];
                                    byte g = scan0[i + 1];
                                    byte r = scan0[i + 2];
                                    byte a = scan0[i + 3];
                                    c = System.Drawing.Color.FromArgb(a, r, g, b);
                                    break;
                                }

                            case 24:
                                {
                                    byte b = scan0[i];
                                    byte g = scan0[i + 1];
                                    byte r = scan0[i + 2];
                                    c = System.Drawing.Color.FromArgb(r, g, b);
                                    break;
                                }

                            case 8:
                                {
                                    byte b = scan0[i];
                                    c = System.Drawing.Color.FromArgb(b, b, b);
                                    break;
                                }
                        }

                        Color color = new Color(c.R, c.G, c.B, c.A);
                        if (ignoreC.Contains(color))
                            this[x, y] = Color.Empty;
                        else
                            this[x, y] = new Color(c.R, c.G, c.B, c.A);
                    }
                }

                bmp.UnlockBits(bmpData);
            }
        }
        private static Sprite LoadFromSpr(string path)
        {
            Color Parse(short col)
            {
                switch (col & 0xF)
                {
                    case 0x0: return Color.Presets.Black;
                    case 0x1: return Color.Presets.DarkBlue;
                    case 0x2: return Color.Presets.DarkGreen;
                    case 0x3: return Color.Presets.DarkCyan;
                    case 0x4: return Color.Presets.DarkRed;
                    case 0x5: return Color.Presets.DarkMagenta;
                    case 0x6: return Color.Presets.DarkYellow;
                    case 0x7: return Color.Presets.Grey;
                    case 0x8: return Color.Presets.DarkGrey;
                    case 0x9: return Color.Presets.Blue;
                    case 0xA: return Color.Presets.Green;
                    case 0xB: return Color.Presets.Cyan;
                    case 0xC: return Color.Presets.Red;
                    case 0xD: return Color.Presets.Magenta;
                    case 0xE: return Color.Presets.Yellow;
                    case 0xF: return Color.Presets.White;
                }

                return Color.Empty;
            }

            Sprite spr;

            using (Stream stream = File.OpenRead(path))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int w = reader.ReadInt32();
                int h = reader.ReadInt32();

                spr = new Sprite(w, h);

                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                        spr[j, i] = Parse(reader.ReadInt16());
            }

            return spr;
        }

        public static Sprite Load(string path, params Color[] ignoreColors)
        {
            if (!File.Exists(path))
                return new Sprite(8, 8);

            if (path.EndsWith(".spr"))
            {
                return LoadFromSpr(path);
            }
            else
            {
                using (Bitmap bmp = (Bitmap)Image.FromFile(path))
                {
                    Sprite spr = new Sprite(bmp.Width, bmp.Height);
                    spr.LoadFromBitmapSlow(bmp, ignoreColors);
                    return spr;
                }
            }

        }
        public static void Save(Sprite spr, string path)
        {
            unsafe
            {
                using (Bitmap bmp = new Bitmap(spr.Width, spr.Height))
                {
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                    BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

                    byte* scan0 = (byte*)bmpData.Scan0;

                    int depth = Image.GetPixelFormatSize(bmp.PixelFormat);

                    int length = spr.Width * spr.Height * depth / 8;

                    for (int x = 0; x < spr.Width; x++)
                    {
                        for (int y = 0; y < spr.Height; y++)
                        {
                            Color p = spr[x, y];

                            int i = ((y * spr.Width) + x) * depth / 8;

                            scan0[i] = p.B;
                            scan0[i + 1] = p.G;
                            scan0[i + 2] = p.R;
                            scan0[i + 3] = p.A;
                        }
                    }

                    bmp.UnlockBits(bmpData);

                    bmp.Save(path);
                }
            }
        }
        public static void Copy(Sprite src, Sprite dest)
        {
            if (src.colorData.Length != dest.colorData.Length)
                return;

            src.colorData.CopyTo(dest.colorData, 0);
        }


        public Color this[int x, int y]
        {
            get => GetPixel(x, y);
            set => SetPixel(x, y, value);
        }

        private Color GetPixel(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return colorData[y * Width + x];
            else
                return Color.Empty;
        }
        private void SetPixel(int x, int y, Color p)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                colorData[y * Width + x] = p;
        }

    }
}