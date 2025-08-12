namespace ESign.Entity.Request
{
    public class SignUrlRequest
    {
        public string signFlowId {  get; set; }
        public bool needLogin { get; set; } = false;
        public int urlType { get; set; } = 2;
        public Operator Operator { get; set; }
        public Organization organization { get; set; }
        public SignUrlRedirectConfig redirectConfig { get; set; }
        /// <summary>
        /// 指定客户端类型，当urlType为2（签署链接）时生效
        /// H5 - 移动端适配
        /// PC - PC端适配
        /// ALL - 自动适配移动端或PC端（默认值）
        /// </summary>
        public string clientType { get; set; }
        public string appScheme {  get; set; }
    }

    public class Operator
    {
        public string psnAccount {  get; set; }
        public string psnId { get; set; }
    }

    public class Organization
    {
        public string orgId {  get; set; }
        public string orgName { get; set; }
    }

    public class SignUrlRedirectConfig
    {
        public string redirectUrl {  get; set; }
        public int redirectDelayTime { get; set; } = 3;
    }
}
