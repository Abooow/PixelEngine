using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelEngine
{
	public struct Color
	{
		public byte R { get; private set; }
		public byte G { get; private set; }
		public byte B { get; private set; }
		public byte A { get; private set; }

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

		public enum Mode
		{
			Normal,
			Adjective,
			Negative,
			Multiply,
			Xor,
			Mask,
			Alpha,
			Custom
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

		#region Presets
		public enum Presets : uint
		{
			White = 0xffffff,
			Grey = 0xa9a9a9,
			Red = 0xff0000,
			Yellow = 0xffff00,
			Green = 0x00ff00,
			Cyan = 0x00ffff,
			Blue = 0x0000ff,
			Magenta = 0xff00ff,
			Brown = 0x9a6324,
			Orange = 0xf58231,
			Purple = 0x911eb4,
			Lime = 0xbfef45,
			Pink = 0xfabebe,
			Snow = 0xFFFAFA,
			Teal = 0x469990,
			Lavender = 0xe6beff,
			Beige = 0xfffac8,
			Maroon = 0x800000,
			Mint = 0xaaffc3,
			Olive = 0x808000,
			Apricot = 0xffd8b1,
			Navy = 0x000075,
			Black = 0x000000,
			DarkGrey = 0x8B8B8B,
			DarkRed = 0x8B0000,
			DarkYellow = 0x8B8B00,
			DarkGreen = 0x008B00,
			DarkCyan = 0x008B8B,
			DarkBlue = 0x00008B,
			DarkMagenta = 0x8B008B
		}

		public static readonly Color Empty = new Color(0, 0, 0, 0);

		private static Dictionary<Presets, Color> presetPixels;
		public static Color[] PresetPixels => presetPixels.Values.ToArray();
		#endregion

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

		static Color()
		{
			Color ToPixel(Presets p)
			{
				string hex = p.ToString("X");

				byte r = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
				byte g = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);
				byte b = (byte)Convert.ToUInt32(hex.Substring(6, 2), 16);

				return new Color(r, g, b);
			}

			Presets[] presets = (Presets[])Enum.GetValues(typeof(Presets));
			presetPixels = presets.ToDictionary(p => p, p => ToPixel(p));
		}

		public static Color Tint(Color color, Color tint)
		{
			float[] normal = Normalize(tint);
			return new Color((byte)(color.R * normal[0]), (byte)(color.G * normal[1]), (byte)(color.B * normal[2]), (byte)(color.A * normal[3]));
		}

		public static float[] Normalize(Color color)
		{
			return new float[]{ color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f };
		}

		public static Color Lerp(Color a, Color b, float fraction)
		{
			float R = (b.R - a.R) * fraction + a.R;
			float G = (b.G - a.G) * fraction + a.G;
			float B = (b.B - a.B) * fraction + a.B;
			float A = (b.A - a.A) * fraction + a.A;

			return new Color((byte)R, (byte)G, (byte)B, (byte)A);
		}

		public static Color operator *(Color a, float value)
		{
			return new Color(a.R, a.G, a.B, (byte)(a.A * value));
		}

		public static Color operator +(Color a, Color b)
		{
			return new Color(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
		}

		public static Color operator -(Color a, Color b)
		{
			return new Color(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
		}

		public static Color operator *(Color a, Color b)
		{
			return Tint(a, b);
		}

		public static Color operator ^(Color a, Color b)
		{
			return new Color(a.R ^ b.R, a.G ^ b.G, a.B ^ b.B, a.A ^ b.A);
		}

		public static bool operator ==(Color a, Color b)
		{
			return (a.R == b.R) && (a.G == b.G) && (a.B == b.B) && (a.A == b.A);
		}

		public static bool operator !=(Color a, Color b) => !(a == b);

		public static implicit operator Color(Presets p)
		{
			if (presetPixels.TryGetValue(p, out Color pix))
				return pix;
			return Empty;
		}

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
	}
}