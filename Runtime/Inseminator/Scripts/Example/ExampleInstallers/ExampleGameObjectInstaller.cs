namespace Inseminator.Scripts.Example.ExampleInstallers
{
    using Core.ViewManager;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleGameObjectInstaller : InseminatorInstaller
    {
        #region Installer Injection Test
        [InseminatorAttributes.Inseminate] private ViewManager sceneViewManager;
        #endregion
        #region Inspector
        [SerializeField] private MessageData sampleMessage;
        [SerializeField] private MessageData secondaryMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            inseminatorDependencyResolver.Bind<MessageData>(sampleMessage, "SampleMessage");
            inseminatorDependencyResolver.Bind<MessageData>(secondaryMessage, "SecondaryMessage");
            
            sceneViewManager = ResolveInParent<ViewManager>(inseminatorDependencyResolver.Parent);
            inseminatorDependencyResolver.Bind<ViewManager>(sceneViewManager);
            
            Debug.Log($"Is VM injected: {sceneViewManager != null}", sceneViewManager);
        }
        #endregion

    }
}