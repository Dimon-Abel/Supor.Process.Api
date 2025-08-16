using System;
using System.Collections.Generic;

namespace ESign.Entity.Request
{
    public class SendRequest
    {
        public string title { get; set; } = "签署文件";

        public List<FileInformation> Docs { get; set; }

        public List<FileInformation> Attachments { get; set; }
    }

    public class FileInformation
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name {  get; set; }

        /// <summary>
        /// 文件流
        /// </summary>
        public byte[] FileBytes { get; set; }
    }
}
