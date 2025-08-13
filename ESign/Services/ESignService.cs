using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESign.Entity.Result;
using ESign.Options;
using ESign.Services.Interfaces;

namespace ESign.Services
{
    public class ESignService : IESignService
    {
        private readonly IESignApiProxy _proxy;
        private readonly ESignOption _option;
        public ESignService(IESignApiProxy proxy, ESignOption option)
        {
            _proxy = proxy;
            _option = option;
        }
        public async Task<SignUrl> Send(string fileName, string title = "签署文件")
        {
            using (var cts = new CancellationTokenSource())
            {
                var identityInfo = await _proxy.GetOrganizationIdentityInfo();
                if (identityInfo?.Code != 0)
                    throw new Exception("机构授权验证失败");

                var uploadInfo = await _proxy.FileUpload(fileName);
                if (uploadInfo.Code != 0)
                    throw new Exception(uploadInfo.Message);

                return await PollForSignUrlAsync(uploadInfo.Data.fileId, title, cts.Token);
            }
        }

        public async Task Callback(string signFlowId)
        {
            var resp = await _proxy.GetDownLoadFile(signFlowId);
            if (resp.Code == 0)
            {
                foreach (var file in resp.Data.files)
                {
                    var path = Path.Combine(_option.UploadFile, signFlowId);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var localPath = Path.Combine(path, file.fileName);
                    await DownloadFileAsync(file.downloadUrl, localPath);
                }
            }
            else
            {
                throw new Exception(resp.Message);
            }
        }

        private static async Task DownloadFileAsync(string url, string localPath)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    using (var fileStream = new FileStream(localPath, FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }
            }
        }

        /// <summary>
        /// 轮询上传文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="title"></param>
        /// <param name="identityData"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        private async Task<SignUrl> PollForSignUrlAsync(string fileId, string title, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var status = await _proxy.QueryUploadStatus(fileId);
                if (status.Data.fileStatus == 2 || status.Data.fileStatus == 5)
                {
                    var keywords = await _proxy.QueryKeyword(fileId);
                    var created = await _proxy.CreateByFile(fileId, title, keywords.Data);
                    return (await _proxy.GetSignUrl(created.Data.signFlowId)).Data;
                }
                await Task.Delay(1000, token);
            }
            throw new OperationCanceledException();
        }

    }
}
