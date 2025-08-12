namespace ESign.Entity.Result
{
    public class OrgIdentity
    {
        /// <summary>
        /// 实名认证状态  0 - 未实名，1 - 已实名
        /// </summary>
        public int realnameStatus { get; set; }

        /// <summary>
        /// 是否授权身份信息给当前应用
        /// true - 已授权，false - 未授权
        ///【注】发起授权认证时需要授权：get_org_identity_info 权限，并操作授权完成后，才能返回已授权状态
        /// </summary>
        public bool authorizeUserInfo { get; set; }

        /// <summary>
        /// 机构账号
        /// </summary>
        public string orgId { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string orgName { get;set; }

        /// <summary>
        /// 机构实名认证完成时使用的认证方式（如果多次认证则取最近一次认证）
        /// </summary>
        public string orgAuthMode { get; set; }

        /// <summary>
        /// 机构认证信息
        /// </summary>
        public OrgInfo orgInfo { get; set; }
    }

    public class OrgInfo
    {
        /// <summary>
        /// 组织机构证件号
        /// </summary>
        public string orgIDCardNum { get; set; }
        public string orgIDCardType { get; set; }
        public string legalRepName { get; set; }
        public string legalRepIDCardNum { get; set; }
        public string legalRepIDCardType { get; set; }
        public string corporateAccount { get; set; }
        public string orgBankAccountNum { get; set; }
        public string cnapsCode { get; set; }
        public string authorizationDownloadUrl { get; set; }
        public string licenseDownloadUrl { get; set; }
        public string adminName { get; set; }
        public string adminAccount { get; set; }
    }
}
