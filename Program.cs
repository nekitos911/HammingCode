using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HammingCoder
{
    class Program
    {
        static void Main(string[] args)
        { 
            Console.WriteLine("Enter " + Config.k1 + " bits: ");
            var bits = "";
            try
            {
                bits = Console.ReadLine(); // Input bits
                if(bits.Length != Config.k1) throw new Exception("Bits count should be " + Config.k1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }

            Console.WriteLine();
            var pol = new List<byte>();
            foreach (var b in bits)
            {
                pol.Add(byte.Parse(b.ToString()));
            }
            PrintPolynomAndBits("f(x)",Config.fx);

            PrintPolynomAndBits("g(x)",Config.gx);

            PrintPolynomAndBits("h(x)",Config.hx);
           
            PrintPolynomAndBits("Incoming polynom u(x)",new Polynom(pol.ToArray()));

            var encoding = Encoding(pol); // Encoding word
            PrintPolynomAndBits("Encoding polynom v(x)",encoding);

            var ex = new Polynom(new Bits("00000000010000",Config.n)); // Error
            PrintPolynomAndBits("Input error e(x)",ex);

            encoding = encoding + ex; // Encoding with error
            PrintPolynomAndBits("Encoding with error v'(x)",encoding);

            ex = FindError(encoding); // Find error in encoding word

            PrintPolynomAndBits("Found Error e'(x)",ex);

            var decoding = Decoding(encoding, ex); // Decoding
                
            PrintPolynomAndBits("decoding u'(x)",decoding);

            Console.ReadKey();
            Console.ReadKey();
        }

        static Polynom Encoding(List<byte> pol)
        {
            var vx = new Polynom(pol.ToArray()) * new Polynom("x^" + (Config.n - Config.k));
            vx = vx + vx % Config.gx;
            return vx;
        }

        static Polynom Decoding(Polynom encoding,Polynom ex)
        {
            
            return new Polynom(new Bits(((encoding + ex) / new Polynom("x^" + (Config.n - Config.k))).bits.value, Config.k1));
        }

        static Polynom FindError(Polynom encoding)
        {
            // Append 0 to Config.n
            encoding = new Polynom(new Bits(new StringBuilder().Append('0',Config.n - Config.n1)
                                            + encoding.bits.value, encoding.Koefs.Length + Config.n - Config.n1)); 
            var sx = encoding % Config.gx;
            PrintPolynomAndBits("Syndrome s(x)", sx);

            var offset = 0; // Encoding offset

            var encodingOffset = encoding;

            // While syndrome has more than 1 "1"
            while ((sx.bits.value.Count(x => x == '1')) > 1)
            {
                encodingOffset = encodingOffset >> 1;
                sx = encodingOffset % Config.gx;
                offset++;
            }

            var ostBits = sx.bits.value; 
            // Syndrome with offset
            for (; offset > 0; offset--)
            {
                ostBits += '0';
            }
            
            return new Polynom(new Bits(ostBits, ostBits.Length));
        }

        static void PrintPolynomAndBits(string polynomName,Polynom polynom)
        {
            Console.WriteLine(polynomName + " = " + polynom.StrPolynom);
            Console.Write(polynomName + " bits: ");
            foreach (var b in polynom.Koefs)
            {
                Console.Write(b);
            }
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}
