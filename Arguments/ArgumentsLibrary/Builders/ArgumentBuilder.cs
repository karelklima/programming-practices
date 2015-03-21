using System;

namespace ArgumentsLibrary.Builders
{
    public class ArgumentBuilder<T>
    {
        internal ArgumentBuilder(Option option, bool required)
        {
            
        }

        public ArgumentBuilder<T> WithPredicate(Func<T, bool> predicateFunc)
        {
            return this;
        }

        public ArgumentBuilder<T> WithEnumeratedValue(T[] valuesList)
        {
            return this;
        }

        public ArgumentBuilder<T> WithDelegate(Action<T> setterAction)
        {
            return this;
        }
    }
}