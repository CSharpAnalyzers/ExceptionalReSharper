using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.TextControl;
using ReSharper.Exceptional.Highlightings;

namespace ReSharper.Exceptional.QuickFixes
{
    // TODO: Implement AddToOptionalMethodExceptionsFix

    //[QuickFix]
    //internal class AddToOptionalMethodExceptionsFix : SingleActionFix
    //{
    //    private ExceptionNotDocumentedHighlighting Error { get; set; }

    //    public AddToOptionalMethodExceptionsFix(ExceptionNotDocumentedHighlighting error)
    //    {
    //        Error = error;
    //    }

    //    /// <summary>Executes QuickFix or ContextAction. Returns post-execute method. </summary>
    //    /// <returns>Action to execute after document and PSI transaction finish. Use to open TextControls, navigate caret, etc. </returns>
    //    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    //    {
    //        Error.
    //    }

    //    /// <summary>Popup menu item text. </summary>
    //    public override string Text
    //    {
    //        get { return "Add to list of excluded method exceptions. [Exceptional]"; } // TODO: Translate
    //    }
    //}
}