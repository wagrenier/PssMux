using PssMux.Operations;

namespace PssMux.Handler
{
    public static class PssHandler
    {
        /// <summary>
        /// Switches the target's audio to the source's audio.
        /// </summary>
        /// <param name="source">The source PSS file with the full path</param>
        /// <param name="target">The target PSS file with the full path</param>
        /// <returns>Byte array representing the new mux file</returns>
        public static byte[] SwitchPssAudio(byte[] source, byte[] target)
        {
            var pssHandler = new PssAudioMux(source, target);
            return pssHandler.TransferAudio();
        }
    }
}