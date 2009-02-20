using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class ThrownExceptionsModel : IModel
    {
        public List<ThrowStatementModel> ThrownExceptions { get; private set; }

        public ThrownExceptionsModel()
        {
            this.ThrownExceptions = new List<ThrowStatementModel>();
        }

        public void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            
        }

        public void Accept(Visitor visitor)
        {
            foreach (var throwStatementModel in this.ThrownExceptions)
            {
                throwStatementModel.Accept(visitor);
            }
        }
    }
}