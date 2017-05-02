namespace EnterpriseSDK.Metrics {
    public interface ILoadCollector {
        void RegisterLoad(double load);
    }
}