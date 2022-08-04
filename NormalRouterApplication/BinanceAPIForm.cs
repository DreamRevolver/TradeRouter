using System;
using System.Windows.Forms;
namespace RouterApplication
{
    public partial class BinanceAPIForm : Form
    {
        public BinanceAPIForm()
        {
            InitializeComponent();
            coefficientRadioButton.Checked = true;
            coefficientTextBox.Text = "1";
        }

        private void OKButton_Click(object sender, EventArgs e)
        {

        }

        private void fixedRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fixedRadioButton.Checked) 
            {
                coefficientRadioButton.Checked = false;
                fixedTextBox.Enabled = true;
                coefficientTextBox.Enabled = false;
                coefficientTextBox.Text = "";
            }
        }

        private void coefficientRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (coefficientRadioButton.Checked)
            {
                fixedRadioButton.Checked = false;
                coefficientTextBox.Enabled = true;
                fixedTextBox.Enabled = false;
                fixedTextBox.Text = "";
            }
        }

        private void fixedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void coefficientTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }
    }
}
