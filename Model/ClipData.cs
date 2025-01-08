namespace ClipEditor.Model
{
    internal struct ClipData
    {
        public TimeSpan StartPosition { get; private set; }
        public TimeSpan EndPosition { get; private set; }
        public TimeSpan ClipLength { get; private set; }
        public int Bitrate { get; private set; }

        public ClipData(TimeSpan startPosition, TimeSpan endPosition, int bitrate)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            ClipLength = endPosition - startPosition;
            Bitrate = bitrate;
        }
    }
}
 