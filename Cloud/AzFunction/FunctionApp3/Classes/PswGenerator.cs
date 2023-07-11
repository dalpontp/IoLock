using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp3.Classes
{
    public class PswGenerator
    {
        private Random random;
        public PswGenerator()
        {
            random = new Random();
        }
        public int GenerateRandomNumber()
        {
            return random.Next(10000, 100000);
        }
    }
}