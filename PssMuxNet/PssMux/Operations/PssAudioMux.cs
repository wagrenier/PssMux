using PssMux.Extensions;

namespace PssMux.Operations
{
    internal class PssAudioMux
    {
        private readonly ArrayAsStream _source;
        private readonly ArrayAsStream _target;

        internal PssAudioMux(byte[] source, byte[] target)
        {
            _source = new ArrayAsStream(source);
            _target = new ArrayAsStream(target);
        }

        internal byte[] TransferAudio()
        {
            var sourceAudioBuffer = _source.BuildFullAudioBuffer();

            _source.SeekNextAudioBlock();
            _target.SeekNextAudioBlock();

            var sourceCurrentBlockSize = _source.InitialAudioBlock(out var sourceAudioSize);
            var targetCurrentBlockSize = _target.InitialAudioBlock(out var targetAudioSize);
            
            _source.SeekRelative(sourceCurrentBlockSize);
            
            _target.Write(sourceAudioBuffer.Read(targetCurrentBlockSize));

            while (true)
            {
                var isDoneSource = _source.SeekNextAudioBlock();
                var isDoneTarget = _target.SeekNextAudioBlock();

                if (isDoneSource || isDoneTarget)
                {
                    break;
                }

                sourceCurrentBlockSize = _source.AudioBlock();
                targetCurrentBlockSize = _target.AudioBlock();
                
                _source.SeekRelative(sourceCurrentBlockSize);
                
                _target.Write(sourceAudioBuffer.Read(targetCurrentBlockSize));
            }

            return _target.GetBuffer();
        }
    }
}