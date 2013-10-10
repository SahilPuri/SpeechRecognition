using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace DigitalCircuitDesign
{
   public partial class SecondUIThreadForm : Form
   {
     static void Main2(object state)
     {
       Application.Run((Form)state);
     }

     public static SecondUIThreadForm Create()
     {
       SecondUIThreadForm form = new SecondUIThreadForm();
       Thread thread = new Thread(Main2);
       thread.SetApartmentState(ApartmentState.STA);
       thread.Start(form);
       return form;
     }

     public SecondUIThreadForm()
     {
       InitializeComponent();
       Text = "Recognizing...";
     }

     private void pictureBox1_Click(object sender, EventArgs e)
     {

     }
   }
   
    
}
