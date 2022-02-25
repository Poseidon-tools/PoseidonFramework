namespace Inseminator.Scripts.Example.ExampleInstallers
{
    using Core.ViewManager;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleSceneInstaller : InseminatorInstaller
    {
        #region Inspector
        [SerializeField] private ViewManager sceneViewManager;
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            inseminatorDependencyResolver.Bind<ITextLogger>(new TestLogger(), "TestLogger");
            inseminatorDependencyResolver.Bind<ITextLogger>(new GreenTextLogger(), "GreenTextLogger");
            inseminatorDependencyResolver.Bind<ITextLogger>(new CustomLogger(Color.red, 60), "CustomLoggerRed60");
            
            inseminatorDependencyResolver.Bind<ViewManager>(sceneViewManager);
            
            inseminatorDependencyResolver.Bind<MessageData>(sampleMessage);
            inseminatorDependencyResolver.Bind("007-ABCD", "AccessCode");
        }
        #endregion
    }
}