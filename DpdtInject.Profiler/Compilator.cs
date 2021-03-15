using DpdtInject.Generator;
using DpdtInject.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DpdtInject.Profiler
{
    public class Compilator
    {
        private readonly string _clusterSource;
        private readonly string _callerFilePath;

        internal FakeDiagnosticReporter DiagnosticReporter
        {
            get;
        }

        public Compilator(
            string clusterSource,
            [CallerFilePath] string callerFilePath = ""
            )
        {
            if (string.IsNullOrEmpty(clusterSource))
            {
                throw new ArgumentException($"'{nameof(clusterSource)}' cannot be null or empty", nameof(clusterSource));
            }

            _clusterSource = clusterSource;
            _callerFilePath = callerFilePath;

            DiagnosticReporter = new FakeDiagnosticReporter();
        }

        public void Check()
        {
            try
            {
                var clusterSourceText = SourceText.From(_clusterSource, Encoding.UTF8);
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

                var typeInfoContainer = new ProfilerTypeInfoContainer(
                    compilation
                    );

                var internalGenerator = new DpdtInternalGenerator(
                    DiagnosticReporter,
                    true
                    );

                internalGenerator.Execute(
                    typeInfoContainer
                    );

                //var compiledDllPath = Path.Combine(
                //    _testResultsDirectory,
                //    Guid.NewGuid() + ".dll"
                //    );

                //using (new DTimer(DiagnosticReporter, "Dpdt unit test emit time taken"))
                //{
                //    var emitResult = typeInfoContainer.Emit(compiledDllPath);

                //    Assert.IsTrue(emitResult.Success, string.Join(Environment.NewLine, emitResult.Diagnostics));
                //}

                //TestAssemblyLoadContext talContext = null;
                //try
                //{
                //    talContext = new TestAssemblyLoadContext();

                //    var compiledAssembly = talContext.LoadFromAssemblyPath(compiledDllPath);

                //    var testerType = compiledAssembly.GetTypes().FirstOrDefault(t => t.Name == _testerClassName);

                //    if (testerType == null)
                //    {
                //        throw new InvalidOperationException(
                //            $"Type {_testerClassName} does not found"
                //            );
                //    }

                //    var tester = Activator.CreateInstance(testerType);

                //    const string MethodName = "PerformClusterTesting";

                //    var method = testerType.GetMethod(
                //        MethodName,
                //        BindingFlags.Public | BindingFlags.Instance
                //        );

                //    if (method is null)
                //    {
                //        Assert.Fail($"{MethodName} does not found.");
                //    }

                //    method.Invoke(tester, null);
                //}
                //finally
                //{
                //    talContext?.Unload();
                //}
            }
            catch (Exception excp)
            {
                this.DiagnosticReporter.ReportException(
                    excp
                    );
            }
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
