
namespace Lacalizer.Mobile.Navigation;

public class NativeBackButtonService
{
    public static NativeBackButtonService Instance { get; } = new();

    private Func<Task<bool>>? _backHandler;

    public async Task<bool> HandleBackRequestedAsync()
    {
        if (_backHandler != null)
            return await _backHandler.Invoke();

        return false;
    }

    public void RegisterBackHandler(Func<Task<bool>> handler)
    {
        _backHandler = handler;
    }

    public void UnregisterBackHandler()
    {
        _backHandler = null;
    }
}

