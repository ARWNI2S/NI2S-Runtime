using ARWNI2S.Lifecycle;
using ARWNI2S.Runtime.Lifecycle;
using System.Collections.ObjectModel;

namespace ARWNI2S.Runtime.Engine
{
    public abstract class WorkingQueue<TInput, TOutput> : ILifecycleParticipant<IEngineLifecycleSubject>
    {
        public virtual Name Name { get; } = Name.None;

        private readonly Thread _workerThread;
        private CancellationTokenSource cts;

        private IList<TInput> _pendingItems;
        private IList<TOutput> _readyItems;

        public virtual int Stage { get; }

        private WorkingQueue()
        {
            _workerThread = new Thread(() => EntryPoint(cts.Token));
        }

        private void EntryPoint(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    TInput inputItem;
                    lock (_pendingItems)
                    {
                        if (_pendingItems.Count == 0)
                            Monitor.Wait(_pendingItems);

                        if (!CheckConditions(_pendingItems[0]))
                            continue;

                        inputItem = _pendingItems[0];
                        _pendingItems.RemoveAt(0);
                    }

                    TOutput result = ProcessItem(inputItem);

                    lock (_readyItems)
                    {
                        _readyItems.Add(result);
                        OnItemAdded(result, _readyItems);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions
                Console.Error.WriteLine($"Error in {nameof(EntryPoint)}: {ex.Message}");
            }
        }

        protected abstract void OnItemAdded(TOutput result, IList<TOutput> readyItems);
        protected abstract bool CheckConditions(TInput input);
        protected abstract TOutput ProcessItem(TInput inputItem);

        public void Participate(IEngineLifecycleSubject lifecycle)
        {
            lifecycle.Subscribe(GetType().Name, Stage, OnStart, OnStop);
        }

        private Task OnStop(CancellationToken token)
        {
            cts.Cancel();
            _workerThread.Join();
            return Task.CompletedTask;
        }

        private Task OnStart(CancellationToken token)
        {

            cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _workerThread.Start();

            return Task.CompletedTask;
        }
    }

    public abstract class WorkingQueue<T> : WorkingQueue<T, T> { }
}
