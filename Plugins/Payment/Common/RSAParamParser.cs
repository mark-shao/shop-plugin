using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;


namespace Hishop.Plugins.Payment
{
	/// <summary>
	/// 从密钥文件或者PUBLICKEYBLOB中提取密钥
	/// </summary>
	public class RSAParamParser
	{

		private byte btype;
		private uint magic;
		private int bitlen; 
		private uint pubexp;
		private uint algid;
		private byte[] RSAmodulus;
		private byte[] Exponent;

		private byte[] P;
		private byte[] Q;
		private byte[] DP;
		private byte[] DQ;
		private byte[] IQ;
		private byte[] D;

		private bool dotnetkey = false;
		private bool extradata = false;

		//--  PUBLICKEYSTRUCT (BLOBHEADER) structure values for CryptoAPI keyblobs  (8 bytes) ----
		const byte SIMPLEBLOB		= 	0x01;
		const byte PUBLICKEYBLOB	= 	0x06;
		const byte PRIVATEKEYBLOB	=	0x07;
		const byte PLANTEXTKEYBLOB	=	0x08;
		const byte OPAQUEKEYBLOB	=	0x09;
		const byte PUBLICKEYBLOBEX	=	0x0A;
		const byte SYMMETRICWRAPKEYBLOB	=	0x0B;


		const byte CUR_BLOB_VERSION = 	0x02;
		const ushort reserved = 	0x0000;
		const uint CALG_RSA_KEYX = 	0x0000a400;
		const uint CALG_RSA_SIGN = 	0x00002400;

		//--  RSAPUBKEY structure values for RSA blob (12 bytes) ----
		const uint RSA1 =		0x31415352;  //"RSA1" publickeyblob
		const uint RSA2 = 		0x32415352;  //"RSA2" privatekeyblob

		private RSAParamParser()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		public bool isValidPublickeyblob(BinaryReader r)
		{
			try
			{
				//---- read the BLOBHEADER struct ------
	 
				btype = r.ReadByte();  
				if(btype != PUBLICKEYBLOB && btype != PRIVATEKEYBLOB)
				{	//possibly a .NET publickey
					r.BaseStream.Seek(11, SeekOrigin.Current);		//advance past 3 int headers, minus 1 byte read
					btype=r.ReadByte();
					if(btype != PUBLICKEYBLOB && btype != PRIVATEKEYBLOB)
						return false;
					dotnetkey = true;
				}
				if(r.ReadByte() != CUR_BLOB_VERSION)
					return false;
				if(r.ReadUInt16() != reserved)
					return false;
				algid = r.ReadUInt32();
				if(algid != CALG_RSA_KEYX && algid != CALG_RSA_SIGN)
					return false;

				//---- read the RSAPUBKEY struct --------
				magic = r.ReadUInt32();
				if(magic != RSA1 && magic !=RSA2)
					return false;
				bitlen = r.ReadInt32() ;   //get RSA bit length

				//---- read RSA public exponent ------
				pubexp = r.ReadUInt32() ;   //get public exponent
				Exponent = BitConverter.GetBytes(pubexp); //returns bytes in little-endian order
				Array.Reverse(Exponent);    //convert to BIG-endian order
	
				//---- read RSA modulus -----------
				//Reverse byte array for little-endian to big-endian conversion 
				RSAmodulus = r.ReadBytes(bitlen/8);
				Array.Reverse(RSAmodulus);
				//-- if this is a valid unencrypted publicKEYBLOB, read RSA public key properties
				if(btype == PRIVATEKEYBLOB)
				{
					int bitlen16 = bitlen/16;
					P = r.ReadBytes(bitlen16);
					Array.Reverse(P);
					if(P.Length  != bitlen16)
						return false;
					Q = r.ReadBytes(bitlen16);
					Array.Reverse(Q);
					if(Q.Length  != bitlen16)
						return false;
					DP = r.ReadBytes(bitlen16);
					Array.Reverse(DP);
					if(DP.Length  != bitlen16)
						return false;
					DQ = r.ReadBytes(bitlen16);
					Array.Reverse(DQ);
					if(DQ.Length  != bitlen16)
						return false;
					IQ = r.ReadBytes(bitlen16);
					Array.Reverse(IQ);
					if(IQ.Length  != bitlen16)
						return false;
					D = r.ReadBytes(bitlen/8);
					Array.Reverse(D);
					if(D.Length  != bitlen/8)
						return false;
				}

				try 
				{   //check if EOS
					r.ReadByte();
					extradata=true; //we unexpectedly have more data!! 
				}
				catch(EndOfStreamException) {;} //Normal exception for EOS

				return true;
			}
			catch(Exception exc)  //for whatever reason, not a valid PUBLICKEYBLOB structure
			{
				Console.WriteLine(exc.Message) ;
				return false;
			}
		}


