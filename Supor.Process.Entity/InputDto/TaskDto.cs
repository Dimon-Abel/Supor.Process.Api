using System;
using System.Collections.Generic;

namespace Supor.Process.Entity.InputDto
{
    /// <summary>
    /// 任务输入Dto
    /// </summary>
    public class TaskDto
    {
        /// <summary>
        /// 流程名
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 数据来源
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string CreateUserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 流程数据 准备校验
        /// </summary>
        public List<object> ProcessData { get; set; }
    }
}
