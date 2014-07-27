
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Drawing 
{
	[Serializable]
	public struct Color {
		int value;

		public string Name {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool IsKnownColor {
			get{
				throw new NotImplementedException ();
			}
		}

		public bool IsSystemColor {
			get{
				throw new NotImplementedException ();
			}
		}

		public bool IsNamedColor {
			get{
				throw new NotImplementedException ();
			}
		}
		#endregion

		public static Color FromArgb (int red, int green, int blue)
		{
			return FromArgb (255, red, green, blue);
		}

		public static Color FromArgb (int alpha, int red, int green, int blue)
		{
			if((red > 255) || (red < 0))
				throw CreateColorArgumentException(red, "red");
			if((green > 255) || (green < 0))
				throw CreateColorArgumentException (green, "green");
			if((blue > 255) || (blue < 0))
				throw CreateColorArgumentException (blue, "blue");
			if((alpha > 255) || (alpha < 0))
				throw CreateColorArgumentException (alpha, "alpha");

			Color color = new Color ();
			color.value = (int)((uint) alpha << 24) + (red << 16) + (green << 8) + blue;
			return color;
		}

		public int ToArgb()
		{
			return (int) value;
		} 

		public static Color FromArgb (int alpha, Color baseColor)
		{
			return FromArgb (alpha, baseColor.R, baseColor.G, baseColor.B);
		}

		public static Color FromArgb (int argb)
		{
			return FromArgb ((argb >> 24) & 0x0FF, (argb >> 16) & 0x0FF, (argb >> 8) & 0x0FF, argb & 0x0FF);
		}




		public static readonly Color Empty;

		public static bool operator == (Color left, Color right)
		{
			return left.value == right.value;
		}

		public static bool operator != (Color left, Color right)
		{
			return left.value != right.value;
		}

		public float GetBrightness ()
		{
			byte minval = Math.Min (R, Math.Min (G, B));
			byte maxval = Math.Max (R, Math.Max (G, B));

			return (float)(maxval + minval) / 510;
		}

		public float GetSaturation ()
		{
			byte minval = (byte) Math.Min (R, Math.Min (G, B));
			byte maxval = (byte) Math.Max (R, Math.Max (G, B));

			if (maxval == minval)
				return 0.0f;

			int sum = maxval + minval;
			if (sum > 255)
				sum = 510 - sum;

			return (float)(maxval - minval) / sum;
		}

		public float GetHue ()
		{
			int r = R;
			int g = G;
			int b = B;
			byte minval = (byte) Math.Min (r, Math.Min (g, b));
			byte maxval = (byte) Math.Max (r, Math.Max (g, b));

			if (maxval == minval)
				return 0.0f;

			float diff = (float)(maxval - minval);
			float rnorm = (maxval - r) / diff;
			float gnorm = (maxval - g) / diff;
			float bnorm = (maxval - b) / diff;

			float hue = 0.0f;
			if (r == maxval) 
				hue = 60.0f * (6.0f + bnorm - gnorm);
			if (g == maxval) 
				hue = 60.0f * (2.0f + rnorm - bnorm);
			if (b  == maxval) 
				hue = 60.0f * (4.0f + gnorm - rnorm);
			if (hue > 360.0f) 
				hue = hue - 360.0f;

			return hue;
		}


		public bool IsEmpty 
		{
			get {
				return value == 0;
			}
		}

		public byte A {
			get { return (byte) (value >> 24); }
		}

		public byte R {
			get { return (byte) (value >> 16); }
		}

		public byte G {
			get { return (byte) (value >> 8); }
		}

		public byte B {
			get { return (byte) value; }
		}

		public override bool Equals (object obj)
		{
			if (!(obj is Color))
				return false;
			Color c = (Color) obj;
			return this == c;
		}

		public override int GetHashCode ()
		{
			return value;
		}

		public override string ToString ()
		{
			if (IsEmpty)
				return "Color [Empty]";

			return String.Format ("Color [A={0}, R={1}, G={2}, B={3}]", A, R, G, B);
		}

		private static ArgumentException CreateColorArgumentException (int value, string color)
		{
			return new ArgumentException (string.Format ("'{0}' is not a valid"
				+ " value for '{1}'. '{1}' should be greater or equal to 0 and"
				+ " less than or equal to 255.", value, color));
		}

	
	}
}

