namespace Inseminator.Scripts.DependencyResolvers.GameObject
{
    using System.Collections.Generic;
    using Data;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class GameObjectDependencyResolver : InseminatorDependencyResolver
    {
        #region Resolving
        protected override void GetTargetObjects()
        {
            List<GameObject> childrenList = new List<GameObject>();
            GetChildren(gameObject, childrenList);
            var components = InseminatorHelpers.GetAllComponents(childrenList);
            foreach (var component in components)
            {
                var instance = (object)component;
                ResolveDependencies(ref instance);
            }
        }

        protected override void Install(List<InseminatorInstaller> installers)
        {
            base.Install(installers);
            
            registeredDependencies.Add(typeof(InseminatorDependencyResolver), new List<InstallerEntity>
            {
                new InstallerEntity
                {
                    Id = "",
                    ObjectInstance = this
                }
            });
        }

        private void GetChildren(GameObject parentObject, List<GameObject> outputList)
        {
            int childCount = parentObject.transform.childCount;
            outputList.Add(parentObject);
            for (int i = 0; i < childCount; i++)
            {
                GetChildren(parentObject.transform.GetChild(i).gameObject, outputList);
            }
        }
        #endregion
    }
}