﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Specialized;

namespace NotesFor.HtmlToOpenXml
{
	/// <summary>
	/// Represents the collection of attributes present in the current html tag.
	/// </summary>
	sealed class HtmlAttributeCollection
	{
		// This regex split the attributes. This line is valid and all the attributes are well discovered:
		// <table border="1" contenteditable style="text-align: center; color: #ff00e6" cellpadding=0 cellspacing='0' align="center">
		private static Regex stripAttributesRegex = new Regex(@"
#tag and its value surrounded by "" or '
((?<tag>\w+)=(?<sep>""|')\s*(?<val>\#?.*?)(\k<sep>|>))
|
# tag whereas the value is not delimited: cellspacing=0
(?<tag>\w+)=(?<val>\w+)
|
# single tag (with no value): contenteditable
\b(?<tag>\w+)\b", RegexOptions.Compiled| RegexOptions.IgnorePatternWhitespace);

		private static Regex stripStyleAttributesRegex = new Regex(@"(?<name>.+?):\s*(?<val>[^;]+);*\s*", RegexOptions.Compiled);

		private StringDictionary attributes;



		internal HtmlAttributeCollection(String htmlTag, bool targetStyleAttribute)
		{
			this.attributes = new StringDictionary();

			if (!String.IsNullOrEmpty(htmlTag))
			{
				if (targetStyleAttribute) ParseStyle(htmlTag, attributes);
				else Parse(htmlTag, attributes);
			}
		}

		private static void Parse(String htmlTag, StringDictionary attributes)
		{
			// We remove the name of the tag (due to our regex) and ensure there are at least one parameter
			int startIndex;
			for (startIndex = 0; startIndex < htmlTag.Length; startIndex++)
			{
				if (Char.IsWhiteSpace(htmlTag[startIndex]))
				{
					startIndex++;
					break;
				}
				else if (htmlTag[startIndex] == '>' || htmlTag[startIndex] == '/')
				{
					// no attribute in this tag
					return;
				}
			}

			MatchCollection matches = stripAttributesRegex.Matches(htmlTag, startIndex);
			foreach(Match m in matches)
			{
				attributes[m.Groups["tag"].Value] = m.Groups["val"].Value;
			}
		}

		private static void ParseStyle(String htmlTag, StringDictionary attributes)
		{
			MatchCollection matches = stripStyleAttributesRegex.Matches(htmlTag);
			foreach (Match m in matches)
			{
				attributes[m.Groups["name"].Value] = m.Groups["val"].Value;
			}
		}

		/// <summary>
		/// Gets the number of attributes for this tag.
		/// </summary>
		public int Count
		{
			get { return attributes.Count; }
		}

		/// <summary>
		/// Gets the named attribute.
		/// </summary>
		public String this[String name]
		{
			get{ return attributes[name]; }
		}

		/// <summary>
		/// Gets an attribute representing an integer.
		/// </summary>
		public Int32? GetAsInt(String name)
		{
			string attrValue = this[name];
			int val;
			if (attrValue != null && Int32.TryParse(attrValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
				return val;

			return null;
		}

		/// <summary>
		/// Gets an attribute representing a color (named color, hexadecimal or hexadecimal 
		/// without the preceding # character).
		/// </summary>
		public System.Drawing.Color GetAsColor(String name)
		{
			string attrValue = this[name];
			if(attrValue != null)
			{
				return ConverterUtility.ConvertToForeColor(attrValue);
			}
			return System.Drawing.Color.Empty;
		}

		/// <summary>
		/// Gets an attribute representing an unit: 120px, 10pt, 5em, 20%, ...
		/// </summary>
		/// <returns>If the attribute is misformed, the <see cref="Unit.IsValid"/> property is set to false.</returns>
		public Unit GetAsUnit(String name)
		{
			string attrValue = this[name];
			return Unit.Parse(attrValue);
		}

        /// <summary>
        /// Gets an attribute representing the 4 unit sides.
        /// </summary>
        /// <returns>If the attribute is misformed, the <see cref="Margin.IsValid"/> property is set to false.</returns>
        public Margin GetAsMargin(String name)
        {
            string attrValue = this[name];
            return Margin.Parse(attrValue);
        }
	}
}
