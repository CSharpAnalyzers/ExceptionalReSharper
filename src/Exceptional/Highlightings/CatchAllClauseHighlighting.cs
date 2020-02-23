using System;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharper.Exceptional;
using ReSharper.Exceptional.Highlightings;

using JetBrains.ReSharper.Feature.Services.Daemon;

[assembly: RegisterConfigurableSeverity(CatchAllClauseHighlighting.Id, Constants.CompoundName, HighlightingGroupIds.BestPractice,
    "Exceptional.CatchAllClause",
    "Exceptional.CatchAllClause",
    Severity.SUGGESTION
    )]


namespace ReSharper.Exceptional.Highlightings
{
    [ConfigurableSeverityHighlighting(Id, CSharpLanguage.Name)]
    public class CatchAllClauseHighlighting : HighlightingBase
    {
        public const string Id = "CatchAllClause";

        /// <summary>Gets the message which is shown in the editor. </summary>
        protected override string Message
        {
            get { return String.Format(Resources.HighlightCatchAllClauses); }
        }
    }
}