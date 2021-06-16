using System;
using System.Text;

namespace Hishop.Plugins.Payment.YeePay
{
	/// <summary>
	/// Digest ¿‡ 
	/// </summary>
	internal class Digest
	{
		internal Digest(){}

		internal string HmacSign(string aValue, string aKey)
		{
			byte[] k_ipad = new byte[64];
			byte[] k_opad = new byte[64];
			byte[] keyb;
			byte[] Value;
			keyb = Encoding.UTF8.GetBytes(aKey);
			Value = Encoding.UTF8.GetBytes(aValue);

			for( int i=keyb.Length; i<64; i++)
				k_ipad[i] = 54;

			for( int i=keyb.Length; i<64; i++)
				k_opad[i] = 92;

			for(int i = 0; i < keyb.Length; i++)
			{
				k_ipad[i] = (byte)(keyb[i] ^ 0x36);
				k_opad[i] = (byte)(keyb[i] ^ 0x5c);
			}

			//Console.WriteLine( "k_ipad: {0}", toHex( k_ipad ) );
			//Console.WriteLine( "Value: {0}", toHex( Value ) );

			HmacMD5 md = new HmacMD5();
							
			md.Update(k_ipad, (uint)k_ipad.Length);
			md.Update(Value, (uint)Value.Length);			
			byte[] dg = md.Finalize1();
			md.Init();			
			md.Update(k_opad, (uint)k_opad.Length);
			md.Update(dg, 16);
			dg = md.Finalize1();
			
			return toHex(dg);
		}

		internal static String toHex(byte[] input)
		{
			if(input == null)
				return null;
			
			StringBuilder output = new StringBuilder(input.Length * 2);
			
			for(int i = 0; i < input.Length; i++)
			{
				int current = input[i] & 0xff;
				if(current < 16)
					output.Append("0");
				output.Append( current.ToString("x") );
			}

			return output.ToString();
		}
	}
}