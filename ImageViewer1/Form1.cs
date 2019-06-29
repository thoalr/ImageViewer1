using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Input;
using System.Drawing.Drawing2D;
using System.Runtime.Caching;

namespace ImageViewer1
{
    public partial class Form1 : Form
    {

        abstract class Win32Messages
        {
            public const int WM_MOUSEHWHEEL = 0x020E;//discovered via Spy++ 
        }
        abstract class Utils
        {
            internal static Int32 HIWORD(IntPtr ptr)
            {
                Int32 val32 = ptr.ToInt32();
                return ((val32 >> 16) & 0xFFFF);
            }

            internal static Int32 LOWORD(IntPtr ptr)
            {
                Int32 val32 = ptr.ToInt32();
                return (val32 & 0xFFFF);
            }

        }

        string imagefilepath = @"C:\Users\thors\Documents\Frami\1.jpg";
        string currentDir = @"C:\";
        int currentImageIndex = 0;
        string[] filelist;

        //Dictionary<string, Bitmap> imageCache;
        //MemoryCache imageCache = MemoryCache.Default;

        readonly string[] extensions = new string[] { ".jpg", ".jpeg", ".png" };


        // Values for current image
        private Point image_offset = new Point(0, 0);
        Bitmap Image;
        float Zoom = 100;
        float ZoomValue = 1.3f;
        bool firstDraw = true;

        enum ZoomType
        {
            Center,
            Free
        }
        ZoomType zoomType = ZoomType.Center;

        Point mouseLastPos = new Point(0, 0);


        public Form1()
        {
            InitializeComponent();
            Form1_Construct();
        }

        public Form1(string arg)
        {
            imagefilepath = arg;

            InitializeComponent();
            Form1_Construct();
        }

        private void Form1_Construct()
        {
            //this.SuspendLayout();
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            //this.ResumeLayout();            
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Image = (Bitmap)Bitmap.FromFile(imagefilepath);

            this.WindowState = FormWindowState.Maximized;


            //pictureBox1.Image = screenBuffer;

            currentDir = Path.GetDirectoryName(imagefilepath);
            fileSystemWatcher1.Path = currentDir;
            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.NotifyFilter = NotifyFilters.LastAccess
                       | NotifyFilters.LastWrite
                       | NotifyFilters.FileName
                       | NotifyFilters.DirectoryName;

            filelist = Directory.GetFiles(currentDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => extensions.Contains(Path.GetExtension(s))).ToArray();

            currentImageIndex = Array.IndexOf(filelist, imagefilepath);

        }

        private void CenterImage()
        {
            if (zoomType == ZoomType.Center)
            {
                if (Image.Size.Height > Image.Size.Width)
                {
                    image_offset.X = (int)((pictureBox1.Width - (Zoom * Image.Size.Width / 100.0f)) / 2);
                    image_offset.Y = (int)((pictureBox1.Height - (Zoom * Image.Size.Height / 100.0f)) / 2);
                }
                else
                {
                    image_offset.X = (int)((pictureBox1.Width - (Zoom * Image.Size.Width / 100.0f)) / 2);
                    image_offset.Y = (int)((pictureBox1.Height - (Zoom * Image.Size.Height / 100.0f)) / 2);
                }
            }
            if (zoomType == ZoomType.Free)
            {
                if (Image.Size.Height > Image.Size.Width && (Zoom * Image.Size.Width / 100.0f) <= pictureBox1.Width)
                {
                    image_offset.X = (int)((pictureBox1.Width - (Zoom * Image.Size.Width / 100.0f)) / 2);
                }
                else if ((int)(Zoom * Image.Size.Height / 100.0f) <= pictureBox1.Height)
                {
                    image_offset.Y = (int)((pictureBox1.Height - (Zoom * Image.Size.Height / 100.0f)) / 2);
                }
            }

            int tmpWidth = PercentOfCalc(Image.Width, Zoom);
            int tmpHeight = PercentOfCalc(Image.Height, Zoom);
            if (image_offset.X > pictureBox1.Width - tmpWidth)
                image_offset.X = pictureBox1.Width - tmpWidth;
            if (image_offset.Y > pictureBox1.Height - tmpHeight)
                image_offset.Y = pictureBox1.Height - tmpHeight;
            if (image_offset.X < -(Zoom * Image.Width / 100) + tmpWidth)
                image_offset.X = (int)-(Zoom * Image.Width / 100) + tmpWidth;
            if (image_offset.Y < -(Zoom * Image.Height / 100) + tmpHeight)
                image_offset.Y = (int)-(Zoom * Image.Height / 100) + tmpHeight;


        }

