using Dapper;
using ESign.Entity.Table;
using ESign.Services.Interfaces;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ESign.Services
{
    public class AttachmentService: IAttachmentService
    {

        public async Task<List<ProcAttachment>> GetProcAttInfo(string guid)
        {
            List<ProcAttachment> procatt = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                string sql = @"SELECT * FROM PROC_ATTACHMENT WHERE FK_GUID=@guid";
                conn.Open();
                procatt = conn.Query<ProcAttachment>(sql, new { guid = guid }).ToList();
            }
            return await Task.FromResult(procatt);
        }
    }
}
