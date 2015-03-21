using System;
using System.Collections.Generic;

namespace ArgumentsLibrary
{
    public class Argument<T>
    {

        public Argument<T> WithTest(Func<T, bool> test)
        {
            return this;
        }

    }

}