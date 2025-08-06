namespace ImSharp;

public static partial class Im
{
    /// <summary> A wrapper around a ImGui Drag and Drop Target. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public unsafe ref struct DragDropTargetDisposable
    {
        /// <summary> Whether creating the drag and drop target succeeded. </summary>
        public readonly bool Success;

        /// <summary> Whether the drag and drop target is already ended. </summary>
        public bool Alive { get; private set; }

        /// <summary> Open a new Drag and Drop target on the last item and close it on leaving scope. </summary>
        /// <returns> A disposable object that indicates whether the target is active. Use with using. </returns>
        /// <remarks> You can use the returned object to check for a specific payload dropping with <see cref="IsDropping(Utf8LabelHandler,DragDropTargetFlags)"/>. </remarks>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        internal DragDropTargetDisposable(bool _)
        {
            Success = Native.Methods.DragDrop.BeginDragDropTarget();
            Alive   = true;
        }

        /// <summary> Check whether a specific payload is currently dropping on this target. </summary>
        /// <param name="type"> A user-defined string of at most 32 characters as text. If this is a UTF8 string, it HAS to be null-terminated. It should not start with '_' as those are reserved for ImGui internals. </param>
        /// <param name="flags"> Additional flags to control the payload checking. </param>
        /// <returns> True if the specified payload is currently dropping. </returns>
        /// <remarks> Use <seealso cref="Im.DragDrop.TryAcceptPayload(Utf8LabelHandler,DragDropTargetFlags,out Payload)"/> instead if you need the actual payload. </remarks>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public bool IsDropping(Utf8LabelHandler type, DragDropTargetFlags flags = DragDropTargetFlags.None)
            => Success && Native.Methods.DragDrop.AcceptDragDropPayload(type.Start(), flags) != null;

        /// <summary> Conversion to bool. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static implicit operator bool(DragDropTargetDisposable value)
            => value.Success;

        /// <summary> Conversion to bool. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool operator true(DragDropTargetDisposable i)
            => i.Success;

        /// <summary> Conversion to bool. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool operator false(DragDropTargetDisposable i)
            => !i.Success;

        /// <summary> Conversion to bool on NOT operators. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool operator !(DragDropTargetDisposable i)
            => !i.Success;

        /// <summary> Conversion to bool on AND operators. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool operator &(DragDropTargetDisposable i, bool value)
            => i.Success && value;

        /// <summary> Conversion to bool on OR operators. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool operator |(DragDropTargetDisposable i, bool value)
            => i.Success || value;

        /// <summary> End the drag and drop target on leaving scope. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public void Dispose()
        {
            if (!Alive)
                return;

            if (Success)
                Native.Methods.DragDrop.EndDragDropTarget();
            Alive = false;
        }

        /// <summary> End a drag and drop target without using an IDisposable.</summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static void EndUnsafe()
            => Native.Methods.DragDrop.EndDragDropTarget();
    }
}
