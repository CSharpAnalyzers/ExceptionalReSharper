/// <copyright file="AnalyzerBase.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>A base class for all analyzers.</summary>
    internal abstract class AnalyzerBase
    {
        public CSharpDaemonStageProcessBase Process { get; set; }

        /// <summary>Performs analyze of throw <paramref name="throwStatementModel"/>.</summary>
        /// <param name="throwStatementModel">Throw statement model to analyze.</param>
        public virtual void Visit(ThrowStatementModel throwStatementModel) {}

        /// <summary>Performs analyze of <paramref name="catchClauseModel"/>.</summary>
        /// <param name="catchClauseModel">Catch clause to analyze.</param>
        public virtual void Visit(CatchClauseModel catchClauseModel) { }

        /// <summary>Performs analyze of <paramref name="exceptionDocumentationModel"/>.</summary>
        /// <param name="exceptionDocumentationModel">Exception documentation to analyze.</param>
        public virtual void Visit(ExceptionDocCommentModel exceptionDocumentationModel) { }

        /// <summary>Performs analyze of <paramref name="docCommentBlockModel"/>.</summary>
        /// <param name="docCommentBlockModel">Documentation to analyze.</param>
        public virtual void Visit(DocCommentBlockModel docCommentBlockModel) { }

        /// <summary>Performs analyze of <paramref name="tryStatementModel"/>.</summary>
        /// <param name="tryStatementModel">Try statement to analyze.</param>
        public virtual void Visit(TryStatementModel tryStatementModel) { }

        /// <summary>Performs analyze of <paramref name="thrownExceptionModel"/>.</summary>
        /// <param name="thrownExceptionModel">Thrown exception to analyze.</param>
        public virtual void Visit(ThrownExceptionModel thrownExceptionModel) { }
    }
}