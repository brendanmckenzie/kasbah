using System;

namespace Kasbah.Content.Attributes
{
    public class FieldEditorAttribute : Attribute
    {
        public FieldEditorAttribute(string editor)
        {
            this.Editor = editor;

        }
        public string Editor { get; set; }
    }
}