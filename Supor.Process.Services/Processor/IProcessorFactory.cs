namespace Supor.Process.Services.Processor
{
    public interface IProcessorFactory
    {
        IProcessor GetProcessor(string type);
    }
}
