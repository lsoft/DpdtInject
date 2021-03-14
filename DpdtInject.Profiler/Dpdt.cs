using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.IO;

namespace DpdtInject.Profiler
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [MemoryDiagnoser]
    [GcServer(true)]
    //[EtwProfiler]
    public class Dpdt
    {
        private string _combinedBody;

        [GlobalSetup]
        public void Setup()
        {
            var pathToSolution =
                //Path.GetFullPath("..\\..\\..\\..\\DpdtInject.Tests.Performance\\DpdtInject.Tests.Performance.sln");
                @"C:\projects\github\DpdtInject\DpdtInject.Tests.Performance\DpdtInject.Tests.Performance.sln";

            var pathToSubjectFilePath =
                Path.Combine(
                    new FileInfo(pathToSolution).Directory.FullName,
                    "TimeConsume\\BigTree0\\Singleton\\Subject.cs"
                    );
            var subjectBody = File.ReadAllText(pathToSubjectFilePath);

            var pathToClusterFilePath =
                Path.Combine(
                    new FileInfo(pathToSolution).Directory.FullName,
                    "TimeConsume\\BigTree0\\Singleton\\DpdtCluster.cs"
                    );

            var clusterBody = File.ReadAllText(pathToClusterFilePath);

            _combinedBody = "#define SUPPRESS_RESOLUTION" + Environment.NewLine + clusterBody + Environment.NewLine + subjectBody;
        }

        [Benchmark]
        public void GenerateTest()
        {
            var preparation = new Compilator(
                _combinedBody
                );

            preparation.Check();
        }

    }

}
