using System;
using System.Collections.Generic;

namespace ESign.Entity.Request
{
    public class CreateByFileRequest
    {
        public List<Doc> docs { get; set; } = new List<Doc>();
        public List<Attachment> attachments { get; set; } = new List<Attachment>();
        public SignFlowConfig signFlowConfig { get; set; } = new SignFlowConfig();
        public SignFlowInitiator signFlowInitiator { get; set; } = new SignFlowInitiator();
        public List<Signer> signers { get; set; } = new List<Signer>();
        public List<Copier> copiers { get; set; } = new List<Copier>();
    }

    public class Doc
    {
        public string fileId { get; set; }
        public string fileName { get; set; }
    }

    public class Attachment : Doc { }

    public class SignFlowConfig
    {
        public string signFlowTitle { get; set; }
        public long? signFlowExpireTime { get; set; }
        public bool autoStart { get; set; } = true;
        public bool autoFinish { get; set; } = false;
        public NoticeConfig noticeConfig { get; set; }
        public bool identityVerify { get; set; }
        public SignConfig signConfig { get; set; } = new SignConfig();
        public string notifyUrl { get; set; }
        public RedirectConfig redirectConfig { get; set; } = new RedirectConfig();
        public AuthConfig authConfig { get; set; } = new AuthConfig();
    }

    public class SignConfig
    {
        public string availableSignClientTypes { get; set; }
        public bool showBatchDropSealButton { get; set; }
    }

    public class RedirectConfig
    {
        public string redirectUrl { get; set; }
    }

    public class AuthConfig
    {
        public List<string> psnAvailableAuthModes { get; set; } = new List<string>();
        public List<string> willingnessAuthModes { get; set; } = new List<string>();
        public List<string> orgAvailableAuthModes { get; set; } = new List<string>();
    }

    public class SignFlowInitiator
    {
        public OrgInitiator orgInitiator { get; set; } = new OrgInitiator();
    }

    public class OrgInitiator
    {
        public string orgId { get; set; }
        public Transactor transactor { get; set; } = new Transactor();
    }

    public class Transactor
    {
        public string psnId { get; set; }
    }

    public class Signer
    {
        public SignerSignConfig signConfig { get; set; } = new SignerSignConfig();
        public NoticeConfig noticeConfig { get; set; } = new NoticeConfig();
        public SignerType signerType { get; set; }
        public PsnSignerInfo psnSignerInfo { get; set; }
        public OrgSignerInfo orgSignerInfo { get; set; }
        public List<SignField> signFields { get; set; } = new List<SignField>();
    }


    public enum SignerType
    {
        /// <summary>
        /// 个人
        /// </summary>
        Personal,
        /// <summary>
        /// 机构
        /// </summary>
        Org,
        /// <summary>
        /// 法人
        /// </summary>
        LegalPerson,
        /// <summary>
        /// 经办人
        /// </summary>
        Operator
    }

    public class SignerSignConfig
    {
        public string forcedReadingTime { get; set; }
        public int signOrder { get; set; } = 1;
    }

    public class NoticeConfig
    {
        public string noticeTypes { get; set; }
    }

    public class PsnSignerInfo
    {
        public string psnAccount { get; set; }
        public PsnInfo psnInfo { get; set; } = new PsnInfo();
    }

    public class PsnInfo
    {
        public string psnName { get; set; }
        public string psnIDCardNum { get; set; }
        public string psnIDCardType { get; set; }
    }

    public class OrgSignerInfo
    {
        public string orgName { get; set; }
        public OrgInfo orgInfo { get; set; } = new OrgInfo();
        public TransactorInfo transactorInfo { get; set; } = new TransactorInfo();
    }

    public class OrgInfo
    {
        public string orgIDCardNum { get; set; }
        public string orgIDCardType { get; set; }
    }

    public class TransactorInfo
    {
        public string psnAccount { get; set; }
        public PsnInfo psnInfo { get; set; } = new PsnInfo();
    }

    public class SignField
    {
        public string fileId { get; set; }
        public string customBizNum { get; set; }
        public int signFieldType { get; set; }
        public NormalSignFieldConfig normalSignFieldConfig { get; set; } = new NormalSignFieldConfig();
        public SignDateConfig signDateConfig { get; set; } = new SignDateConfig();
    }

    public class NormalSignFieldConfig
    {
        public bool autoSign { get; set; }
        public bool freeMode { get; set; }
        public bool movableSignField { get; set; }
        public string psnSealStyles { get; set; }
        public string signFieldSize { get; set; }
        public int signFieldStyle { get; set; } = 1;
        public SignFieldPosition signFieldPosition { get; set; } = new SignFieldPosition();
    }

    public class SignFieldPosition
    {
        public string positionPage { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
    }

    public class SignDateConfig
    {
        public string dateFormat { get; set; }
        public int showSignDate { get; set; }
        public int signDatePositionX { get; set; }
        public int signDatePositionY { get; set; }
    }

    public class Copier
    {
        public CopierOrgInfo copierOrgInfo { get; set; }
        public CopierPsnInfo copierPsnInfo { get; set; }
    }

    public class CopierOrgInfo
    {
        public string orgName { get; set; }
    }

    public class CopierPsnInfo
    {
        public string psnAccount { get; set; }
    }

}
