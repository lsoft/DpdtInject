using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DpdtInject.Extension.Helper
{
    /// <summary>
    /// Taken from  https://github.com/bert2/microscope completely.
    /// Take a look to that repo, it's amazing!
    /// </summary>
    public static class MethodHelper
    {
        public static async Task<T?> GetSymbolAtAsync<T>(this Document doc, TextSpan span, CancellationToken ct)
            where T : ISymbol
        {
            var rootNode = await doc.GetSyntaxRootAsync(ct).ConfigureAwait(false);

            if (rootNode == null)
            {
                return default(T);
            }

            var syntaxNode = rootNode.FindNode(span);

            var semanticModel = await doc.GetSemanticModelAsync(ct).ConfigureAwait(false);

            if (semanticModel == null)
            {
                return default(T);
            }

            var symbol = semanticModel.GetDeclaredSymbol(syntaxNode, ct);

            if (symbol == null)
            {
                return default(T);
            }

            return ((T)symbol)!;
        }
    }
}
