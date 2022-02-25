namespace Inseminator.Scripts.ReflectionBaking.BakingModules
{
    using Data.Baking;
    using Resolver.Helpers;

    public abstract class InseminatorBakingModule
    {
        protected MemberInfoExtractor memberInfoExtractor = new MemberInfoExtractor();
        public abstract void Run(ref object sourceObject, ReflectionBakingData bakingData, ReflectionBaker reflectionBaker);
    }
}