        int PercentOfCalc(int value, double zoom)
        {
            return (int)(value * (zoom * .1) / 100); // return 20% (1/5) of image width with current zoom level
        }


        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (Image == null) return;
            //e.Graphics.Clear(Color.White);

            if (firstDraw)
            {
                firstDraw = false;
                Zoom = 100.0f * pictureBox1.Height / Image.Height;
                zoomToolStripMenuItem.Text = "Zoom: " + Math.Floor(Zoom) + "% (+/-)";
                CenterImage();
            }
            e.Graphics.DrawImage(Image, new Rectangle(image_offset, new Size((int)(Zoom * Image.Size.Width / 100.0f),
                (int)(Zoom * Image.Size.Height / 100.0f) + 1)), new Rectangle(new Point(0, 0), Image.Size),
                GraphicsUnit.Pixel);
            pictureBox1.Invalidate();


        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('+'))
            {
                Zoom *= ZoomValue;
                if (Zoom > 500) Zoom = 500;
                zoomToolStripMenuItem.Text = "Zoom: " + Math.Floor(Zoom) + "% (+/-)";
                //CenterImage();
            }
            if (e.KeyChar == '-')
            {
                Zoom /= ZoomValue;
                if (Zoom < 10) Zoom = 10;
                zoomToolStripMenuItem.Text = "Zoom: " + Math.Floor(Zoom) + "% (+/-)";
                //CenterImage();
            }

            CenterImage();

