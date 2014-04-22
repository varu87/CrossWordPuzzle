
namespace RandomArrayProject
{
    public class Words
    {
        public string Word { get; set; }

        public string Meaning { get; set; }

        public int Direction { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public string Options { get; set; }

        public Words(string w, string m)
        {
            Word = w;
            Meaning = m;
            Direction = 0;
            StartIndex = -1;
            EndIndex = -1;
            Options = string.Empty;
        }

        public Words(string w, string m, int d, int s, int e, string o)
        {
            Word = w;
            Meaning = m;
            Direction = d;
            StartIndex = s;
            EndIndex = e;
            Options = o;
        }
    }
}
