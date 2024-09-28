using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Generators
{
    public interface IGenerator<OutputType>
    {
        public OutputType Generate(int seed);
    }
}
