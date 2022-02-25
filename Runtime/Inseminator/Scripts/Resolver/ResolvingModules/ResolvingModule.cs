namespace Inseminator.Scripts.Resolver.ResolvingModules
{
    using UnityEngine;

    public abstract class ResolvingModule : MonoBehaviour
    {
        public abstract void Run(InseminatorDependencyResolver dependencyResolver, object sourceObject);
    }
}