            pictureBox1.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.XButton1)
            {
                currentImageIndex = currentImageIndex > 0 ? currentImageIndex - 1 : filelist.Length - 1;

                Image = (Bitmap)Bitmap.FromFile(filelist[currentImageIndex]);
                zoomType = ZoomType.Center;

                firstDraw = true;

                pictureBox1.Invalidate();
            }
            if (keyData == Keys.Right || keyData == Keys.XButton2)
            {
                currentImageIndex = currentImageIndex < filelist.Length - 1 ? currentImageIndex + 1 : 0;

                Image = (Bitmap)Bitmap.FromFile(filelist[currentImageIndex]);
                zoomType = ZoomType.Center;

                firstDraw = true;

                pictureBox1.Invalidate();
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldZoom = Zoom;
            int sign = Math.Sign(e.Delta);
            Zoom *= sign > 0 ? ZoomValue : 1 / ZoomValue;
            if (Zoom > 500) Zoom = 500;
            if (Zoom < 10) Zoom = 10;
            zoomToolStripMenuItem.Text = "Zoom: " + Math.Floor(Zoom) + "% (+/-)";

            image_offset.X = (int)(image_offset.X * Zoom / oldZoom) + (int)(e.Location.X * (1 - Zoom / oldZoom));
            image_offset.Y = (int)(image_offset.Y * Zoom / oldZoom) + (int)(e.Location.Y * (1 - Zoom / oldZoom));

            CenterImage();

            pictureBox1.Invalidate();
        }

        // Horizontal Scrolling from
        // http://www.philosophicalgeek.com/2007/07/27/mouse-tilt-wheel-horizontal-scrolling-in-c/
        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
                if (m.HWnd != this.Handle)
                {
                    return;
                }
                switch (m.Msg)
                {
                    case Win32Messages.WM_MOUSEHWHEEL:
                        FireMouseHWheel(m.WParam, m.LParam);
                        m.Result = (IntPtr)1;
                        break;
                    default:
                        break;

                }
            }
            catch { };
        }

        public event EventHandler<MouseEventArgs> MouseHWheel;
        protected void FireMouseHWheel(IntPtr wParam, IntPtr lParam)
        {
            Int32 tilt = (Int16)Utils.HIWORD(wParam);
            Int32 keys = Utils.LOWORD(wParam);
            Int32 x = Utils.LOWORD(lParam);
            Int32 y = Utils.HIWORD(lParam);

            FireMouseHWheel(MouseButtons.None, 0, x, y, tilt);
        }

        protected void FireMouseHWheel(MouseButtons buttons, int clicks, int x, int y, int delta)
        {
            MouseEventArgs args = new MouseEventArgs(buttons,
                                         clicks, x, y, delta);
            OnMouseHWheel(args);
            //let everybody else have a crack at it 
            MouseHWheel?.Invoke(this, args);
        }
        protected virtual void OnMouseHWheel(MouseEventArgs e)
        {
            if (e.Delta <= -10)
            {
                currentImageIndex = currentImageIndex > 0 ? currentImageIndex - 1 : filelist.Length - 1;

                Image = (Bitmap)Bitmap.FromFile(filelist[currentImageIndex]);
                zoomType = ZoomType.Center;

                firstDraw = true;

                pictureBox1.Invalidate();
            }

            if (e.Delta >= 10)
            {
                currentImageIndex = currentImageIndex < filelist.Length - 1 ? currentImageIndex + 1 : 0;

                Image = (Bitmap)Bitmap.FromFile(filelist[currentImageIndex]);
                zoomType = ZoomType.Center;

                firstDraw = true;

                pictureBox1.Invalidate();
            }
        }
        // Horizontal scrolling END

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.XButton1)
            {
                currentImageIndex = currentImageIndex > 0 ? currentImageIndex - 1 : filelist.Length - 1;

                Image = (Bitmap)Bitmap.FromFile(filelist[currentImageIndex]);
                zoomType = ZoomType.Center;

                firstDraw = true;

                pictureBox1.Invalidate();
                return;
            }
            if (e.Button == MouseButtons.XButton2)
            {
                currentImageIndex = currentImageIndex < filelist.Length - 1 ? currentImageIndex + 1 : 0;

                Image = (Bitmap)Bitmap.FromFile(filelist[currentImageIndex]);
                zoomType = ZoomType.Center;

                firstDraw = true;

                pictureBox1.Invalidate();
                return;
            }


            zoomType = ZoomType.Free;
            pictureBox1.Capture = true;
            mouseLastPos = e.Location;
        }

        private void PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Capture = false;
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!pictureBox1.Capture || !(e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)) return;
            image_offset = Point.Add(image_offset, Size.Subtract((Size)e.Location, (Size)mouseLastPos));
            mouseLastPos = e.Location;
            CenterImage();
            pictureBox1.Invalidate();
        }

        private void FileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            // Reload when file changes
            if(e.ChangeType == WatcherChangeTypes.Created && extensions.Contains( Path.GetExtension( e.FullPath)) )
            {
                filelist = Directory.GetFiles(currentDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => extensions.Contains(Path.GetExtension(s))).ToArray();

                currentImageIndex = Array.IndexOf(filelist, imagefilepath);
            }
            if (e.ChangeType == WatcherChangeTypes.Renamed && extensions.Contains(Path.GetExtension(e.FullPath)))
            {
                filelist = Directory.GetFiles(currentDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => extensions.Contains(Path.GetExtension(s))).ToArray();

                currentImageIndex = Array.IndexOf(filelist, imagefilepath);
                if (currentImageIndex < 0) currentImageIndex = 0;
            }
            if (e.ChangeType == WatcherChangeTypes.Deleted && extensions.Contains(Path.GetExtension(e.FullPath)))
            {
                filelist = Directory.GetFiles(currentDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => extensions.Contains(Path.GetExtension(s))).ToArray();

                currentImageIndex = Array.IndexOf(filelist, imagefilepath);
            }

        }

        class FileSystemStruct
        {
            string path;
            bool Chached;
            Bitmap Bitmap;

        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {

                openFileDialog1.InitialDirectory = currentDir;
                openFileDialog1.Filter = "image files|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    imagefilepath = openFileDialog1.FileName;

                    currentDir = Path.GetDirectoryName(imagefilepath);

                    filelist = Directory.GetFiles(currentDir, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => extensions.Contains(Path.GetExtension(s))).ToArray();

                    currentImageIndex = Array.IndexOf(filelist, imagefilepath);

                    Image = (Bitmap)Bitmap.FromFile(imagefilepath);
                    zoomType = ZoomType.Center;

                    firstDraw = true;

                    pictureBox1.Invalidate();
                }
            }
        }

        private void PictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
                this.Activate();
            }
        }

        private void ZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomType = ZoomType.Center;
            this.firstDraw = true;
            pictureBox1.Invalidate();
        }

        private void FullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
                this.Activate();
            }
        }
    }
}
