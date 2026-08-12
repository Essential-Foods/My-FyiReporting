using SkiaSharp;


namespace Majorsilence.Drawing
{
    public class Font : IDisposable
    {
        private static readonly Dictionary<(string, SKFontStyle), SKTypeface> TypefaceCache = new();
        private SKTypeface _typeface;
        private SKFont _skFont;

        public string FontFamily { get; }
        public float Size { get; }
        public FontStyle Style { get; }
        public GraphicsUnit Unit { get; }

        public string Name => FontFamily;
        public bool Bold => (Style & FontStyle.Bold) != 0;
        public bool Italic => (Style & FontStyle.Italic) != 0;
        public bool Underline => (Style & FontStyle.Underline) != 0;
        public bool Strikeout => (Style & FontStyle.Strikeout) != 0;

        public float Height
        {
            get
            {
                var m = _skFont.Metrics;
                return (float)Math.Ceiling((m.Descent - m.Ascent) * (96f / 72f));
            }
        }

        public Font(string fontFamily, float size)
            : this(fontFamily, size, FontStyle.Regular, GraphicsUnit.Point)
        {
        }

        public Font(string fontFamily, float size, FontStyle style)
            : this(fontFamily, size, style, GraphicsUnit.Point)
        {
        }

        public Font(string fontFamily, float size, FontStyle style, GraphicsUnit unit)
        {
            FontFamily = fontFamily;
            Size = size;
            Style = style;
            Unit = unit;

            var typefaceStyle = GetSkFontStyle(style);

            var cacheKey = (fontFamily, typefaceStyle);
            if (!TypefaceCache.TryGetValue(cacheKey, out _typeface))
            {
                _typeface = FontSubstitution.Resolve(fontFamily, typefaceStyle);
                TypefaceCache[cacheKey] = _typeface;
            }

            _skFont = new SKFont(_typeface, size);
        }

        public Font(Drawing.FontFamily fontFamily, float size, FontStyle style)
            : this(fontFamily.Name, size, style)
        {
        }

        public Font(Drawing.FontFamily fontFamily, float size)
            : this(fontFamily.Name, size)
        {
        }

        private static SKFontStyle GetSkFontStyle(FontStyle style)
        {
            bool bold = (style & FontStyle.Bold) != 0;
            bool italic = (style & FontStyle.Italic) != 0;
            if (bold && italic) return SKFontStyle.BoldItalic;
            if (bold) return SKFontStyle.Bold;
            if (italic) return SKFontStyle.Italic;
            return SKFontStyle.Normal;
        }

        public double GetHeight(Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException(nameof(g));

            var m = _skFont.Metrics;
            return Math.Ceiling((m.Descent - m.Ascent) * (g.DpiX / 72f));
        }

        public double GetHeight()
        {
            var m = _skFont.Metrics;
            return Math.Ceiling((m.Descent - m.Ascent) * (96f / 72f));
        }

        public SKFont ToSkFont() => _skFont;

        public void Dispose()
        {
            _skFont?.Dispose();
            _skFont = null;
            // Typeface is cached and shared — do not dispose here
        }

        public override string ToString() => $"{FontFamily} {Size}pt {Style}";
    }
}
