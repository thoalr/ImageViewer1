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

namespace ImageViewer1
{
    public partial class Form1 : Form
    {
        string imagefilepath = @"C:\Users\thors\Documents\Frami\1.jpg";


        private Point image_offset = new Point(0, 0);
        Bitmap Image;
        float Zoom = 100;
        float ZoomValue = 1.3f;
        bool firstDraw = true;
        //Matrix transform = new Matrix();

        enum ZoomType
        {
            Center,
            Free
        }
        ZoomType zoomType = ZoomType.Center;


        Point offset = new Point(0, 0); // x y
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
            this.SuspendLayout();
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            this.ResumeLayout();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Image = (Bitmap) Bitmap.FromFile(imagefilepath);

            this.WindowState = FormWindowState.Maximized;
            //screenBuffer = new Bitmap(this.Width, this.Height);
            //Graphics g = Graphics.FromImage(screenBuffer);
            //g.Clear(Color.White);

            //Zoom = 100.0f * screenBuffer.Height / Image.Height;
            //CenterImage();

            //g.DrawImage(Image, new Rectangle(image_offset, new Size((int)(Zoom * Image.Size.Width / 100.0f),
            //    (int)(Zoom * Image.Size.Height / 100.0f) + 1)),
            //    new Rectangle(new Point(0, 0), Image.Size),
            //    GraphicsUnit.Pixel);

            //pictureBox1.Image = screenBuffer;

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
                    image_offset.X = (int)(((Zoom * Image.Size.Width / 100.0f) - pictureBox1.Width) / 2);
                    image_offset.Y = (int)(((Zoom * Image.Size.Height / 100.0f) - pictureBox1.Height) / 2);
                }
            }
            if(zoomType == ZoomType.Free)
            {
                if (Image.Size.Height > Image.Size.Width && (Zoom * Image.Size.Width / 100.0f) < pictureBox1.Width)
                {
                    image_offset.X = (int)((pictureBox1.Width - (Zoom * Image.Size.Width / 100.0f)) / 2);
                }
                else if((Zoom * Image.Size.Height / 100.0f) < pictureBox1.Height)
                {
                    image_offset.Y = (int)(((Zoom * Image.Size.Height / 100.0f) - pictureBox1.Height) / 2);
                }
            }

            if (image_offset.X > pictureBox1.Width) image_offset.X = pictureBox1.Width;
            if (image_offset.Y > pictureBox1.Height) image_offset.Y = pictureBox1.Height;
            if (image_offset.X < -(Zoom * Image.Width / 100)) image_offset.X = (int)(Zoom * Image.Width / 100);
            if (image_offset.Y < -(Zoom * Image.Height / 100)) image_offset.Y = (int)(Zoom * Image.Height / 100);


        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (Image == null) return;
            //e.Graphics.Clear(Color.White);

            if(firstDraw)
            {
                firstDraw = false;
                Zoom = 100.0f * pictureBox1.Height / Image.Height;
                //transform.Translate(image_offset.X, image_offset.Y);
                //transform.Scale(Zoom / 100, Zoom / 100);
                CenterImage();
            }

            //e.Graphics.Transform = transform;
            //e.Graphics.DrawImageUnscaled(Image, new Point(0, 0));
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
            }
            if (e.KeyChar == '-')
            {
                Zoom /= ZoomValue;
                if (Zoom < 10) Zoom = 10;
            }


            CenterImage();

            pictureBox1.Invalidate();
        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldZoom = Zoom;
            int sign = Math.Sign(e.Delta);
            Zoom *= sign > 0 ? ZoomValue : 1/ZoomValue;
            if (Zoom > 400) Zoom = 400;
            if (Zoom < 5) Zoom = 5;

            image_offset.X = (int)(image_offset.X * Zoom / oldZoom) + (int)( e.Location.X * (1 - Zoom / oldZoom));
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
    }
}
