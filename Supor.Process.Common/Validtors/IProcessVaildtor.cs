namespace Supor.Process.Common.Validtors
{
    public interface IProcessVaildtor
    {
        /// <summary>
        /// 字段数据校验
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        bool FieldValid(object jsonData, out string message);
    }
}
