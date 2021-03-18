using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DpdtInject.Generator.Core;
using DpdtInject.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DpdtInject.Profiler
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [MemoryDiagnoser]
    [GcServer(true)]
    //[EtwProfiler]
    public class Dpdt
    {
        private ProfilerTypeInfoContainer _typeInfoContainer;

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

            var clusterSource = "#define SUPPRESS_RESOLUTION" + Environment.NewLine + clusterBody + Environment.NewLine + subjectBody;

            var clusterSourceText = SourceText.From(clusterSource, Encoding.UTF8);
            var clusterSyntaxTree = SyntaxFactory.ParseSyntaxTree(clusterSourceText, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest), "");

            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            var references = trustedAssembliesPaths
                .Where(path => !IsSkippedAssembly(path))
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

            references.Add(
                MetadataReference.CreateFromFile(
                    Path.GetFullPath("DpdtInject.Injector.dll")
                    )
                );

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithOptimizationLevel(OptimizationLevel.Debug)
                ;

            var compilation = CSharpCompilation.Create(
                Guid.NewGuid() + ".dll",
                new SyntaxTree[] { clusterSyntaxTree }
              , references
              , compilationOptions
                );

            _typeInfoContainer = new ProfilerTypeInfoContainer(
                compilation
                );
        }

        [Benchmark]
        public void GenerateTest()
        {
            var diagnosticReporter = new FakeDiagnosticReporter();

            var internalGenerator = new DpdtInternalGenerator(
                diagnosticReporter,
                true
                );

            internalGenerator.Execute(
                _typeInfoContainer
                );
        }




        public static bool IsSkippedAssembly(
            string assemblyPath
            )
        {
            if (assemblyPath is null)
            {
                throw new ArgumentNullException(nameof(assemblyPath));
            }

            if (assemblyPath.Contains("DpdtInject.Generator.dll"))
            {
                return true;
            }

            if (assemblyPath.Contains("DpdtInject.TestConsole.exe"))
            {
                return true;
            }

            if (assemblyPath.Contains("DpdtInject.Tests.dll"))
            {
                return true;
            }

            return false;
        }

    }

}
