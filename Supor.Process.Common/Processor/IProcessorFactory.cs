namespace Supor.Process.Common.Processor
{
    public interface IProcessorFactory
    {
        IProcessor GetProcessor(string type);
    }
}
