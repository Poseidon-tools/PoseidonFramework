namespace Inseminator.Scripts.Installers
{
    using Resolver;

    public interface IInseminatorInstaller
    {
        void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver);
    }
}