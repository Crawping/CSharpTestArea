using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IISAppPools
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeCheck();            
            InitializeComponent();
            //Debug.Listeners.Add(new ConsoleTraceListener());
            FillControls();
        }

    }
}
