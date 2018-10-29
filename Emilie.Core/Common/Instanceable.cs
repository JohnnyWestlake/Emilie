namespace Emilie.Core
{
    /// <summary>
    /// Allows classes to have static singleton instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindableInstanceable<T> where T : new()
    {
        private static T _instance = new T();

        /// <summary>
        /// Returns a static, singleton instance of this class
        /// </summary>
        public static T Instance => _instance;
    }
}
