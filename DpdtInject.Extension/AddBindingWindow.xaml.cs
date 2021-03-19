using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
using EnvDTE80;
using Microsoft.CodeAnalysis.Editing;

using Task = System.Threading.Tasks.Task;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using DpdtInject.Extension.Options;
using DpdtInject.Extension.ViewModel.Add;

namespace DpdtInject.Extension
{
    /// <summary>
    /// Interaction logic for AddBindingWindow.xaml
    /// </summary>
    public partial class AddBindingWindow : DialogWindow
    {
        private readonly CodeLensTarget? _target;
        private CancellationTokenSource? _cts;

        public AddBindingWindow()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public AddBindingWindow(
            CodeLensTarget target
            )
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            _target = target;

            InitializeComponent();

            this.HasMaximizeButton = true;
            this.HasMinimizeButton = false;
        }

        private async void AddBindingWindow_OnLoaded(
            object sender,
            RoutedEventArgs e
            )
        {
            var viewModel = new AddBindingViewModel(
                _target!
                );
            DataContext = viewModel;

            _cts = new CancellationTokenSource();
            viewModel.LoadWindowDataAsync(_cts)
                .FileAndForget(nameof(AddBindingWindow_OnLoaded))
                ;
        }

        private void ButtonBase_OnClick(
            object sender,
            RoutedEventArgs e
            )
        {
            MessageBox.Show("Test");
        }

