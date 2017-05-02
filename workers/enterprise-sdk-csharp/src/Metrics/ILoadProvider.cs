namespace Improbable.Enterprise.Metrics {
    public interface ILoadProvider {
        double SampleLoad(bool reset);
    }
}