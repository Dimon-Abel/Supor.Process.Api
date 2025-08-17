namespace ESign.Options
{
    public class ESignOption
    {
        /// <summary>
        /// AppId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// AppSecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// Api 请求地址
        /// </summary>
        public string ESignUrl { get; set; }

        /// <summary>
        /// 文件上传地址
        /// </summary>
        public string ESignFileServer { get; set; }

        /// <summary>
        /// 认证机构信息
        /// </summary>
        public string ESignOrgName { get; set; }

        public string UploadFile { get; set; }

        public string UploadUrl { get; set; }
        public string Keyword { get; set; }

        public string PsnAccount { get; set; }

        public bool AutoStart { get; set; }
        public bool AutoFinish { get; set; }
        public string NoticeTypes { get; set; }
        public string RedirectUrl { get; set; }

        public string SignRedirectUrl { get; set; }
        public bool NeedLogin { get; set; }
        public int UrlType { get; set; }
        public string ClientType { get; set; }
        public int RedirectDelayTime { get; set; }
    }
}
