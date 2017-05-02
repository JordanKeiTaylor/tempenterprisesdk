using System.Collections.Generic;

namespace Improbable.Enterprise.Metrics {
    public interface IMetricProvider {
        Dictionary<string, double> SampleMetrics(bool reset);
    }
}