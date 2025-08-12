using System;

namespace ESign.Entity
{
    public class HttpRespResult
    {
        /* 网络请求是否成功 */
        public bool IsNetworkSuccess { get; set; }
        /* 网络请求信息 */
        public string NetworkMsg { get; set; }
        /* 状态码 */
        public int HttpStatusCode { get; set; }
        /* 提示信息 */
        public string HttpStatusCodeMsg { get; set; }
        /* 返回数据 */
        public Object RespData { get; set; }
    }
}
