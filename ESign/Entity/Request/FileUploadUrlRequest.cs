using System.IO;

namespace ESign.Entity.Request
{
    public class FileUploadUrlRequest
    {
        /// <summary>
        /// 文件的Content-MD5值。 先获取文件MD5的128位二进制数组，再对此二进制进行Base64编码。
        /// </summary>
        public string contentMd5 {  get; set; }
        /// <summary>
        /// 目标文件的MIME类型 可填写 application/octet-stream 或 application/pdf
        /// </summary>
        public string contentType { get; set; } = "application/octet-stream";
        /// <summary>
        /// 是否需要转换成PDF文档，默认值 false
        /// </summary>
        public bool convertToPDF { get; set; }
        /// <summary>
        /// 文件名称（必须带上文件扩展名，不然会导致后续发起流程校验过不去。示例：合同名1.pdf 、合同名2.docx）
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 文件大小，单位: byte字节
        /// </summary>
        public int fileSize { get; set; }
    }
}
