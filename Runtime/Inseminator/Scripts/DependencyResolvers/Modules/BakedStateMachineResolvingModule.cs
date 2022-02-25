namespace Inseminator.Scripts.DependencyResolvers.Modules
{
    using ReflectionBaking;
    using Resolver;
    using Resolver.ResolvingModules;

    public class BakedStateMachineResolvingModule : ResolvingModule
    {
        #region Private Variables
        private StateMachineResolver stateMachineResolver;
        private InseminatorDependencyResolver dependencyResolver;
        #endregion
        #region Public API
        public override void Run(InseminatorDependencyResolver dependencyResolver, object sourceObject)
        {
            stateMachineResolver = new StateMachineResolver();
            this.dependencyResolver = dependencyResolver;
            stateMachineResolver.ResolveStateMachinesBaked(sourceObject, ResolveWithRefWrapper, ReflectionBaker.Instance.BakingData);
        }
        #endregion
        
        #region Helpers
        private void ResolveWithRefWrapper(object resolvedObject)
        {
            dependencyResolver.ResolveDependencies(ref resolvedObject);
        }
        #endregion
    }
}