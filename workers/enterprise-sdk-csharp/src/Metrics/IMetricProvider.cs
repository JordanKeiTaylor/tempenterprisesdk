using System.Collections.Generic;

namespace WorkerAPI.Metrics {
    public interface IMetricProvider {
        Dictionary<string, double> SampleMetrics(bool reset);
    }
}