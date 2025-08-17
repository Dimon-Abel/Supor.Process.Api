using System;
using System.Collections.Generic;

namespace ESign.Entity.Request
{
    public class CreateByFileRequest
    {
        /// <summary>
        /// 设置待签署文件信息
        /// </summary>
        public List<Doc> docs { get; set; } = new List<Doc>();
        /// <summary>
        /// 设置附属材料信息
        /// </summary>
        public List<Attachment> attachments { get; set; } = new List<Attachment>();
        /// <summary>
        /// 签署流程配置项
        /// </summary>
        public SignFlowConfig signFlowConfig { get; set; } = new SignFlowConfig();
        /// <summary>
        /// 签署流程的发起方
        /// </summary>
        public SignFlowInitiator signFlowInitiator { get; set; } = new SignFlowInitiator();
        /// <summary>
        /// 签署方信息
        /// </summary>
        public List<Signer> signers { get; set; } = new List<Signer>();
        /// <summary>
        /// 抄送方信息
        /// </summary>
        public List<Copier> copiers { get; set; } = new List<Copier>();
    }

    public class Doc
    {
        /// <summary>
        /// 待签署文件ID
        /// </summary>
        public string fileId { get; set; }
        /// <summary>
        /// 文件名称（需要添加PDF文件后缀名，“xxx.pdf”）
        /// </summary>
        public string fileName { get; set; }
    }

    public class Attachment : Doc { }

    public class SignFlowConfig
    {
        /// <summary>
        /// 签署流程主题（将展示在签署通知和签署页的任务信息中）
        /// </summary>
        public string signFlowTitle { get; set; }
        /// <summary>
        /// 签署截止时间
        /// </summary>
        public long? signFlowExpireTime { get; set; }
        /// <summary>
        /// 自动开启签署流程，默认值 true
        /// </summary>
        public bool autoStart { get; set; } = true;
        /// <summary>
        /// 自动开启签署流程，默认值 true
        /// </summary>
        public bool autoFinish { get; set; } = true;
        /// <summary>
        /// 流程整体通知配置项
        /// </summary>
        public NoticeConfig noticeConfig { get; set; }
        /// <summary>
        /// 身份校验配置项（当开发者指定的签署人信息与该签署人在e签宝已有的身份信息不一致时如何处理），默认：true
        /// true - 接口报错（提示：传入的指定签署人信息与实名信息不一致相关报错）
        /// false - 不报错，正常发起
        /// </summary>
        public bool identityVerify { get; set; }
        /// <summary>
        /// 签署配置项
        /// </summary>
        public SignConfig signConfig { get; set; } = new SignConfig();
        /// <summary>
        /// 接收相关回调通知的Web地址
        /// </summary>
        public string notifyUrl { get; set; }
        /// <summary>
        /// 重定向配置项
        /// </summary>
        public RedirectConfig redirectConfig { get; set; } = new RedirectConfig();
        /// <summary>
        /// 流程整体认证配置项
        /// </summary>
        public AuthConfig authConfig { get; set; } = new AuthConfig();
    }

    public class SignConfig
    {
        /// <summary>
        /// 签署终端类型，默认值：1,2（英文逗号分隔）
        /// </summary>
        public string availableSignClientTypes { get; set; }
        /// <summary>
        /// 签署页面是否显示“同时盖在所有签署区”按钮，默认值 true
        /// true - 显示（显示按钮并默认开启）
        /// false - 不显示（不显示按钮，即：不能同时盖在所有签署区）
        /// </summary>
        public bool showBatchDropSealButton { get; set; }
    }

    public class RedirectConfig
    {
        /// <summary>
        /// 签署完成后跳转页面
        /// </summary>
        public string redirectUrl { get; set; }
    }

    public class AuthConfig
    {
        /// <summary>
        /// 个人实名认证方式
        /// </summary>
        public List<string> psnAvailableAuthModes { get; set; } = new List<string>();
        /// <summary>
        /// 签署意愿认证方式
        /// </summary>
        public List<string> willingnessAuthModes { get; set; } = new List<string>();
        /// <summary>
        /// 机构实名认证方式
        /// </summary>
        public List<string> orgAvailableAuthModes { get; set; } = new List<string>();
    }

    public class SignFlowInitiator
    {
        /// <summary>
        /// 机构发起方信息
        /// </summary>
        public OrgInitiator orgInitiator { get; set; } = new OrgInitiator();
    }

    public class OrgInitiator
    {
        /// <summary>
        /// 机构账号ID
        /// </summary>
        public string orgId { get; set; }
        /// <summary>
        /// 机构发起方的经办人
        /// </summary>
        public Transactor transactor { get; set; } = new Transactor();
    }

    public class Transactor
    {
        /// <summary>
        /// 经办人账号ID
        /// </summary>
        public string psnId { get; set; }
    }

