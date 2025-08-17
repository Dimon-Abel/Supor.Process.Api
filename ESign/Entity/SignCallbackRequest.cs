namespace ESign.Entity
{
    public class SignCallbackRequest
    {
        /// <summary>
        /// action固定为：SIGN_MISSON_COMPLETE，此类型用于接收签署方的签署结果，以流程中签署方维度通知。
        /// action固定为：SIGN_FLOW_COMPLETE，此类型当整个签署流程完结，触发该类型的回调通知
        /// </summary>
        public string action {  get; set; }
        /// <summary>
        /// signResult签署结果：2 - 签署完成，4 - 拒签
        /// </summary>
        public SignResult signResult { get; set; }
        /// <summary>
        /// signFlowStatus签署流程最终状态
        /// </summary>
        public SignFlowStatus signFlowStatus { get; set; }

        /// <summary>
        /// 签署流程Id
        /// </summary>
        public string signFlowId { get; set; }

        /// <summary>
        /// 文件存储路径
        /// </summary>
        public string fileSavePath { get; set; }
    }

    public enum SignFlowStatus
    {
        /// <summary>
        /// 所有签署方完成签署
        /// </summary>
        Complete = 2,
        /// <summary>
        /// 发起方撤销签署任务
        /// </summary>
        Revoke = 3,
        /// <summary>
        /// 签署截止日到期后触发
        /// </summary>
        Exprie = 5,
        /// <summary>
        /// 签署方拒绝签署
        /// </summary>
        Reject = 7
    }

    public enum SignResult
    {
        /// <summary>
        /// 签署完成
        /// </summary>
        Complete=2,
        /// <summary>
        /// 拒签
        /// </summary>
        Reject = 4
    }
}
