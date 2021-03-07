using DpdtInject.Generator;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DpdtInject.Tests
{
    internal class Preparator
    {
        private readonly TestContext _testContext;
        private readonly string _testerClassName;
        private readonly string _clusterFileName;
        private readonly string _clusterSource;
        private readonly string _callerFilePath;

        internal FakeDiagnosticReporter DiagnosticReporter
        {
            get;
        }

        public Preparator(
            TestContext testContext,
            string testerClassName,
            string clusterFileName,
            string clusterSource,
            [CallerFilePath] string callerFilePath = ""
            )
        {
            if (testContext is null)
            {
                throw new ArgumentNullException(nameof(testContext));
            }

            if (string.IsNullOrEmpty(testerClassName))
            {
                throw new ArgumentException($"'{nameof(testerClassName)}' cannot be null or empty", nameof(testerClassName));
            }

            if (string.IsNullOrEmpty(clusterFileName))
            {
                throw new ArgumentException($"'{nameof(clusterFileName)}' cannot be null or empty", nameof(clusterFileName));
            }

            if (string.IsNullOrEmpty(clusterSource))
            {
                throw new ArgumentException($"'{nameof(clusterSource)}' cannot be null or empty", nameof(clusterSource));
            }

            var modifiedClusterSource = Regex.Replace(clusterSource, @"FakeCluster<(?i:([a-zA-z\d]+))>", "$1");
            modifiedClusterSource = Regex.Replace(modifiedClusterSource, @"IFakeFactory<(?i:([a-zA-z\d]+))>", "$1");
            modifiedClusterSource = Regex.Replace(modifiedClusterSource, @"IFakeProxy<(?i:([a-zA-z\d]+))>", "$1");

            _testContext = testContext;
            _testerClassName = testerClassName;
            _clusterFileName = clusterFileName;
            _clusterSource = 
                "#define IN_UNIT_TEST_SYMBOL" 
                + Environment.NewLine 
                + modifiedClusterSource
                ;
            _callerFilePath = callerFilePath;

            DiagnosticReporter = new FakeDiagnosticReporter();
        }

        public void Check()
        {
            try
            {
                var clusterSourceText = SourceText.From(_clusterSource, Encoding.UTF8);
                var clusterSyntaxTree = SyntaxFactory.ParseSyntaxTree(clusterSourceText, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest), "");

                var trustedAssembliesPaths = ((string) AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
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

                var typeInfoContainer = new PreparatorTypeInfoContainer(
                    compilation,
                    true,
                    new FileInfo(_callerFilePath).Directory.FullName
                    );

                var internalGenerator = new DpdtInternalGenerator(
                    DiagnosticReporter
                    );

                internalGenerator.Execute(
                    typeInfoContainer
                    );

                var compiledDllPath = Path.Combine(
                    _testContext.TestResultsDirectory,
                    Guid.NewGuid() + ".dll"
                    );

                using (new DTimer(DiagnosticReporter, "Dpdt unit test emit time taken"))
                {
                    var emitResult = typeInfoContainer.Emit(compiledDllPath);

                    Assert.IsTrue(emitResult.Success, string.Join(Environment.NewLine, emitResult.Diagnostics));
                }

                TestAssemblyLoadContext talContext = null;
                try
                {
                    talContext = new TestAssemblyLoadContext();

                    var compiledAssembly = talContext.LoadFromAssemblyPath(compiledDllPath);

                    var testerType = compiledAssembly.GetTypes().FirstOrDefault(t => t.Name == _testerClassName);

                    if (testerType == null)
                    {
                        throw new InvalidOperationException(
                            $"Type {_testerClassName} does not found"
                            );
                    }

                    var tester = Activator.CreateInstance(testerType);

                    const string MethodName = "PerformClusterTesting";

                    var method = testerType.GetMethod(
                        MethodName,
                        BindingFlags.Public | BindingFlags.Instance
                        );

                    if (method is null)
                    {
                        Assert.Fail($"{MethodName} does not found.");
                    }

                    method.Invoke(tester, null);
                }
                finally
                {
                    talContext?.Unload();
                }
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
