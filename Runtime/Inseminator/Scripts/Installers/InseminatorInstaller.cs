namespace Inseminator.Scripts.Installers
{
    using JetBrains.Annotations;
    using Resolver;
    using UnityEngine;

    public abstract class InseminatorInstaller : MonoBehaviour, IInseminatorInstaller
    {
        #region Installer API
        [UsedImplicitly]
        public abstract void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver);
        protected T ResolveInParent<T>(InseminatorDependencyResolver parentResolver, string objectID = "")
        {
            var searchedObject =
                (T)InseminatorHelpers.TryResolveInParentHierarchy<T>(parentResolver, objectID);
            if (searchedObject != null) return searchedObject;
            Debug.LogError("Can't find matching ViewManager in any of parent resolvers");
            return default;
        }
        #endregion
    }
}