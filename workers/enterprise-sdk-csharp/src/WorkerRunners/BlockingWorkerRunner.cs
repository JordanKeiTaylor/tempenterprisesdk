using System.Diagnostics;
using Improbable.Worker;

namespace WorkerAPIFacade.WorkerRunners
{
    public class BlockingWorkerRunner : WorkerRunner
    {
        private readonly uint _opListTimeoutMillis = 0;

        public BlockingWorkerRunner(uint opListTimeoutMillis, Connection connection, Dispatcher dispatcher)
            : base(connection, dispatcher)
        {
            _opListTimeoutMillis = opListTimeoutMillis;
        }

        public override void Run(WorkMillisCallback cb)
        {
            var running = true;
            lock (_dispatcher)
            {
                _dispatcher.OnDisconnect(op => {
                    running = false;
                });
            }

            var sw = new Stopwatch();
            sw.Start();
            while (running)
            {
                var opList = FetchOpList(_opListTimeoutMillis);
                sw.Restart();
                ProcessOpList(opList);
                cb(sw.ElapsedMilliseconds);
            }
        }

        protected virtual OpList FetchOpList(uint timeoutMillis)
        {
            OpList opList;
            lock (_connection)
            {
                opList = _connection.GetOpList(timeoutMillis);
            }
            return opList;
        }

        protected virtual void ProcessOpList(OpList opList)
        {
            lock (_dispatcher)
            {
                _dispatcher.Process(opList);
            }
            opList.Dispose();
        }
    }
}
