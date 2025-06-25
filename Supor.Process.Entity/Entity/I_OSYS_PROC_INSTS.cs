using Supor.Process.Entity.Attributies;
using System;

namespace Supor.Process.Entity.Entity
{
    /// <summary>
    /// 实例表
    /// </summary>
    public class I_OSYS_PROC_INSTS
    {
        [PrimaryKey]
        public string PROC_INST_ID { get; set; }
        public string PROC_APPLYNO { get; set; }
        public string PROC_INST_NAME { get; set; }
        public string DEF_ID { get; set; }
        public string DEF_NAME { get; set; }
        public string PROC_INST_STATUS { get; set; }
        public DateTime? STARTED_DATE { get; set; }
        public DateTime? COMPLETED_DATE { get; set; }
        public string PROC_INITIATOR_AD { get; set; }
        public string PROC_INITIATOR_NAME { get; set; }
        public string PROC_INITIATOR_PERCODE { get; set; }
        public string PROC_INITIATOR_EMAIL { get; set; }
        public string PROC_INITIATOR_ORGPATH { get; set; }
        public string PROC_INITIATOR_ORGCODE { get; set; }
        public string PROC_INITIATOR_ORGNAME { get; set; }
        public string PROC_INITIATOR_DEPTCODE { get; set; }
        public string PROC_INITIATOR_DEPTNAME { get; set; }
        public string PROC_INITIATOR_RESCENTERCODE { get; set; }
        public string PROC_INITIATOR_RESCENTERNAME { get; set; }
        public string PROC_INITIATOR_BUCODE { get; set; }
        public string PROC_INITIATOR_BUNAME { get; set; }
        public string FromSysName { get; set; }
    }
}
