using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace ESign.Helper
{
    public static class FileHelper
    {
        public static string GetFileContentMD5(byte[] fileBytes, out int length)
        {
            string contentMD5 = null;
            byte[] md5Bytes = null;
            try
            {
                using (MemoryStream stream = new MemoryStream(fileBytes))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();

                    md5Bytes = md5.ComputeHash(stream);
                    length = md5Bytes.Length;

                    // 再对这个二进制数组进行base64编码
                    contentMD5 = Convert.ToBase64String(md5Bytes).ToString();
                    return contentMD5;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetFileContentMD5(string filePath, out int length)
        {
            string contentMD5 = null;
            byte[] md5Bytes = null;
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();

                if (filePath.Contains("http"))
                {
                    md5Bytes = md5.ComputeHash(GetRemoteFileBinary(filePath));
                    length = md5Bytes.Length;
                }
                else
                {
                    FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    // 先计算出上传内容的MD5，其值是一个128位（128 bit）的二进制数组
                    md5Bytes = md5.ComputeHash(file);
                    length = md5Bytes.Length;
                    file.Close();
                }

                // 再对这个二进制数组进行base64编码
                contentMD5 = Convert.ToBase64String(md5Bytes).ToString();
                return contentMD5;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static byte[] GetRemoteFileBinary(string url)
        {
            WebResponse response = null;
            try
            {
                var request = WebRequest.Create(url);
                response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            finally
            {
                response?.Close();
            }
        }

        public static byte[] ReadFileAsBinary(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                return buffer;
            }
        }
    }
}
