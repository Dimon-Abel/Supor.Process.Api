using Supor.Process.Entity.Enums;
using Supor.Process.Entity.Attributies;

namespace Supor.Process.Entity.Entity
{
	/// <summary>
	/// 任务表
	/// </summary>
	[TableName("I_OSYS_PROCDATA_ITEMS")]
	public class ApiTaskEntity
	{
		/// <summary>
		/// 标识
		/// </summary>
		[PrimaryKey]
		public string GUID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string SysSource { get; set; }
		/// <summary>
		/// 二次提交业务数据存储字段
		/// </summary>
		public string JsonNewData { get; set; }
		/// <summary>
		/// 业务数据 --- 初始
		/// </summary>
		public string JsonData { get; set; }
		/// <summary>
		/// 第三方标识
		/// </summary>
		public string JsonKey { get; set; }
		/// <summary>
		/// 提交状态
		/// </summary>
		public EnumTaskStatus Status { get; set; }
		/// <summary>
		/// 运行状态
		/// </summary>
		public EnumLanchStatus LaunchStatus { get; set; }
		/// <summary>
		/// 流程名
		/// </summary>
		public string ProcessName { get; set; }
		/// <summary>
		/// 流程实例号
		/// </summary>
		public string ProcInstId { get; set; }
		/// <summary>
		/// 申请单号
		/// </summary>
		public string ProcApplyNo { get; set; }
		/// <summary>
		/// 流程数据
		/// </summary>
		public string ProcessData { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		public string CreateTime { get; set; }
		/// <summary>
		/// 是否提交
		/// </summary>
		public string IsSubmit { get; set; }
		/// <summary>
		/// 创建人用户
		/// </summary>
		public string CreateUserID { get; set; }
		/// <summary>
		/// 最后更新时间
		/// </summary>
		public string LastUpdateTime { get; set; }
		/// <summary>
		/// 是否报错
		/// </summary>
		public string IsError { get; set; }

	}
}
