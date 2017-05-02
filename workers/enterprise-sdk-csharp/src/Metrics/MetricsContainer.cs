using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseSDK.Metrics
{
    public class MetricsContainer : IMetricCollector, ILoadCollector,
                                    IMetricProvider, ILoadProvider {
        private readonly ConcurrentDictionary<string, double> _numericalMetrics;
        private double _load = 0.0;

        public MetricsContainer() {
            _numericalMetrics = new ConcurrentDictionary<string, double>();
        }

        public void RegisterMetric(string name, double value, AggregationMethod aggMethod) {
            var fullName = "Worker:" + name;
            _numericalMetrics.AddOrUpdate(fullName, value, (key, existingValue) => aggMethod.Apply(value, existingValue));
        }

        public void RegisterLoad(double load) {
            _load = load;
        }

        public Dictionary<string, double> SampleMetrics(bool reset) {
            var metrics = new Dictionary<string, double>(_numericalMetrics);
            if (reset) {
                foreach (var key in _numericalMetrics.Keys.ToList()) {
                    _numericalMetrics[key] = 0.0;
                }
            }
            return metrics;
        }

        public double SampleLoad(bool reset) {
            var load = _load;
            if (reset) {
                _load = 0.0;
            }
            return load;
        }
    }

    static class AggregationMethods
    {
        public static double Apply(this AggregationMethod aggMethod, double value, double prevValue) {
            switch (aggMethod) {
                case AggregationMethod.Sum: {
                        return prevValue + value;
                    }
                case AggregationMethod.Last:
                default: {
                        return value;
                    }
            }
        }
    }
}
