namespace Inseminator.Scripts.Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Installers;
    using ResolvingModules;
    using UnityEngine;

    public abstract class InseminatorDependencyResolver : MonoBehaviour
    {
        #region Public Variables
        public Dictionary<Type, List<InstallerEntity>> RegisteredDependencies => registeredDependencies;
        public InseminatorDependencyResolver Parent { get; private set; }
        #endregion
        #region Private Variables
        protected Dictionary<Type, List<InstallerEntity>> registeredDependencies = new Dictionary<Type, List<InstallerEntity>>();
        #endregion
        #region Inspector
        [SerializeField, Header("Declared Installers")]
        protected List<InseminatorInstaller> declaredInstallers = new List<InseminatorInstaller>();

        [SerializeField, Header("Modules")]
        protected List<ResolvingModule> resolvingModules = new List<ResolvingModule>();
        #endregion

        #region Public API
        public virtual void InitializeResolver(InseminatorDependencyResolver parent = null)
        {
            Parent = parent;
            OnBeforeInstall();
            Install(declaredInstallers);
            OnAfterInstall();
            
            OnBeforeGetObjects();
            GetTargetObjects();
            OnAfterGetObjects();
        }
        
        public virtual void ResolveExternalGameObject(ref GameObject externalInstance)
        {
            var components = InseminatorHelpers.GetAllComponents(new List<GameObject>() {externalInstance});
            foreach (var externalComponent in components)
            {
                var instance = (object)externalComponent;
                ResolveDependencies(ref instance);
            }
        }

        public void Bind<T>(T objectInstance, string instanceId = "")
        {
            InstallDependency(typeof(T), new InstallerEntity
            {
                Id = instanceId,
                ObjectInstance = objectInstance
            });
        }
        
        public object GetValueForType(Type targetType, object instanceId = null)
        {
            if (!registeredDependencies.TryGetValue(targetType, out var dependency))
            {
                Debug.LogError($"Cannot get dependency instance for {targetType.Name} | {targetType}");
                return default;
            }
            if (instanceId == null)
            {
                return dependency[0].ObjectInstance;
            }

            var matchingInstance = dependency.FirstOrDefault(instance => instance.Id.Equals(instanceId));
            return matchingInstance?.ObjectInstance;
        }
        #endregion


        #region Lifecycle
        public virtual void OnBeforeInstall(){ }
        public virtual void OnAfterInstall(){ }
        public virtual void OnBeforeGetObjects(){ }
        public virtual void OnAfterGetObjects(){ }
        #endregion
        
        
        #region Resolving Core
        protected abstract void GetTargetObjects();
        public virtual void ResolveDependencies(ref object instanceObject)
        {
            foreach (var resolvingModule in resolvingModules)
            {
                resolvingModule.Run(this, instanceObject);
            }
        }
        #endregion
        
        #region Installing
        protected virtual void InstallDependency(Type targetType, InstallerEntity installerEntity)
        {
            if (registeredDependencies.TryGetValue(targetType, out var entry))
            {
                entry.Add(installerEntity);
                return;
            }
            registeredDependencies.Add(targetType, new List<InstallerEntity> {installerEntity});
        }

        protected virtual void Install(List<InseminatorInstaller> installers)
        {
            foreach (var installer in installers)
            {
                installer.InstallBindings(this);
            }
        }
        #endregion
       
    }
}