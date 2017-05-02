namespace WorkerAPI.Metrics {
    public interface IMetricCollector {
        void RegisterMetric(string name, double value, AggregationMethod aggMethod);
    }
}