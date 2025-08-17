using System;
using System.Collections.Generic;

namespace ESign.Entity.Request
{
    public class SendRequest
    {
        /// <summary>
        /// 签署流程主题
        /// </summary>
        public string Title { get; set; } = "签署文件";

        /// <summary>
        /// 合同需盖章文件
        /// </summary>
        public List<FileInformation> Docs { get; set; }

        /// <summary>
        /// 合同附件
        /// </summary>
        public List<FileInformation> Attachments { get; set; }

        /// <summary>
        /// 机构账号ID
        /// </summary>
        public string OrgId { get; set; }
        /// <summary>
        /// 组织机构名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 组织机构证件号
        /// </summary>
        public string OrgIDCardNum { get; set; }
        /// <summary>
        /// 组织机构证件类型（传orgIDCardNum时，该参数为必传）
        /// CRED_ORG_USCC - 统一社会信用代码
        /// CRED_ORG_REGCODE - 工商注册号
        /// </summary>
        public string OrgIDCardType { get; set; }
        /// <summary>
        /// 个人账号ID
        /// </summary>
        public string PsnId { get; set; }
        /// <summary>
        /// 个人账号标识（手机号或邮箱）
        /// </summary>
        public string PsnAccount { get; set; }
        /// <summary>
        /// 签署信息
        /// </summary>
        public List<SignInfo> SignInfo { get; set; }
    }

    public class FileInformation
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件流
        /// </summary>
        public byte[] FileBytes { get; set; }
    }

    public class SignInfo
    {
        /// <summary>
        /// 企业/机构名称（账号标识）
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 企业/机构签署方信息（将展示在机构认证页面）
        /// </summary>
        public OrgInfo OrgInfo { get; set; }
        /// <summary>
        /// 企业/机构经办人信息
        /// 企业/机构手动签署（autoSign为false），经办人信息必传；
        /// 企业/机构自动落章（autoSign为true），请不要传该参数。
        /// </summary>
        public TransactorInfo TransactorInfo { get; set; }
        /// <summary>
        /// 签署人配置项
        /// </summary>
        public SignerSignConfig SignConfig { get; set; }
    }
}
