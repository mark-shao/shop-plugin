using System;
using System.Collections.Generic;
using System.Text;

namespace Com.HisunCmpay
{
    /// <summary>
    /// 
    /// </summary>
    public class HmacMD5
    { 
        // Fields
        private byte[] buffer = new byte[0x40];
        private uint[] count = new uint[2];
        private byte[] digest = new byte[0x10];
        private static byte[] pad;
        private const uint S11 = 7;
        private const uint S12 = 12;
        private const uint S13 = 0x11;
        private const uint S14 = 0x16;
        private const uint S21 = 5;
        private const uint S22 = 9;
        private const uint S23 = 14;
        private const uint S24 = 20;
        private const uint S31 = 4;
        private const uint S32 = 11;
        private const uint S33 = 0x10;
        private const uint S34 = 0x17;
        private const uint S41 = 6;
        private const uint S42 = 10;
        private const uint S43 = 15;
        private const uint S44 = 0x15;
        private uint[] state = new uint[4];

        // Methods
        static HmacMD5()
        {
            byte[] b = new byte[0x40];
            b[0] = 0x80;
            pad = b;
        }

        public HmacMD5()
        {
            this.init();
        }

        private void decode(ref uint[] output, byte[] input, uint len)
        {
            uint i;
            uint j;
            if (BitConverter.IsLittleEndian)
            {
                i = 0;
                for (j = 0; j < len; j += 4)
                {
                    output[i] = (uint) (((input[j] | (input[(int) ((IntPtr) (j + 1))] << 8)) | (input[(int) ((IntPtr) (j + 2))] << 0x10)) | (input[(int) ((IntPtr) (j + 3))] << 0x18));
                    i++;
                }
            }
            else
            {
                i = 0;
                for (j = 0; j < len; j += 4)
                {
                    output[i] = (uint) (((input[(int) ((IntPtr) (j + 3))] | (input[(int) ((IntPtr) (j + 2))] << 8)) | (input[(int) ((IntPtr) (j + 1))] << 0x10)) | (input[j] << 0x18));
                    i++;
                }
            }
        }

        private void encode(ref byte[] output, uint[] input, uint len)
        {
            uint i;
            uint j;
            if (BitConverter.IsLittleEndian)
            {
                i = 0;
                for (j = 0; j < len; j += 4)
                {
                    output[j] = (byte) (input[i] & 0xff);
                    output[(int) ((IntPtr) (j + 1))] = (byte) ((input[i] >> 8) & 0xff);
                    output[(int) ((IntPtr) (j + 2))] = (byte) ((input[i] >> 0x10) & 0xff);
                    output[(int) ((IntPtr) (j + 3))] = (byte) ((input[i] >> 0x18) & 0xff);
                    i++;
                }
            }
            else
            {
                i = 0;
                for (j = 0; j < len; j += 4)
                {
                    output[(int) ((IntPtr) (j + 3))] = (byte) (input[i] & 0xff);
                    output[(int) ((IntPtr) (j + 2))] = (byte) ((input[i] >> 8) & 0xff);
                    output[(int) ((IntPtr) (j + 1))] = (byte) ((input[i] >> 0x10) & 0xff);
                    output[j] = (byte) ((input[i] >> 0x18) & 0xff);
                    i++;
                }
            }
        }

        private uint F(uint x, uint y, uint z)
        {
            return ((x & y) | (~x & z));
        }

