using Autofac;

namespace Supor.Process.Common
{
    public static class ServiceLocator
    {
        public static IContainer Container { get; private set; }

        public static void SetContainer(IContainer container)
        {
            Container = container;
        }
    }
}