    public class Signer
    {
        /// <summary>
        /// 签署人配置项
        /// </summary>
        public SignerSignConfig signConfig { get; set; } = new SignerSignConfig();
        /// <summary>
        /// 设置签署方的通知方式
        /// </summary>
        public NoticeConfig noticeConfig { get; set; } = new NoticeConfig();
        /// <summary>
        /// 签署方类型，0 - 个人，1 - 企业/机构，2 - 法定代表人，3 - 经办人
        /// </summary>
        public SignerType signerType { get; set; }
        /// <summary>
        /// 个人签署方信息
        /// </summary>
        public PsnSignerInfo psnSignerInfo { get; set; }
        /// <summary>
        /// 企业/机构签署方信息
        /// </summary>
        public OrgSignerInfo orgSignerInfo { get; set; }
        /// <summary>
        /// 签署区信息（设置签署方 盖章/签名/文字输入的区域）
        /// </summary>
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
        /// <summary>
        /// 设置签署页面强制阅读倒计时时间，默认值为 0（单位：秒，最大值999）
        /// </summary>
        public string forcedReadingTime { get; set; }
        /// <summary>
        /// 设置签署方的签署顺序
        /// 按序签时支持传入顺序值 1 - 255（值小的先签署）
        /// 同时签时，允许值重复
        /// </summary>
        public int signOrder { get; set; } = 1;
    }

    public class NoticeConfig
    {
        /// <summary>
        /// 签署通知类型，默认不通知（值为""空字符串），允许多种通知方式，请使用英文逗号分隔
        /// "" - 不通知（默认值）
        /// 1 - 短信通知（如果套餐内带“分项”字样，请确保开通【电子签名流量费（分项）认证】中的子项：【短信服务】，否则短信通知收不到）
        /// 2 - 邮件通知
        /// 3 - 钉钉工作通知（需使用e签宝钉签版产品）
        /// 5 - 微信通知（用户需关注“e签宝电子签名”微信公众号且使用过e签宝微信小程序）
        /// 6 - 企业微信通知（需要使用e签宝企微版产品）
        /// 7 - 飞书通知（需要使用e签宝飞书版产品）
        /// </summary>
        public string noticeTypes { get; set; }
    }

    public class PsnSignerInfo
    {
        /// <summary>
        /// 个人账号标识（手机号或邮箱）用于登录e签宝官网的凭证
        /// </summary>
        public string psnAccount { get; set; }
        /// <summary>
        /// 个人签署方身份信息
        /// </summary>
        public PsnInfo psnInfo { get; set; } = new PsnInfo();
    }

    public class PsnInfo
    {
        /// <summary>
        /// 个人姓名
        ///【注】传psnAccount（个人账号标识）时，该参数为必传项
        /// </summary>
        public string psnName { get; set; }
        /// <summary>
        /// 个人证件号
        /// </summary>
        public string psnIDCardNum { get; set; }
        /// <summary>
        /// 个人证件类型，可选值如下：
        /// CRED_PSN_CH_IDCARD - 中国大陆居民身份证（默认值）
        /// CRED_PSN_CH_HONGKONG - 香港来往大陆通行证（回乡证）
        /// CRED_PSN_CH_MACAO - 澳门来往大陆通行证（回乡证）
        /// CRED_PSN_CH_TWCARD - 台湾来往大陆通行证（台胞证）
        /// CRED_PSN_PASSPORT - 护照
        ///【注】CRED_PSN_CH_IDCARD 类型同时兼容港澳台居住证（81、82、83开头18位证件号）、外国人永久居住证（9开头18位证件号）
        /// </summary>
        public string psnIDCardType { get; set; }
    }

    public class OrgSignerInfo
    {
        /// <summary>
        /// 企业/机构名称（账号标识）
        /// </summary>
        public string orgName { get; set; }
        /// <summary>
        /// 企业/机构签署方信息（将展示在机构认证页面）
        /// </summary>
        public OrgInfo orgInfo { get; set; } = new OrgInfo();
        /// <summary>
        /// 企业/机构经办人信息
        /// </summary>
        public TransactorInfo transactorInfo { get; set; } = new TransactorInfo();
    }

    public class OrgInfo
    {
        /// <summary>
        /// 企业/机构证件号
        /// </summary>
        public string orgIDCardNum { get; set; }
        /// <summary>
        /// 企业/机构证件类型，可选值如下：
        /// CRED_ORG_USCC - 统一社会信用代码
        /// CRED_ORG_REGCODE - 工商注册号
        /// </summary>
        public string orgIDCardType { get; set; }
    }

