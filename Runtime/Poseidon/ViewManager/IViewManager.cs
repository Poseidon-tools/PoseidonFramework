namespace Core.ViewManager
{
    public interface IViewManager
    {
        void Initialize();
        T GetView<T>() where T : View;
        T SwitchView<T>() where T : View;
        View GetCurrentView();
        View GetPreviousView();
    }
}
