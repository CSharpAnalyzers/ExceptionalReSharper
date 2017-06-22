using System;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.Exceptional;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
using JetBrains.DocumentModel;

#if R9 || R10
using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
#endif

namespace ReSharper.Exceptional.Highlightings
{
    /// <summary>Base class for all highlightings.</summary>
    /// <remarks>Provides default implementation.</remarks>
    public abstract class HighlightingBase :  IHighlighting
    {
        public bool IsValid()
        {
            return true;
        }

        public DocumentRange CalculateRange() {
            throw new NotImplementedException();
        }

        public virtual string ToolTip
        {
            get { return Message; }
        }

        public virtual string ErrorStripeToolTip
        {
            get { return Message; }
        }

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected abstract string Message { get; }

        public virtual int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}