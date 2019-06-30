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
        double interval;
        double fps = 24;
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
                return;
            }
            currentFrame = 1;

            fdimension = new FrameDimension(Image.FrameDimensionsList[0]);
            gifFrameCount = Image.GetFrameCount(fdimension);
            Image.SelectActiveFrame(fdimension, currentFrame);
            
            isGIF = true;

            PropertyItem item = Image.GetPropertyItem(0x5100);
            interval = (item.Value[0] + item.Value[1] * 256) * 10;
            //interval = 100 / fps;

            timer1.Interval = (int)interval;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            currentFrame = (currentFrame + 1) % gifFrameCount;
            Image.SelectActiveFrame(fdimension, currentFrame);
            pictureBox1.Invalidate();
        }



    }
}
