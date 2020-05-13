using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelEngine
{
	public struct Color
	{
        private static Color empty = new Color(0, 0, 0, 0);
        private static Color white = new Color(0xffffffff);
		private static Color grey = new Color(0xa9a9a9ff);
		private static Color red = new Color(0xff0000ff);
		private static Color yellow = new Color(0xffff00ff);
		private static Color green = new Color(0x00ff00ff);
		private static Color cyan = new Color(0x00ffffff);
		private static Color blue = new Color(0x0000ffff);
		private static Color magenta = new Color(0xff00ffff);
		private static Color brown = new Color(0x9a6324ff);
		private static Color orange = new Color(0xf58231ff);
		private static Color purple = new Color(0x911eb4ff);
		private static Color lime = new Color(0xbfef45ff);
		private static Color pink = new Color(0xfabebeff);
		private static Color snow = new Color(0xFFFAFAff);
		private static Color teal = new Color(0x469990ff);
		private static Color lavender = new Color(0xe6beffff);
		private static Color beige = new Color(0xfffac8ff);
		private static Color maroon = new Color(0x800000ff);
		private static Color mint = new Color(0xaaffc3ff);
		private static Color olive = new Color(0x808000ff);
		private static Color apricot = new Color(0xffd8b1ff);
		private static Color navy = new Color(0x000075ff);
		private static Color black = new Color(0x000000ff);
		private static Color darkGrey = new Color(0x8B8B8Bff);
		private static Color darkRed = new Color(0x8B0000ff);
		private static Color darkYellow = new Color(0x8B8B00ff);
		private static Color darkGreen = new Color(0x008B00ff);
		private static Color darkCyan = new Color(0x008B8Bff);
		private static Color darkBlue = new Color(0x00008Bff);
        private static Color darkMagenta = new Color(0x8B008Bff);

		public static readonly Color Empty = empty;
		public static readonly Color White = white;
		public static readonly Color Grey = grey;
		public static readonly Color Red = red;
		public static readonly Color Yellow = yellow;
		public static readonly Color Green = green;
		public static readonly Color Cyan = cyan;
		public static readonly Color Blue = blue;
		public static readonly Color Magenta = magenta;
		public static readonly Color Brown = brown;
		public static readonly Color Orange = orange;
		public static readonly Color Purple = purple;
		public static readonly Color Lime = lime;
		public static readonly Color Pink = pink;
		public static readonly Color Snow = snow;
		public static readonly Color Teal = teal;
		public static readonly Color Lavender = lavender;
		public static readonly Color Beige = beige;
		public static readonly Color Maroon = maroon;
		public static readonly Color Mint = mint;
		public static readonly Color Olive = olive;
		public static readonly Color Apricot = apricot;
		public static readonly Color Navy = navy;
		public static readonly Color Black = black;
		public static readonly Color DarkGrey = darkGrey;
		public static readonly Color DarkRed = darkRed;
		public static readonly Color DarkYellow = darkYellow;
		public static readonly Color DarkGreen = darkGreen;
		public static readonly Color DarkCyan = darkCyan;
		public static readonly Color DarkBlue = darkBlue;
        public static readonly Color DarkMagenta = darkMagenta;

        public byte R;
		public byte G;
		public byte B;
		public byte A;

		public Color(uint argb)
		{
			A = (byte)(argb & 0xFF);
			B = (byte)((argb >> 8) & 0xFF);
			G = (byte)((argb >> 16) & 0xFF);
			R = (byte)((argb >> 24) & 0xFF);
		}
		public Color(byte grey, byte alpha = 255) : this(grey, grey, grey, alpha)
		{
		}
		public Color(byte red, byte green, byte blue, byte alpha = 255)
		{
			R = red;
			G = green;
			B = blue;
			A = alpha;
		}
		public Color(float red, float green, float blue, float alpha = 1f)
		{
			R = (byte)(255 * red);
			G = (byte)(255 * green);
			B = (byte)(255 * blue);
			A = (byte)(255 * alpha);
		}

		public static Color Random()
		{
			byte[] vals = Randoms.RandomBytes(3);
			return new Color(vals[0], vals[1], vals[2]);
		}
		public static Color RandomAlpha()
		{
			byte[] vals = Randoms.RandomBytes(4);
			return new Color(vals[0], vals[1], vals[2], vals[3]);
		}
		public static Color FromRgb(uint rgb)
		{
			byte a = (byte)(rgb & 0xFF);
			byte b = (byte)((rgb >> 8) & 0xFF);
			byte g = (byte)((rgb >> 16) & 0xFF);
			byte r = (byte)((rgb >> 24) & 0xFF);

			return new Color(r, g, b, a);
		}
		public static Color FromHsv(float h, float s, float v)
		{
			float c = v * s;
			float nh = (h / 60) % 6;
			float x = c * (1 - Math.Abs(nh % 2 - 1));
			float m = v - c;

			float r, g, b;

			if (0 <= nh && nh < 1)
			{
				r = c;
				g = x;
				b = 0;
			}
			else if (1 <= nh && nh < 2)
			{
				r = x;
				g = c;
				b = 0;
			}
			else if (2 <= nh && nh < 3)
			{
				r = 0;
				g = c;
				b = x;
			}
			else if (3 <= nh && nh < 4)
			{
				r = 0;
				g = x;
				b = c;
			}
			else if (4 <= nh && nh < 5)
			{
				r = x;
				g = 0;
				b = c;
			}
			else if (5 <= nh && nh < 6)
			{
				r = c;
				g = 0;
				b = x;
			}
			else
			{
				r = 0;
				g = 0;
				b = 0;
			}

			r += m;
			g += m;
			b += m;

			return new Color((byte)Math.Floor(r * 255), (byte)Math.Floor(g * 255), (byte)Math.Floor(b * 255));
		}
		public static Color Tint(Color color, Color tint)
		{
			float[] normal = Normalize(tint);
			return new Color((byte)(color.R * normal[0]), (byte)(color.G * normal[1]), (byte)(color.B * normal[2]), (byte)(color.A * normal[3]));
		}
		public static Color Lerp(Color a, Color b, float fraction)
		{
			float R = (b.R - a.R) * fraction + a.R;
			float G = (b.G - a.G) * fraction + a.G;
			float B = (b.B - a.B) * fraction + a.B;
			float A = (b.A - a.A) * fraction + a.A;

			return new Color((byte)R, (byte)G, (byte)B, (byte)A);
		}
		public static float[] Normalize(Color color)
		{
			return new float[] { color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f };
		}

        #region Operator overloading
        public static Color operator *(Color a, float value)
		{
			return new Color(a.R, a.G, a.B, (byte)(a.A * value));
		}
		public static Color operator +(Color a, Color b)
		{
			return new Color(a.R + b.R, a.G + b.G, a.B + b.B);
		}
		public static Color operator -(Color a, Color b)
		{
			return new Color(a.R - b.R, a.G - b.G, a.B - b.B);
		}
		public static Color operator *(Color a, Color b)
		{
			return Tint(a, b);
		}
		public static Color operator ^(Color a, Color b)
		{
			return new Color(a.R ^ b.R, a.G ^ b.G, a.B ^ b.B);
		}
		public static bool operator ==(Color a, Color b)
		{
			return (a.R == b.R) && (a.G == b.G) && (a.B == b.B) && (a.A == b.A);
		}
		public static bool operator !=(Color a, Color b) => !(a == b);

		public override bool Equals(object obj)
		{
			return (obj is Color c) ? this == c : false;
		}
		public override int GetHashCode()
		{
			int hashCode = 196078;
			hashCode = hashCode * -152113 + R.GetHashCode();
			hashCode = hashCode * -152113 + G.GetHashCode();
			hashCode = hashCode * -152113 + B.GetHashCode();
			hashCode = hashCode * -152113 + A.GetHashCode();
			return hashCode;
		}
        #endregion
    }
}