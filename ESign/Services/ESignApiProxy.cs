using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ESign.Constants;
using ESign.Entity;
using ESign.Entity.Request;
using ESign.Entity.Result;
using ESign.Helper;
using ESign.Options;
using Newtonsoft.Json;

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
        public async Task<ESignApiResult<OrgIdentity>> GetOrganizationIdentityInfo()
        {
            const string endpoint = "/v3/organizations/identity-info";
            var query = $"?orgName={_option.ESignOrgName}";
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

        public async Task<ESignApiResult<PersonIdentity>> GetPersonIdentityInfo(string account)
        {
            const string endpoint = "/v3/persons/identity-info";
            var query = $"?psnAccount={account}";
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
        /// 上传文件流程_返回fileId
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ESignApiResult<FileUploadUrl>> FileUpload(string fileName)
        {
            var filePath = Path.Combine(_option.UploadFile, fileName);
            if (!File.Exists(filePath)) return null;

            var fileInfo = new FileInfo(filePath);
            var request = new FileUploadUrlRequest
            {
                contentMd5 = FileHelper.GetFileContentMD5(filePath),
                contentType = "application/octet-stream",
                fileName = fileName,
                convertToPDF = !fileInfo.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase),
                fileSize = (int)fileInfo.Length
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
        public async Task<ESignApiResult<CreateByFile>> CreateByFile(string fileId, string signFlowTitle, QueryKeyWord queryKeyWord)
        {
            var signFields = queryKeyWord.keywordPositions
                .SelectMany(k => k.positions)
                .SelectMany(p => p.coordinates.Select(pos => new SignField
                {
                    fileId = fileId,
                    normalSignFieldConfig = new NormalSignFieldConfig
                    {
                        signFieldPosition = new SignFieldPosition
                        {
                            positionPage = p.pageNum.ToString(),
                            positionX = Convert.ToInt32(pos.positionX),
                            positionY = Convert.ToInt32(pos.positionY)
                        }
                    }
                }))
                .ToList();

            var endpoint = ApiUris.Create_By_File;

            var request = new CreateByFileRequest
            {
                docs = new List<Doc> { new Doc() { fileId = fileId } },
                signFlowConfig = new SignFlowConfig { signFlowTitle = signFlowTitle, autoStart = true, autoFinish = true },
                signFlowInitiator = null,
                signers = new List<Signer>
                {
                    new Signer()
                    {
                        signerType = SignerType.Org,
                        orgSignerInfo = new OrgSignerInfo
                        {
                            orgName = _option.ESignOrgName,
                            transactorInfo = new TransactorInfo { psnAccount = _option.PsnAccount }
                        },
                        signConfig = new SignerSignConfig { signOrder = 1 },
                        signFields = signFields
                    }
                }
            };

            var jsonParam = JsonConvert.SerializeObject(request);
            var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, jsonParam, HttpType.POST, endpoint);

            var response = await ExecuteHttpRequestAsync( $"{_option.ESignUrl}{endpoint}", HttpType.POST, jsonParam, headers);

            return response.GetResult<ESignApiResult<CreateByFile>>();
        }

        /// <summary>
        /// 获取签署页面链接
        /// </summary>
        /// <param name="signFlowId"></param>
        /// <returns></returns>
        public async Task<ESignApiResult<SignUrl>> GetSignUrl(string signFlowId)
        {
            string endpoint = $"/v3/sign-flow/{signFlowId}/sign-url";
            var apiUrl = $"{_option.ESignUrl}{endpoint}";

            var request = new SignUrlRequest()
            {
                signFlowId = signFlowId,
                Operator = null
            };

            var jsonParam = JsonConvert.SerializeObject(request);
            var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, jsonParam, HttpType.POST, endpoint);

            var response = await ExecuteHttpRequestAsync(apiUrl, HttpType.POST, jsonParam, headers);

            return response.GetResult<ESignApiResult<SignUrl>>();
        }

        public async Task<ESignApiResult<FileDownLoad>> GetDownLoadFile(string signFlowId)
        {
            string endpoint = $"/v3/sign-flow/{signFlowId}/file-download-url";
            var apiUrl = $"{_option.ESignUrl}{endpoint}";

            var headers = HttpHelper.SignAndBuildSignAndJsonHeader(_option.AppId, _option.AppSecret, null, HttpType.GET, endpoint);

            var response = await ExecuteHttpRequestAsync(apiUrl, HttpType.GET, null, headers);

            return response.GetResult<ESignApiResult<FileDownLoad>>();
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
        private static async Task<HttpRespResult> UploadFileAsync(string url, string filePath, Dictionary<string, string> headers, string contentType = "application/octet-stream")
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var fileStream = File.OpenRead(filePath))
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
            }
            catch (HttpRequestException ex)
            {
                return HandleHttpException(ex);
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
            catch (WebException webEx)
            {
                result.IsNetworkSuccess = false;
                result.NetworkMsg = webEx.Message;

                if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response != null)
                {
                    webResp = (HttpWebResponse)webEx.Response;
                    result.HttpStatusCode = (int)webResp.StatusCode;
                    result.HttpStatusCodeMsg = webResp.StatusCode.ToString();

                    using (var reader = new StreamReader(webResp.GetResponseStream(), Encoding.UTF8))
                    {
                        result.RespData = await reader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }
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
