namespace ImSharp;

public static partial class Im
{
    /// <summary> A wrapper around pushing text wrap positions. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref struct TextWrapDisposable
    {
        /// <summary> The number of text wrap positions currently pushed using this disposable. </summary>
        public int Count { get; private set; }

        /// <summary> Push a text wrap position to the text wrap stack. </summary>
        /// <param name="localX"> The local X coordinate at which to wrap text. </param>
        /// <param name="condition"> If this is false, the position is not pushed. </param>
        /// <returns> A disposable object that can be used to push further text wrap positions and pops those positions after leaving scope. Use with using. </returns>
        /// <remarks> If you need to keep text wrap positions pushed longer than the current scope, use without using and use <seealso cref="PopUnsafe"/>. </remarks>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public TextWrapDisposable Push(float localX, bool condition = true)
        {
            if (condition)
            {
                Native.Methods.Stacks.PushTextWrapPos(localX);
                ++Count;
            }

            return this;
        }

        /// <summary> Pop a number of text wrap positions. </summary>
        /// <param name="num"> The number of text wrap positions to pop. This is clamped to the number of positions pushed by this object. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public void Pop(int num = 1)
        {
            num   =  Math.Min(num, Count);
            Count -= num;
            while (num-- > 0)
                Native.Methods.Stacks.PopTextWrapPos();
        }

        /// <summary> Pop all pushed text wrap positions. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public void Dispose()
            => Pop(Count);

        /// <summary> Pop a number of text wrap positions. </summary>
        /// <param name="num"> The number of text wrap positions to pop. The number is not checked against the text wrap stack. </param>
        /// <remarks> Avoid using this function, and text wrap positions across scopes, as much as possible. </remarks>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void PopUnsafe(int num = 1)
        {
            while (num-- > 0)
                Native.Methods.Stacks.PopTextWrapPos();
        }
    }
}
