using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DpdtInject.Generator
{
    [Generator]
    public class DpdtGenerator

        : ISourceGenerator
    {

        public DpdtGenerator()
        {
        }

        public void Initialize(InitializationContext context)
        {
        }

        public void Execute(SourceGeneratorContext context)
        {
            try
            {

                //var compilation = context.Compilation;

                //var allTypes = compilation.GlobalNamespace.GetAllTypes().ToList();
                //var builderTypes = allTypes
                //    .Where(t => t.GetFullName() == typeof(D2pdt2KernelBuilder).FullName)
                //    .ToList()
                //    ;

                //Debugger.Launch();

                //if (builderTypes.Count != 1)
                //{
                //    throw new InvalidOperationException("builder's type should exists only one!");
                //}

                //var builderType = builderTypes[0];

                ////compilation.GetSemanticModel().



//                context.AddSource(
//                    "additional.cs",
//                    SourceText.From(@"
//namespace DpdtInject.Generator
//{
//    //public partial class D2pdt2KernelBuilder
//    //{
//    //    private T GetKernelPrivate<T>()
//    //        where T : ID2pdt2Kernel
//    //    {
//    //        return new D2pdt2Kernel0();
//    //    }
//    //}

//    //public class D2pdt2Kernel0 : ID2pdt2Kernel
//    //{
//    //    public void ShowMessage()
//    //    {
//    //        System.Console.WriteLine(""--== message showed ==--"");
//    //    }

//    //}
//}

//", Encoding.UTF8)
//                    );

                var unitsGenerated = 1;

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "DPDTINJECT001",
                            title: "Dpdt generator successfully finished its work",
                            messageFormat: "Dpdt generator successfully finished its work, {0} compilation unit(s) generated. [it's an info message, not a real warning; swithing from 'warning' to 'message' results in no message are shown in 'Error List' window, don't know why]",
                            category: "DpDtInject",
                            DiagnosticSeverity.Warning,
                            isEnabledByDefault: true
                            ),
                        Location.None,
                        unitsGenerated
                        )
                    );
            }
            catch (Exception excp)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "DPDTINJECT100",
                            title: "Couldn't generate a binding boilerplate code",
                            messageFormat: "Couldn't generate a binding boilerplate code '{0}' {1}",
                            category: "DpDtInject",
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true
                            ),
                        Location.None,
                        excp.Message,
                        excp.StackTrace
                        )
                    );
            }
        }



    }
}
