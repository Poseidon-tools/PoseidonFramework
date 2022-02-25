namespace Inseminator.Scripts
{
    using System.Collections.Generic;
    using DependencyResolvers.Scene;
    using ReflectionBaking;
    using Resolver;
    using UnityEngine;

    [DefaultExecutionOrder(-50)]
    public class InseminatorManager : MonoBehaviour
    {
        #region Private Methods
        private void Awake()
        {
            ReflectionBaker.Instance.Initialize();
            
            ResolveTree(BuildResolversTree());
        }

        private void ResolveTree(ResolverTreeNode node)
        {
            node.Resolver.InitializeResolver(node.Parent?.Resolver);
            foreach (var childNode in node.ChildNodes)
            {
                ResolveTree(childNode);
            }
        }
        #endregion
        
        #region Collecting resolvers
        private class ResolverTreeNode
        {
            public ResolverTreeNode Parent;
            public InseminatorDependencyResolver Resolver;
            public List<ResolverTreeNode> ChildNodes = new List<ResolverTreeNode>();
        }
        private ResolverTreeNode BuildResolversTree()
        {
            var mainNode = new ResolverTreeNode()
            {
                Resolver = FindObjectOfType<SceneDependencyResolver>(),
            };
            // get scene objects
            var sceneObjects = InseminatorHelpers.GetRootSceneObjects(gameObject.scene);
            foreach (var sceneObject in sceneObjects)
            {
                SearchForResolver(sceneObject.transform, mainNode);
            }
            return mainNode;
        }

        private ResolverTreeNode AddNode(ResolverTreeNode parentNode, InseminatorDependencyResolver currentResolver)
        {
            var node = new ResolverTreeNode()
            {
                Resolver = currentResolver,
                ChildNodes = new List<ResolverTreeNode>(),
                Parent =  parentNode
            };
            parentNode.ChildNodes.Add(node);
            //Debug.Log($"Added {node.Resolver.name} into {parentNode.Resolver.name} child list.");
            return node;
        }

        private void SearchForResolver(Transform target, ResolverTreeNode parentNode)
        {
            //Debug.Log($"Searching in {target.name}, parent: {parentNode.Resolver.name}");
            var childCount = target.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = target.GetChild(i);
                var resolver = child.GetComponent<InseminatorDependencyResolver>();
                if (resolver is SceneDependencyResolver)
                {
                    continue;
                }
                if (resolver == null)
                {
                    SearchForResolver(child, parentNode);
                    continue;
                }
                var newParentNode = AddNode(parentNode, resolver);
                SearchForResolver(child, newParentNode);
            }
        }
        #endregion
    }
}