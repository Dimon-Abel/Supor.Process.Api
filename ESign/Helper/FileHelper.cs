using System;
using System.IO;

namespace ESign.Helper
{
    public static class FileHelper
    {
        public static string GetFileContentMD5(string filePath)
        {
            string contentMD5 = null;
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                // 先计算出上传内容的MD5，其值是一个128位（128 bit）的二进制数组
                byte[] md5Bytes = md5.ComputeHash(file);
                file.Close();
                // 再对这个二进制数组进行base64编码
                contentMD5 = Convert.ToBase64String(md5Bytes).ToString();
                return contentMD5;
            }
            catch (Exception ex)
            {
                throw ex;
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
