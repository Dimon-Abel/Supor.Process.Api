namespace Supor.Process.Api
{
    /// <summary>
    /// http result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// Status
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public T Data { get; set; }

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
        public ApiResult(Status status, T data, string message)
        {
            Status = status;
            Data = data;
            Message = message;
        }

        public static ApiResult<T> Success(T data = default, string message = null)
        {
            return new ApiResult<T>(Status.Success, data, message);
        }

        public static ApiResult<T> Faild(T data = default, string message = null)
        {
            return new ApiResult<T>(Status.Faild, data, message);
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