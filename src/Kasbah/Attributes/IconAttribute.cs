using System;

namespace Kasbah.Content.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class IconAttribute : Attribute
    {
        public IconAttribute(string icon, string colour = null)
        {
            this.Icon = icon;
            this.Colour = colour;
        }

        public string Icon { get; private set; }

        public string Colour { get; private set; }
    }
}