        private void AddBindingWindow_OnKeyUp(
            object sender,
            KeyEventArgs e
            )
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                e.Handled = true;
            }
        }
    }

    public class DocumentModifier
    {
        private readonly MethodBindContainer _targetMethod;

        public DocumentModifier(
            MethodBindContainer targetMethod
            )
        {
            if (targetMethod is null)
            {
                throw new ArgumentNullException(nameof(targetMethod));
            }

            _targetMethod = targetMethod;
        }

        public async Task DoSurgeryAsync(
            NewBindingInfo newBindingInfo
            )
        {
            if (newBindingInfo is null)
            {
                throw new ArgumentNullException(nameof(newBindingInfo));
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2;
            if (dte == null)
            {
                return;
            }

            var componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
            if (componentModel == null)
            {
                return;
            }

            var textManager = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager;
            if (textManager == null)
            {
                return;
            }

            var workspace = (Workspace)componentModel.GetService<VisualStudioWorkspace>();
            if (workspace == null)
            {
                return;
            }

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out var currentActiveView));

            var currentDocumentFilePath = System.IO.Path.Combine(dte.ActiveDocument.Path, dte.ActiveDocument.Name);
            currentActiveView.GetCaretPos(out var currentLine, out var currentColumn);


            var methods = _targetMethod.ClusterType.GetMembers(_targetMethod.MethodSyntax.Identifier.Text);
            if (methods.Length != 1)
            {
                return;
            }

            var method = methods[0];
            if (method.Locations.Length != 1)
            {
                return;
            }

            var methodLocation = method.Locations[0];
            var methodFilePath = methodLocation.SourceTree?.FilePath;

            if (methodFilePath == null)
            {
                return;
            }

            #region open modified document

            var modifiedDocumentHelper = new VisualStudioDocumentHelper(
                methodFilePath
                );

            var lineSpan = methodLocation.GetLineSpan();
            modifiedDocumentHelper.OpenAndNavigate(
                lineSpan.StartLinePosition.Line,
                lineSpan.StartLinePosition.Character,
                lineSpan.EndLinePosition.Line,
                lineSpan.EndLinePosition.Character
                );

            #endregion

            #region switch back to source document if needed

            if (newBindingInfo.IsBindingComplete)
            {
                var sourceDocumentHelper = new VisualStudioDocumentHelper(
                    currentDocumentFilePath
                    );

                sourceDocumentHelper.OpenAndNavigate(
                    currentLine,
                    currentColumn,
                    currentLine,
                    currentColumn
                    );
            }

            #endregion

            var document = workspace.GetDocument(
                methodFilePath
                );
            if (document == null)
            {
                return;
            }

            //OptionSet options = workspace.Options;
            //options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, true);
            //options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInProperties, true);


            var surgeon = new SyntaxSurgeon(
                _targetMethod
                );

            var (surgedDocument, addedBinding) = await surgeon.SurgeAsync(
                document,
                newBindingInfo
                );

            if (surgedDocument == null)
            {
                return;
            }

            workspace.TryApplyChanges(surgedDocument.Project.Solution);

            if (!newBindingInfo.IsBindingComplete && addedBinding != null)
            {
                var addedBindingLineSpan = addedBinding.GetLocation().GetLineSpan();
                modifiedDocumentHelper.OpenAndNavigate(
                    addedBindingLineSpan.StartLinePosition.Line,
                    addedBindingLineSpan.StartLinePosition.Character,
                    addedBindingLineSpan.EndLinePosition.Line,
                    addedBindingLineSpan.EndLinePosition.Character
                    );
            }
        }
    }
    
    public class SyntaxSurgeon
    {
        private readonly MethodBindContainer _targetMethod;

        public SyntaxSurgeon(
            MethodBindContainer targetMethod
            )
        {
            if (targetMethod is null)
            {
                throw new ArgumentNullException(nameof(targetMethod));
            }

            _targetMethod = targetMethod;
        }

        public async Task<(Document?, SyntaxNode?)> SurgeAsync(
            Document document,
            NewBindingInfo newBindingInfo
            )
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (newBindingInfo is null)
            {
                throw new ArgumentNullException(nameof(newBindingInfo));
            }

            var documentEditor = await DocumentEditor.CreateAsync(
                document
                );

            #region add new namespaces

            var existingUsings = documentEditor.OriginalRoot
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .ToList()
                ;

            var additionalNamespaces = newBindingInfo.GetUniqueUsings(existingUsings).ToList();
            
            if (existingUsings.Count > 0)
            {
                documentEditor.InsertAfter(existingUsings.Last(), additionalNamespaces);
            }
            else
            {
                documentEditor.InsertAfter(
                    documentEditor.OriginalRoot.DescendantNodes().First(),
                    additionalNamespaces
                    );
            }

            #endregion

            #region add new binding

            var methodSyntax = documentEditor.OriginalRoot
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(m => m.Identifier.Text == _targetMethod.MethodSyntax.Identifier.Text)
                ;
            if (methodSyntax == null)
            {
                return (null, null);
            }
            if (methodSyntax.Body == null)
            {
                return (null, null);
            }

            var leadingTrivia = methodSyntax.Body.GetLeadingTrivia().ToString();


            var syntaxAnnotation = new SyntaxAnnotation();
            var bcp = new BindClauseProducer(newBindingInfo);
            var producedBinding = bcp.ProduceBinding(leadingTrivia)
                .WithAdditionalAnnotations(syntaxAnnotation);

            var modifiedMethodSyntax = methodSyntax.WithBody(
                methodSyntax.Body.AddStatements(new[] { producedBinding })
                );

            documentEditor.ReplaceNode(
                methodSyntax,
                modifiedMethodSyntax
                );

            #endregion

            var changedDocument = documentEditor.GetChangedDocument();
            var changedRoot = await changedDocument.GetSyntaxRootAsync();

            var opts = GeneralOptions.Instance;
            if (changedRoot != null && opts.EnableWhitespaceNormalization)
            {
                var changedSyntaxRoot = changedRoot.NormalizeWhitespace();
                changedDocument = changedDocument.WithSyntaxRoot(changedSyntaxRoot);
                changedRoot = await changedDocument.GetSyntaxRootAsync();
            }

            var addedBinding = changedRoot
                ?.DescendantNodes()
                .Where(n => n.HasAnnotation(syntaxAnnotation))
                .FirstOrDefault()
                ?? null;

            return (changedDocument, addedBinding);
        }
    }

    public class BindClauseProducer
    {
        private readonly NewBindingInfo _newBindingInfo;

        public BindClauseProducer(
            NewBindingInfo newBindingInfo
            )
        {
            if (newBindingInfo is null)
            {
                throw new ArgumentNullException(nameof(newBindingInfo));
            }

            _newBindingInfo = newBindingInfo;
        }

        public StatementSyntax ProduceBinding(
            string leadingTrivia
            )
        {
            var indend1 = leadingTrivia + "    ";
            var indend2 = indend1 + "    ";

            var clauses = new List<string>();

            var bindFroms = string.Join(",", _newBindingInfo.BindFroms.Select(b => b.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            clauses.Add($"Bind<{bindFroms}>()");

            if (_newBindingInfo.BindScope != BindScopeEnum.Constant)
            {
                clauses.Add($"{indend2}.To<{_newBindingInfo.BindTo.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>()");
                clauses.Add($"{indend2}.With{_newBindingInfo.BindScope}Scope()");
            }
            else
            {
                clauses.Add($"{indend2}.WithConstScope(/* place here your (static) readonly field */)");
            }


            if (_newBindingInfo.IsConditional)
            {
                clauses.Add($"{indend2}.When(rt => /* compose predicate against rt */ )");
            }

            foreach (var mca in _newBindingInfo.ManualConstructorArguments)
            {
                clauses.Add($"{indend2}.Configure(new ConstructorArgument(\"{mca.Name}\", /* your parameter value */))");
            }

            var clause = string.Join(Environment.NewLine, clauses);

            return
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.ParseExpression(
                        clause
                        )
                    ).WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithLeadingTrivia(SyntaxFactory.Whitespace(indend1))
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                ;

        }
    }

    public class NewBindingInfo
    {
        public IReadOnlyCollection<INamedTypeSymbol> BindFroms
        {
            get;
        }
        public INamedTypeSymbol BindTo
        {
            get;
        }
        public IMethodSymbol Constructor
        {
            get;
        }
        public List<IParameterSymbol> ManualConstructorArguments
        {
            get;
        }
        public BindScopeEnum BindScope
        {
            get;
        }
        public bool IsConditional
        {
            get;
        }

        public bool IsBindingComplete
        {
            get
            {
                if (ManualConstructorArguments.Count > 0)
                {
                    return false;
                }
                if (IsConditional)
                {
                    return false;
                }
                if (BindScope == BindScopeEnum.Constant)
                {
                    return false;
                }
                if (BindTo.IsGenericType)
                {
                    return false;
                }

                return true;
            }
        }

        public NewBindingInfo(
            IReadOnlyCollection<INamedTypeSymbol> bindFroms,
            INamedTypeSymbol bindTo,
            IMethodSymbol constructor,
            List<IParameterSymbol> manualConstructorArguments,
            BindScopeEnum bindScope,
            bool isConditional
            )
        {
            if (bindFroms is null)
            {
                throw new ArgumentNullException(nameof(bindFroms));
            }

            if (bindTo is null)
            {
                throw new ArgumentNullException(nameof(bindTo));
            }

            if (constructor is null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            if (manualConstructorArguments is null)
            {
                throw new ArgumentNullException(nameof(manualConstructorArguments));
            }

            BindFroms = bindFroms;
            BindTo = bindTo;
            Constructor = constructor;
            ManualConstructorArguments = manualConstructorArguments;
            BindScope = bindScope;
            IsConditional = isConditional;
        }

        public IReadOnlyList<UsingDirectiveSyntax> GetNewUsings(
            )
        {
            var result = new Dictionary<string, UsingDirectiveSyntax>();

            foreach (var bindFrom in BindFroms)
            {
                var key = bindFrom.ContainingNamespace.ToDisplayString();
                result[key] = 
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(
                            " " + key
                            )
                        ).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    ;
            }
            {
                var key = BindTo.ContainingNamespace.ToDisplayString();
                result[key] = 
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(
                            " " + key
                            )
                        ).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    ;
            }
            foreach (var constructorArgument in ManualConstructorArguments)
            {
                var key = constructorArgument.Type.ContainingNamespace.ToDisplayString();
                result[key] = 
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(
                            " " + key
                            )
                        ).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    ;
            }

            var constructorArgumentKey = typeof(ConstructorArgument).Namespace;
            result[constructorArgumentKey] =
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.ParseName(
                        " " + constructorArgumentKey
                        )
                    );

            return result.Values.ToList();
        }

        public IEnumerable<UsingDirectiveSyntax> GetUniqueUsings(
            IReadOnlyList<UsingDirectiveSyntax> existingNamespaces
            )
        {
            var newAll = GetNewUsings();
            foreach (var newn in newAll)
            {
                if (existingNamespaces.Any(en => en.Name.ToString() == newn.Name.ToString()))
                {
                    continue;
                }

                yield return newn;
            }
        }

    }
}
