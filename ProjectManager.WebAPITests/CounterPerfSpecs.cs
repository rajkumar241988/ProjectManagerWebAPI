using NBench;

namespace ProjectManager.WebAPITests
{

    public class CounterPerfSpecs
    {
        private Counter _counter;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _counter = context.GetCounter("UserCounter");
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 500, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("UserCounter", MustBe.GreaterThan, 10000000.0d)]
        [MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.LessThanOrEqualTo, ByteConstants.ThirtyTwoKb)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 0.0d)]
        public void Benchmark()
        {
            _counter.Increment();
            ProjectControllerTest pro = new ProjectControllerTest();
            pro.GetAllProjectsIntegrationTest();
        }

        [PerfCleanup]
        public void Cleanup()
        {
            // does nothing
        }
    }
}
