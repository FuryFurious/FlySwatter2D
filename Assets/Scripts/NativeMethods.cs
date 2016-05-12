using System.Runtime.InteropServices;
public static class NativeMethods
{
    [DllImport("user32")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

    [DllImport("user32", SetLastError = true)]
    private static extern void keybd_event(byte key, byte bScan, int keyEvent, int dwExtraInfo);

    [DllImport("user32")]
    private static extern byte MapVirtualKey(byte wCode, byte wMapType);

    [DllImport("user32")]
    private static extern short GetAsyncKeyState(byte vKey);

    public static void MoveMouse(int dx, int dy)
    {
        NativeMethods.mouse_event(MOUSE_MOVE, dx, dy, 0, 0);
    }

    public static void KeyPress(byte key)
    {
        NativeMethods.keybd_event(key, NativeMethods.MapVirtualKey(key, 0), KEY_EVENT_DOWN, 0);
    }

    public static void KeyRelease(byte key)
    {
        NativeMethods.keybd_event(key, NativeMethods.MapVirtualKey(key, 0), KEY_EVENT_UP, 0);
    }


    public static bool IsKeyDown(byte key)
    {
        return 0 != (NativeMethods.GetAsyncKeyState(key) & 0x8000);
    }

    public static bool IsKeyUp(byte key)
    {
        return !IsKeyDown(key);
    }

    //members
    public const int MOUSE_MOVE = 0x0001;
    public const int MOUSE_LEFT_DOWN = 0x0002;
    public const int MOUSE_LEFT_UP = 0x0004;
    public const int MOUSE_RIGHT_DOWN = 0x0008;
    public const int MOUSE_RIGHT_UP = 0x0010;
    public const int MOUSE_MIDDLE_DOWN = 0x0020;
    public const int MOUSE_MIDDLE_UP = 0x0040;

    //unimportant probably:
    public const int MOUSEEVENTF_WHEEL = 0x0800;
    public const int MOUSEEVENTF_XDOWN = 0x0080;
    public const int MOUSEEVENTF_XUP = 0x0100;
    public const int MOUSEEVENTF_HWHEEL = 0x01000;
    public const int MOUSEEVENTF_ABSOLUTE = 0x8000;

    //key events -> third parameter
    public const int KEY_EVENT_DOWN = 0x0001;
    public const int KEY_EVENT_UP = 0x0002;

    //keys -> first parameter
    public const byte KEY_A = 0x41;
    public const byte KEY_B = 0x42;
    public const byte KEY_C = 0x43;
    public const byte KEY_D = 0x44;
    public const byte KEY_E = 0x45;
    public const byte KEY_F = 0x46;
    public const byte KEY_G = 0x47;
    public const byte KEY_H = 0x48;
    public const byte KEY_I = 0x49;
    public const byte KEY_J = 0x4A;
    public const byte KEY_K = 0x4B;
    public const byte KEY_L = 0x4C;
    public const byte KEY_M = 0x4D;
    public const byte KEY_N = 0x4E;
    public const byte KEY_O = 0x4F;
    public const byte KEY_P = 0x50;
    public const byte KEY_Q = 0x51;
    public const byte KEY_R = 0x52;
    public const byte KEY_S = 0x53;
    public const byte KEY_T = 0x54;
    public const byte KEY_U = 0x55;
    public const byte KEY_V = 0x56;
    public const byte KEY_W = 0x57;
    public const byte KEY_X = 0x58;
    public const byte KEY_Y = 0x59;
    public const byte KEY_Z = 0x5A;

    public const byte KEY_R_CONTROL = 0xA3;
    public const byte KEY_ESCAPE = 0x1B;
    public const byte KEY_L_SHIFT = 0xA0;
    public const byte KEY_R_SHIFT = 0xA1;
    public const byte KEY_L_CONTROL = 0xA2;
    public const byte KEY_BACK = 0x08;
    public const byte KEY_BACKSPACE = 0x08;
    public const byte KEY_TAB = 0x09;
    public const byte KEY_ENTER = 0x0D;
    public const byte KEY_RETURN = 0x0D;
    public const byte KEY_SPACE = 0x20;

    public const byte KEY_0 = 0x30;
    public const byte KEY_1 = 0x31;
    public const byte KEY_2 = 0x32;
    public const byte KEY_3 = 0x33;
    public const byte KEY_4 = 0x34;
    public const byte KEY_5 = 0x35;
    public const byte KEY_6 = 0x36;
    public const byte KEY_7 = 0x37;
    public const byte KEY_8 = 0x38;
    public const byte KEY_9 = 0x39;

    public const byte KEY_F1 = 0x70;
    public const byte KEY_F2 = 0x71;
    public const byte KEY_F3 = 0x72;
    public const byte KEY_F4 = 0x73;
    public const byte KEY_F5 = 0x74;
    public const byte KEY_F6 = 0x75;
    public const byte KEY_F7 = 0x76;
    public const byte KEY_F8 = 0x77;
    public const byte KEY_F9 = 0x78;
    public const byte KEY_F10 = 0x79;
    public const byte KEY_F11 = 0x7A;
    public const byte KEY_F12 = 0x7B;
}
