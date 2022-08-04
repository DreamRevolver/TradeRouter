using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AccountTracker;
using BinanceBackEnd;
using BinanceFrontEnd;
using BinanceFuturesConnector;
using NormalRouterApplication;
using Shared.Core;
using Shared.Interfaces;
using Shared.Models;

using SharedBinance.Interfaces;
using SharedBinance.Models.ProxyConfigurationSection;
using WeifenLuo.WinFormsUI.Docking;
using SharedBinance.Services;
using Supervisor;
using Utility.ExecutionContext;

namespace RouterApplication
{
    public partial class ListenersWindow : DockContent
    {
        private readonly FrontEndClient master;
        private AccountTrack accountTracker;
        private readonly Dictionary<string, BackEndClient> BackEndsList;
        private readonly Dictionary<string, IExecutionContext> queueList;
        private AccountLoader accountLoader;
        public ILogger Logger;
        public Positions masterPositionsForm;
        public SlavePositions slavePositionsForm;
        public MasterOrderForm masterOrdersForm;
        public SlaveOrderForm slaveOrdersForm;

        public ListenersWindow(ILogger logger, Positions masterPositions, SlavePositions slavePositions,
            MasterOrderForm masterOrder, SlaveOrderForm slaveOrder)
        {
            InitializeComponent();
            masterOrdersForm = masterOrder;
            masterPositionsForm = masterPositions;
            slaveOrdersForm = slaveOrder;
            slavePositionsForm = slavePositions;
            Logger = logger;
            ProxyWrapper.Configure((ProxyConfig)ConfigurationManager.GetSection("Proxy"),logger);
            accountLoader = new AccountLoader(Logger);
            BackEndsList = new Dictionary<string, BackEndClient>();
            queueList = new Dictionary<string, IExecutionContext>();
            var masterSettings = new List<(string key, string value)>
            {
                ("APIKey", ConfigurationManager.AppSettings["APIKey"]),
                ("APISecret", ConfigurationManager.AppSettings["APISecret"]),
                ("Url", ConfigurationManager.AppSettings["Url"]),
                ("Wss", ConfigurationManager.AppSettings["Wss"])
            };
            master = new BinanceFront(Logger, "Master", masterSettings);
            IExecutionContext TrackerQueue = new Worker(Logger);
            queueList.Add("tracker", TrackerQueue);
            accountTracker = new AccountTrack(TrackerQueue, Logger, master);
            slavePositionsForm.TabText = "Slave positions";
            masterPositionsForm.TabText = "Master positions";
            masterOrdersForm.TabText = "Master open orders";
            slaveOrdersForm.TabText = "Slave open orders";
            masterPositionsForm.SetConnector(master);
            masterOrdersForm.SetConnector(master);
            GenerateID();
            new PingSender(ConfigurationManager.AppSettings["RouterId"], Logger, ConfigurationManager.AppSettings);
        }

