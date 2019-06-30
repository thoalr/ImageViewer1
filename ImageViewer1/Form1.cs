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

        FileInfo imagefilepath;
        DirectoryInfo currentDir = new DirectoryInfo(@"C:\");
        int currentImageIndex = 0;
        FileInfo[] filelist;

        //Dictionary<string, Bitmap> imageCache;
        //MemoryCache imageCache = MemoryCache.Default;

        readonly string[] extensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };


        // Values for current image
        private Point image_offset = new Point(0, 0);
        Bitmap Image;
        float Zoom = 100;
        int maxZoom = 1000;
        int minZoom = 5;
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
            imagefilepath = new FileInfo(arg);

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

            while (imagefilepath == null)
            {
                using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
                {
                    openFileDialog1.InitialDirectory = currentDir.FullName;
                    openFileDialog1.Filter = "image files|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
                    openFileDialog1.FilterIndex = 1;
                    openFileDialog1.RestoreDirectory = true;

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        imagefilepath = new FileInfo( openFileDialog1.FileName);
                    }
                }
            }

            Image = (Bitmap)Bitmap.FromFile(imagefilepath.FullName);

            this.WindowState = FormWindowState.Maximized;


            //pictureBox1.Image = screenBuffer;

            currentDir = new DirectoryInfo( imagefilepath.DirectoryName);
            fileSystemWatcher1.Path = currentDir.FullName;
            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.NotifyFilter = NotifyFilters.LastAccess
                       | NotifyFilters.LastWrite
                       | NotifyFilters.FileName
                       | NotifyFilters.DirectoryName;

            newFileList();

            this.BringToFront();

            this.Text = "ImageViewer - " + imagefilepath.FullName;

            setGIF();

        }

        void newFileList()
        {
            if (currentDir == null) return;

            var tmp = currentDir.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                .Where(s => extensions.Contains(s.Extension));

            if (nameToolStripMenuItem.Checked)
            {
                if (ascendingToolStripMenuItem.Checked)
                    tmp = tmp.OrderBy(p => p.Name);
                else
                    tmp = tmp.OrderByDescending(p => p.Name);
            }
            else
            {
                if (ascendingToolStripMenuItem.Checked)
                    tmp = tmp.OrderBy(p => p.CreationTimeUtc);
                else
                    tmp = tmp.OrderByDescending(p => p.CreationTimeUtc);
            }

            filelist = tmp.ToArray(); 

            for(int i = 0; i < filelist.Length; i++)
            {
                if (filelist[i].FullName == imagefilepath.FullName)
                {
                    currentImageIndex = i;
                    break;
                }
            }
        }

        private void CenterImage()
        {
            if (Image == null) return;
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
            //pictureBox1.Invalidate();

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        

        private void FileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            // Reload when file changes
            if ((e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Renamed ||
                e.ChangeType == WatcherChangeTypes.Deleted) && extensions.Contains(Path.GetExtension(e.FullPath)))
            {
                newFileList();
            }

            if (currentImageIndex < 0)
            {
                if(isGIF) timer1.Stop();
                currentImageIndex = 0;
                setGIF();
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(isGIF) timer1.Stop();
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {

                openFileDialog1.InitialDirectory = currentDir.FullName;
                openFileDialog1.Filter = "image files|*.jpg;*.jpeg;*.png;*.gif|All files|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    imagefilepath = new FileInfo(openFileDialog1.FileName);

                    currentDir = new DirectoryInfo(imagefilepath.DirectoryName);

                    newFileList();

                    Image = (Bitmap)Bitmap.FromFile(imagefilepath.FullName);
                    zoomType = ZoomType.Center;

                    firstDraw = true;

                    pictureBox1.Invalidate();
                    fileSystemWatcher1.Path = currentDir.FullName;
                    this.Text = "ImageViewer " + imagefilepath.FullName;
                }
            }
            setGIF();
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

        private void NameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nameToolStripMenuItem.Checked) return;
            imagefilepath = new FileInfo(filelist[currentImageIndex].FullName);

            nameToolStripMenuItem.Checked = true;
            dateToolStripMenuItem.Checked = false;

            newFileList();

            //nameToolStripMenuItem.CheckState = CheckState.Checked;

        }

        private void DateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dateToolStripMenuItem.Checked) return;
            imagefilepath = new FileInfo(filelist[currentImageIndex].FullName);

            nameToolStripMenuItem.Checked = false;
            dateToolStripMenuItem.Checked = true;

            newFileList();

        }

        private void AscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ascendingToolStripMenuItem.Checked) return;
            imagefilepath = new FileInfo(filelist[currentImageIndex].FullName);
            Array.Reverse(filelist);
            for (int i = 0; i < filelist.Length; i++)
            {
                if (filelist[i].FullName == imagefilepath.FullName)
                {
                    currentImageIndex = i;
                    break;
                }
            }

            ascendingToolStripMenuItem.Checked = true;
            descendingToolStripMenuItem.Checked = false;
            //ascendingToolStripMenuItem.CheckState =
        }

        private void DescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (descendingToolStripMenuItem.Checked) return;
            imagefilepath = new FileInfo(filelist[currentImageIndex].FullName);
            Array.Reverse(filelist);
            for (int i = 0; i < filelist.Length; i++)
            {
                if (filelist[i].FullName == imagefilepath.FullName)
                {
                    currentImageIndex = i;
                    break;
                }
            }
            descendingToolStripMenuItem.Checked = true;
            ascendingToolStripMenuItem.Checked = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Image.Dispose();
            pictureBox1.Capture = false;
        }

        private void IncreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void PreviousFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
