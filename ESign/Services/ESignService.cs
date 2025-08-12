using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESign.Entity.Result;
using ESign.Services.Interfaces;

namespace ESign.Services
{
    public class ESignService : IESignService
    {
        private readonly IESignApiProxy _proxy;
        public ESignService(IESignApiProxy proxy)
        {
            _proxy = proxy;
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

                return await PollForSignUrlAsync(uploadInfo.Data.fileId, title, identityInfo.Data, cts.Token);
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
        private async Task<SignUrl> PollForSignUrlAsync(string fileId, string title, OrgIdentity identityData, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var status = await _proxy.QueryUploadStatus(fileId);
                if (status.Data.fileStatus == 2 || status.Data.fileStatus == 5)
                {
                    var keywords = await _proxy.QueryKeyword(fileId);
                    var created = await _proxy.CreateByFile(fileId, title, identityData, keywords.Data);
                    return (await _proxy.GetSignUrl(created.Data.signFlowId)).Data;
                }
                await Task.Delay(1000, token);
            }
            throw new OperationCanceledException();
        }

    }
}
