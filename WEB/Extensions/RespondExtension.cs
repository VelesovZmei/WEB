using System.Collections.Generic;

using WEB.NLP.Texterra;

namespace WEB.Extensions
{
    public static class RespondExtension
    {
        public static int RateIt(this Respond respond, Dictionary<string, int> dictionary)
        {
            int result = 0;

            if (respond == null
                || !(respond.annotations?.lemma?.Count > 0)
                || !(dictionary?.Count > 0))
            {
                return result;
            }

            int rate;

            foreach (var item in respond.annotations.lemma)
            {
                if (dictionary.TryGetValue(item.value, out rate))
                {
                    result += rate;
                }
            }

            return result;
        }
    }
}
