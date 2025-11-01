using UnityEngine.Events;

namespace Core.Shared
{
    /// <summary>
    /// Field, that have event on value changes. Can be any type.
    /// </summary>
    /// <typeparam name="T">Type of the field.</typeparam>
    public class Observable<T>
    {
        private T _value;


        /// <summary>
        /// Event invokes when value of observable has changed.
        /// </summary>
        public event UnityAction OnValueChanged;


        /// <summary>
        /// Current value of observable.
        /// </summary>
        public T Value
        {
            get { return _value; }

            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged?.Invoke();
                }
            }
        }


        /// <param name="value">Start value.</param>
        public Observable(T value = default)
        {
            _value = value;
        }
    }
}
