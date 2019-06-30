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
using System.Drawing.Imaging;

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

        private void Form1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '+': // zoom in
                    Zoom *= ZoomValue;
                    if (Zoom > maxZoom) Zoom = maxZoom;
                    zoomToolStripMenuItem.Text = "Zoom: " + Math.Floor(Zoom) + "% (+/-)";
                    CenterImage();
                    pictureBox1.Invalidate();
                    break;
                case '-': // zoom out
                    Zoom /= ZoomValue;
                    if (Zoom < minZoom) Zoom = minZoom;
                    zoomToolStripMenuItem.Text = "Zoom: " + Math.Floor(Zoom) + "% (+/-)";
                    CenterImage();
                    pictureBox1.Invalidate();
                    break;
                case 'o': // open new file from filedialog
                    if (isGIF) timer1.Stop();
                    openNewFile();
                    CenterImage();
                    setGIF();
                    pictureBox1.Invalidate();
                    break;
                case 's': // start stop timer for GIF
                    if(isGIF) timer1.Enabled = timer1.Enabled ? false : true;
                    break;
                case 'n': // next frame
                    if (isGIF)
                    {
                        currentFrame = (currentFrame + 1) % gifFrameCount;
                        Image.SelectActiveFrame(fdimension, currentFrame);
                        pictureBox1.Invalidate();
                    }
                    break;
                case 'b': // previous frame
                    if (isGIF)
                    {
                        currentFrame = (currentFrame - 1 + gifFrameCount) % gifFrameCount;
                        Image.SelectActiveFrame(fdimension, currentFrame);
                        pictureBox1.Invalidate();
                    }
                    break;
                case 'x': // default speed as set by file
                    if (isGIF)
                    {
                        PropertyItem item = Image.GetPropertyItem(0x5100);
                        interval = (item.Value[0] + item.Value[1] * 256);
                    }
                    break;
                case 'c': // speed increase so decrease interval
                    if (isGIF)
                    {
                        interval = Math.Max(interval - 2, 1);
                        timer1.Interval = interval;
                    }
                    break;
                case 'v': // speed decrease so increase interval
                    if (isGIF)
                    {
                        interval = Math.Min(interval + 2, 100);
                        timer1.Interval = interval;
                    }
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.XButton1)
            {
                if (isGIF) timer1.Stop();
                ChangeImage(-1);
                setGIF();
            }
            if (keyData == Keys.Right || keyData == Keys.XButton2)
            {
                if (isGIF) timer1.Stop();
                ChangeImage(1);
                setGIF();
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (isGIF) timer1.Stop();
            float oldZoom = Zoom;
            int sign = Math.Sign(e.Delta);
            Zoom *= sign > 0 ? ZoomValue : 1 / ZoomValue;
            if (Zoom > maxZoom) Zoom = maxZoom;
            if (Zoom < minZoom) Zoom = minZoom;
            zoomToolStripMenuItem.Text = "Zoom: " + Math.Floor(Zoom) + "% (+/-)";

            image_offset.X = (int)(image_offset.X * Zoom / oldZoom) + (int)(e.Location.X * (1 - Zoom / oldZoom));
            image_offset.Y = (int)(image_offset.Y * Zoom / oldZoom) + (int)(e.Location.Y * (1 - Zoom / oldZoom));

            CenterImage();
            pictureBox1.Invalidate();
            if (isGIF) timer1.Start();
        }

        // Horizontal Scrolling from
        // http://www.philosophicalgeek.com/2007/07/27/mouse-tilt-wheel-horizontal-scrolling-in-c/
        protected override void WndProc(ref Message m)
        {
            //try
            //{
            base.WndProc(ref m);
            if (this.IsDisposed || m.HWnd != this.Handle)
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
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("Error: " + e, "Error has ocurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //};
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
            //try
            //{
            if (e.Delta <= -10)
            {
                if (isGIF) timer1.Stop();
                ChangeImage(-1);
                setGIF();
            }

            if (e.Delta >= 10)
            {
                if (isGIF) timer1.Stop();
                ChangeImage(1);
                setGIF();
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error: " + ex, "Error has ocurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //};
        }
        // Horizontal scrolling END

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            zoomType = ZoomType.Free;
            pictureBox1.Capture = true;
            mouseLastPos = e.Location;
            //try
            //{
            if (e.Button == MouseButtons.XButton1)
            {
                if (isGIF) timer1.Stop();
                ChangeImage(-1);
                setGIF();
                return;
            }
            if (e.Button == MouseButtons.XButton2)
            {
                if (isGIF) timer1.Stop();
                ChangeImage(1);
                setGIF();
                return;
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error: " + ex, "Error has ocurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //};
        }

        private void ChangeImage(int direction)
        {

            currentImageIndex = (currentImageIndex + direction + filelist.Length) % (filelist.Length);

            Image = (Bitmap)Bitmap.FromFile(filelist[currentImageIndex].FullName);
            zoomType = ZoomType.Center;

            firstDraw = true;

            pictureBox1.Invalidate();

            this.Text = "ImageViewer - " + filelist[currentImageIndex].FullName;
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

    }
}
