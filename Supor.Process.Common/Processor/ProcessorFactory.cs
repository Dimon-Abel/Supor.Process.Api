using Supor.Process.Entity.InputDto;
using System.Collections.Generic;
using System.Linq;

namespace Supor.Process.Common.Processor
{
    public class ProcessorFactory : IProcessorFactory
    {
        private IEnumerable<IProcessor> _processors;

        public ProcessorFactory(IEnumerable<IProcessor> processors)
        {
            _processors = processors;
        }

        public IProcessor GetProcessor(string type)
        {
            return _processors.FirstOrDefault(x => x.GetTag() == type);
        }
    }
}
