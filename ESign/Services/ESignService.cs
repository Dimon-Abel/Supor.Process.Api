using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESign.Entity.Request;
using ESign.Entity.Result;
using ESign.Options;
using ESign.Services.Interfaces;
using Newtonsoft.Json;

namespace ESign.Services
{
    public class ESignService : IESignService
    {
        private readonly IESignApiProxy _proxy;
        private readonly ESignOption _option;
        private readonly IAttachmentService _attach;
        public ESignService(IESignApiProxy proxy, ESignOption option, IAttachmentService attach)
        {
            _proxy = proxy;
            _option = option;
            _attach = attach;
        }

        public async Task<SignUrl> Send(SendRequest request)
        {

            using (var cts = new CancellationTokenSource())
            {
                var identityInfo = await _proxy.GetOrganizationIdentityInfo(request.OrgId, request.OrgName, request.OrgIDCardNum, request.OrgIDCardType);

                if (identityInfo?.Code != 0)
                    throw new Exception("机构授权验证失败");

                var host = _option.UploadUrl;

                //循环上传需盖章文件
                var conFieldIds = new List<string>();
                foreach (var item in request.Docs)
                {
                    var resp = await _proxy.FileUpload(item);
                    if (resp.Code != 0)
                        throw new Exception(resp.Message);
                    conFieldIds.Add(resp.Data.fileId);
                }

                //循环上传合同附件
                var fieldIDs = new List<string>();
                foreach (var item in request.Attachments)
                {
                    var attresp = await _proxy.FileUpload(item);
                    if (attresp.Code != 0)
                        throw new Exception(attresp.Message);
                    fieldIDs.Add(attresp.Data.fileId);
                }

                return await PollForSignUrlAsync(conFieldIds, fieldIDs, request, cts.Token);
            }
        }

        public async Task Callback(string signFlowId, string fileSavePath)
        {
            var resp = await _proxy.GetDownLoadFile(signFlowId);
            if (resp.Code == 0)
            {
                var files = resp.Data.files;
                files.AddRange(resp.Data.attachments);
                foreach (var file in files)
                {
                    if (!string.IsNullOrWhiteSpace(fileSavePath))
                    {
                        await DownloadFileAsync(file.downloadUrl, fileSavePath);
                    }
                    else
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
            }
            else
            {
                throw new Exception(resp.Message);
            }
        }

        private async Task DownloadFileAsync(string url, string localPath = "")
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    if (localPath.StartsWith("http"))
                    {
                        // 将文件流 推送给第三方
                        var bytes = await response.Content.ReadAsByteArrayAsync();
                        await _proxy.UploadFileAsync(localPath, bytes, null);
                    }
                    else
                    {
                        using (var fileStream = new FileStream(localPath, FileMode.Create))
                        {
                            await response.Content.CopyToAsync(fileStream);
                        }
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

        private async Task<SignUrl> PollForSignUrlAsync(List<string> conFieldIds, List<string> fileIds, SendRequest request, CancellationToken token)
        {
            var statusMap = conFieldIds.Union(fileIds).ToDictionary(id => id, _ => 0);
            int count = 0;
            //循环验证文件状态是否都为2或5
            while (!token.IsCancellationRequested)
            {
                foreach (var fileId in statusMap.Keys.ToList())
                {
                    var status = await _proxy.QueryUploadStatus(fileId);
                    if (status.Data.fileStatus == 2 || status.Data.fileStatus == 5)
                        statusMap[fileId] = status.Data.fileStatus;
                }

                if (statusMap.Values.All(v => v == 2 || v == 5))
                {
                    var fileIdList = new Dictionary<string, QueryKeyWord>();
                    foreach (var item in statusMap.Keys)
                    {
                        var resp = await _proxy.QueryKeyword(item);
                        fileIdList.Add(item, resp.Data);
                    }

                    var created = await _proxy.CreateByFile(conFieldIds, fileIds, request.Title, fileIdList, request.SignInfo);
                    return (await _proxy.GetSignUrl(created.Data.signFlowId, request.PsnId, request.PsnAccount)).Data;
                }

                count++;
                if (count > 10)
                {
                    throw new Exception("文件上传失败");
                }
                await Task.Delay(1000, token);
            }
            throw new OperationCanceledException();
        }
    }
}
