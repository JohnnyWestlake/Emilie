using System;
using System.Reflection;

namespace Emilie.Core.Extensions
{
    /// <summary>
    /// Extension Methods for enums
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description attribute of an enum.
        /// </summary>
        /// 
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
              (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();

        }
    }

    public class DescriptionAttribute : Attribute
    {
        public string Description { get; set; }

        public DescriptionAttribute(string name)
        {
            Description = name;
        }
    }
}