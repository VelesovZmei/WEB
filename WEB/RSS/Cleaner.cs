using System.Text.RegularExpressions;

namespace WEB.RSS
{
    public static class Cleaner
    {
        public static string CleanHtml(this string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText)) { return null; }

            var txt = Regex.Replace(htmlText, @"<img.+?>", "", RegexOptions.Singleline);  //Remove images
            txt = Regex.Replace(txt, @"<!--SWF.+?>", "", RegexOptions.Singleline);  //Remove video
            txt = Regex.Replace(txt, @"<figure.+?figure>", "", RegexOptions.Singleline);  //Remove figures
            txt = Regex.Replace(txt, @"<script.+?</script>", "", RegexOptions.Singleline);  //Remove scripts
            txt = Regex.Replace(txt, @"<blockquote.+?>", "", RegexOptions.Singleline);
            txt = Regex.Replace(txt, @"<blockquote.+?blockquote>", "", RegexOptions.Singleline);  //Remove blockquotes                                                                                      //Remove blockquotes
            txt = Regex.Replace(txt, @"<a.+?>.+?</a>", "", RegexOptions.Singleline);  //Remove links
            txt = Regex.Replace(txt, @"<p>?</p>", "", RegexOptions.Singleline);  //Remove empty blocks
            txt = Regex.Replace(txt, @"\n{2,}", "", RegexOptions.Singleline);  //Remove empty strings

            return txt;
        }

        public static string GetPlainText(this string htmlText)
        {
            return Regex.Replace(htmlText, "@<.+?>", "", RegexOptions.Singleline);  //Remove all tags
        }
    }
}
