using Client.Library.API;
using Supor.Process.Entity.Entity;
using Supor.Process.Entity.InputDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supor.Process.Common.Processor
{
    public class SECProcessor: BaseProcessor
    {
        public override string GetTag()
        {
            return "SEC";
        }

        public override bool BusDataToDB(ApiTaskEntity task, ProcessDataDto dto, object processData, TaskEntity te)
        {
            return true;
        }
    }
}
