using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HammingCoder
{
    public class Polynom
    {
        public byte[] Koefs { get; private set; }

        public Bits bits { get; private set; }

        private string strPolynom;

        public string StrPolynom => strPolynom.Length == 0 ? "0" : strPolynom;

        public override string ToString()
        {
            return strPolynom;
        }

        public Polynom TrimZero(int length)
        {
            return new Polynom(new Bits(this.bits.value,length));
        }

        public Polynom(byte[] koefs)
        {
            Koefs = koefs;
            bits = new Bits(string.Join("", Koefs.Select(n => n.ToString())),Koefs.Length);
            BuildPolynom();
        }

        public Polynom(Bits bits)
        {
            this.bits = bits;
            var koefs = new List<byte>();
            foreach (var b in bits.value)
            {
                koefs.Add(byte.Parse(b.ToString()));
            }

            Koefs = koefs.ToArray();
            BuildPolynom();
        }

        public Polynom(string strPolynom)
        {
            this.strPolynom = strPolynom.Trim();
            if (this.strPolynom.Length == 0)
            {
                Koefs = new byte[] {0};
            }
            else
            {
                BuildKoefs();
            }
            bits = new Bits(string.Join("", Koefs.Select(n => n.ToString())),Koefs.Length);

        }

        private void BuildPolynom()
        {
            var builder = new StringBuilder("");
            for (int i = 0; i < Koefs.Length; i++)
            {
                if ((int)Koefs[i] == 0) continue;

                if (i == Koefs.Length - 1)
                    builder.Append("1");
                else if (i == Koefs.Length - 2)
                    builder.Append("x + ");
                else
                {
                    builder.Append("x^" + (Koefs.Length - i - 1) + " + ");
                }

            }

            if (builder.Length > 3)
            {
                if (builder.ToString()[builder.Length - 2] == '+')
                {
                    builder.Remove(builder.Length - 3, 3);
                }
            }

            this.strPolynom = builder.ToString();
        }

        private void BuildKoefs()
        {
            var hasOne = this.strPolynom.Last() == '1';
            var matches = Regex.Matches(this.strPolynom, @"x\^?(?<power>\d+(\.\d+)?)?");
            var powers = new List<int>();
            foreach (Match part in matches)
            {
                if (part.Value != "")
                {
                    if (!int.TryParse(part.Groups["power"].Value, out var power))
                        power = part.Value.Contains("x") ? 1 : 0;
                    powers.Add(power);
                }
            }

            var koefs = new List<byte>();
            if (powers.Count != 0)
            {
                powers.Sort();
                powers.Reverse();
                var j = 0;
                for (int i = 0; i < powers[0]; i++)
                {
                    try
                    {
                        if (powers[j] + i == powers[0])
                        {
                            koefs.Add(1);
                            j++;
                        }
                        else
                        {
                            koefs.Add(0);
                        }
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        koefs.Add(0);
                    }
                }
            }

            koefs.Add(hasOne ? (byte)1 : (byte)0);

            this.Koefs = koefs.ToArray();

        }

        public static Polynom operator -(Polynom p1, Polynom p2)
        {
            var up = p1.Koefs.Length >= p2.Koefs.Length ? p1 : p2;
            var down = up == p1 ? p2 : p1;

            var res = new List<byte>();

            var c = up.Koefs.Length - down.Koefs.Length;

            for (int i = 0; i < c; i++)
            {
                res.Add(up.Koefs[i]);
            }

            for (int i = 0,j = c; i < down.Koefs.Length; i++,j++)
            {
                res.Add((byte)(Math.Abs(up.Koefs[j] - down.Koefs[i]) % 2));
            }
            return new Polynom(res.ToArray());
        }

        public static Polynom operator +(Polynom p1, Polynom p2)
        {
            var up = p1.Koefs.Length >= p2.Koefs.Length ? p1 : p2;
            var down = up == p1 ? p2 : p1;

            var res = new List<byte>();

            var c = up.Koefs.Length - down.Koefs.Length;

            for (int i = 0; i < c; i++)
            {
                res.Add(up.Koefs[i]);
            }

            for (int i = 0, j = c; i < down.Koefs.Length; i++, j++)
            {
                res.Add((byte)(Math.Abs(up.Koefs[j] + down.Koefs[i]) % 2));
            }
            return new Polynom(res.ToArray());
        }

        public static Polynom operator >>(Polynom p, int offset)
        {
            var a = p.bits >> offset;
            var bitsArr = new List<byte>();
            foreach (var c in a.value)
            {
                bitsArr.Add((byte.Parse(c.ToString())));
            }
            return new Polynom(bitsArr.ToArray());
        }

        public static Polynom operator <<(Polynom p, int offset)
        {
            var a = p.bits << offset;
            var bitsArr = new List<byte>();
            foreach (var c in a.value)
            {
                bitsArr.Add((byte.Parse(c.ToString())));
            }
            return new Polynom(bitsArr.ToArray());
        }

        public static Polynom operator /(Polynom p1, Polynom p2)
        {
            if(p1.Koefs.Length < p2.Koefs.Length)
                return new Polynom(new byte[]{0});
            var remainder = (byte[])p1.Koefs.Clone();
            var quotient = new byte[remainder.Length - p2.Koefs.Length + 1];
            for (int i = 0; i < quotient.Length; i++)
            {
                var koeff = remainder[i];
                quotient[i] = koeff;
                for (int j = 0; j < p2.Koefs.Length; j++)
                {
                    remainder[i + j] -= (byte)(koeff * p2.Koefs[j]);
                    remainder[i + j] = (byte)(Math.Abs(remainder[i + j]) % 2);
                }
            }
            return new Polynom(quotient);
        }

        public override bool Equals(object obj)
        {
            var p = (Polynom) obj;
            return StrPolynom == p.StrPolynom;
        }

        public static bool operator ==(Polynom p1, Polynom p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Polynom p1, Polynom p2)
        {
            return !p1.Equals(p2);
        }

        public static Polynom operator %(Polynom p1, Polynom p2)
        {
            if (p1.Koefs.Length < p2.Koefs.Length)
                return new Polynom(p1.Koefs);
            var remainder = (byte[])p1.Koefs.Clone();
            var quotient = new byte[remainder.Length - p2.Koefs.Length + 1];
            for (int i = 0; i < quotient.Length; i++)
            {
                var koeff = remainder[i];
                quotient[i] = koeff;
                for (int j = 0; j < p2.Koefs.Length; j++)
                {
                    remainder[i + j] -= (byte)(koeff * p2.Koefs[j]);
                    remainder[i + j] = (byte)(Math.Abs(remainder[i + j]) % 2);
                }
            }
            return new Polynom(remainder);
        }


        public static bool operator >=(Polynom p1, Polynom p2)
        {
            return p1.Koefs.Length >= p2.Koefs.Length;
        }

        public static bool operator <=(Polynom p1, Polynom p2)
        {
            return p1.Koefs.Length <= p2.Koefs.Length;
        }

        public static Polynom operator *(Polynom polynom1, Polynom polynom2)
        {
            var coeffs = new byte[polynom1.Koefs.Length + polynom2.Koefs.Length - 1];
            for (int i = 0; i < polynom1.Koefs.Length; ++i)
            {
                for (int j = 0; j < polynom2.Koefs.Length; ++j)
                {
                    coeffs[i + j] += (byte)(polynom1.Koefs[i] * polynom2.Koefs[j]);
                    coeffs[i + j] = (byte)(coeffs[i + j] % 2);
                }
            }
            return new Polynom(coeffs);
        }
    }
}
