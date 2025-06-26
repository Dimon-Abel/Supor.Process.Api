using NLog;
using Supor.Process.Entity.Entity;
using Supor.Process.Entity.Enums;
using Supor.Process.Services.Dapper;
using Supor.Process.Services.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supor.Process.Services.Services
{
    /// <summary>
    /// 任务服务
    /// </summary>
    public partial class TaskService : ITaskService
    {
        private readonly ILogger _logger;

        private readonly IDapperExecutor _dapperExecutor;

        private readonly II_OSYS_PROC_INSTSRepository _i_OSYS_PROC_INSTSRepository;

        private readonly II_OSYS_PROCDATA_ITEMSRepository _i_OSYS_PROCDATA_ITEMSRepository;

        private readonly II_OSYS_WF_WORKITEMSRepository _i_OSYS_WF_WORKITEMSRepository;

        public TaskService(ILogger logger, IDapperExecutor dapperExecutor)
        {
            _logger = logger;
            _dapperExecutor = dapperExecutor;
        }

        public async Task<bool> AddOrUpdateTaskAsync(IEnumerable<(I_OSYS_PROCDATA_ITEMS, I_OSYS_PROC_INSTS, I_OSYS_WF_WORKITEMS)> data)
        {
            return await _dapperExecutor.ExcuteTranAsync((conn, tran) =>
            {
                foreach (var item in data)
                {
                    if (item.Item1.Status == EnumTaskStatus.Submit)
                    {
                        _i_OSYS_PROCDATA_ITEMSRepository.InsertTranAsync(conn, tran, item.Item1);
                        _i_OSYS_PROC_INSTSRepository.InsertTranAsync(conn, tran, item.Item2);
                        _i_OSYS_WF_WORKITEMSRepository.InsertTranAsync(conn, tran, item.Item3);
                    }
                    else
                    {
                        _i_OSYS_PROCDATA_ITEMSRepository.UpdateTranAsync(conn, tran, item.Item1);
                        _i_OSYS_PROC_INSTSRepository.UpdateTranAsync(conn, tran, item.Item2);
                        _i_OSYS_WF_WORKITEMSRepository.UpdateTranAsync(conn, tran, item.Item3);
                    }
                }
            });
        }
    }
}
