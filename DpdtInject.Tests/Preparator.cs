using DpdtInject.Generator;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
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
        private readonly string _moduleFileName;
        private readonly string _moduleSource;
        private readonly string _callerFilePath;
        
        internal FakeDiagnosticReporter DiagnosticReporter
        {
            get;
        }

        public Preparator(
            TestContext testContext,
            string testerClassName,
            string moduleFileName,
            string moduleSource,
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

            if (string.IsNullOrEmpty(moduleFileName))
            {
                throw new ArgumentException($"'{nameof(moduleFileName)}' cannot be null or empty", nameof(moduleFileName));
            }

            if (string.IsNullOrEmpty(moduleSource))
            {
                throw new ArgumentException($"'{nameof(moduleSource)}' cannot be null or empty", nameof(moduleSource));
            }
            _testContext = testContext;
            _testerClassName = testerClassName;
            _moduleFileName = moduleFileName;
            _moduleSource = "#define IN_UNIT_TEST_SYMBOL" + Environment.NewLine + Regex.Replace(moduleSource, @"FakeModule<(?i:([a-zA-z\d]+))>", "$1");
            _callerFilePath = callerFilePath;

            DiagnosticReporter = new FakeDiagnosticReporter();
        }

        public void Check()
        {
            try
            {
                var moduleSourceText = SourceText.From(_moduleSource, Encoding.UTF8);
                var moduleSyntaxTree = SyntaxFactory.ParseSyntaxTree(moduleSourceText, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest), "");

                var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
                var references = trustedAssembliesPaths
                    .Where(path => !IsSkippedAssembly(path))
                    .Select(p => MetadataReference.CreateFromFile(p))
                    .ToList();



                var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true)
                    .WithOptimizationLevel(OptimizationLevel.Debug)
                    ;

                var compilation = CSharpCompilation.Create(
                    Guid.NewGuid() + ".dll",
                    new SyntaxTree[] { moduleSyntaxTree }
                    , references
                    , compilationOptions
                    );

                var generatedSourceFilePath = Path.Combine(
                    new FileInfo(_callerFilePath).Directory.FullName,
                    _moduleFileName
                    ) + ".Pregenerated.cs";
                var internalGenerator = new DpdtInternalGenerator(
                    DiagnosticReporter,
                    generatedSourceFilePath
                    );

                var modificationDescriptions = internalGenerator.Execute(compilation);

                Assert.AreEqual(1, modificationDescriptions.Count, "Allowed only one module per request");

                var modificationDescription = modificationDescriptions[0];

                var generatorSyntaxTree = SyntaxFactory.ParseSyntaxTree(
                    SourceText.From(modificationDescription.NewFileBody),
                    CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest),
                    ""
                    );

                compilation = compilation.AddSyntaxTrees(generatorSyntaxTree);


                var compiledDllPath = Path.Combine(
                    _testContext.TestResultsDirectory,
                    modificationDescription.ModifiedTypeName + ".dll"
                    );

                Microsoft.CodeAnalysis.Emit.EmitResult emitResult;
                using (new DTimer(DiagnosticReporter, "Dpdt unit test actual compilation time taken"))
                {
                    emitResult = compilation.Emit(compiledDllPath);

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
                        throw new DpdtException(
                            DpdtExceptionTypeEnum.InvalidTestConfiguration,
                            $"Type {_testerClassName} does not found",
                            _testerClassName
                            );
                    }

                    var tester = Activator.CreateInstance(testerType);

                    var method = testerType.GetMethod(
                        "PerformModuleTesting",
                        BindingFlags.Public | BindingFlags.Instance
                        );

                    method.Invoke(tester, null);
                }
                finally
                {
                    talContext?.Unload();
                }
            }
            catch(Exception excp)
            {
                this.DiagnosticReporter.ReportException(
                    excp
                    );

                //throw;
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

            //var paths = Directory
            //    .GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll")
            //    .ToList()
            //    ;
            //paths.Add(assemblyPath);

            //var resolver = new PathAssemblyResolver(
            //    paths
            //    );

            //using (var mlc = new MetadataLoadContext(resolver))
            //{
            //    var assembly = mlc.LoadFromAssemblyPath(assemblyPath);

            //    try
            //    {
            //        var attrs = assembly.GetCustomAttributesData().Select(ca => ca.AttributeType.Name).ToList();
            //        var attr = assembly.GetCustomAttributesData().FirstOrDefault(ca => ca.AttributeType.Name == typeof(DpDtSkipAssembly).Name);
            //        if (attr != null)
            //        {
            //            return true;
            //        }
            //    }
            //    catch (Exception excp)
            //    {
            //        Debug.WriteLine("FAIL WITH RESOLVE ASSEMBLY ATTRIBUTE: " + assembly.Location);
            //        Debug.WriteLine(excp.Message);
            //        Debug.WriteLine(excp.StackTrace);
            //    }
            //}

            //return false;
        }

    }
}
