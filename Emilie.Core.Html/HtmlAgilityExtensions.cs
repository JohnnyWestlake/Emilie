using HtmlAgilityPack;
using System;
using System.Linq;

namespace Emilie.Core.Extensions
{
    /// <summary>
    /// Useful extensions when working with HtmlNode's from HtmlAgilityPack.
    /// Use when attempting to parse or extract data from HTML
    /// </summary>
    public static class HtmlAgilityExtensions
    {
        /// <summary>
        /// Returns whether or not a node contains an attribute of the given name
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static Boolean SafeHasAttribute(this HtmlNode node, string attribute, bool allowEmpty = false)
        {
            return node.Attributes != null && node.Attributes.Any() && node.Attributes.Contains(attribute) &&
                ((allowEmpty) || !String.IsNullOrWhiteSpace(node.Attributes[attribute].Value));
        }

        /// <summary>
        /// Returns whether a node has the matching name and contains this attribute
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static Boolean SafeNameHasAttribute(this HtmlNode node, string name, string attribute, bool allowEmpty = false)
        {
            return node.Name.Equals(name) && node.SafeHasAttribute(attribute);
        }

        /// <summary>
        /// Returns whether or not the HtmlNode has an attribute of the given name who's value matches the input
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean SafeHasAttributeMatching(this HtmlNode node, string attribute, string value, bool allowEmpty = false)
        {
            return node.SafeHasAttribute(attribute) && node.Attributes[attribute].Value.Equals(value);
        }

        /// <summary>
        /// Returns whether or not the HtmlNode has an attribute of the given name who's value matches the input
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean SafeNameHasAttributeMatching(this HtmlNode node, string name, string attribute, string value, bool allowEmpty = false)
        {
            return node.Name.Equals(name) && node.SafeHasAttributeMatching(attribute, value);
        }

        /// <summary>
        /// Returns whether or not the HtmlNode has an attribute of the given name who's contains the input
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean SafeHasAttributeContaining(this HtmlNode node, string attribute, string value, bool allowEmpty = false)
        {
            return node.SafeHasAttribute(attribute) && node.Attributes[attribute].Value.Contains(value);
        }

        /// <summary>
        /// Returns whether or not the HtmlNode has an attribute of the given name who's value matches the input
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean SafeNameHasAttributeContaining(this HtmlNode node, string name, string attribute, string value, bool allowEmpty = false)
        {
            return node.Name.Equals(name) && node.SafeHasAttributeContaining(attribute, value);
        }
    }
}
