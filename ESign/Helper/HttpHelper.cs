using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using ESign.Entity;

namespace ESign.Helper
{
    public static class HttpHelper
    {
        /**
         * @description 创建文件流上传 请求头
         *
         * @param fileContentMd5
         * @param contentType
         * @return
         * @author 澄泓
         */
        public static Dictionary<string, string> BuildUploadHeader(String fileContentMd5)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Content-MD5", fileContentMd5);
            return header;
        }

        /// <summary>
        /// 构造json+签名鉴权请求头
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="contentMD5"></param>
        /// <param name="accept"></param>
        /// <param name="contentType"></param>
        /// <param name="authMode"></param>
        /// <returns></returns>
        public static Dictionary<string, string> BuildSignAndJsonHeader(string projectId, string contentMD5, string accept, string contentType, string authMode)
        {

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("X-Tsign-Open-App-Id", projectId);
            header.Add("X-Tsign-Open-Ca-Timestamp", Convert.ToString(TimestampHelper.GetTimestamp()));
            header.Add("Content-MD5", contentMD5);
            header.Add("X-Tsign-Open-Auth-Mode", authMode);
            return header;
        }

        /**
         * 签名计算并且构建一个签名鉴权+json数据的esign请求头
         * @param  httpMethod
         *      *         The name of a supported {@linkplain java.nio.charset.Charset
         *      *         charset}
         * @return
         */
        public static Dictionary<string, string> SignAndBuildSignAndJsonHeader(string appid, string secret, string reqData, string reqType, string url)
        {
            string contentMD5 = "";
            if ("GET".Equals(reqType))
            {
                reqData = null;
                contentMD5 = "";
            }
            else
            {
                //对body体做md5摘要
                contentMD5 = CryptoHelper.doContentMD5(reqData);
            }
            string contentType = "application/json; charset=UTF-8";
            string Mode = "Signature";
            string Accept = "*/*";


            //构造一个初步的请求头
            Dictionary<string, string> esignHeaderMap = BuildSignAndJsonHeader(appid, contentMD5, Accept, contentType, Mode);

            //传入生成的bodyMd5,加上其他请求头部信息拼接成字符串
            string stringToSign = CryptoHelper.AppendSignDatastring(reqType, esignHeaderMap["Content-MD5"], Accept, contentType, "", "", url);
            //整体做sha256签名
            string reqSignature = CryptoHelper.DoSignatureBase64(stringToSign, secret);
            //请求头添加签名值
            esignHeaderMap.Add("X-Tsign-Open-Ca-Signature", reqSignature);

            return esignHeaderMap;
        }
    }
}
