package improbable.enterprise.logging;

import improbable.worker.Connection;
import improbable.worker.LogLevel;

public class Logger {

    public static final Logger log = new Logger();

    private Connection connection = null;
    private String workerId = null;

    public void info(String message) {
        sendLogMessage(LogLevel.INFO, message);
    }

    public void warn(String message) {
        sendLogMessage(LogLevel.WARN, message);
    }

    public void error(String message) {
        sendLogMessage(LogLevel.ERROR, message);
    }

    public void error(String message, Exception e) {
        error(message + "\n" + e.toString() + convertStackTrace(e));
    }

    public void error(Exception e) {
        error("Unhandled Exception ", e);
    }

    private String convertStackTrace(Exception e) {
        StringBuilder sb = new StringBuilder("");
        for (StackTraceElement element : e.getStackTrace()) {
            if (!element.isNativeMethod()) {
                sb.append("\n").append(element.getClassName()).append("   ").append(element.getFileName()).append(" : ").append(element.getLineNumber());
            } else {
                sb.append("\n [Native code]");
            }
        }
        return sb.toString();
    }

    public void sendLogMessage(LogLevel level, String message) {
        if (connection == null) {
            System.err.println(level.toString() + "  " + message);
        } else {
            connection.sendLogMessage(level, workerId, message);
        }
    }

    public void connect(Connection connection, String workerId) {
        this.connection = connection;
        this.workerId = workerId;
    }
}