		public static void DisplayBytes(byte[] data) 
		{
			//	for(int i=1; i<=data.Length; i++){	
			//	 Console.Write("{0:x2}  ", data[i-1]) ;
			//	  if(i%16 == 0)
			//		Console.WriteLine("");
			//	}

			for(int i=1; i<=data.Length; i++)
			{	
				Console.Write("{0}, ", data[i-1]) ;
				if(i%16 == 0)
					Console.WriteLine("");
			}

			Console.WriteLine();
		}


		public static void usage() 
		{
			Console.WriteLine("\nUsage:\nRSAKeyblob.exe [publickeyblob | publickeyblob | .NET publickey file]");
		}

		public static RSAParameters ParseRSAParam(byte[] PubKeyData)
		{
			MemoryStream stream = new MemoryStream(PubKeyData);
			BinaryReader r = new BinaryReader(stream);
			return ParseRSAParam(r);
		}

		public static RSAParameters ParseRSAParam(string keyFilePath)
		{
			String FILE_NAME = keyFilePath ;
			if (!File.Exists(FILE_NAME))
			{
				Console.WriteLine("File '{0}' not found.", FILE_NAME);
				throw new IOException("密钥文件未找到");
			
			}

		
			// Create the reader for data.
			FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
			BinaryReader r = new BinaryReader(fs);
			// Read data from Test.data.
			return ParseRSAParam(r);
			
		}

		public static RSAParameters ParseRSAParam(BinaryReader r)
		{
			RSAParamParser keyblob = new RSAParamParser();
			RSAParameters param=new RSAParameters();
			if(keyblob.isValidPublickeyblob(r))
			{
				

				if(keyblob.btype == RSAParamParser.PUBLICKEYBLOB)
					if(keyblob.dotnetkey)
						Console.WriteLine("\nValid .NET publickey");
					else
						Console.WriteLine("\nValid PUBLICKEYBLOB");
				else
					Console.WriteLine("\nValid PRIVATEKEYBLOB");


				if(keyblob.algid == RSAParamParser.CALG_RSA_KEYX)
					Console.WriteLine("Key type:  RSA KeyExchange");
				else
					Console.WriteLine("Key type:  RSA Signature");

				Console.WriteLine("bitlength: {0}\n", keyblob.bitlen);
				Console.WriteLine("RSA exponent(big-endian byte order):");

				RSAParamParser.DisplayBytes(keyblob.Exponent) ;
				param.Exponent=keyblob.Exponent;
				
				Console.WriteLine("RSA modulus (big-endian byte order):");
				RSAParamParser.DisplayBytes(keyblob.RSAmodulus) ;
				param.Modulus = keyblob.RSAmodulus;

				if(keyblob.btype == RSAParamParser.PRIVATEKEYBLOB)
				{
					Console.WriteLine("P:");
					RSAParamParser.DisplayBytes(keyblob.P) ; param.P = keyblob.P;
					Console.WriteLine("Q:");
					RSAParamParser.DisplayBytes(keyblob.Q) ;param.Q = keyblob.Q;
					Console.WriteLine("DP:");
					RSAParamParser.DisplayBytes(keyblob.DP) ;param.DP = keyblob.DP;
					Console.WriteLine("DQ:");
					RSAParamParser.DisplayBytes(keyblob.DQ) ;param.DQ = keyblob.DQ;
					Console.WriteLine("IQ:");
					RSAParamParser.DisplayBytes(keyblob.IQ) ;param.InverseQ = keyblob.IQ;
					Console.WriteLine("D:");
					RSAParamParser.DisplayBytes(keyblob.D) ;param.D = keyblob.D;
				}
			}
			else 
			{
				// isValidPublickeyblob returns false
				Console.WriteLine("NOT a valid CryptoAPI keyblob file") ;
				throw new ApplicationException("不是有效的 CryptoAPI keyblob 文件");
			}

			if(keyblob.extradata)
				Console.WriteLine("There is unexpected extra data at end of blob") ;
		
			r.Close();

			return param;
		}

		
	}
}
