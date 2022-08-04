using System;
using System.Windows.Forms;
using Shared.Core;
namespace RouterApplication
{
    public partial class FrontEndSettingsForm : Form
    {
        public BackEndClient _client;
        public FrontEndSettingsForm(BackEndClient client)
        {
            InitializeComponent();
            _client = client;
            //client.Settings.AvailableKeys.Select(i => dataGridView.Rows.Add(i, client.Settings.Get(i))).ToList();
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            foreach (var row in dataGridView.Rows) 
            {
                _client.Settings.Set((row as DataGridViewRow).Cells[0].Value as string, (row as DataGridViewRow).Cells[1].Value as string);
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrontEndSettingsForm_Load(object sender, EventArgs e)
        {
            dataGridView.Rows.Add("APIKey", _client.Settings.Get("APIKey"));
            dataGridView.Rows.Add("APISecret", _client.Settings.Get("APISecret"));
            dataGridView.Rows.Add("ModeValue", _client.Settings.Get("ModeValue"));
            dataGridView.Rows[0].ReadOnly = true;
            dataGridView.Rows[1].ReadOnly = true;
            dataGridView.Rows[2].ReadOnly = false;
        }
    }
}
