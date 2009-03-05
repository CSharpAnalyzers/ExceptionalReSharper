using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class ExceptionCreationModel : ModelBase
    {
        public IObjectCreationExpression ObjectCreationExpression { get; set; }

        private IObjectCreationExpressionNode ObjectCreationExpressionNode
        {
            get { return this.ObjectCreationExpression as IObjectCreationExpressionNode; }
        }

        private IList<ICSharpArgumentNode> Arguments
        {
            get { return this.ObjectCreationExpressionNode.ArgumentList.Arguments; }
        }

        public ExceptionCreationModel(IObjectCreationExpression objectCreationExpression)
        {
            ObjectCreationExpression = objectCreationExpression;
        }

        public bool HasMessage
        {
            get { return GetFirstArgument() != null; }
        }

        public bool HasInnerException
        {
            get { return GetSecondArgument() != null; }
        }

        private ICSharpArgumentNode GetFirstArgument()
        {
            return this.Arguments.Count > 0 ? this.Arguments[0] : null;
        }

        private ICSharpArgumentNode GetSecondArgument()
        {
            return this.Arguments.Count > 1 ? this.Arguments[1] : null;
        }

        public DocumentRange AddMessage()
        {
            var codeFactory = new CodeElementFactory(this.ObjectCreationExpression.GetProject());
            var messageArgument = codeFactory.CreateArgument("\"See inner exception for details.\"");
            this.ObjectCreationExpressionNode.AddArgumentAfter(messageArgument, null);

            return DocumentRange.InvalidRange;
        }

        public DocumentRange AddInnerException(string variableName)
        {
            if(this.HasMessage == false) return DocumentRange.InvalidRange;

            var messageArgument = GetFirstArgument();

            var codeFactory = new CodeElementFactory(this.ObjectCreationExpression.GetProject());
            var variableArgument = codeFactory.CreateArgument(variableName);
            this.ObjectCreationExpressionNode.AddArgumentAfter(variableArgument, messageArgument);

            return DocumentRange.InvalidRange;
        }
    }
}