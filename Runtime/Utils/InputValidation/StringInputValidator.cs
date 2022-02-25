namespace Utils.InputValidation
{
    using System;

    public class StringInputValidator : InputValidator<string>
    {
        #region Public Methods
        public StringInputValidator()
        {
            validatorPredicate = CompareStrings;
        }
        #endregion
        #region Private Methods
        private bool CompareStrings(string comparedValue)
        {
            return expectedValue.Equals(comparedValue, StringComparison.CurrentCultureIgnoreCase);
        }
        #endregion
    }
}