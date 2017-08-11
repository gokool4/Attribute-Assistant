using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace ArcGIS4LocalGovernment
{
    public partial class DependantValuesForm : Form
    {

        

        public DependantValuesForm()
        {
            InitializeComponent();
            
            
        }
 

  private void DependantValuesForm_Load(object sender, EventArgs e)
  {

  }

  public void button1_Click(object sender, EventArgs e)
  {
      if(comboBox1.SelectedItem!= null)
      {
        // GetValue(comboBox1.SelectedItem.ToString());
      }
     
  }

  public void button2_Click(object sender, EventArgs e)
  {
      this.Close();
  }

  //public void GetValue(IFeature Fc, string val)
  //{
  //    Fc.set_Value();
  //}
     
}
    
}
