using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

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