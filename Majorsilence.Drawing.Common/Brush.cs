using SkiaSharp;


namespace Majorsilence.Drawing
{
    public class Brush : IDisposable
    {
        protected SKPaint _paint;

        public Brush(Color color)
        {
            _paint = new SKPaint
            {
                Color = new SKColor((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A),
                Style = SKPaintStyle.Fill
            };
        }

        public virtual void Dispose()
        {
            _paint?.Dispose();
            _paint = null;
        }

        internal virtual SKPaint ToSkPaint()
        {
            return _paint;
        }
    }
}
