/* 
 * This file contains GIF functions for the image viewer
 */


using System;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ImageViewer1
{
    public partial class Form1 : Form
    {
        private bool isGIF = false;
        private FrameDimension fdimension;
        private int gifFrameCount = 1;  // Number of frames in the GIF image
        private int currentFrame = 0;   // Current frame displayed of GIF image
        int interval;                   // The interval between the frames of the GIF

        // Check wether current image is a gif and set everything up accordingly
        void setGIF()
        {
            if (!filelist[currentImageIndex].Extension.Equals(".gif"))
            {
                isGIF = false;
                timer1.Stop();
                timer1.Enabled = false;
                gIFPropertyToolStripMenuItem.Enabled = false;
                return;
            }
            if (firstDraw)
            {
                currentFrame = 1;
                fdimension = new FrameDimension(Image.FrameDimensionsList[0]);
                gifFrameCount = Image.GetFrameCount(fdimension);
                Image.SelectActiveFrame(fdimension, currentFrame);
                isGIF = false;
                PropertyItem[] t = Image.PropertyItems;
                for (int i = 0; i < t.Length; i++)
                {
                    if (t[i].Id == 0x5100) // Timing data
                    {
                        interval = (t[i].Value[0] + t[i].Value[1] * 256);
                        isGIF = true;
                    }
                }
                if (!isGIF) return;  
                // The file has a gif extension but no timing data so it will be displayed as a static image
            }

            //isGIF = true;
            gIFPropertyToolStripMenuItem.Enabled = true;

            timer1.Interval = interval;
            timer1.Start();
        }

        // Function called by timer
        private void Timer1_Tick(object sender, EventArgs e)
        {
            currentFrame = (currentFrame + 1) % gifFrameCount;
            Image.SelectActiveFrame(fdimension, currentFrame);
            pictureBox1.Invalidate();
        }

        // Display next frame
        private void NextFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFrame = (currentFrame + 1) % gifFrameCount;
            Image.SelectActiveFrame(fdimension, currentFrame);
            pictureBox1.Invalidate();
        }

        // Display previous frame
        private void PreviousFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFrame = (currentFrame - 1 + gifFrameCount) % gifFrameCount;
            Image.SelectActiveFrame(fdimension, currentFrame);
            pictureBox1.Invalidate();
        }

        // Start or stop playing the GIF
        private void StartStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = timer1.Enabled ? false : true;
        }

        // Set the speed defined by the GIF file
        private void PutValueHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PropertyItem item = Image.GetPropertyItem(0x5100);
            interval = (item.Value[0] + item.Value[1] * 256);
        }

        // Increase GIF play speed by decreasing interval
        private void IncreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            interval = Math.Max(interval - 10, 1);
            timer1.Interval = (int)interval;
        }

        // Decrease GIF play speed by increasing interval
        private void DecreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            interval = Math.Min(interval + 10, 100);
            timer1.Interval = (int)interval;
        }

    }
}
