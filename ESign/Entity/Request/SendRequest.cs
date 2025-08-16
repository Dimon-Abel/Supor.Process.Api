using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESign.Entity.Request
{
    public class SendRequest
    {
        public string title { get; set; } = "签署文件";
        public string guid { get; set; }
        //public List<string> DocNames { get; set; }

        //public List<string> Attachments { get; set; }

        //public SendRequest() { 
        
        //    DocNames = new List<string>();
        //    Attachments = new List<string>();
        //}
    }
}
