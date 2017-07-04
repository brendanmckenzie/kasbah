namespace Kasbah.Content
{
    public class TypeDefinitionBuilder<T> : TypeDefinitionBuilder
    {
        public TypeDefinitionBuilder()
            : base(typeof(T))
        {
        }
    }
}
