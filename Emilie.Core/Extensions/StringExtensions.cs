using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Emilie.Core.Extensions
{
    public static class StringExtensions
    {
        //------------------------------------------------------
        //
        //  Byte Conversions
        //
        //------------------------------------------------------

        #region Byte Conversions

        public static Stream AsStream(this string data) => Encoding.UTF8.GetBytes(data).AsStream();
        /// <summary>
        /// Returns a stream that represents the same data as the string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Task<MemoryStream> AsStreamAsync(this string data, Encoding encoding = null)
        {
            return Task.Run(() =>
            {
                var enc = (encoding != null) ? encoding : Encoding.UTF8;
                return enc.GetBytes(data).AsStream();
            });
        }

        /// <summary>
        /// Converts a String to a Byte Array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Task<byte[]> AsBytesAsync(this string data, Encoding encoding = null)
        {
            return Task.Run(() =>
            {
                var enc = encoding ?? Encoding.UTF8;
                return enc.GetBytes(data);
            });
        }


        #endregion





        //------------------------------------------------------
        //
        //  Decoding & Verification
        //
        //------------------------------------------------------

        #region Decoding & Verification

        public static string StripHtml(this string text)
        {
            string newText = WebUtility.HtmlDecode(text);
            newText = WebUtility.HtmlDecode(text);
            newText = DecodeHtmlChars(newText);
            newText = Regex.Replace(newText, @"<(.|\n)*?>", string.Empty);
            return newText;
        }

        public static string DecodeHtmlChars(string source)
        {
            string[] parts = source.Split(new string[] { "&#" }, StringSplitOptions.None);
            for (int i = 1; i < parts.Length; i++)
            {
                int n = parts[i].IndexOf(';');
                string number = parts[i].Substring(0, n);
                try
                {
                    int unicode = Convert.ToInt32(number, 16);
                    parts[i] = ((char)unicode) + parts[i].Substring(n + 1);
                }
                catch
                { }
            }
            return String.Join("", parts);
        }


        public static bool IsValidUnicodeString(this string str)
        {
            int i = 0;
            while (i < str.Length)
            {
                char c = str[i++];

                // (high surrogate, low surrogate) makes a valid pair, anything else is invalid:
                if (char.IsHighSurrogate(c))
                {
                    if (i < str.Length && char.IsLowSurrogate(str[i]))
                    {
                        i++;
                    }
                    else
                    {
                        // high surrogate not followed by low surrogate
                        return false;
                    }
                }
                else if (char.IsLowSurrogate(c))
                {
                    // previous character wasn't a high surrogate
                    return false;
                }
            }

            return true;
        }

        #endregion





        //------------------------------------------------------
        //
        //  String Casing
        //
        //------------------------------------------------------

        #region String Casing

        private static readonly Func<char, char> s_toLower = char.ToLower;
        private static readonly Func<char, char> s_toUpper = char.ToUpper;

        public static string ToPascalCase(this string shortName, bool trimLeadingTypePrefix = true)
        {
            return ConvertCase(shortName, trimLeadingTypePrefix, s_toUpper);
        }

        public static string ToCamelCase(this string shortName, bool trimLeadingTypePrefix = true)
        {
            return ConvertCase(shortName, trimLeadingTypePrefix, s_toLower);
        }





        private static string ConvertCase(
            this string shortName,
            bool trimLeadingTypePrefix,
            Func<char, char> convert)
        {
            // Special case the common .net pattern of "IFoo" as a type name.  In this case we
            // want to generate "foo" as the parameter name.  
            if (!string.IsNullOrEmpty(shortName))
            {
                if (trimLeadingTypePrefix && (shortName.LooksLikeInterfaceName() || shortName.LooksLikeTypeParameterName()))
                {
                    return convert(shortName[1]) + shortName.Substring(2);
                }

                int indexOfChar = 0;

                for (int i=0;i<shortName.Length;i++)
                {
                    if (Char.IsLetter(shortName[i]))
                    {
                        indexOfChar = i;
                        break;
                    }
                }

                if (convert(shortName[indexOfChar]) != shortName[indexOfChar])
                {
                    if (indexOfChar > 0)
                        return shortName.Substring(0, indexOfChar) +  convert(shortName[indexOfChar]) + shortName.Substring(indexOfChar + 1);
                    else
                        return convert(shortName[indexOfChar]) + shortName.Substring(indexOfChar + 1);
                }
            }

            return shortName;
        }

        private static bool LooksLikeInterfaceName(this string name) => name.Length >= 3 && name[0] == 'I' && char.IsUpper(name[1]) && char.IsLower(name[2]);

        private static bool LooksLikeTypeParameterName(this string name) => name.Length >= 3 && name[0] == 'T' && char.IsUpper(name[1]) && char.IsLower(name[2]);

        #endregion





        //------------------------------------------------------
        //
        //  Comparisons
        //
        //------------------------------------------------------

        #region Comparisons

        /// <summary>
        /// Returns the first instance of a Not-null-or-empty string from an list of inputs.
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string FirstValid(params string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                if (!String.IsNullOrEmpty(strings[i]))
                    return strings[i];
            }

            return null;
        }

        /// <summary>
        /// Checks if the given string matches any of the options, using StringComparison.OrdinalIgnoreCase
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool MatchesAny(this string source, params String[] options)
        {
            if (null == options)
                throw new ArgumentNullException(nameof(options));

            return options.Any(o => o.Equals(source, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the given string matches any of the options
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comparison"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool MatchesAny(this string source, StringComparison comparison, params String[] options)
        {
            if (null == options)
                throw new ArgumentNullException(nameof(options));

            return options.Any(o => o.Equals(source, comparison));
        }

        #endregion

        public static string TrimStart(this string name, string pattern)
        {
            if (name.StartsWith(pattern))
                return name.Substring(pattern.Length).TrimStart(pattern);

            return name;
        }
        public static string TrimEnd(this string name, string pattern)
        {
            if (name.EndsWith(pattern))
                return name.Substring(0, name.Length - pattern.Length).TrimEnd(pattern);

            return name;
        }
        public static string Trim(this string name, string pattern)
        {
            return name.TrimStart(pattern).TrimEnd(pattern);
        }
    }
}
