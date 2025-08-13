using System.Collections.Generic;

namespace ESign.Entity.Result
{
    public class FileDownLoad
    {
        public List<DownLoadFile> files { get; set; }
        public List<DownLoadFile> attachments {  get; set; }
        public string certificateDownloadUrl {  get; set; }
    }

    public class DownLoadFile
    {
        public string fileId {  get; set; }
        public string fileName { get; set; }
        public string downloadUrl {  get; set; }
    }
}
