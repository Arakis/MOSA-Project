// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System
{
	/// <summary>
	///
	/// </summary>
	public struct Int64
	{
		public const long MaxValue = 0x7fffffffffffffff;
		public const long MinValue = -9223372036854775808;

		internal long _value;

		public int CompareTo(long value)
		{
			if (_value < value) return -1;
			else if (_value > value) return 1;
			return 0;
		}

		public bool Equals(long obj)
		{
			return Equals((object)obj);
		}

		public override bool Equals(object obj)
		{
			return ((long)obj) == _value;
		}

		public override int GetHashCode()
		{
			return (int)_value;
		}

		public static bool TryParse(string s, out long result)
		{
			try
			{
				result = Parse(s);
				return true;
			}
			catch
			{
				result = 0;
				return false;
			}
		}

		public static long Parse(string s)
		{
			const string digits = "0123456789";
			long result = 0;

			int z = 0;
			bool neg = false;

			if (s.Length >= 1)
			{
				if (s[0] == '+') z = 1;
				if (s[0] == '-')
				{
					z = 1;
					neg = true;
				}
			}

			for (int i = z; i < s.Length; i++)
			{
				int ind = (int)digits.IndexOf(s[i]);
				if (ind == -1)
				{
					throw new Exception("Format is incorrect");
				}
				result = (long)((result * 10) + ind);
			}

			if (neg) result *= -1;

			return result;
		}

		public override string ToString()
		{
			return CreateString((ulong)_value, true, false);
		}

		public string ToString(string format)
		{
			return CreateString((ulong)_value, false, true);
		}

		unsafe internal static string CreateString(ulong value, bool signed, bool hex)
		{
			int offset = 0;

			ulong uvalue = value;
			ushort divisor = hex ? (ushort)16 : (ushort)10;
			int length = 0;
			int count = 0;
			ulong temp;
			bool negative = false;

			if (value < 0 && !hex && signed)
			{
				count++;
				// TODO: Cannot negate ulong!
				uvalue = (ulong)-(uint)value;
				negative = true;
			}

			temp = uvalue;

			do
			{
				temp /= divisor;
				count++;
			}
			while (temp != 0);

			length = count;
			String result = String.InternalAllocateString(length);

			char* chars = result.first_char;

			if (negative)
			{
				*(chars + offset) = '-';
				offset++;
				count--;
			}

			for (int i = 0; i < count; i++)
			{
				ulong remainder = uvalue % divisor;

				if (remainder < 10)
					*(chars + offset + count - 1 - i) = (char)('0' + remainder);
				else
					*(chars + offset + count - 1 - i) = (char)('A' + remainder - 10);

				uvalue /= divisor;
			}

			return result;
		}

	}
}
