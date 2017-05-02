using System;
using Improbable.Worker;

namespace WorkerAPIFacade {
    public class WorkerFlagUtil {
        private Connection _connection;
        public WorkerFlagUtil(Connection connection) {
            _connection = connection;
        }

        public bool TryGetFlag<T>(string flagName, out T value) {
            lock (_connection) {
                var flagVal = _connection.GetWorkerFlag(flagName);
                if (flagVal.HasValue) {
                    value = (T)Convert.ChangeType(flagVal.Value, typeof(T));
                }
                else {
                    value = default(T);
                }
                return flagVal.HasValue;
            }
        }
    }
}