namespace ESign.Entity.Result
{
    public class FileUploadStatus
    {
        public string fileId { get; set; }
        public string fileName { get; set; }
        public int? fileSize { get; set; }
        /// <summary>
        /// 文件状态
        ///0 - 文件未上传
        ///1 - 文件上传中
        ///2 - 文件上传已完成 或 文件已转换（HTML）
        ///3 - 文件上传失败
        ///4 - 文件等待转换（PDF）
        ///5 - 文件已转换（PDF）
        ///6 - 加水印中
        ///7 - 加水印完毕
        ///8 - 文件转化中（PDF）
        ///9 - 文件转换失败（PDF）
        ///10 - 文件等待转换（HTML）
        ///11 - 文件转换中（HTML）
        ///12 - 文件转换失败（HTML）
        ///【注】文件添加水印功能仅e签宝SaaS高级版支持，具体功能如何接入请联系对接技术指导
        /// </summary>
        public int fileStatus { get; set; }

        /// <summary>
        /// 文件下载地址（有效期为60分钟，过期后可以重新调用接口获取新的下载地址）
        /// </summary>
        public string fileDownloadUrl {  get; set; }
        /// <summary>
        /// pdf文件总页数
        /// </summary>
        public int? fileTotalPageCount {  get; set; }
        public float? pageWidth {  get; set; }
        public float? pageHeight { get; set; }
    }
}
