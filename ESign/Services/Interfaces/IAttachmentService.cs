using ESign.Entity.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESign.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<List<ProcAttachment>> GetProcAttInfo(string guid);
    }
}
