namespace Inseminator.Scripts.Example.ExampleInstallers
{
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleScriptableObjectInstaller : InseminatorInstaller
    {
        #region Inspector
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            inseminatorDependencyResolver.Bind<MessageData>(sampleMessage);
        }
        #endregion
    }
}