        private void GenerateID()
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings["RouterId"].Value == "")
                {
                    settings["RouterId"].Value = Guid.NewGuid().ToString();
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Logger.Log(LogPriority.Error, "Error writing id in app settings", "Id generation");
            }
        }

        private void startListenerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FrontEndsTreeView.SelectedNode != null)
            {
                //queueList[FrontEndsTreeView.SelectedNode.Name].Start();
                master.SignalEvent += BackEndsList[FrontEndsTreeView.SelectedNode.Name].PushSignal;
                master.LeverageChanged += BackEndsList[FrontEndsTreeView.SelectedNode.Name].ChangeLeverage;
                BackEndsList[FrontEndsTreeView.SelectedNode.Name].Start();
                slaveOrdersForm.SetConnector(BackEndsList[FrontEndsTreeView.SelectedNode.Name]);
                slaveOrdersForm.TabText = $"{BackEndsList[FrontEndsTreeView.SelectedNode.Name].Name} open orders";
                slavePositionsForm.SetConnector(BackEndsList[FrontEndsTreeView.SelectedNode.Name]);
                slavePositionsForm.TabText = $"{BackEndsList[FrontEndsTreeView.SelectedNode.Name].Name} positions";
                FrontEndsTreeView.Refresh();
            }
        }

        private void stopListenerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FrontEndsTreeView.SelectedNode != null && BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                master.SignalEvent -= BackEndsList[FrontEndsTreeView.SelectedNode.Name].PushSignal;
                master.LeverageChanged -= BackEndsList[FrontEndsTreeView.SelectedNode.Name].ChangeLeverage;
                BackEndsList[FrontEndsTreeView.SelectedNode.Name].Stop();
                FrontEndsTreeView.Refresh();
            }
        }

        private void deleteListenerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FrontEndsTreeView.SelectedNode != null)
            {
                if (BackEndsList.ContainsKey(FrontEndsTreeView.SelectedNode.Name))
                {
                    if (BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
                    {
                        MessageBox.Show(
                            "Please stop connector before deleting",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        var result = MessageBox.Show("Are you shure you want to delete account", "Delete",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            queueList[FrontEndsTreeView.SelectedNode.Name].Stop();
                            queueList.Remove(FrontEndsTreeView.SelectedNode.Name);
                            BackEndsList[FrontEndsTreeView.SelectedNode.Name].OrderUpdateEvent -= accountTracker.Check;
                            BackEndsList[FrontEndsTreeView.SelectedNode.Name].BalanceChangeEvent -=
                                accountTracker.HandleBalanceChange;
                            BackEndsList.Remove(FrontEndsTreeView.SelectedNode.Name);
                            FrontEndsTreeView.Nodes.Remove(FrontEndsTreeView.SelectedNode);
                            accountLoader.SaveToFile(BackEndsList);
                        }
                    }
                }
            }
        }

        private void FrontEndsTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            var node_here = FrontEndsTreeView.GetNodeAt(e.X, e.Y);
            FrontEndsTreeView.SelectedNode = node_here;
            if (node_here == null)
            {
                return;
            }

            if (e.Button == MouseButtons.Right && FrontEndsTreeView.SelectedNode.Level == 0)
            {
                treeViewContextMenuStrip.Show(FrontEndsTreeView, new Point(e.X, e.Y));
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                MessageBox.Show(
                    "Please stop connector to open settings",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                if (BackEndsList[FrontEndsTreeView.SelectedNode.Name].AllowChangeSettings())
                {
                    try
                    {
                        var newSettingsForm =
                            new FrontEndSettingsForm(BackEndsList[FrontEndsTreeView.SelectedNode.Name]);
                        newSettingsForm.ShowDialog();
                        if (newSettingsForm.DialogResult == DialogResult.OK)
                        {
                            BackEndsList.Remove(FrontEndsTreeView.SelectedNode.Name);
                            BackEndsList.Add(FrontEndsTreeView.SelectedNode.Name, newSettingsForm._client);
                            var settings = BackEndsList[FrontEndsTreeView.SelectedNode.Name].Settings.AvailableKeys;
                            foreach (TreeNode i in FrontEndsTreeView.SelectedNode.Nodes)
                            {
                                if (settings.Contains(i.Name))
                                {
                                    i.Text = BackEndsList[FrontEndsTreeView.SelectedNode.Name].Settings.Get(i.Name);
                                }
                            }

                            newSettingsForm.Dispose();
                            accountLoader.SaveToFile(BackEndsList);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogPriority.Error, ex, "ListenersWindow.settingsToolStripMenuItem_Click", null);
                        Logger.Log(LogPriority.Debug, ex, "ListenersWindow.settingsToolStripMenuItem_Click", null);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Please cancel all orders and close all positions before changing settings",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var newBinanceForm = new BinanceAPIForm();
                newBinanceForm.ShowDialog();
                if (newBinanceForm.DialogResult == DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(newBinanceForm.NameTextBox.Text) && !string.IsNullOrEmpty(newBinanceForm
                                                                                   .APIKeyTextBox.Text)
                                                                               && !string.IsNullOrEmpty(newBinanceForm
                                                                                   .APISecretTextBox.Text))
                    {
                        var slaveNode = new TreeNode
                        {
                            Name = newBinanceForm.NameTextBox.Text, Text = newBinanceForm.NameTextBox.Text,
                            Tag = "slave"
                        };
                        slaveNode.Nodes.Add(new TreeNode {Name = "APIKey", Text = newBinanceForm.APIKeyTextBox.Text});
                        slaveNode.Nodes.Add(new TreeNode
                            {Name = "APISecret", Text = newBinanceForm.APISecretTextBox.Text});
                        var settings = new List<(string key, string value)>
                        {
                            ("APIKey", newBinanceForm.APIKeyTextBox.Text),
                            ("APISecret", newBinanceForm.APISecretTextBox.Text),
                            ("Url", ConfigurationManager.AppSettings["Url"]),
                            ("Wss", ConfigurationManager.AppSettings["Wss"]),
                            ("CopyMasterOrders", ConfigurationManager.AppSettings["CopyMasterOrders"])
                        };
                        if (newBinanceForm.fixedRadioButton.Checked)
                        {
                            settings.Add(("Mode", "Fixed"));
                            settings.Add(("ModeValue", newBinanceForm.fixedTextBox.Text));
                            slaveNode.Nodes.Add(new TreeNode {Name = "Mode", Text = "Fixed"});
                            slaveNode.Nodes.Add(new TreeNode
                                {Name = "ModeValue", Text = newBinanceForm.fixedTextBox.Text});
                        }
                        else
                        {
                            settings.Add(("Mode", "Amt Multiplier"));
                            settings.Add(("ModeValue", newBinanceForm.coefficientTextBox.Text));
                            slaveNode.Nodes.Add(new TreeNode {Name = "Mode", Text = "Amt Multiplier"});
                            slaveNode.Nodes.Add(new TreeNode
                                {Name = "ModeValue", Text = newBinanceForm.coefficientTextBox.Text});
                        }

                        slaveNode.Nodes.Add(new TreeNode
                        {
                            Name = "CopyMasterOrders", Text = $"{ConfigurationManager.AppSettings["CopyMasterOrders"]}"
                        });
                        IExecutionContext newQueue = new Worker(Logger);
                        //newQueue.Start();
                        queueList.Add(newBinanceForm.NameTextBox.Text, newQueue);
                        BackEndClient slave = new BinanceBack(newQueue, Logger, newBinanceForm.NameTextBox.Text,
                            settings, master);
                        if (BackEndsList.ContainsKey(newBinanceForm.NameTextBox.Text) ||
                            FrontEndsTreeView.Nodes.Contains(new TreeNode
                                {Tag = "slave", Name = newBinanceForm.NameTextBox.Text}))
                        {
                            MessageBox.Show(
                                "Current name already exist. Your object wasn't added",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            BackEndsList.Add(newBinanceForm.NameTextBox.Text, slave);
                            FrontEndsTreeView.Nodes.Add(slaveNode);
                            accountLoader.SaveToFile(BackEndsList);
                            slave.ConnectionEvent += ev =>
                            {
                                Logger.Log(LogPriority.Info, ev.ToString(), slave.Name);
                                switch (ev)
                                {
                                    case ConnectionEvent.Started:
                                        FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.LightGray;
                                        break;
                                    case ConnectionEvent.Stopped:
                                        FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.White;
                                        //queueList[slave.Name].Stop();
                                        break;
                                    case ConnectionEvent.Logon:
                                        FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.Green;
                                        break;
                                    case ConnectionEvent.Logout:
                                        FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.Red;
                                        break;
                                    default:
                                        FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.White;
                                        break;
                                }
                            };
                            slave.OrderUpdateEvent += accountTracker.Check;
                            slave.BalanceChangeEvent += accountTracker.HandleBalanceChange;
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "All fields must be filled",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    newBinanceForm.Dispose();
                }
                else if (newBinanceForm.DialogResult == DialogResult.Cancel)
                {
                    newBinanceForm.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogPriority.Error, ex, "ListenersWindow.addToolStripMenuItem_Click", null);
                Logger.Log(LogPriority.Debug, ex, "ListenersWindow.addToolStripMenuItem_Click", null);
            }
        }

        private void ListenersWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            balanceTimer.Stop();
            try
            {
                foreach (var x in BackEndsList)
                {
                    if (BackEndsList[x.Key].IsRunning)
                    {
                        BackEndsList[x.Key].Stop();
                        //queueList[x.Key].Stop();
                    }
                }
                master.Stop();
                foreach (TreeNode node in FrontEndsTreeView.Nodes)
                {
                    if (node.Level == 0)
                    {
                        node.Text = node.Name;
                    }
                }

                accountLoader.SaveToFile(BackEndsList);
                Logger.Log(LogPriority.Debug, "Application was closed manually", "ListenerWindow");
            }
            catch (Exception ex)
            {
                Logger.Log(LogPriority.Error, ex, "ListenersWindow.ListenersWindow_FormClosing", null);
                Logger.Log(LogPriority.Debug, ex, "ListenersWindow.ListenersWindow_FormClosing", null);
            }
        }

        public IEnumerable<TreeNode> GetChildren(TreeNode _parent)
            => _parent.Nodes.Cast<TreeNode>().Concat(
                _parent.Nodes.Cast<TreeNode>().SelectMany(GetChildren));

        private void ListenersWindow_Load(object sender, EventArgs e)
        {
            master.Start();
            accountTracker._queue.Start();
            IdLabel.Text = ConfigurationManager.AppSettings["RouterId"];
            master.ConnectionEvent += ev =>
            {
                Logger.Log(LogPriority.Info, ev.ToString(), "Master");
                BeginInvoke((Action)(() =>
                {
                    masterStatusToolStripStatusLabel.Text = ev.ToString();
                }));
            };
            try
            {
                var slaveIdentities = accountLoader.LoadFromFile();
                foreach (var identity in slaveIdentities)
                {
                    var settings = new List<(string key, string value)>
                    {
                        ("APIKey", identity.ApiKey),
                        ("APISecret", identity.ApiSecret),
                        ("Mode", identity.Mode),
                        ("ModeValue", identity.ModeValue),
                        ("Url", ConfigurationManager.AppSettings["Url"]),
                        ("Wss", ConfigurationManager.AppSettings["Wss"]),
                        ("CopyMasterOrders", ConfigurationManager.AppSettings["CopyMasterOrders"]),
                        ("RetryAttempts", ConfigurationManager.AppSettings["RetryAttempts"]),
                        ("RetryDelayMs", ConfigurationManager.AppSettings["RetryDelayMs"])
                    };
                    IExecutionContext newQueue = new Worker(Logger);
                    queueList.Add(identity.Name, newQueue);
                    BackEndClient slave = new BinanceBack(newQueue, Logger, identity.Name, settings, master);
                    BackEndsList.Add(slave.Name, slave);
                    slave.OrderUpdateEvent += accountTracker.Check;
                    slave.BalanceChangeEvent += accountTracker.HandleBalanceChange;
                    var slaveNode = new TreeNode {Name = identity.Name, Text = identity.Name};
                    slaveNode.Nodes.Add(new TreeNode {Name = "APIKey", Text = identity.ApiKey});
                    slaveNode.Nodes.Add(new TreeNode {Name = "APISecret", Text = identity.ApiSecret});
                    slaveNode.Nodes.Add(new TreeNode {Name = "Mode", Text = identity.Mode});
                    slaveNode.Nodes.Add(new TreeNode {Name = "ModeValue", Text = identity.ModeValue});
                    slaveNode.Nodes.Add(new TreeNode {Name = "CopyMasterOrders", Text = identity.CopyMasterOrders});
                    FrontEndsTreeView.Nodes.Add(slaveNode);
                    slave.ConnectionEvent += ev =>
                    {
                        Logger.Log(LogPriority.Info, ev.ToString(), slave.Name);
                        switch (ev)
                        {
                            case ConnectionEvent.Started:
                                FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.LightGray;
                                break;
                            case ConnectionEvent.Stopped:
                                FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.White;
                                break;
                            case ConnectionEvent.Logon:
                                FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.Green;
                                break;
                            case ConnectionEvent.Logout:
                                FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.Red;
                                break;
                            default:
                                FrontEndsTreeView.Nodes.Find(slave.Name, false)[0].BackColor = Color.White;
                                break;
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogPriority.Error, ex, "ListenersWindow.ListenersWindow_Load", null);
                Logger.Log(LogPriority.Debug, ex, "ListenersWindow.ListenersWindow_Load", null);
            }

            balanceTimer.Start();
        }

        private void statusStrip1_DoubleClick(object sender, EventArgs e)
        {
            if (master.IsRunning)
            {
                master.Stop();
            }
            else
            {
                master.Start();
            }
        }

        private void statusStrip1_MouseHover(object sender, EventArgs e)
        {
            var t = new ToolTip();
            t.SetToolTip(statusStrip1, "Double click to start/stop connector");
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            slavePositionsForm.SetConnector(BackEndsList[FrontEndsTreeView.SelectedNode.Name]);
            slavePositionsForm.TabText = $"{BackEndsList[FrontEndsTreeView.SelectedNode.Name].Name} positions";
            slaveOrdersForm.SetConnector(BackEndsList[FrontEndsTreeView.SelectedNode.Name]);
            slaveOrdersForm.TabText = $"{BackEndsList[FrontEndsTreeView.SelectedNode.Name].Name} open orders";
        }

        private void balanceTimer_Tick(object sender, EventArgs e)
        {
            foreach (TreeNode node in FrontEndsTreeView.Nodes)
            {
                if (node.Level == 0)
                {
                    var USDTbalance = BackEndsList[node.Name].GetBalanceValue(WalletCurrency.USDT);
                    node.Text = node.Name + " USDT: " + USDTbalance;
                }
            }
        }

        private void closeAllOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FrontEndsTreeView.SelectedNode != null && BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                var result = MessageBox.Show("Are you shure you want to cancel all open orders?",
                    "Cancel all open orders", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    BackEndsList[FrontEndsTreeView.SelectedNode.Name].CancelAllOrders();
                }
            }

            if (!BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                MessageBox.Show(
                    "Please start connector to cancel all orders",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void closeAllPositionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FrontEndsTreeView.SelectedNode != null && BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                var result = MessageBox.Show("Are you shure you want to close all positions?", "Close all positions",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    BackEndsList[FrontEndsTreeView.SelectedNode.Name].CloseAllPositions();
                }
            }

            if (!BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                MessageBox.Show(
                    "Please start connector to close all positions",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private async void startAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var backEnd in BackEndsList)
                {
                    if (!backEnd.Value.IsRunning)
                    {
                        master.SignalEvent += backEnd.Value.PushSignal;
                        master.LeverageChanged += backEnd.Value.ChangeLeverage;
                        queueList[backEnd.Value.Name].Start();
                        await backEnd.Value.Start();
                        await Task.Delay(6000);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogPriority.Error, ex, "ListenersWindow.startAllToolStripMenuItem1_Click",
                    "Failed to start all slaves. Try again.");
                Logger.Log(LogPriority.Debug, ex, "ListenersWindow.startAllToolStripMenuItem1_Click",
                    "Failed to start all slaves. Try again.");
            }
        }

        private async void cancelAllOrdersToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you shure you want to cancel ALL OPEN ORDERS in ALL SLAVES?",
                "Cancel all open orders", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (var backEnd in BackEndsList)
                    {
                        if (!backEnd.Value.IsRunning)
                        {
                            MessageBox.Show(
                                $"Please start connector {backEnd.Value.Name} to cancel all orders",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            await backEnd.Value.CancelAllOrders();
                            await Task.Delay(3000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogPriority.Error, ex, "ListenersWindow.cancelAllOrdersToolStripMenuItem1_Click",
                        "Failed to cancel all orders in all slaves. Try again.");
                    Logger.Log(LogPriority.Debug, ex, "ListenersWindow.cancelAllOrdersToolStripMenuItem1_Click",
                        "Failed to cancel all orders in all slaves. Try again.");
                }
            }
        }

        private async void closeAllPositionsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you shure you want to close ALL POSITIONS in ALL SLAVES?",
                "Close all positions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (var backEnd in BackEndsList)
                    {
                        if (!backEnd.Value.IsRunning)
                        {
                            MessageBox.Show(
                                $"Please start connector {backEnd.Value.Name} to close all positions",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            await backEnd.Value.CloseAllPositions();
                            await Task.Delay(3000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogPriority.Error, ex, "ListenersWindow.closeAllPositionsToolStripMenuItem2_Click",
                        "Failed to close all positions in all slaves. Try again.");
                    Logger.Log(LogPriority.Debug, ex, "ListenersWindow.closeAllPositionsToolStripMenuItem2_Click",
                        "Failed to close all positions in all slaves. Try again.");
                }
            }
        }

        private void stopAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you shure you want to STOP ALL SLAVES?", "Stop all",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (var backEnd in BackEndsList)
                    {
                        if (backEnd.Value.IsRunning)
                        {
                            backEnd.Value.Stop();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogPriority.Error, ex, "ListenersWindow.stopAllToolStripMenuItem_Click",
                        "Failed to stop all slaves. Try again.");
                    Logger.Log(LogPriority.Debug, ex, "ListenersWindow.stopAllToolStripMenuItem_Click",
                        "Failed to stop all slaves. Try again.");
                }
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
            => accountTracker.Start();

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
            => accountTracker.Stop();

        private void synchronizeOrdersToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (FrontEndsTreeView.SelectedNode != null && BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                var result = MessageBox.Show("Are you shure you want to synchronize all open orders?",
                    "Synchronize orders", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    BackEndsList[FrontEndsTreeView.SelectedNode.Name].SynchronizeOrders();
                }
            }

            if (!BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                MessageBox.Show(
                    "Please start connector to synchronize all open orders",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void synchronizePositionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FrontEndsTreeView.SelectedNode != null && BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                var result = MessageBox.Show("Are you shure you want to synchronize all positions?",
                    "Synchronize positions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    BackEndsList[FrontEndsTreeView.SelectedNode.Name].SynchronizePositions();
                }
            }

            if (!BackEndsList[FrontEndsTreeView.SelectedNode.Name].IsRunning)
            {
                MessageBox.Show(
                    "Please start connector to synchronize positions",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void synchronizeOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you shure you want to synchronize all open orders in ALL SLAVES?",
                "Synchronize all orders", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (var backEnd in BackEndsList)
                    {
                        if (!backEnd.Value.IsRunning)
                        {
                            MessageBox.Show(
                                $"Please start connector {backEnd.Value.Name} to synchronize all open orders",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            backEnd.Value.SynchronizeOrders();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogPriority.Error, ex, "ListenersWindow.synchronizeOrdersToolStripMenuItem_Click",
                        "Failed to synchronize all open orders in ALL SLAVES. Try again.");
                    Logger.Log(LogPriority.Debug, ex, "ListenersWindow.synchronizeOrdersToolStripMenuItem_Click",
                        "Failed to synchronize all open orders in ALL SLAVES. Try again.");
                }
            }
        }

        private void synchronizeAllPositionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you shure you want to synchronize all positions in ALL SLAVES?",
                "Synchronize all positions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (var backEnd in BackEndsList)
                    {
                        if (!backEnd.Value.IsRunning)
                        {
                            MessageBox.Show(
                                $"Please start connector {backEnd.Value.Name} to synchronize all positions",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            backEnd.Value.SynchronizePositions();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogPriority.Error, ex, "ListenersWindow.synchronizeAllPositionsToolStripMenuItem_Click",
                        "Failed to synchronize all positions in ALL SLAVES. Try again.");
                    Logger.Log(LogPriority.Debug, ex, "ListenersWindow.synchronizeAllPositionsToolStripMenuItem_Click",
                        "Failed to synchronize all positions in ALL SLAVES. Try again.");
                }
            }
        }
    }
}
