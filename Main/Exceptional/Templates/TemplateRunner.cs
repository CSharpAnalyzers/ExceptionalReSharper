using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.LiveTemplates.Execution;
using JetBrains.TextControl;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Templates
{
    public static class TemplateRunner
    {
        public static void Run(ISolution solution, ITextControl textControl, TextRange selectionRange, params TemplateHotSpot[] hotSpots)
        {
            var fields = new List<TemplateFieldInfo>();
            foreach (var hotSpot in hotSpots)
            {
                fields.Add(hotSpot.Prepare());
            }

            JetBrains.ReSharper.Intentions.Util.TemplateUtil.ExecuteTemplate(
                    solution, textControl, selectionRange, fields.ToArray());

            var currentSession = LiveTemplatesController.Instance.CurrentSession;
            if (currentSession != null)
            {
                currentSession.TemplateSession.Closed +=
                    delegate
                    {
                        textControl.SelectionModel.RemoveSelection();
                    };
            }
        }
    }
}