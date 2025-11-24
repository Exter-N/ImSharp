namespace ImSharp;

/// <summary> A string with its size with the given font, mainly for use in caches. </summary>
public record SizedString
{
    /// <summary> The static subscriber to update cached string sizes when the font changes. </summary>
    private static readonly SizedStringSubscriber Subscriber = new();

    /// <summary> Get the cached size of the text. </summary>
    public Vector2 Size
    {
        get => float.IsNaN(field.X) ? field = Im.Font.CalculateSize(Text) : field;
        private set;
    }

    /// <summary> The text. </summary>
    public StringU8 Text { get; init; }

    public void ResetSize()
        => Size = new Vector2(float.NaN, float.NaN);

    ~SizedString()
        => Subscriber.Strings.TryRemove(this, out _);

    public SizedString(StringU8 text)
        : this(text, Vector2.NaN)
    { }

    public SizedString(Utf8InterpolatedStringHandler text)
        : this(new StringU8(ref text))
    { }

    public unsafe SizedString(byte* text)
        : this(new StringU8(text))
    { }

    public SizedString(ReadOnlySpan<byte> text)
        : this(new StringU8(text))
    { }

    public SizedString(ReadOnlyMemory<byte> text)
        : this(new StringU8(text))
    { }

    /// <summary> A string with its size with the given font, mainly for use in caches. </summary>
    /// <param name="text"> The text. </param>
    /// <param name="size"> The font size of the text in pixels. </param>
    public SizedString(StringU8 text, Vector2 size)
    {
        Text = text;
        Size = size;
        Subscriber.Strings.TryAdd(this, 0);
    }

    public void Deconstruct(out StringU8 text, out Vector2 size)
    {
        text = Text;
        size = Size;
    }


    /// <summary> Update sized strings automatically when the font or global scale change. </summary>
    private class SizedStringSubscriber
    {
        public readonly ConcurrentDictionary<SizedString, byte> Strings = [];

        public SizedStringSubscriber()
            => CacheManager.Instance.OnFontDirty += OnFontDirty;

        private void OnFontDirty()
        {
            foreach (var s in Strings.Keys)
                s.ResetSize();
        }

        ~SizedStringSubscriber()
        {
            CacheManager.Instance.OnFontDirty -= OnFontDirty;
        }
    }
}
