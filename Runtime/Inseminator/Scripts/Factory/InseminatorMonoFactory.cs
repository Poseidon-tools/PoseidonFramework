namespace Inseminator.Scripts.Factory
{
    using DependencyResolvers.GameObject;
    using Resolver;
    using UnityEngine;

    public class InseminatorMonoFactory
    {
        #region Public Variables
        public InseminatorDependencyResolver AssignedResolver { get; private set; }
        #endregion
        #region Public API
        public void AssignResolver(InseminatorDependencyResolver resolver) => AssignedResolver = resolver;
        public virtual T Create<T>(T templateObject, Transform parent = null) where T : Component
        {
            templateObject.gameObject.SetActive(false);
            
            var objectInstance = Object.Instantiate(templateObject, parent);
            var instanceGameObject = objectInstance.gameObject;

            var gameObjectResolver = CheckForGameObjectContext(instanceGameObject);
            if (gameObjectResolver != null)
            {
                gameObjectResolver.InitializeResolver(AssignedResolver);
                objectInstance.gameObject.SetActive(true);
                return objectInstance;
            }
            
            AssignedResolver.ResolveExternalGameObject(ref instanceGameObject);
            
            templateObject.gameObject.SetActive(true);
            objectInstance.gameObject.SetActive(true);
            
            return objectInstance;
        }
        #endregion

        #region Private Methods
        private GameObjectDependencyResolver CheckForGameObjectContext(GameObject sourceObject)
        {
            return sourceObject.GetComponent<GameObjectDependencyResolver>();
        }
        #endregion
    }
}