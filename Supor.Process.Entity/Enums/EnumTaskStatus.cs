namespace Supor.Process.Entity.Enums
{
    public enum EnumTaskStatus
    {
        /// <summary>
        /// 提交
        /// </summary>
        Submit = 0,
        /// <summary>
        /// 统一
        /// </summary>
        Agree = 1,
        /// <summary>
        /// 不同意
        /// </summary>
        DisAgree = 2,
        /// <summary>
        /// 驳回至申请人
        /// </summary>
        Reject = 3,
        /// <summary>
        /// 驳回至上一审批节点
        /// </summary>
        Return = 4
    }
}
