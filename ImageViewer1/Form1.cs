using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace ImageViewer1
{
    public partial class Form1 : Form
    {
        FileInfo imagefilepath;    // a mostly temporary variable for storing an image file path
        DirectoryInfo currentDir = new DirectoryInfo(@"C:\");
        int currentImageIndex = 0; // Index of the current image being viewed
        FileInfo[] filelist;       // contains a sorted array of all files in a directory

        // The file extensions this program will be able to read and display
        readonly string[] extensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };


        // Values for current image
        private Point image_offset = new Point(0, 0);
        Bitmap Image;            // The image to be displayed
        float Zoom = 100;        // The zoom of the image
        int maxZoom = 2000;      // The maximum allowed zoom
        int minZoom = 5;         // The minimum allowed zoom
        float ZoomValue = 1.3f;  // The step for increaing and decreasing the zoom
        bool firstDraw = true;   // Value that says wether this is the first drawing of the image

        enum ZoomType
        {
            Center, // The image is fixed in the center of the screen
            Free    // The image is allowed to move freely
        }
        ZoomType zoomType = ZoomType.Center; // The current zoom type

        // Default constructor
        public Form1()
        {
            InitializeComponent();
            Form1_Construct();
        }

        // Constructor for passing in commandline argument
        public Form1(string arg)
        {
            imagefilepath = new FileInfo(arg);

            InitializeComponent();
            Form1_Construct();
        }

        // Helper contructor to be called by all contructors
        private void Form1_Construct()
        {
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;          
        }

        // Loads the provided image or forces the user to select an image from a filedialog
        private void Form1_Load(object sender, EventArgs e)
        {
            while (imagefilepath == null)
            {
                openNewFile();
            }
            if (Path.HasExtension(imagefilepath.FullName))
                currentDir = imagefilepath.Directory;
            else
                currentDir = new DirectoryInfo(imagefilepath.FullName);

            // Populate filelist with all files in current directory whith allowed file extensions
            newFileList();

            // Load image
            Image = (Bitmap)Bitmap.FromFile(filelist[currentImageIndex].FullName);
            // Maximaise window
            this.WindowState = FormWindowState.Maximized;
            // Bring this form to the front of the screen
            this.BringToFront();

            updateFormText();

            setGIF(); // Check wether image is a GIF
        }


        private void updateFormText()
        {
            this.Text = "ImageViewer - " + imagefilepath.FullName + " - Zoom: " + Zoom;
        }

        void newFileList()
        {
            if (currentDir == null) return;
            // Get files and order them
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
            
            // Find index of current image
            for(int i = 0; i < filelist.Length; i++)
            {
                if (filelist[i].FullName == imagefilepath.FullName)
                {
                    currentImageIndex = i;
                    break;
                }
            }
            imagefilepath = filelist[currentImageIndex];
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
            // Keep images within bounds of screen
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
            return (int)(value * (zoom * .1) / 100); // return 10% of image size with current zoom level
        }

        // Overriden Paint method to handle the drawing of images ourselves
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
                updateFormText();
            }

            e.Graphics.DrawImage(Image, new Rectangle(image_offset, new Size((int)(Zoom * Image.Size.Width / 100.0f),
                (int)(Zoom * Image.Size.Height / 100.0f) + 1)), new Rectangle(new Point(0, 0), Image.Size),
                GraphicsUnit.Pixel);
            //pictureBox1.Invalidate();

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openNewFile();
        }

        // Go fullscreen or exit fullscreen
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

        // Go fullscreen or exit fullscreen
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

        // Set sorting order to name
        private void NameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nameToolStripMenuItem.Checked) return;
            imagefilepath = new FileInfo(filelist[currentImageIndex].FullName);

            nameToolStripMenuItem.Checked = true;
            dateToolStripMenuItem.Checked = false;

            newFileList();

            //nameToolStripMenuItem.CheckState = CheckState.Checked;

        }

        // Set sorting order to creation date
        private void DateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dateToolStripMenuItem.Checked) return;
            imagefilepath = new FileInfo(filelist[currentImageIndex].FullName);

            nameToolStripMenuItem.Checked = false;
            dateToolStripMenuItem.Checked = true;

            newFileList();

        }

        // Set sorting order to ascending
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

        // Set sorteing order to descending
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

        // This is probably unnecessary
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Image.Dispose();
            pictureBox1.Capture = false;
        }

        // Open filedialog to ask user to select new file
        private void openNewFile()
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
                    updateFormText();

                    setGIF();
                }
            }
        }

        // Refresh filelist
        private void RefreshDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFileList();
            
            if (currentImageIndex < 0)
            {
                if (isGIF) timer1.Stop();
                currentImageIndex = 0;
                setGIF();
            }
        }
    }
}
