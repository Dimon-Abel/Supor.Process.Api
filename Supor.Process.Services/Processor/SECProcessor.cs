using Client.Library.API;
using NLog;
using Supor.Process.Entity.Entity;
using Supor.Process.Entity.InputDto;
using Supor.Process.Services.Repositories;
using Supor.Process.Services.Services;

namespace Supor.Process.Services.Processor
{
    public class SECProcessor : BaseProcessor
    {
        public SECProcessor(ILogger logger, II_OSYS_PROCDATA_ITEMSRepository i_OSYS_PROCDATA_ITEMSRepository,
            II_OSYS_PROC_INSTSRepository i_OSYS_PROC_INSTSRepository,
            II_OSYS_WF_WORKITEMSRepository i_OSYS_WF_WORKITEMSRepository,
            IPerInfoService perInfoService, IProcessItemsService processItemsService, IOrgInfoService orgInfoService)
            : base(logger, i_OSYS_PROCDATA_ITEMSRepository, i_OSYS_PROC_INSTSRepository, i_OSYS_WF_WORKITEMSRepository,
                  perInfoService, processItemsService, orgInfoService)
        {
        }

        public override string GetTag()
        {
            return "SEC";
        }

        public override bool BusDataToDB(I_OSYS_PROCDATA_ITEMS task, ProcessDataDto dto, object processData, TaskEntity te)
        {
            return true;
        }
    }
}
