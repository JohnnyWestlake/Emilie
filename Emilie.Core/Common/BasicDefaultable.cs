namespace Emilie.Core
{
    /// <summary>
    /// Allows classes to have static singleton instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasicDefaultable<T> where T : new()
    {
        private static T _defaultInstance;
        public static T Default
        {
            get
            {
                if (_defaultInstance == null)
                    _defaultInstance = new T();
                return _defaultInstance;
            }
        }
    }
}
