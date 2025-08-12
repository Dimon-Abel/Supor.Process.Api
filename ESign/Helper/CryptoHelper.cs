using System;
using System.Security.Cryptography;
using System.Text;

namespace ESign.Helper
{
    public static class CryptoHelper
    {
        /// <summary>
        ///  HmacSHA256 加密
        /// </summary>
        /// <param name="secret">projectSecret</param>
        /// <param name="data">请求的JSON参数</param>
        /// <returns></returns>
        public static string GetSignature(string data, string secret)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(data);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte test in hashmessage)
                {
                    sb.Append(test.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        #region 计算请求签名值
        /// <summary>
        /// HmacSHA256 计算请求签名值的Base64
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string DoSignatureBase64(string message, string secret)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
        #endregion

        #region 获取MD5
        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string doContentMD5(string str)
        {
            string ContentMD5 = null;
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                ContentMD5 = Convert.ToBase64String(data).ToString();

            }
            catch (Exception ex)
            {
                throw new Exception("计算ContentMD5值时发生异常：" + ex.Message);
            }
            return ContentMD5;
        }
        #endregion

        // 构建待签名字符串
        public static string AppendSignDatastring(string reqType, string contentMd5, string accept, string contentType, string headers, string date, string url)
        {

            System.Text.StringBuilder stringToSign = new System.Text.StringBuilder();
            stringToSign.Append(reqType).Append("\n").Append(accept).Append("\n").Append(contentMd5).Append("\n")
                    .Append(contentType).Append("\n").Append(date).Append("\n");
            if ("".Equals(headers))
            {
                stringToSign.Append(headers).Append(url);
            }
            else
            {
                stringToSign.Append(headers).Append("\n").Append(url);
            }
            return stringToSign.ToString();
        }

    }
}
