using System;

namespace UI.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ViewAttribute : Attribute
    {
        public string ViewTitle { get; set; }

        public string ModuleName { get; set; }

        public string MenuLabel { get; set; }

        public Type ToolbarStripContentType { get; set; }

        public Type ViewModelType { get; set; }
    }
}
