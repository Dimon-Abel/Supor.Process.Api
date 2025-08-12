using Newtonsoft.Json;

namespace ESign.Entity
{
    public class ESignApiResult<T>
    {
        /// <summary>
        /// 业务码, 0:成功_非0:失败
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 业务信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 业务数据
        /// </summary>
        public T Data { get; set; }

        public ESignApiResult()
        {
        }
    }
}
