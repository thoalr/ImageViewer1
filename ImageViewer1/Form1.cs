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
                    image_offset.Y = (int)(((Zoom * Image.Size.Height / 100.0f) - pictureBox1.Height) / 2);
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
            return (int)(value * zoom / (100*5)); // return 20% (1/5) of image width with current zoom level
        }


        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (Image == null) return;
            //e.Graphics.Clear(Color.White);

            if (firstDraw)
            {
                firstDraw = false;
                Zoom = 100.0f * pictureBox1.Height / Image.Height;
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
                if (Zoom > 400) Zoom = 400;
                //CenterImage();
            }
            if (e.KeyChar == '-')
            {
                Zoom /= ZoomValue;
                if (Zoom < 10) Zoom = 10;
                //CenterImage();
            }

            CenterImage();

            pictureBox1.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                currentImageIndex = currentImageIndex > 0 ? currentImageIndex - 1 : filelist.Length-1;

                Image = (Bitmap)Bitmap.FromFile(filelist[currentImageIndex]);
                zoomType = ZoomType.Center;

                firstDraw = true;

                pictureBox1.Invalidate();
            }
            if (keyData == Keys.Right)
            {
                currentImageIndex = currentImageIndex < filelist.Length-1 ? currentImageIndex + 1 : 0;

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
            if (Zoom > 400) Zoom = 400;
            if (Zoom < 5) Zoom = 5;

            image_offset.X = (int)(image_offset.X * Zoom / oldZoom) + (int)(e.Location.X * (1 - Zoom / oldZoom));
            image_offset.Y = (int)(image_offset.Y * Zoom / oldZoom) + (int)(e.Location.Y * (1 - Zoom / oldZoom));

            CenterImage();

            pictureBox1.Invalidate();
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
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
            if (!pictureBox1.Capture) return;
            image_offset = Point.Add(image_offset, Size.Subtract((Size)e.Location, (Size)mouseLastPos));
            mouseLastPos = e.Location;
            CenterImage();
            pictureBox1.Invalidate();
        }

        private void FileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {

        }

        class FileSystemStruct
        {
            string path;
            bool Chached;
            Bitmap Bitmap;

        }
    }
}
