using System.Collections.Generic;

namespace EnterpriseSDK.Metrics {
    public interface IMetricProvider {
        Dictionary<string, double> SampleMetrics(bool reset);
    }
}