namespace ImSharp;

public unsafe struct InputStringHandlerBuffer : IStringHandlerBuffer
{
    public static int Size
        => ImSharpConfiguration.Context->InputBufferSize;

    public static byte* Buffer
        => ImSharpConfiguration.Context->InputBuffer;

    public static Span<byte> Span
        => new(Buffer, Size);

    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static bool Write(ReadOnlySpan<char> text, out byte* end)
        => IStringHandlerBuffer.Write<InputStringHandlerBuffer>(text, out end);

    /// <summary> Get the actual input data span for a text input for input after deactivation. </summary>
    /// <param name="id"> The ID of the input. </param>
    /// <param name="input"> The input data. </param>
    /// <returns> The actual buffer to use. </returns>
    public static Span<byte> GetInputSpan(ImGuiId id, ref Utf8TextHandler input)
    {
        if (id.ActivePreviousFrame)
            return Span;

        var begin = input.Start(out var end);
        if (begin != TextStringHandlerBuffer.Buffer)
        {
            var span = new ReadOnlySpan<byte>(begin, (int)(end - begin));
            span.CopyTo(TextStringHandlerBuffer.Span);
            TextStringHandlerBuffer.Span[span.Length] = 0;
        }

        return TextStringHandlerBuffer.Span;
    }

    /// <summary> Copy the current data over to the input buffer and set the ID. </summary>
    /// <param name="buffer"> The current data. </param>
    public static void SetActive(ReadOnlySpan<byte> buffer)
    {
        var ptr    = buffer.Start();
        var ownPtr = Buffer;
        if (ptr == ownPtr)
            return;

        while (*ptr++ is not 0)
            *ownPtr++ = *ptr;

        *ownPtr = 0;
    }
}
