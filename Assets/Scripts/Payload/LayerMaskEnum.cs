using System;

namespace FGSTest.Payload
{
    [Flags]
    public enum LayerMaskEnum
    {
        None = 0,
        Default = 1 << 0,
        Ground = 1 << 1,
        Obstacle = 1 << 2,
    }
}