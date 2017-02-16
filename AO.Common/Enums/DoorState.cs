using System;

namespace AO.Common
{
    [Flags]
    public enum DoorState
    {
        Locked = 0x40,
        Open = 0x80
    }
}
