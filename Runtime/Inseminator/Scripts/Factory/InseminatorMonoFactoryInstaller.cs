namespace Inseminator.Scripts.Factory
{
    using Installers;
    using Resolver;

    public class InseminatorMonoFactoryInstaller : InseminatorInstaller
    {
        #region Public Methods
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            var factory = new InseminatorMonoFactory();
            factory.AssignResolver(inseminatorDependencyResolver);
            inseminatorDependencyResolver.Bind<InseminatorMonoFactory>(factory);
        }
        #endregion
    }
}