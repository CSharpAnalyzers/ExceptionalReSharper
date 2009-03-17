/// <copyright file="CatchAllClauseHighlighting.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.SUGGESTION)]
    public class CatchAllClauseHighlighting : HighlightingBase
    {
        private CatchClauseModel CatchClauseModel { get; set; }

        internal CatchAllClauseHighlighting(CatchClauseModel catchClauseModel)
        {
            CatchClauseModel = catchClauseModel;
        }

        public override DocumentRange Range
        {
            get { return this.CatchClauseModel.DocumentRange; }
        }

        protected override string Message
        {
            get { return String.Format(Resources.HighLightCatchAllClauses); }
        }
    }
}