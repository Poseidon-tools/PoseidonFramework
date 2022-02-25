namespace Utils.Factory
{
    using System;

    public static class ObjectFactory
    {
        #region Public Methods
        public static T Create<T>(Action<T> objectSetupMethod) where T : new()
        {
            T result = new T();
            objectSetupMethod.Invoke(result);
            return result;
        }
        #endregion
    }
}