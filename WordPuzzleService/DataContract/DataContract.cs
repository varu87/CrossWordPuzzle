using System.Runtime.Serialization;

namespace WordPuzzleService
{
    [DataContract]
    public class CrossWordPuzzle
    {
        [DataMember]
        public string Grid { get; set; }

        [DataMember]
        public string[] Check { get; set; }

        [DataMember]
        public string[] Meanings { get; set; }
    }
}