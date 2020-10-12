using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WEB.NLP.Texterra
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// https://texterra.ispras.ru/
    /// https://github.com/dkocich/afinn-165-multilingual/tree/master/build
    /// </remarks>
    public static class NlpHelper
    {
        private const string TEXTERRA_API_URL = "http://api.ispras.ru/texterra/v1/nlp?targetType=lemma&apikey=";
        
        public static async Task<string> GetLemmasAsync(
            string apiKey, string json, IHttpClientFactory clientFactory)
        {
            using (var client = clientFactory.CreateClient())
            {
                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                using (var response = await client.PostAsync(TEXTERRA_API_URL + apiKey, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    // or
                    // response.EnsureSuccessStatusCode(); - throw

                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
