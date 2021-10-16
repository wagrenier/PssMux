namespace PssMux.Extensions
{
    public static class ByteArrayExtensions
    {
        internal static bool StartsWith(this byte[] container, byte[] sequenceToFind)
        {
            for (var i = 0; i < sequenceToFind.Length && i < container.Length; i++)
            {
                if (container[i] != sequenceToFind[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}