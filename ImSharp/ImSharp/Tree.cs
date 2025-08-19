namespace ImSharp;

public static partial class Im
{
    public static unsafe class Tree
    {
        /// <inheritdoc cref="TreeNodeDisposable(ref Utf8LabelHandler,TreeNodeFlags)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static TreeNodeDisposable Node(Utf8LabelHandler label, TreeNodeFlags flags = TreeNodeFlags.None)
            => new(ref label, flags);

        /// <inheritdoc cref="TreeNodeDisposable(ref Utf8LabelHandler)"/>
        public static TreeNodeDisposable Push(Utf8LabelHandler label)
            => new(ref label);

        /// <inheritdoc cref="TreeNodeDisposable(nint)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static TreeNodeDisposable Push(nint id)
            => new(id);

        /// <summary> Draw a collapsing header button that does not indent or push an ID and returns true when open. </summary>
        /// <param name="label"> The header label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="flags"> Additional flags to control the header's behaviour. </param>
        /// <returns> True if the header is open, false if it is collapsed. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool Header(Utf8LabelHandler label, TreeNodeFlags flags = TreeNodeFlags.None)
            => Native.Methods.Tree.CollapsingHeader(label.Start(), flags);

        /// <summary> Draw a collapsing header button that does not indent or push an ID and returns true when open. </summary>
        /// <param name="label"> The header label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="visible"> If true, displays a small close button on the upper right of the header, which will set this to false when clicked. If false, do not display the header at all. </param>
        /// <param name="flags"> Additional flags to control the header's behaviour. </param>
        /// <returns> True if the header is open, false if it is collapsed. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool Header(Utf8LabelHandler label, ref bool visible, TreeNodeFlags flags = TreeNodeFlags.None)
            => Native.Methods.Tree.CollapsingHeader(label.Start(), (ImBool*)Unsafe.AsPointer(ref visible), flags);

        /// <inheritdoc cref="HeaderDisposable(ref Utf8LabelHandler,TreeNodeFlags)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static HeaderDisposable HeaderId(Utf8LabelHandler label, TreeNodeFlags flags = TreeNodeFlags.None)
            => new(ref label, flags);

        /// <inheritdoc cref="HeaderDisposable(ref Utf8LabelHandler,ref bool, TreeNodeFlags)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static HeaderDisposable HeaderId(Utf8LabelHandler label, ref bool visible, TreeNodeFlags flags = TreeNodeFlags.None)
            => new(ref label, ref visible, flags);

        /// <summary> Set the next <seealso cref="Node"/>'s or <seealso cref="Header(Utf8LabelHandler,TreeNodeFlags)"/>'s open state. </summary>
        /// <param name="openState"> Whether the tree node should be open or not. </param>
        /// <param name="condition"> Conditions on when to apply the open state. </param>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void SetNextOpen(bool openState = true, Condition condition = Condition.None)
            => Native.Methods.Tree.SetNextItemOpen(openState, condition);
    }
}
