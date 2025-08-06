namespace ImSharp;

public static partial class Im
{
    public static partial class Native
    {
        public static partial class Internal
        {
            [StructLayout(LayoutKind.Explicit)]
            public struct StyleMod
            {
                [FieldOffset(0)]  public ImGuiStyle VarIdx;
                [FieldOffset(8)]  public int        BackupInt1;
                [FieldOffset(12)] public int        BackupInt2;

                [FieldOffset(8)]  public int BackupFloat1;
                [FieldOffset(12)] public int BackupFloat2;

                [FieldOffset(8)] public ImVec2 BackupVec;
            }
        }
    }
}
