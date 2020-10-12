using System;

namespace WEB.Extensions
{
    public static class HtmlExtension
    {
        public static string TrimText(this string html, int maxLength, int minLength)
        {
            if (maxLength <= 0 || maxLength < minLength)
            {
                throw new ArgumentException();
            }

            if (html == null || html.Length <= maxLength)
            {
                return html;
            }

            var pivot = (maxLength - minLength) / 2 + minLength;
            var i = html.LastIndexOf("</p>", pivot);
            if (i >= minLength)
            {
                return html.Substring(0, i) + "</p>";
            }
            else
            {
                i = html.IndexOf("</p>", pivot);
                if (i >= pivot && i <= maxLength)
                {
                    return html.Substring(0, i) + "</p>";
                }
                else
                {
                    i = maxLength - 1;
                    var ch = html[i];
                    while (Char.IsLetterOrDigit(ch) || ch == '&' || Char.IsSymbol(ch))
                    {
                        i -= 1;
                        ch = html[i];
                    }

                    if (Char.IsWhiteSpace(html[i]))
                    {
                        return html.Substring(0, i) + "...</p>";
                    }
                    else if (html[i] == '.')
                    {
                        return html.Substring(0, i + 1) + "</p>";
                    }

                    return html.Substring(0, i + 1) + "...</p>";
                }
            }
        }
    }
}
