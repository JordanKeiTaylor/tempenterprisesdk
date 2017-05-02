using Improbable.Worker;

namespace Improbable.Enterprise.WorkerRunners
{
    public abstract class WorkerRunner {
        protected Connection _connection;
        protected Dispatcher _dispatcher;

        public WorkerRunner(Connection connection, Dispatcher dispatcher) {
            _connection = connection;
            _dispatcher = dispatcher;
        }

        public delegate void WorkMillisCallback(long millis);

        public abstract void Run(WorkMillisCallback cb);
    }
}
