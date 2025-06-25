using Supor.Process.Entity.Attributies;
using System;

namespace Supor.Process.Entity.Entity
{
    /// <summary>
    /// 流程表
    /// </summary>
    public class I_OSYS_WF_WORKITEMS
    {
        [PrimaryKey]
        public string WORK_ITEM_ID { get; set; }
        public string PROC_INST_ID { get; set; }
        public string PROC_APPLYNO { get; set; }
        public string DEF_NAME { get; set; }
        public string STEPID { get; set; }
        public string STEPNAME { get; set; }
        public string SOURCE_WORK_ITEM_ID { get; set; }
        public DateTime? ASSIGNED_DATE { get; set; }
        public DateTime? SIGN_DATE { get; set; }
        public string WORKITEMSTATUS { get; set; }
        public string HANDLINGOPINION { get; set; }
        public string HANDLINGREMARKS { get; set; }
        public string HANDLER_AD { get; set; }
        public string HANDLER_NAME { get; set; }
        public string HANDLER_PERCODE { get; set; }
        public string HANDLER_PEREMAIL { get; set; }
        public string HANDLER_ORGPATH { get; set; }
        public string HANDLER_ORGCODE { get; set; }
        public string HANDLER_ORGNAME { get; set; }
        public string HANDLER_DEPTCODE { get; set; }
        public string HANDLER_DEPTNAME { get; set; }
        public string HANDLER_RESCENTERCODE { get; set; }
        public string HANDLER_RESCENTERNAME { get; set; }
        public string HANDLER_BUCODE { get; set; }
        public string HANDLER_BUNAME { get; set; }
        public string PCUrl { get; set; }
        public string mobileUrl { get; set; }
        public string FromSysName { get; set; }
    }
}
