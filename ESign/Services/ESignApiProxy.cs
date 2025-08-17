using ESign.Constants;
using ESign.Entity;
using ESign.Entity.Request;
using ESign.Entity.Result;
using ESign.Helper;
using ESign.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ESign.Services
{
    public class ESignApiProxy : IESignApiProxy
    {
        private readonly ESignOption _option;

        public ESignApiProxy(ESignOption option)
        {
            _option = option;
        }

        /// <summary>
        /// 机构认证授权
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ESignApiResult<OrgIdentity>> GetOrganizationIdentityInfo(string orgId, string orgName, string orgIDCardNum, string orgIDCardType)
        {
            const string endpoint = "/v3/organizations/identity-info";
            var queryParams = new List<string>();

            // 只有非空且非空白的值才添加
            if (!string.IsNullOrWhiteSpace(orgId))
                queryParams.Add($"orgId={orgId}");

            if (!string.IsNullOrWhiteSpace(orgName))
                queryParams.Add($"orgName={orgName}");

            // 注意：orgIDCardNum 和 orgIDCardType 要同时存在才添加
            if (!string.IsNullOrWhiteSpace(orgIDCardNum) &&
                !string.IsNullOrWhiteSpace(orgIDCardType))
            {
                queryParams.Add($"orgIDCardNum={orgIDCardNum}");
                queryParams.Add($"orgIDCardType={orgIDCardType}");
            }

            // 拼接：有参数则以 ? 开头，否则返回空字符串
           var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

            var apiUrl = $"{_option.ESignUrl}{endpoint}{query}";

            try
            {
                var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, null, HttpType.GET, $"{endpoint}{query}");

                var response = await ExecuteHttpRequestAsync(apiUrl, HttpType.GET, null, headers);

                return response.GetResult<ESignApiResult<OrgIdentity>>();
            }
            catch (Exception ex)
            {
                throw new Exception($"机构认证授权失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 查询个人信息认证
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ESignApiResult<PersonIdentity>> GetPersonIdentityInfo(string psnId, string psnAccount, string psnIDCardNum, string psnIDCardType)
        {
            const string endpoint = "/v3/persons/identity-info";

            var queryParams = new List<string>();
            var query = "?";
            if (!string.IsNullOrWhiteSpace(psnId))
                queryParams.Add($"psnId={psnId}");

            if (!string.IsNullOrWhiteSpace(psnAccount))
                queryParams.Add($"psnAccount={psnAccount}");

            if (!string.IsNullOrWhiteSpace(psnIDCardNum) &&
                !string.IsNullOrWhiteSpace(psnIDCardType))
            {
                queryParams.Add($"psnIDCardNum={psnIDCardNum}");
                queryParams.Add($"psnIDCardType={psnIDCardType}");
            }

            if (queryParams.Count > 0)
            {
                query += "&" + string.Join("&", queryParams);
            }

            var apiUrl = $"{_option.ESignUrl}{endpoint}{query}";

            try
            {
                var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, null, HttpType.GET, $"{endpoint}{query}");

                var response = await ExecuteHttpRequestAsync(apiUrl, HttpType.GET, null, headers);

                return response.GetResult<ESignApiResult<PersonIdentity>>();
            }
            catch (Exception ex)
            {
                throw new Exception($"查询个人认证信息: {ex.Message}");
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ESignApiResult<FileUploadUrl>> FileUpload(FileInformation file)
        {
            var extension = Path.GetExtension(file.Name);
            var request = new FileUploadUrlRequest
            {
                contentMd5 = FileHelper.GetFileContentMD5(file.FileBytes, out var length),
                contentType = "application/octet-stream",
                fileName = file.Name,
                convertToPDF = !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase),
                fileSize = length
            };

            try
            {
                // 获取上传URL
                var uploadUrlResult = await GetFileUploadUrlAsync(request);
                if (uploadUrlResult.Code != 0)
                    throw new Exception(uploadUrlResult.Message);


                //var td = FileHelper.GetFileContentMD5("D:\\Code\\doc\\e签宝调用接口清单.xlsx", out var l);
                //var uploadResult = await UploadFileAsync(
                //    uploadUrlResult.Data.fileUploadUrl,
                //    "D:\\Code\\doc\\e签宝调用接口清单.xlsx",
                //    new Dictionary<string, string> { ["Content-MD5"] = request.contentMd5 });

                // 实际上传文件
                var uploadResult = await UploadFileAsync(
                    uploadUrlResult.Data.fileUploadUrl,
                    file.FileBytes,
                    new Dictionary<string, string> { ["Content-MD5"] = request.contentMd5 });

                return uploadResult.HttpStatusCode == 200
                    ? uploadUrlResult
                    : throw new Exception(uploadResult.HttpStatusCodeMsg);
            }
            catch (Exception ex)
            {
                throw new Exception($"文件上传失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 上传文件流程_返回fileId
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ESignApiResult<FileUploadUrl>> FileUpload(string fileName)
        {
            var filePath = fileName.StartsWith("http") ? fileName : Path.Combine(_option.UploadFile, fileName);
            if (!fileName.StartsWith("http") && !File.Exists(filePath)) throw new Exception("文件不存在");

            var extension = fileName.StartsWith("http") ? Path.GetExtension(fileName) : fileName.Split('.')[1];
            var request = new FileUploadUrlRequest
            {
                contentMd5 = FileHelper.GetFileContentMD5(filePath, out var length),
                contentType = "application/octet-stream",
                fileName = fileName.StartsWith("http") ? Path.GetFileName(fileName) : fileName,
                convertToPDF = !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase),
                fileSize = length
            };

            try
            {
                // 获取上传URL
                var uploadUrlResult = await GetFileUploadUrlAsync(request);
                if (uploadUrlResult.Code != 0)
                    throw new Exception(uploadUrlResult.Message);

                // 实际上传文件
                var uploadResult = await UploadFileAsync(
                    uploadUrlResult.Data.fileUploadUrl,
                    filePath,
                    new Dictionary<string, string> { ["Content-MD5"] = request.contentMd5 });

                return uploadResult.HttpStatusCode == 200
                    ? uploadUrlResult
                    : throw new Exception(uploadResult.HttpStatusCodeMsg);
            }
            catch (Exception ex)
            {
                throw new Exception($"文件上传失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 查询文件上传状态
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<ESignApiResult<FileUploadStatus>> QueryUploadStatus(string fileId)
        {
            string endpoint = $"/v3/files/{fileId}";
            var apiUrl = $"{_option.ESignUrl}{endpoint}";

            var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, null, HttpType.GET, $"{endpoint}");

            var response = await ExecuteHttpRequestAsync(apiUrl, HttpType.GET, null, headers);

            return response.GetResult<ESignApiResult<FileUploadStatus>>();
        }

        /// <summary>
        /// 检索文件关键字坐标
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ESignApiResult<QueryKeyWord>> QueryKeyword(string fileId)
        {
            string endpoint = $"/v3/files/{fileId}/keyword-positions";
            var apiUrl = $"{_option.ESignUrl}{endpoint}";

            var request = new KeywordRequest()
            {
                fileId = fileId,
                keywords = _option.Keyword?.Split(';').ToList() ?? new List<string>()
            };

            var jsonParam = JsonConvert.SerializeObject(request);
            var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, jsonParam, HttpType.POST, endpoint);

            var response = await ExecuteHttpRequestAsync(apiUrl, HttpType.POST, jsonParam, headers);

            return response.GetResult<ESignApiResult<QueryKeyWord>>();
        }

        /// <summary>
        /// 基于文件发起签署
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="signFlowTitle"></param>
        /// <param name="queryKeyWord"></param>
        /// <returns></returns>
        public async Task<ESignApiResult<CreateByFile>> CreateByFile(List<string> conFieldIds, List<string> fileId, string signFlowTitle, Dictionary<string, QueryKeyWord> queryKeyWord, List<SignInfo> singnInfo)
        {
            var conFieldIdSet = new HashSet<string>(conFieldIds);
            //构建签署区信息集合
            var signFields = queryKeyWord
                .Where(x => conFieldIdSet.Contains(x.Key))
                .SelectMany(item => item.Value.keywordPositions
                    .SelectMany(k => k.positions)
                    .SelectMany(p => p.coordinates
                        .Select(pos => new SignField
                        {
                            fileId = item.Key,
                            normalSignFieldConfig = new NormalSignFieldConfig
                            {
                                signFieldPosition = new SignFieldPosition
                                {
                                    positionPage = p.pageNum.ToString(),
                                    positionX = Convert.ToInt32(pos.positionX),
                                    positionY = Convert.ToInt32(pos.positionY)
                                }
                            }
                        })
                    )
                )
                .ToList();
            //构建签署方信息集合
            var signerList = singnInfo?
                .Where(s => s != null)
                .Select(s => new Signer
                {
                    signerType = SignerType.Org,
                    orgSignerInfo = new OrgSignerInfo
                    {
                        orgName = s.OrgName,
                        orgInfo = s.OrgInfo,
                        transactorInfo = s.TransactorInfo
                    },
                    signConfig = s.SignConfig,
                    signFields = signFields
                })
                .ToList()
                ?? new List<Signer>();

            var endpoint = ApiUris.Create_By_File;

            var request = new CreateByFileRequest
            {
                docs = conFieldIds.Select(x => new Doc() { fileId = x }).ToList(),
                signFlowConfig = new SignFlowConfig { signFlowTitle = signFlowTitle, autoStart =_option. AutoStart, autoFinish = _option.AutoFinish, noticeConfig = new NoticeConfig { noticeTypes = _option.NoticeTypes }, redirectConfig = new RedirectConfig { redirectUrl = _option.RedirectUrl } },
                attachments = fileId.Select(x => new Attachment() { fileId = x }).ToList(),
                signFlowInitiator = null,
                signers = signerList
            };

            var jsonParam = JsonConvert.SerializeObject(request);

            var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, jsonParam, HttpType.POST, endpoint);

            var response = await ExecuteHttpRequestAsync($"{_option.ESignUrl}{endpoint}", HttpType.POST, jsonParam, headers);

            return response.GetResult<ESignApiResult<CreateByFile>>();
        }

        /// <summary>
        /// 获取签署页面链接
        /// </summary>
        /// <param name="signFlowId"></param>
        /// <returns></returns>
        public async Task<ESignApiResult<SignUrl>> GetSignUrl(string signFlowId,string psnId,string psnAccount)
        {
            string endpoint = $"/v3/sign-flow/{signFlowId}/sign-url";
            var apiUrl = $"{_option.ESignUrl}{endpoint}";

            var request = new SignUrlRequest()
            {
                clientType = _option.ClientType,
                needLogin = _option.NeedLogin,
                redirectConfig = new SignUrlRedirectConfig { redirectDelayTime = _option.RedirectDelayTime, redirectUrl = _option.SignRedirectUrl },
                @operator = new Operator() { psnId= psnId, psnAccount = psnAccount },
                urlType = _option.UrlType
            };


            var jsonParam = JsonConvert.SerializeObject(request);

            var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, jsonParam, HttpType.POST, endpoint);

            var response = await ExecuteHttpRequestAsync(apiUrl, HttpType.POST, jsonParam, headers);

            return response.GetResult<ESignApiResult<SignUrl>>();
        }

        /// <summary>
        /// 获取下载文件路径
        /// </summary>
        /// <param name="signFlowId"></param>
        /// <returns></returns>
        public async Task<ESignApiResult<FileDownLoad>> GetDownLoadFile(string signFlowId)
        {
            string endpoint = $"/v3/sign-flow/{signFlowId}/file-download-url";
            var apiUrl = $"{_option.ESignUrl}{endpoint}";

            var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, null, HttpType.GET, endpoint);

            var response = await ExecuteHttpRequestAsync(apiUrl, HttpType.GET, null, headers);

            return response.GetResult<ESignApiResult<FileDownLoad>>();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<HttpRespResult> UploadFileAsync(string url, byte[] fileByte, Dictionary<string, string> headers, string contentType = "application/octet-stream")
        {
            Stream fileStream = new MemoryStream(fileByte);

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var content = new StreamContent(fileStream))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                        foreach (var header in headers)
                        {
                            content.Headers.Add(header.Key, header.Value);
                        }

                        var response = await httpClient.PutAsync(url, content);
                        var responseContent = await response.Content.ReadAsStringAsync();

                        return new HttpRespResult
                        {
                            IsNetworkSuccess = response.IsSuccessStatusCode,
                            HttpStatusCode = (int)response.StatusCode,
                            RespData = responseContent,
                            NetworkMsg = response.ReasonPhrase
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region private methods

        /// <summary>
        /// 获取上传文件路径
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<ESignApiResult<FileUploadUrl>> GetFileUploadUrlAsync(FileUploadUrlRequest request)
        {
            var jsonParam = JsonConvert.SerializeObject(request);
            var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, jsonParam, HttpType.POST, ApiUris.File_Upload_Url);

            var response = await ExecuteHttpRequestAsync(_option.ESignUrl + ApiUris.File_Upload_Url, HttpType.POST, jsonParam, headers);

            return response.HttpStatusCode == 200
                ? JsonConvert.DeserializeObject<ESignApiResult<FileUploadUrl>>(response.RespData.ToString())
                : throw new Exception(response.HttpStatusCodeMsg);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private async Task<HttpRespResult> UploadFileAsync(string url, string filePath, Dictionary<string, string> headers, string contentType = "application/octet-stream")
        {
            Stream fileStream = null;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    if (filePath.StartsWith("http"))
                    {
                        fileStream = new MemoryStream(FileHelper.GetRemoteFileBinary(filePath));
                    }
                    else
                    {
                        fileStream = File.OpenRead(filePath);
                    }

                    using (var content = new StreamContent(fileStream))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                        foreach (var header in headers)
                        {
                            content.Headers.Add(header.Key, header.Value);
                        }

                        var response = await httpClient.PutAsync(url, content);
                        var responseContent = await response.Content.ReadAsStringAsync();

                        return new HttpRespResult
                        {
                            IsNetworkSuccess = response.IsSuccessStatusCode,
                            HttpStatusCode = (int)response.StatusCode,
                            RespData = responseContent,
                            NetworkMsg = response.ReasonPhrase
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
        }
        /// <summary>
        /// 发送 http 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="reqType"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private async Task<HttpRespResult> ExecuteHttpRequestAsync(string url, string reqType, string data, Dictionary<string, string> headers)
        {
            HttpWebRequest webReq = null;
            HttpWebResponse webResp = null;
            var result = new HttpRespResult();

            try
            {
                webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.Method = reqType;
                webReq.Accept = "*/*";
                webReq.ContentType = "application/json; charset=UTF-8";
                webReq.MaximumAutomaticRedirections = 50;
                webReq.AllowAutoRedirect = true;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        webReq.Headers.Add(header.Key, header.Value);
                    }
                }

                if (reqType == "POST" || reqType == "PUT")
                {
                    byte[] bodyBytes = Encoding.UTF8.GetBytes(data);
                    webReq.ContentLength = bodyBytes.Length;

                    using (var stream = await webReq.GetRequestStreamAsync().ConfigureAwait(false))
                    {
                        await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length).ConfigureAwait(false);
                    }
                }

                webResp = (HttpWebResponse)await webReq.GetResponseAsync().ConfigureAwait(false);
                result.HttpStatusCode = (int)webResp.StatusCode;
                result.HttpStatusCodeMsg = webResp.StatusCode.ToString();

                using (var reader = new StreamReader(webResp.GetResponseStream(), Encoding.UTF8))
                {
                    result.RespData = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                result.IsNetworkSuccess = true;
                result.NetworkMsg = "网络请求成功";
            }
            catch (Exception ex)
            {
                result.IsNetworkSuccess = false;
                result.NetworkMsg = ex.Message;
            }
            finally
            {
                webResp?.Close();
                webReq?.Abort();
            }

            return result;
        }
        /// <summary>
        /// 错误信息捕获
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static HttpRespResult HandleHttpException(HttpRequestException ex)
        {
            return new HttpRespResult
            {
                IsNetworkSuccess = false,
                NetworkMsg = ex.Message,
                HttpStatusCode = 0
            };
        }

        #endregion
    }

}
