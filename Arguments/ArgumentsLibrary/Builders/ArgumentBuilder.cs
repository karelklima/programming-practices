using System;

namespace ArgumentsLibrary.Builders
{
    public class ArgumentBuilder<T>
    {

        internal Argument<T> Argument { get; private set; }

        internal ArgumentBuilder()
        {
            Argument = new Argument<T>();
        }

        #region API

        public ArgumentBuilder<T> SetOptional(bool flag)
        {
            // TODO implement
            return this;
        }

        public ArgumentBuilder<T> WithDefaultValue(T value)
        {
            // TODO implement
            return this;
        }

        public ArgumentBuilder<T> WithPredicate(Func<T, bool> predicateFunc)
        {
            // TODO implement
            return this;
        }

        public ArgumentBuilder<T> WithEnumeratedValue(params T[] valuesList)
        {
            // TODO implement
            return this;
        }

        public ArgumentBuilder<T> WithAction(Action<T> action)
        {
            // TODO implement
            return this;
        }

        #endregion
    }
}