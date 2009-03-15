using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.LiveTemplates;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;

namespace CodeGears.ReSharper.Exceptional.QuickFixes
{
//    //[QuickFix]
//    internal class IncludeInnerExceptionFix : SingleActionFix
//    {
//        public ThrowFromCatchWithNoInnerExceptionHighlighting Error { get; set; }
//
//        public IncludeInnerExceptionFix(ThrowFromCatchWithNoInnerExceptionHighlighting error)
//        {
//            Error = error;
//        }
//
//        public override void Execute(ISolution solution, ITextControl textControl)
//        {
//            using (CommandCookie.Create(Resources.QuickFixIncludeInnerException))
//            {
//                var model = this.Error.ThrowStatementModel;
//
////                var executeTemplateForMessage = false;
////                var executeTemplateForException = false;
//
//                //PsiManager.GetInstance(solution).DoTransaction(
//                //    delegate
//                //    {
//                //        if (model.ExceptionCreationModel.HasMessage == false)
//                //        {
//                //            model.ExceptionCreationModel.AddMessage();
//                //        }
//
//                //        if (model.ExceptionCreationModel.HasInnerException == false)
//                //        {
//                //            if (model.OuterCatchClauseModel.HasVariable == false)
//                //            {
//                //                model.ExceptionCreationModel.AddInnerException("e");
//                //                model.OuterCatchClauseModel.AddVariable();
//                //            }
//                //            else
//                //            {
//                //                var variableModel = model.OuterCatchClauseModel.VariableModel;
//                //                model.ExceptionCreationModel.AddInnerException(variableModel.VariableName.Name);
//                //            }
//                //        }
//                //    });
//
////                if (executeTemplateForMessage || executeTemplateForException)
////                {
////                    var exceptionCreation = model.ThrowStatement.Exception as IObjectCreationExpressionNode;
////                    if (exceptionCreation == null) return;
////
////                    var arguments = new List<ICSharpArgumentNode>(exceptionCreation.ArgumentList.Arguments);
////
////                    if (arguments.Count > 1)
////                    {
////                        var message = arguments[0];
////                        var variable = arguments[1];
////
////                        var messageRange = message.GetDocumentRange().TextRange.TrimLeft(1).TrimRight(1);
////                        var variableRange = variable.GetDocumentRange().TextRange;
////                        var catchVariableRange = model.OuterCatchClauseModel.GetVariableTextRange();
////
////                        var messageSuggestions = new List<string>(new[] { "See inner exception for details." });
////                        var variableSuggestions = new List<string>(new[] { "e", "ex" });
////
////                        var messageTemplateField = new TemplateFieldInfo(new TemplateField("Message", new ParamNamesExpression(messageSuggestions, messageSuggestions[0]), 0), new[] { messageRange });
////                        var variableTemplateField = new TemplateFieldInfo(new TemplateField("Variable", new ParamNamesExpression(variableSuggestions, variableSuggestions[0]), 0), new[] { variableRange, catchVariableRange });
////
////                        TemplateUtil.ExecuteTemplate(solution, textControl, messageRange, new[] { messageTemplateField, variableTemplateField });
////                        var currentSession = LiveTemplatesController.Instance.CurrentSession;
////                        if(currentSession != null)
////                        {
////                            currentSession.TemplateSession.Closed +=
////                                delegate
////                                    {
////                                        textControl.SelectionModel.RemoveSelection();
////                                    };
////                        }
////                    }
////                }
//            }
//        }
//
//        public override string Text
//        {
//            get { return Resources.QuickFixIncludeInnerException; }
//        }
//
//        
//
//
//    }
}