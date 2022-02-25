using System;
using System.Collections.Generic;

namespace Utils
{
    public class ObservableValue<T>
    {
        public event Action<T> OnValueChanged;
        public T Value
        {
            get => value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(this.value, value)) return;

                OnValueChanged?.Invoke(value);
                this.value = value;
            }
        }

        private T value;
    }
}
