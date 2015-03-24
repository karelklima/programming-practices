using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsLibrary
{
    internal class Option
    {

        private object argument;  

        internal Option()
        {
            // TODO
        }

        public T GetValue<T>()
        {
            return ((Argument<T>) argument).Value;
        }

    }
}
