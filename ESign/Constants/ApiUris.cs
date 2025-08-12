namespace ESign.Constants
{
    public static class ApiUris
    {
        public const string Identity_Info = "/v3/organizations/identity-info";

        /// <summary>
        /// 查询机构认证信息
        /// </summary>
        public const string Identity_Info_orgName = "/v3/organizations/identity-info?orgName={orgName}";

        /// <summary>
        /// 查询企业内部印章
        /// </summary>
        public const string Org_Own_Seal_List = "/v3/seals/org-own-seal-list?orgId={orgId}&pageNum={pageIndex}&pageSize={pageSize}";

        /// <summary>
        /// 获取文件上传地址
        /// </summary>
        public const string File_Upload_Url = "/v3/files/file-upload-url";

        /// <summary>
        /// 查询文件上传状态
        /// </summary>
        public const string File_Status = "/v3/files/{fileId}";

        /// <summary>
        /// 基于文件发起签署
        /// </summary>
        public const string Create_By_File = "/v3/sign-flow/create-by-file";

        /// <summary>
        /// 下载已签署文件及附属材料
        /// </summary>
        public const string File_DownLoad_Url = "/v3/sign-flow/{signFlowId}/file-download-url";
    }
}
