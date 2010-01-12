using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Latino.Visualization;

namespace BDocVisualizer
{
    public partial class MainWnd : Form
    {
        public MainWnd()
        {
            InitializeComponent();
        }

        public DrawableObjectViewer DrawableObjectViewer
        {
            get 
            { 
                return drawableObjectViewer; 
            }
        }

        private void drawableObjectViewer1_DrawableObjectTargetChanged(object sender, DrawableObjectEventArgs args)
        {
            string txt = "";
            foreach (Document obj in drawableObjectViewer.TargetObjects)
            {
                txt += string.Format("{0}: {1}\r\nOpposite: {2}\r\n\r\n", obj.Label, obj.Text, obj.OppositeKeywords);
            }
            txtInfo.Text = txt;
        }
    }
}