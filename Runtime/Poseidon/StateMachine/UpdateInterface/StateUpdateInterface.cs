namespace Poseidon.StateMachine.UpdateInterface
{
    public interface IEarlyUpdate { void OnEarlyUpdate(); }  
    public interface IFixedUpdate { void OnFixedUpdate(); }
    public interface IPreUpdate { void OnPreUpdate(); }
    public interface IUpdate { void OnUpdate(); }
    public interface IPreLateUpdate { void OnPreLateUpdate(); }
    public interface IPostLateUpdate { void OnPostLateUpdate(); }
}
