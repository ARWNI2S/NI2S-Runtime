using ARWNI2S.Lifecycle;

namespace ARWNI2S.Runtime.Lifecycle
{
    /// <summary>
    /// Observable silo lifecycle and observer.
    /// </summary>
    public interface IEngineLifecycleSubject : IEngineLifecycle, ILifecycleObserver
    {
    }
}
