namespace ImSharp;

public static partial class Im
{
    /// <inheritdoc cref="IndentDisposable.Indent(float,bool)"/>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static IndentDisposable Indent(float width, bool condition)
        => new IndentDisposable().Indent(width, condition);

    /// <inheritdoc cref="IndentDisposable.Indent(float,bool)"/>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static IndentDisposable Indent(float width)
        => new IndentDisposable().Indent(width);

    /// <inheritdoc cref="IndentDisposable.Indent(float,bool)"/>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static IndentDisposable Indent(bool condition)
        => new IndentDisposable().Indent(0, condition);

    /// <inheritdoc cref="IndentDisposable.Indent(float,bool)"/>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static IndentDisposable Indent()
        => new IndentDisposable().Indent(0);

    /// <summary> A wrapper class for cursor-related queries or actions in the current window. </summary>
    /// <remarks> Window coordinates are relative to the window position, absolute coordinates are relative to the viewport. </remarks>
    public static class Cursor
    {
        /// <summary> Get or set the cursor position in window coordinates. </summary>
        public static Vector2 Position
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Native.Methods.Layout.GetCursorPos();
            [MethodImpl(ImSharpConfiguration.OptInl)]
            set => Native.Methods.Layout.SetCursorPos(value);
        }

        /// <summary> Get or set the horizontal cursor position in window coordinates. </summary>
        /// <remarks> Be careful with the vertical cursor position generally changing line after any widget when <seealso cref="Line.Same"/> is not called. </remarks>
        public static float X
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Native.Methods.Layout.GetCursorPosX();
            [MethodImpl(ImSharpConfiguration.OptInl)]
            set => Native.Methods.Layout.SetCursorPosX(value);
        }

        /// <summary> Get or set the vertical cursor position in window coordinates. </summary>
        public static float Y
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Native.Methods.Layout.GetCursorPosY();
            [MethodImpl(ImSharpConfiguration.OptInl)]
            set => Native.Methods.Layout.SetCursorPosY(value);
        }

        /// <summary> Get the initial cursor position in window coordinates. </summary>
        public static Vector2 StartPosition
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Native.Methods.Layout.GetCursorStartPos();
        }

        /// <summary> Get or set the cursor position in absolute coordinates. </summary>
        public static Vector2 ScreenPosition
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Native.Methods.Layout.GetCursorScreenPos();
            [MethodImpl(ImSharpConfiguration.OptInl)]
            set => Native.Methods.Layout.SetCursorScreenPos(value);
        }

        /// <summary> Get a rectangle of the given size starting the current cursor position. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static Rectangle ScreenRectangle(Vector2 size)
        {
            var pos = ScreenPosition;
            return new Rectangle(pos, pos + size);
        }

        /// <summary> Get or set the horizontal cursor position in absolute coordinates. </summary>
        public static float ScreenX
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Native.Methods.Layout.GetCursorScreenPos().X;
            [MethodImpl(ImSharpConfiguration.OptInl)]
            set => Native.Methods.Layout.SetCursorScreenPos(ScreenPosition with { X = value });
        }

        /// <summary> Get or set the vertical cursor position in absolute coordinates. </summary>
        /// <remarks> Be careful with the vertical cursor position generally changing line after any widget when <seealso cref="Line.Same"/> is not called. </remarks>
        public static float ScreenY
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Native.Methods.Layout.GetCursorScreenPos().Y;
            [MethodImpl(ImSharpConfiguration.OptInl)]
            set => Native.Methods.Layout.SetCursorScreenPos(ScreenPosition with { Y = value });
        }

        /// <summary> Move the current cursor position vertically so that text aligns with text in regularly framed items for the next item. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void FrameAlign()
            => Native.Methods.Layout.AlignTextToFramePadding();
    }
}
