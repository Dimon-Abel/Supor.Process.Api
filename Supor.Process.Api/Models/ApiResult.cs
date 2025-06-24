namespace Supor.Process.Api
{
    /// <summary>
    /// http result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult
    {
        /// <summary>
        /// Status
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        #region methods

        /// <summary>
        /// constructure
        /// </summary>
        /// <param name="status"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        public ApiResult(Status status, object data, string message)
        {
            Status = status;
            Data = data;
            Message = message;
        }

        public static ApiResult Success(object data = default, string message = null)
        {
            return new ApiResult(Status.Success, data, message);
        }

        public static ApiResult Faild(object data = default, string message = null)
        {
            return new ApiResult(Status.Faild, data, message);
        }

        #endregion
    }

    /// <summary>
    /// http status
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// faild
        /// </summary>
        Faild = 0,
        /// <summary>
        /// success
        /// </summary>
        Success = 1
    }
}