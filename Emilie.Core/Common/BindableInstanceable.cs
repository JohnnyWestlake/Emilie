namespace Emilie.Core
{
    public class BindableDefaultable<T> : BindableBase where T : new()
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

    public class BindableInstanceable<T> : BindableBase where T : new()
    {
        private static T _defaultInstance;
        public static T Instance
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
