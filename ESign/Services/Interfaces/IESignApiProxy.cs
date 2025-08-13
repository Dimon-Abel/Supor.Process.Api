using System.Threading.Tasks;
using ESign.Entity;
using ESign.Entity.Result;

namespace ESign.Services
{
    public interface IESignApiProxy
    {
        /// <summary>
        /// 获取机构认证信息
        /// </summary>
        /// <returns></returns>
        Task<ESignApiResult<OrgIdentity>> GetOrganizationIdentityInfo();
        /// <summary>
        /// 获取上传文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<ESignApiResult<FileUploadUrl>> FileUpload(string fileName);

        /// <summary>
        /// 查询文件上传状态
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        Task<ESignApiResult<FileUploadStatus>> QueryUploadStatus(string fileId);

        /// <summary>
        /// 检索文件关键字坐标
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        Task<ESignApiResult<QueryKeyWord>> QueryKeyword(string fileId);

        /// <summary>
        /// 基于文件发起签署
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="signFlowTitle"></param>
        /// <param name="identityInfo"></param>
        /// <param name="queryKeyWord"></param>
        /// <returns></returns>
        Task<ESignApiResult<CreateByFile>> CreateByFile(string fileId, string signFlowTitle, QueryKeyWord queryKeyWord);

        /// <summary>
        /// 获取下载签署文件
        /// </summary>
        /// <param name="signFlowId"></param>
        /// <returns></returns>
        Task<ESignApiResult<FileDownLoad>> GetDownLoadFile(string signFlowId);

        /// <summary>
        /// 获取签署页面链接
        /// </summary>
        /// <param name="signFlowId"></param>
        /// <returns></returns>
        Task<ESignApiResult<SignUrl>> GetSignUrl(string signFlowId);
    }
}
