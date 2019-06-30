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
using System.Diagnostics;

namespace ImageViewer1
{
    public partial class Form1 : Form
    {
        private bool isGIF = false;
        private FrameDimension fdimension;
        private int gifFrameCount = 1;
        private int currentFrame = 0;
        //private bool gifIsPlaying = false;
        int interval;
        //Stopwatch stopwatch;
        long pTicks;
        long cTick;

        void setGIF()
        {
            if(!filelist[currentImageIndex].Extension.Equals(".gif"))
            {
                isGIF = false;
                timer1.Stop();
                timer1.Enabled = false;
                gIFPropertyToolStripMenuItem.Enabled = false;
                return;
            }
            if(firstDraw)
            {
                currentFrame = 1;
                fdimension = new FrameDimension(Image.FrameDimensionsList[0]);
                gifFrameCount = Image.GetFrameCount(fdimension);
                Image.SelectActiveFrame(fdimension, currentFrame);

                PropertyItem item = Image.GetPropertyItem(0x5100);
                interval = (item.Value[0] + item.Value[1] * 256);
            }

            isGIF = true;
            gIFPropertyToolStripMenuItem.Enabled = true;

            //interval = 100 / fps;

            timer1.Interval = interval;
            timer1.Start();
        }

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
