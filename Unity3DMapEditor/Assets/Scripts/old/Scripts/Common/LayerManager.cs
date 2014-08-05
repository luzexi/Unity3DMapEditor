using System;

public class LayerManager
{
    public const int DefaultLayer = 0;
    public const int ExportLayer = 8;
    public const int GroundLayer = 9;
    public const int UILayer = 10;
    public const int DialogueLayer = 11;// 剧情对话专用 [4/24/2012 Ivan]
    public const int ActorLayer = 20;
    public const int FakeObjectLayer = 30;

    public const int DefaultMask = 1 << DefaultLayer;
    public const int ExportMask = 1 << ExportLayer;
    public const int GroundMask = 1 << GroundLayer;
    public const int UIMask = 1 << UILayer;
    public const int DialogueMask = 1 << DialogueLayer;
    public const int ActorMask = 1 << ActorLayer;
    public const int FakeObjectMask = 1 << FakeObjectLayer;
}