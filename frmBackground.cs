using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace kusa
{
    class frmBackground : Form
    {
        public frmBackground(Bitmap bitmap)
        {
            InitializeComponent();
            SelectBitmap(bitmap);
        }

        public void SelectBitmap(Bitmap bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap src = bitmap;
                Bitmap dst = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppRgb);
                Graphics gfx = Graphics.FromImage(dst);
                gfx.DrawImage(src, 0, 0);
                bitmap = dst;
                gfx.Dispose();
                src.Dispose();
                dst.Dispose();
            }
            this.Size = bitmap.Size;
            IntPtr screenDc = ApiHelper.GetDC(IntPtr.Zero);
            IntPtr memDc = ApiHelper.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr hOldBitmap = IntPtr.Zero;
            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                hOldBitmap = ApiHelper.SelectObject(memDc, hBitmap);
                ApiHelper.ApiSize newApiSize = new ApiHelper.ApiSize(bitmap.Width, bitmap.Height);
                ApiHelper.ApiPoint sourceLocation = new ApiHelper.ApiPoint(0, 0);
                ApiHelper.ApiPoint newLocation = new ApiHelper.ApiPoint(this.Left, this.Top);
                ApiHelper.BLENDFUNCTION blend = new ApiHelper.BLENDFUNCTION();
                blend.BlendOp = ApiHelper.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = 255;
                blend.AlphaFormat = ApiHelper.AC_SRC_ALPHA;
                ApiHelper.UpdateLayeredWindow(Handle, screenDc, ref newLocation, ref newApiSize, memDc, ref sourceLocation, 0, ref blend, ApiHelper.ULW_ALPHA);
            }
            catch { }
            finally
            {
                ApiHelper.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    ApiHelper.SelectObject(memDc, hOldBitmap);
                    ApiHelper.DeleteObject(hBitmap);
                }
                ApiHelper.DeleteDC(memDc);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                p.ExStyle = p.ExStyle | ApiHelper.WS_EX_LAYERED;
                return p;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // frmBackground
            // 
            this.ClientSize = new System.Drawing.Size(134, 111);
            this.Name = "frmBackground";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmBackground_Load);
            this.ResumeLayout(false);
        }

        private void frmBackground_Load(object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }

    public class ApiHelper
    {
        public const Int32 WS_EX_LAYERED = 524288;
        public const int WM_NCHITTEST = 132;
        public const int HTCLIENT = 1;
        public const int HTCAPTION = 2;
        public const Int32 ULW_ALPHA = 2;
        public const byte AC_SRC_OVER = 0;
        public const byte AC_SRC_ALPHA = 1;


        [StructLayout(LayoutKind.Sequential)]
        public struct ApiPoint
        {
            public int X;
            public int Y;
            public ApiPoint(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public ApiPoint(Point pt)
            {
                this.X = pt.X;
                this.Y = pt.Y;
            }
        }

        public enum BoolEnum
        {
            False = 0,
            True = 1
        }

        public enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ApiSize
        {
            public Int32 cx;
            public Int32 cy;
            public ApiSize(Int32 cx, Int32 cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        public enum GWL : int
        {
            ExStyle = -20
        }

        public enum WS_EX : int
        {
            Transparent = 32,
            Layered = 524288
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern BoolEnum UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref ApiPoint pptDst, ref ApiSize psize, IntPtr hdcSrc, ref ApiPoint pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern BoolEnum DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern BoolEnum DeleteObject(IntPtr hObject);
    }
}