namespace Kasbah.Content
{
    public class TypeDefinitionBuilder<T> : TypeDefinitionBuilder
    {
        public TypeDefinitionBuilder(PropertyMapper propertyMapper)
            : base(typeof(T), propertyMapper)
        {
        }
    }
}
