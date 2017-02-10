using System;

namespace Kasbah.Content.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FieldEditorAttribute : Attribute
    {
        public FieldEditorAttribute(string editor)
        {
            this.Editor = editor;

        }
        public string Editor { get; set; }
    }
}