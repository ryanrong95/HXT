using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.App_Utils
{
    /// <summary>
    /// 用户输入的过滤
    /// </summary>
    public static class InputTextExtends
    {
        /// <summary>
        /// Method to make sure that user's inputs are not malicious
        /// </summary>
        /// <param name="text">User's Input</param>
        /// <returns>The cleaned up version of the input</returns>
        public static string InputText(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            text = text.Trim();
            text = Regex.Replace(text, "[\\s]{2,}", " ");	//two or more spaces
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	//<br>
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);	//any other tags
            text = text.Replace("'", "''");
            return text;
        }

        /// <summary>
        /// Method to make sure that user's inputs are not malicious
        /// </summary>
        /// <param name="text">User's Input</param>
        /// <param name="maxLength">Maximum length of input</param>
        /// <returns>The cleaned up version of the input</returns>
        public static string InputText(this string text, int maxLength)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            if (text.Length > maxLength)
            {
                text = text.Substring(0, maxLength);
            }
            text = Regex.Replace(text, "[\\s]{2,}", " ");	//two or more spaces
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	//<br>
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);	//any other tags
            text = text.Replace("'", "''");
            return text;
        }

        /// <summary>
        /// Method to check whether input has other characters than numbers
        /// </summary>
        public static string CleanNonWord(this string text)
        {
            return Regex.Replace(text, "\\W", "");
        }
    }
}