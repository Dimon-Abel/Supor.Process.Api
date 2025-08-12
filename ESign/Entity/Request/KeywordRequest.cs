using System.Collections.Generic;

namespace ESign.Entity.Request
{
    public class KeywordRequest
    {
        public string fileId { get; set; }
        public List<string> keywords {  get; set; }
    }
}
