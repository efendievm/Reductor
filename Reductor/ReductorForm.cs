using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reductor
{
    public partial class ReductorForm : Form
    {
        public ReductorForm()
        {
            InitializeComponent();
        }
        public void ShowReductor()
        {
            Visible = true;
            Left = Parent.Width / 2 - Width / 2;
            Top = Parent.Top / 2 - Top / 2;
            BringToFront();
        }

        private void ReductorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

    }
}
