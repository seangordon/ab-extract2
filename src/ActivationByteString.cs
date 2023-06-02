using System;

namespace ab_extract
{
	public class ActivationByteString : FixedLengthByteString
	{
		public static ActivationByteString Parse(string hexString)
		{
			if (!TryParse(hexString, 4, out ActivationByteString byteString))
			{
				throw new Exception("Activation Bytes must be 8 hex chars (4 bytes) long.");
			}
			return byteString;
		}
	}
}
