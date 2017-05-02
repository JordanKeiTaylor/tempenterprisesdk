namespace EnterpriseSDK.Metrics {
    public interface ILoadProvider {
        double SampleLoad(bool reset);
    }
}