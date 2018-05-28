using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HammingCoder
{
    public static class Config
    {
        public static int n = 15;
        public static int k = 11;
        public static int r = n - k; // Число проверочных символов 
        public static int n1 = 14; // Длина кодового слова (блока)
        public static int k1 = 10; // Длина информационного слова
        public static Polynom fx = new Polynom("x^" + n + " + 1");
        public static Polynom gx = new Polynom("x^" + r + " + x + 1");
        public static Polynom hx = fx / gx;
    }
}
