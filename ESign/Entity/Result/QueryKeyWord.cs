using System.Collections.Generic;

namespace ESign.Entity.Result
{
    public class QueryKeyWord
    {
        public List<KeywordPositions> keywordPositions {  get; set; }
    }

    public class KeywordPositions
    {
        public string keyword { get; set; }
        public bool searchResult { get; set; }
        public List<Positions> positions { get; set; }
    }

    public class Positions
    {
        public int pageNum { get; set; }

        public List<Coordinates> coordinates { get; set; }
    }

    public class Coordinates
    {
        public float positionX { get; set; }
        public float positionY { get; set; }
    }
}
