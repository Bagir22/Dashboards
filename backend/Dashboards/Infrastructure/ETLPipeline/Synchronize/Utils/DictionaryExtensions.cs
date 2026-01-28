namespace Infrastructure.ETLPipeline.Synchronize.Utils
{
    public static class DictionaryExtensions
    {
        public static TValue? GetNullableValue<TKey, TValue>( this Dictionary<TKey, TValue> dict, TKey? key )
            where TKey : class
            where TValue : struct
        {
            return key != null && dict.TryGetValue( key, out var value ) ? value :   null;
        }
    }
}