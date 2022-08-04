using System;
using System.IO;
using System.Windows.Forms;
using NormalRouterApplication;
using Shared.Interfaces;
using WeifenLuo.WinFormsUI.Docking;
namespace RouterApplication
{
    public partial class MainForm : Form
    {
        private readonly ListenersWindow _listenersForm;
        private readonly NewLogForm _logForm;
        private readonly Positions _masterPositions;
        private readonly SlavePositions _slavePositions;
        private readonly MasterOrderForm _masterOpenOrders;
        private readonly SlaveOrderForm _slaveOpenOrders;
        public MainForm(ILogger logger)
        {
            InitializeComponent();
            _logForm = new NewLogForm();
            _masterPositions = new Positions(logger);
            _slavePositions = new SlavePositions(logger);
            _masterOpenOrders = new MasterOrderForm(logger);
            _slaveOpenOrders = new SlaveOrderForm(logger);
            _listenersForm = new ListenersWindow(logger, _masterPositions, _slavePositions, _masterOpenOrders, _slaveOpenOrders);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("SDRlayout.xml"))
            {
                dockPanel.LoadFromXml("SDRlayout.xml", GetContentFromPersistString);
            }
            else
            {
                _logForm.Show(dockPanel, DockState.DockBottom);
                _masterPositions.Show(dockPanel, DockState.Document);
                _masterOpenOrders.Show(_masterPositions.Pane, DockAlignment.Right, 0.5);
                _slavePositions.Show(dockPanel, DockState.DockTop);
                _slaveOpenOrders.Show(_slavePositions.Pane, DockAlignment.Right, 0.5);
                _listenersForm.Show(dockPanel, DockState.DockLeft);
            }
        }
        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(SlaveOrderForm).ToString())
            {
                return _slaveOpenOrders;
            }
            if (persistString == typeof(MasterOrderForm).ToString())
            {
                return _masterOpenOrders;
            }
            if (persistString == typeof(SlavePositions).ToString())
            {
                return _slavePositions;
            }
            if (persistString == typeof(Positions).ToString())
            {
                return _masterPositions;
            }
            if (persistString == typeof(NewLogForm).ToString())
            {
                return _logForm;
            }
            if (persistString == typeof(ListenersWindow).ToString())
            {
                return _listenersForm;
            }
            return null;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dockPanel.SaveAsXml("SDRlayout.xml");
            _listenersForm.Close();
        }

        private void dockPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
