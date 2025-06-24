namespace Supor.Process.Entity.InputDto
{
    public class ProcessDataDto
    {
        /// <summary>
        /// 申请人名称
        /// </summary>
        public string System_ApplyUserName { get; set; }
        /// <summary>
        /// 用户Cdoe
        /// </summary>
        public string Sys_UserCode { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        public string System_ApplyUserID { get; set; }
        /// <summary>
        /// OrgCode
        /// </summary>
        public string Sys_OrgCode { get; set; }
        /// <summary>
        /// BUCode
        /// </summary>
        public string Sys_BuCode { get; set; }
        /// <summary>
        /// PosCode
        /// </summary>
        public string Sys_PosCode { get; set; }

        public string Status { get; set; }
        /// <summary>
        /// 提交备注
        /// </summary>
        public string SignRemarks { get; set; }

        public string IsUpdatedBData { get; set; }
    }
}
