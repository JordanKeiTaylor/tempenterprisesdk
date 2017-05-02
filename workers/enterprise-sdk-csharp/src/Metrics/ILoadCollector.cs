namespace WorkerAPI.Metrics {
    public interface ILoadCollector {
        void RegisterLoad(double load);
    }
}