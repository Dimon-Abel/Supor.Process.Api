namespace ESign.Entity.Request
{
    public class FileUploadUrlRequest
    {
        public string contentMd5 {  get; set; }

        public string contentType { get; set; } = "application/octet-stream";

        public bool convertToPDF { get; set; }

        public string fileName { get; set; }

        public int fileSize { get; set; }
    }
}
