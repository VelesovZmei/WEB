using System.Collections.Generic;

namespace WEB.NLP.Texterra
{
    public class Lemma
    {
        public int start { get; set; }
        public int end { get; set; }
        public string value { get; set; }
    }

    public class Annotations
    {
        public List<Lemma> lemma { get; set; }
    }

    public class Respond
    {
        public string text { get; set; }
        public Annotations annotations { get; set; }
    }
}
