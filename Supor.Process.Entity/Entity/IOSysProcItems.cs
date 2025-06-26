using System;

namespace Supor.Process.Entity.Entity
{
    public class IOSysProcItems
    {
        /// <summary>
        /// 表主键
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 系统来源  来自于哪个异构系统
        /// </summary>
        public string SysSource { get; set; }
        /// <summary>
        /// 接口名称  调用API接口名称
        /// </summary>
        public string InterfaceName { get; set; }
        /// <summary>
        /// Json数据 传入给接口的数据
        /// </summary>
        public string JsonData { get; set; }
        /// <summary>
        /// Json主键 判断是否唯一的标识
        /// </summary>
        public string JsonKey { get; set; }
        /// <summary>
        /// 操作状态  0：创建新流程；1：重新发起申请；2：终止流程
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 是否以创建流程 0 未创建 1 已创建
        /// </summary>
        public int LaunchStatus { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 流程实例
        /// </summary>
        public string ProcInstId { get; set; }
        /// <summary>
        /// 流程单号
        /// </summary>
        public string ProcApplyNo { get; set; }
        /// <summary>
        /// 流程数据  需要插入到流程表中的数据
        /// </summary>
        public string ProcessData { get; set; }
        /// <summary>
        /// 是否已处理  0：未处理；1：已处理
        /// </summary>
        public int IsSubmit { get; set; }
        /// <summary>
        /// 创建人域账号
        /// </summary>
        public string CreateUserID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
    }
}
