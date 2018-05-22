using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HammingCoder
{
    public class Bits
    {
        public string value { get; }

        public Bits(string bits,int length)
        {
            var builder = new StringBuilder(bits);
            while (builder.ToString().First() == '0' && builder.Length != length)
            {
                builder.Remove(0, 1);
                if(builder.Length == length) break;
            }
            this.value = builder.ToString();
        }

        public static Bits operator >>(Bits b, int offset)
        {
            var builder = new StringBuilder();
            builder.Append('0', b.value.Length);
            var tmp = b.value[b.value.Length - 1];
            for (int i = 0; i < b.value.Length - offset; i++)
            {
                builder[i + offset] = b.value[i];
            }

            builder[0] = tmp;
            return new Bits(builder.ToString(),b.value.Length);
        }

        public static Bits operator <<(Bits b, int offset)
        {
            var builder = new StringBuilder();
            builder.Append('0', b.value.Length);
            var tmp = b.value[0];
            for (int i = 0; i < b.value.Length - offset; i++)
            {
                builder[i] = b.value[i + offset];
            }
            builder[b.value.Length - 1] = tmp;
            return new Bits(builder.ToString(),b.value.Length);
        }
    }
}
