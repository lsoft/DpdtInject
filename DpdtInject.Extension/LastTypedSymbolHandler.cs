using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace DpdtInject.Extension
{
    //[Export(typeof(ICommandHandler))]
    //[Name(nameof(LastTypedSymbolHandler))]
    //[ContentType("text")]
    //[TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    //public class LastTypedSymbolHandler : ICommandHandler<TypeCharCommandArgs>
    //{
    //    private readonly WaitRescanner _rescanner;

    //    public string DisplayName => nameof(LastTypedSymbolHandler);


    //    [ImportingConstructor]
    //    public LastTypedSymbolHandler(
    //        WaitRescanner rescanner
    //        )
    //    {
    //        if (rescanner is null)
    //        {
    //            throw new ArgumentNullException(nameof(rescanner));
    //        }

    //        _rescanner = rescanner;
    //    }

    //    public bool ExecuteCommand(TypeCharCommandArgs args, CommandExecutionContext executionContext)
    //    {
    //        _rescanner.NewChangesIncomes();

    //        //SetNewChanges()
    //        //    .FileAndForget(nameof(LastTypedSymbolHandler));

    //        return false;
    //    }

    //    public CommandState GetCommandState(TypeCharCommandArgs args)
    //    {
    //        return CommandState.Available;
    //    }
    //}
}
