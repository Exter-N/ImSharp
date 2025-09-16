namespace ImSharp;

public static partial class Im
{
    /// <summary> A wrapper around indentation. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class IndentDisposable : IDisposable
    {
        /// <summary> The current indentation pushed by this object. </summary>
        public float CurrentIndent { get; private set; }

        /// <summary> Add to the current indentation. </summary>
        /// <param name="indent"> The value to change the indentation by. </param>
        /// <param name="condition"> If this is false, the current indent is not changed. </param>
        /// <returns> A disposable object that can be used to change the indentation more and reverts to the prior indentation after leaving scope. Use with using. </returns>
        /// <remarks> If you need to keep indentation for longer than the current scope, use without using. </remarks>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public IndentDisposable Indent(float indent, bool condition = true)
        {
            if (condition && indent is not 0)
            {
                if (indent < 0)
                    Native.Methods.Layout.Unindent(indent);
                else
                    Native.Methods.Layout.Indent(indent);
                CurrentIndent += indent;
            }

            return this;
        }

        /// <summary> Subtract from the current indentation. </summary>
        /// <param name="indent"> The value to change the indentation by. </param>
        /// <param name="condition"> If this is false, the current indent is not changed. </param>
        /// <returns> A disposable object that can be used to change the indentation more and reverts to the prior indentation after leaving scope. Use with using. </returns>
        /// <remarks> If you need to keep indentation for longer than the current scope, use without using. </remarks>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public IndentDisposable Unindent(float indent, bool condition = true)
        {
            if (condition && indent is not 0)
            {
                if (indent < 0)
                    Native.Methods.Layout.Indent(indent);
                else
                    Native.Methods.Layout.Unindent(indent);
                CurrentIndent -= indent;
            }

            return this;
        }

        /// <summary> Revert all indentation applied by this object. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public void Dispose()
            => Unindent(CurrentIndent);
    }
}
