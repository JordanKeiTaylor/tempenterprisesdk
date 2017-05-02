namespace WorkerAPI.Metrics {
    public interface ILoadProvider {
        double SampleLoad(bool reset);
    }
}