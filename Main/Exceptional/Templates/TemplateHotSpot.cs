using System.Collections.Generic;
using JetBrains.ReSharper.CodeInsight.Services.Lookup;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.LiveTemplates;
using JetBrains.ReSharper.LiveTemplates.Execution;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Templates
{
    public class TemplateHotSpot
    {
        public string Name { get; private set; }
        public TextRange Range { get; private set; }
        public List<string> Suggestions { get; private set; }

        public TemplateHotSpot(string name, TextRange range)
        {
            Name = name;
            Range = range;
            Suggestions = new List<string>();
        }

        public TemplateFieldInfo Prepare()
        {
            return new TemplateFieldInfo(new TemplateField(this.Name, new ParamNamesExpression(this.Suggestions, this.Suggestions[0]), 0), new[] { this.Range});
        }

        private class ParamNamesExpression : QuickFixTemplateExpression
        {
            private readonly List<string> myNames;

            public ParamNamesExpression(List<string> names, string defaultName)
                : base(defaultName)
            {
                this.myNames = names;
            }

            protected override IList<ILookupItem> GetLookupItems()
            {
                return this.myNames.ConvertAll<ILookupItem>(name => new TextLookupItem(name));
            }
        }
    }
}