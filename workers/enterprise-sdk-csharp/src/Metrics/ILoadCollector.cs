namespace Improbable.Enterprise.Metrics {
    public interface ILoadCollector {
        void RegisterLoad(double load);
    }
}