    public class TransactorInfo
    {
        /// <summary>
        /// 经办人姓名
        ///【注】传psnAccount（经办人账号标识）时，该参数为必传项
        /// </summary>
        public string psnAccount { get; set; }
        /// <summary>
        /// 个人签署方身份信息
        ///补充说明：
        ///已实名用户，若传入的psnInfo与在e签宝绑定的psnAccount一致，则无需重复实名，签署页直接进行签署意愿认证；
        ///已实名用户，若传入的psnInfo与在e签宝绑定的psnAccount不一致，则接口将会报错，建议核实用户身份信息后重新发起流程；
        ///未实名用户，签署页将根据传入的身份信息进行用户实名认证。
        /// </summary>
        public PsnInfo psnInfo { get; set; } = new PsnInfo();
    }

    public class SignField
    {
        /// <summary>
        /// 签署区所在待签署文件ID
        /// </summary>
        public string fileId { get; set; }
        /// <summary>
        /// 开发者自定义业务编号
        /// </summary>
        public string customBizNum { get; set; }
        /// <summary>
        /// 签署区类型，默认值为 0
        /// 0 - 签章区 （添加印章、签名等）
        /// 1 - 备注区（添加备注文字信息等）（点击了解 备注签署）
        /// 2 - 独立签署日期（添加单独的签署日期）
        /// </summary>
        public int signFieldType { get; set; }
        /// <summary>
        /// 签章区配置项（指定signFieldType为 0 - 签章区时，该参数为必传项）
        /// </summary>
        public NormalSignFieldConfig normalSignFieldConfig { get; set; } = new NormalSignFieldConfig();
        /// <summary>
        /// 签署区/备注区的签署日期配置项
        /// </summary>
        public SignDateConfig signDateConfig { get; set; } = new SignDateConfig();
    }

    public class NormalSignFieldConfig
    {
        /// <summary>
        /// 是否是后台自动落章关联的独立签署日期，默认值 false
        /// true - 后台自动落章关联的独立签署日期（平台静默签署）
        /// false - 签署页手动签章关联的独立签署日期
        /// </summary>
        public bool autoSign { get; set; }
        /// <summary>
        /// 是否自由签章，默认值 false
        /// </summary>
        public bool freeMode { get; set; }
        /// <summary>
        /// 页面是否可移动签章区，默认值 false 
        /// true - 可移动 ，false - 不可移动
        /// </summary>
        public bool movableSignField { get; set; }
        /// <summary>
        /// 页面可选个人印章样式，默认值0和1（英文逗号分隔）
        /// 0 - 手写签名
        /// 1 - 姓名印章
        /// 2 - 手写签名AI校验
        /// </summary>
        public string psnSealStyles { get; set; }
        /// <summary>
        /// 签章区尺寸（正方形的边长，单位为px）
        /// </summary>
        public string signFieldSize { get; set; }
        /// <summary>
        /// 签章区样式 
        /// 1 - 单页签章，2 - 骑缝签章
        /// </summary>
        public int signFieldStyle { get; set; } = 1;
        /// <summary>
        /// 签章区位置信息
        /// </summary>
        public SignFieldPosition signFieldPosition { get; set; } = new SignFieldPosition();
    }

    public class SignFieldPosition
    {
        /// <summary>
        /// 签章区所在页码
        /// </summary>
        public string positionPage { get; set; }
        /// <summary>
        /// 签章区所在X坐标（当signFieldStyle为2即骑缝签章时，该参数不生效，可不传值）
        /// </summary>
        public int positionX { get; set; }
        /// <summary>
        /// 签章区所在Y坐标
        /// </summary>
        public int positionY { get; set; }
    }

    public class SignDateConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string dateFormat { get; set; }
        /// <summary>
        /// 日期格式
        /// </summary>
        public int showSignDate { get; set; }
        /// <summary>
        /// 签署日期所在位置X坐标，当showSignDate为 1- 固定位置显示时生效。
        /// </summary>
        public int signDatePositionX { get; set; }
        /// <summary>
        /// 签署日期所在位置Y坐标，当showSignDate为 1- 固定位置显示时生效。
        /// </summary>
        public int signDatePositionY { get; set; }
    }

    public class Copier
    {
        /// <summary>
        /// 机构抄送方信息（orgName与orgId，二选一传值）
        /// </summary>
        public CopierOrgInfo copierOrgInfo { get; set; }
        /// <summary>
        /// 个人抄送方信息（psnAccount与psnId，二选一传值）
        /// </summary>
        public CopierPsnInfo copierPsnInfo { get; set; }
    }

    public class CopierOrgInfo
    {
        /// <summary>
        /// 机构名称
        /// </summary>
        public string orgName { get; set; }
    }

    public class CopierPsnInfo
    {
        /// <summary>
        /// 个人账号标识（手机号/邮箱号）
        /// </summary>
        public string psnAccount { get; set; }
    }

}