        private void FF(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
        {
            a += (this.F(b, c, d) + x) + ac;
            a = this.rotate_left(a, s) + b;
        }

        public byte[] finalize()
        {
            byte[] bits = new byte[8];
            this.encode(ref bits, this.count, 8);
            uint index = (this.count[0] >> 3) & 0x3f;
            uint padLen = (index < 0x38) ? (0x38 - index) : (120 - index);
            this.update(pad, padLen);
            this.update(bits, 8);
            this.encode(ref this.digest, this.state, 0x10);
            for (int i = 0; i < 0x40; i++)
            {
                this.buffer[i] = 0;
            }
            return this.digest;
        }

        private uint G(uint x, uint y, uint z)
        {
            return ((x & z) | (y & ~z));
        }

        private void GG(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
        {
            a += (this.G(b, c, d) + x) + ac;
            a = this.rotate_left(a, s) + b;
        }

        private uint H(uint x, uint y, uint z)
        {
            return ((x ^ y) ^ z);
        }

        private void HH(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
        {
            a += (this.H(b, c, d) + x) + ac;
            a = this.rotate_left(a, s) + b;
        }

        private uint I(uint x, uint y, uint z)
        {
            return (y ^ (x | ~z));
        }

        private void II(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
        {
            a += (this.I(b, c, d) + x) + ac;
            a = this.rotate_left(a, s) + b;
        }

        public void init()
        {
            this.count[0] = 0;
            this.count[1] = 0;
            this.state[0] = 0x67452301;
            this.state[1] = 0xefcdab89;
            this.state[2] = 0x98badcfe;
            this.state[3] = 0x10325476;
        }

        public string md5String()
        {
            string s = "";
            for (int i = 0; i < this.digest.Length; i++)
            {
                s = s + this.digest[i].ToString("x2");
            }
            return s;
        }

        private uint rotate_left(uint x, int n)
        {
            return ((x << n) | (x >> (0x20 - n)));
        }

        private void transform(byte[] data)
        {
            uint a = this.state[0];
            uint b = this.state[1];
            uint c = this.state[2];
            uint d = this.state[3];
            uint[] x = new uint[0x10];
            this.decode(ref x, data, 0x40);
            this.FF(ref a, b, c, d, x[0], 7, 0xd76aa478);
            this.FF(ref d, a, b, c, x[1], 12, 0xe8c7b756);
            this.FF(ref c, d, a, b, x[2], 0x11, 0x242070db);
            this.FF(ref b, c, d, a, x[3], 0x16, 0xc1bdceee);
            this.FF(ref a, b, c, d, x[4], 7, 0xf57c0faf);
            this.FF(ref d, a, b, c, x[5], 12, 0x4787c62a);
            this.FF(ref c, d, a, b, x[6], 0x11, 0xa8304613);
            this.FF(ref b, c, d, a, x[7], 0x16, 0xfd469501);
            this.FF(ref a, b, c, d, x[8], 7, 0x698098d8);
            this.FF(ref d, a, b, c, x[9], 12, 0x8b44f7af);
            this.FF(ref c, d, a, b, x[10], 0x11, 0xffff5bb1);
            this.FF(ref b, c, d, a, x[11], 0x16, 0x895cd7be);
            this.FF(ref a, b, c, d, x[12], 7, 0x6b901122);
            this.FF(ref d, a, b, c, x[13], 12, 0xfd987193);
            this.FF(ref c, d, a, b, x[14], 0x11, 0xa679438e);
            this.FF(ref b, c, d, a, x[15], 0x16, 0x49b40821);
            this.GG(ref a, b, c, d, x[1], 5, 0xf61e2562);
            this.GG(ref d, a, b, c, x[6], 9, 0xc040b340);
            this.GG(ref c, d, a, b, x[11], 14, 0x265e5a51);
            this.GG(ref b, c, d, a, x[0], 20, 0xe9b6c7aa);
            this.GG(ref a, b, c, d, x[5], 5, 0xd62f105d);
            this.GG(ref d, a, b, c, x[10], 9, 0x2441453);
            this.GG(ref c, d, a, b, x[15], 14, 0xd8a1e681);
            this.GG(ref b, c, d, a, x[4], 20, 0xe7d3fbc8);
            this.GG(ref a, b, c, d, x[9], 5, 0x21e1cde6);
            this.GG(ref d, a, b, c, x[14], 9, 0xc33707d6);
            this.GG(ref c, d, a, b, x[3], 14, 0xf4d50d87);
            this.GG(ref b, c, d, a, x[8], 20, 0x455a14ed);
            this.GG(ref a, b, c, d, x[13], 5, 0xa9e3e905);
            this.GG(ref d, a, b, c, x[2], 9, 0xfcefa3f8);
            this.GG(ref c, d, a, b, x[7], 14, 0x676f02d9);
            this.GG(ref b, c, d, a, x[12], 20, 0x8d2a4c8a);
            this.HH(ref a, b, c, d, x[5], 4, 0xfffa3942);
            this.HH(ref d, a, b, c, x[8], 11, 0x8771f681);
            this.HH(ref c, d, a, b, x[11], 0x10, 0x6d9d6122);
            this.HH(ref b, c, d, a, x[14], 0x17, 0xfde5380c);
            this.HH(ref a, b, c, d, x[1], 4, 0xa4beea44);
            this.HH(ref d, a, b, c, x[4], 11, 0x4bdecfa9);
            this.HH(ref c, d, a, b, x[7], 0x10, 0xf6bb4b60);
            this.HH(ref b, c, d, a, x[10], 0x17, 0xbebfbc70);
            this.HH(ref a, b, c, d, x[13], 4, 0x289b7ec6);
            this.HH(ref d, a, b, c, x[0], 11, 0xeaa127fa);
            this.HH(ref c, d, a, b, x[3], 0x10, 0xd4ef3085);
            this.HH(ref b, c, d, a, x[6], 0x17, 0x4881d05);
            this.HH(ref a, b, c, d, x[9], 4, 0xd9d4d039);
            this.HH(ref d, a, b, c, x[12], 11, 0xe6db99e5);
            this.HH(ref c, d, a, b, x[15], 0x10, 0x1fa27cf8);
            this.HH(ref b, c, d, a, x[2], 0x17, 0xc4ac5665);
            this.II(ref a, b, c, d, x[0], 6, 0xf4292244);
            this.II(ref d, a, b, c, x[7], 10, 0x432aff97);
            this.II(ref c, d, a, b, x[14], 15, 0xab9423a7);
            this.II(ref b, c, d, a, x[5], 0x15, 0xfc93a039);
            this.II(ref a, b, c, d, x[12], 6, 0x655b59c3);
            this.II(ref d, a, b, c, x[3], 10, 0x8f0ccc92);
            this.II(ref c, d, a, b, x[10], 15, 0xffeff47d);
            this.II(ref b, c, d, a, x[1], 0x15, 0x85845dd1);
            this.II(ref a, b, c, d, x[8], 6, 0x6fa87e4f);
            this.II(ref d, a, b, c, x[15], 10, 0xfe2ce6e0);
            this.II(ref c, d, a, b, x[6], 15, 0xa3014314);
            this.II(ref b, c, d, a, x[13], 0x15, 0x4e0811a1);
            this.II(ref a, b, c, d, x[4], 6, 0xf7537e82);
            this.II(ref d, a, b, c, x[11], 10, 0xbd3af235);
            this.II(ref c, d, a, b, x[2], 15, 0x2ad7d2bb);
            this.II(ref b, c, d, a, x[9], 0x15, 0xeb86d391);
            this.state[0] += a;
            this.state[1] += b;
            this.state[2] += c;
            this.state[3] += d;
            for (int i = 0; i < 0x10; i++)
            {
                x[i] = 0;
            }
        }

        public void update(byte[] data, uint length)
        {
            uint left = length;
            uint offset = (this.count[0] >> 3) & 0x3f;
            uint bit_length = length << 3;
            uint index = 0;
            if (length > 0)
            {
                this.count[0] += bit_length;
                this.count[1] += length >> 0x1d;
                if (this.count[0] < bit_length)
                {
                    this.count[1]++;
                }
                if (offset > 0)
                {
                    uint space = 0x40 - offset;
                    uint copy = ((offset + length) > 0x40) ? (0x40 - offset) : length;
                    Buffer.BlockCopy(data, 0, this.buffer, (int) offset, (int) copy);
                    if ((offset + copy) < 0x40)
                    {
                        return;
                    }
                    this.transform(this.buffer);
                    index += copy;
                    left -= copy;
                }
                while (left >= 0x40)
                {
                    Buffer.BlockCopy(data, (int) index, this.buffer, 0, 0x40);
                    this.transform(this.buffer);
                    index += 0x40;
                    left -= 0x40;
                }
                if (left > 0)
                {
                    Buffer.BlockCopy(data, (int) index, this.buffer, 0, (int) left);
                }
            }
        }
    }
}
