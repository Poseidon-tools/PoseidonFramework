namespace Utils.InputValidation
{
    using System;

    public class InputValidator<T>
    {
        #region Private Variables
        protected Predicate<T> validatorPredicate;
        protected T expectedValue;
        #endregion
        #region Public Methods
        public void SetExpectedValue(T expectedValue)
        {
            this.expectedValue = expectedValue;
        }
        
        public bool Check(T value)
        {
            return validatorPredicate.Invoke(value);
        }
        #endregion
    }
}