using System;
using Improbable.Worker;

namespace WorkerAPIFacade {
    public class Logging {

        private static Connection _connection = null;
        private static string _workerId;
        private static LogLevel _passLevel = LogLevel.Debug;

        public static void Init(Connection connection, string workerId) {
            _connection = connection;
            _workerId = workerId;
        }

        public static void setPassLevel(LogLevel lvl) {
            _passLevel = lvl;
        }

        private static void SendLog(LogLevel lvl, string message) {
            if (lvl < _passLevel) {
                return;
            }
            if (_connection != null) {
                lock (_connection) {
                    _connection.SendLogMessage(lvl, _workerId, message);
                }
            }
            else {
                Console.Error.WriteLine(lvl.ToString() + ": " + message);
            }
        }
        public static void Error(string message)
        {
            SendLog(LogLevel.Error, message);
        }
        public static void Warn(string message)
        {
            SendLog(LogLevel.Warn, message);
        }
        public static void Info(string message) {
            SendLog(LogLevel.Info, message);
        }
        public static void Debug(string message)
        {
            SendLog(LogLevel.Debug, message);
        }


    }
}