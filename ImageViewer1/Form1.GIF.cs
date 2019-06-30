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
        bool isGIF = false;


        void setGIF()
        {

            ImageAnimator.CanAnimate(Image);
            

            isGIF = true;
        }



    }
}
