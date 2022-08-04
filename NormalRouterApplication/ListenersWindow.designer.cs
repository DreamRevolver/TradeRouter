
using System.ComponentModel;
using System.Windows.Forms;
namespace RouterApplication
{
    partial class ListenersWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
            {
                this.components = new System.ComponentModel.Container();
                this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
                this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.startAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
                this.stopAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.cancelAllOrdersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
                this.closeAllPositionsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
                this.synchronizeOrdersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.synchronizeAllPositionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.trackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.FrontEndsTreeView = new System.Windows.Forms.TreeView();
                this.treeViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
                this.startListenerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.stopListenerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.deleteListenerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.closeAllOrdersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.closeAllPositionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.synchronizeOrdersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
                this.synchronizePositionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.statusStrip1 = new System.Windows.Forms.StatusStrip();
                this.masterToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
                this.masterStatusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
                this.IDtoolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
                this.IdLabel = new System.Windows.Forms.ToolStripStatusLabel();
                this.balanceTimer = new System.Windows.Forms.Timer(this.components);
                this.timer1 = new System.Windows.Forms.Timer(this.components);
                this.mainMenuStrip.SuspendLayout();
                this.treeViewContextMenuStrip.SuspendLayout();
                this.statusStrip1.SuspendLayout();
                this.SuspendLayout();
                // 
                // mainMenuStrip
                // 
                this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(40, 40);

                this.mainMenuStrip.Items.AddRange(
                    new System.Windows.Forms.ToolStripItem[]
                    {
                        this.addToolStripMenuItem, this.menuToolStripMenuItem, this.trackerToolStripMenuItem
                    }
                );

                this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
                this.mainMenuStrip.Name = "mainMenuStrip";
                this.mainMenuStrip.Size = new System.Drawing.Size(514, 24);
                this.mainMenuStrip.TabIndex = 0;
                this.mainMenuStrip.Text = "menuStrip1";
                // 
                // addToolStripMenuItem
                // 
                this.addToolStripMenuItem.Name = "addToolStripMenuItem";
                this.addToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
                this.addToolStripMenuItem.Text = "Add";
                this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);

                // 
                // menuToolStripMenuItem
                // 
                this.menuToolStripMenuItem.DropDownItems.AddRange(
                    new System.Windows.Forms.ToolStripItem[]
                    {
                        this.startAllToolStripMenuItem1, this.stopAllToolStripMenuItem, this.cancelAllOrdersToolStripMenuItem1, this.closeAllPositionsToolStripMenuItem2, this.synchronizeOrdersToolStripMenuItem, this.synchronizeAllPositionsToolStripMenuItem
                    }
                );

                this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
                this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
                this.menuToolStripMenuItem.Text = "Menu";
                // 
                // startAllToolStripMenuItem1
                // 
                this.startAllToolStripMenuItem1.Name = "startAllToolStripMenuItem1";
                this.startAllToolStripMenuItem1.Size = new System.Drawing.Size(204, 22);
                this.startAllToolStripMenuItem1.Text = "Start all";
                this.startAllToolStripMenuItem1.Click += new System.EventHandler(this.startAllToolStripMenuItem1_Click);
                // 
                // stopAllToolStripMenuItem
                // 
                this.stopAllToolStripMenuItem.Name = "stopAllToolStripMenuItem";
                this.stopAllToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
                this.stopAllToolStripMenuItem.Text = "Stop all";
                this.stopAllToolStripMenuItem.Click += new System.EventHandler(this.stopAllToolStripMenuItem_Click);
                // 
                // cancelAllOrdersToolStripMenuItem1
                // 
                this.cancelAllOrdersToolStripMenuItem1.Name = "cancelAllOrdersToolStripMenuItem1";
                this.cancelAllOrdersToolStripMenuItem1.Size = new System.Drawing.Size(204, 22);
                this.cancelAllOrdersToolStripMenuItem1.Text = "Cancel all orders";
                this.cancelAllOrdersToolStripMenuItem1.Click += new System.EventHandler(this.cancelAllOrdersToolStripMenuItem1_Click);
                // 
                // closeAllPositionsToolStripMenuItem2
                // 
                this.closeAllPositionsToolStripMenuItem2.Name = "closeAllPositionsToolStripMenuItem2";
                this.closeAllPositionsToolStripMenuItem2.Size = new System.Drawing.Size(204, 22);
                this.closeAllPositionsToolStripMenuItem2.Text = "Close all positions";
                this.closeAllPositionsToolStripMenuItem2.Click += new System.EventHandler(this.closeAllPositionsToolStripMenuItem2_Click);
                // 
                // synchronizeOrdersToolStripMenuItem
                // 
                this.synchronizeOrdersToolStripMenuItem.Name = "synchronizeOrdersToolStripMenuItem";
                this.synchronizeOrdersToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
                this.synchronizeOrdersToolStripMenuItem.Text = "Synchronize all orders";
                this.synchronizeOrdersToolStripMenuItem.Click += new System.EventHandler(this.synchronizeOrdersToolStripMenuItem_Click);
                // 
                // synchronizeAllPositionsToolStripMenuItem
                // 
                this.synchronizeAllPositionsToolStripMenuItem.Name = "synchronizeAllPositionsToolStripMenuItem";
                this.synchronizeAllPositionsToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
                this.synchronizeAllPositionsToolStripMenuItem.Text = "Synchronize all positions";
                this.synchronizeAllPositionsToolStripMenuItem.Click += new System.EventHandler(this.synchronizeAllPositionsToolStripMenuItem_Click);

                // 
                // trackerToolStripMenuItem
                // 
                this.trackerToolStripMenuItem.DropDownItems.AddRange(
                    new System.Windows.Forms.ToolStripItem[]
                    {
                        this.startToolStripMenuItem, this.stopToolStripMenuItem
                    }
                );

                this.trackerToolStripMenuItem.Name = "trackerToolStripMenuItem";
                this.trackerToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
                this.trackerToolStripMenuItem.Text = "Tracker";
                // 
                // startToolStripMenuItem
                // 
                this.startToolStripMenuItem.Name = "startToolStripMenuItem";
                this.startToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
                this.startToolStripMenuItem.Text = "Start";
                this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
                // 
                // stopToolStripMenuItem
                // 
                this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
                this.stopToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
                this.stopToolStripMenuItem.Text = "Stop";
                this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
                // 
                // FrontEndsTreeView
                // 
                this.FrontEndsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
                this.FrontEndsTreeView.FullRowSelect = true;
                this.FrontEndsTreeView.HideSelection = false;
                this.FrontEndsTreeView.Location = new System.Drawing.Point(0, 24);
                this.FrontEndsTreeView.Name = "FrontEndsTreeView";
                this.FrontEndsTreeView.ShowLines = false;
                this.FrontEndsTreeView.ShowPlusMinus = false;
                this.FrontEndsTreeView.ShowRootLines = false;
                this.FrontEndsTreeView.Size = new System.Drawing.Size(514, 265);
                this.FrontEndsTreeView.TabIndex = 1;
                this.FrontEndsTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrontEndsTreeView_MouseDown);
                // 
                // treeViewContextMenuStrip
                // 
                this.treeViewContextMenuStrip.ImageScalingSize = new System.Drawing.Size(40, 40);

                this.treeViewContextMenuStrip.Items.AddRange(
                    new System.Windows.Forms.ToolStripItem[]
                    {
                        this.startListenerToolStripMenuItem, this.stopListenerToolStripMenuItem, this.deleteListenerToolStripMenuItem, this.settingsToolStripMenuItem, this.showToolStripMenuItem, this.closeAllOrdersToolStripMenuItem, this.closeAllPositionsToolStripMenuItem, this.synchronizeOrdersToolStripMenuItem1, this.synchronizePositionsToolStripMenuItem
                    }
                );

                this.treeViewContextMenuStrip.Name = "treeViewContextMenuStrip";
                this.treeViewContextMenuStrip.Size = new System.Drawing.Size(190, 202);
                // 
                // startListenerToolStripMenuItem
                // 
                this.startListenerToolStripMenuItem.Name = "startListenerToolStripMenuItem";
                this.startListenerToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
                this.startListenerToolStripMenuItem.Text = "Start";
                this.startListenerToolStripMenuItem.Click += new System.EventHandler(this.startListenerToolStripMenuItem_Click);
                // 
                // stopListenerToolStripMenuItem
                // 
                this.stopListenerToolStripMenuItem.Name = "stopListenerToolStripMenuItem";
                this.stopListenerToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
                this.stopListenerToolStripMenuItem.Text = "Stop";
                this.stopListenerToolStripMenuItem.Click += new System.EventHandler(this.stopListenerToolStripMenuItem_Click);
                // 
                // deleteListenerToolStripMenuItem
                // 
                this.deleteListenerToolStripMenuItem.Name = "deleteListenerToolStripMenuItem";
                this.deleteListenerToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
                this.deleteListenerToolStripMenuItem.Text = "Delete";
                this.deleteListenerToolStripMenuItem.Click += new System.EventHandler(this.deleteListenerToolStripMenuItem_Click);
                // 
                // settingsToolStripMenuItem
                // 
                this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
                this.settingsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
                this.settingsToolStripMenuItem.Text = "Settings";
                this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
                // 
                // showToolStripMenuItem
                // 
                this.showToolStripMenuItem.Name = "showToolStripMenuItem";
                this.showToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
                this.showToolStripMenuItem.Text = "Show";
                this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
                // 
                // closeAllOrdersToolStripMenuItem
                // 
                this.closeAllOrdersToolStripMenuItem.Name = "closeAllOrdersToolStripMenuItem";
                this.closeAllOrdersToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
                this.closeAllOrdersToolStripMenuItem.Text = "Cancel orders";
                this.closeAllOrdersToolStripMenuItem.Click += new System.EventHandler(this.closeAllOrdersToolStripMenuItem_Click);
                // 
                // closeAllPositionsToolStripMenuItem
                // 
                this.closeAllPositionsToolStripMenuItem.Name = "closeAllPositionsToolStripMenuItem";
                this.closeAllPositionsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
                this.closeAllPositionsToolStripMenuItem.Text = "Close positions";
                this.closeAllPositionsToolStripMenuItem.Click += new System.EventHandler(this.closeAllPositionsToolStripMenuItem_Click);
                // 
                // synchronizeOrdersToolStripMenuItem1
                // 
                this.synchronizeOrdersToolStripMenuItem1.Name = "synchronizeOrdersToolStripMenuItem1";
                this.synchronizeOrdersToolStripMenuItem1.Size = new System.Drawing.Size(189, 22);
                this.synchronizeOrdersToolStripMenuItem1.Text = "Synchronize orders";
                this.synchronizeOrdersToolStripMenuItem1.Click += new System.EventHandler(this.synchronizeOrdersToolStripMenuItem1_Click);
                // 
                // synchronizePositionsToolStripMenuItem
                // 
                this.synchronizePositionsToolStripMenuItem.Name = "synchronizePositionsToolStripMenuItem";
                this.synchronizePositionsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
                this.synchronizePositionsToolStripMenuItem.Text = "Synchronize positions";
                this.synchronizePositionsToolStripMenuItem.Click += new System.EventHandler(this.synchronizePositionsToolStripMenuItem_Click);
                // 
                // statusStrip1
                // 
                this.statusStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);

                this.statusStrip1.Items.AddRange(
                    new System.Windows.Forms.ToolStripItem[]
                    {
                        this.masterToolStripStatusLabel, this.masterStatusToolStripStatusLabel, this.IDtoolStripStatusLabel, this.IdLabel
                    }
                );

                this.statusStrip1.Location = new System.Drawing.Point(0, 292);
                this.statusStrip1.Name = "statusStrip1";
                this.statusStrip1.Size = new System.Drawing.Size(514, 22);
                this.statusStrip1.TabIndex = 2;
                this.statusStrip1.Text = "statusStrip1";
                this.statusStrip1.DoubleClick += new System.EventHandler(this.statusStrip1_DoubleClick);
                this.statusStrip1.MouseHover += new System.EventHandler(this.statusStrip1_MouseHover);
                // 
                // masterToolStripStatusLabel
                // 
                this.masterToolStripStatusLabel.Name = "masterToolStripStatusLabel";
                this.masterToolStripStatusLabel.Size = new System.Drawing.Size(80, 17);
                this.masterToolStripStatusLabel.Text = "Master status:";
                // 
                // masterStatusToolStripStatusLabel
                // 
                this.masterStatusToolStripStatusLabel.Name = "masterStatusToolStripStatusLabel";
                this.masterStatusToolStripStatusLabel.Size = new System.Drawing.Size(38, 17);
                this.masterStatusToolStripStatusLabel.Text = "status";
                // 
                // IDtoolStripStatusLabel
                // 
                this.IDtoolStripStatusLabel.Name = "IDtoolStripStatusLabel";
                this.IDtoolStripStatusLabel.Size = new System.Drawing.Size(21, 17);
                this.IDtoolStripStatusLabel.Text = "ID:";
                // 
                // IdLabel
                // 
                this.IdLabel.Name = "IdLabel";
                this.IdLabel.Size = new System.Drawing.Size(0, 17);
                // 
                // balanceTimer
                // 
                this.balanceTimer.Interval = 10000;
                this.balanceTimer.Tick += new System.EventHandler(this.balanceTimer_Tick);
                // 
                // ListenersWindow
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.BackColor = System.Drawing.SystemColors.Control;
                this.ClientSize = new System.Drawing.Size(514, 314);
                this.CloseButton = false;
                this.Controls.Add(this.statusStrip1);
                this.Controls.Add(this.FrontEndsTreeView);
                this.Controls.Add(this.mainMenuStrip);
                this.HideOnClose = true;
                this.MainMenuStrip = this.mainMenuStrip;
                this.Name = "ListenersWindow";
                this.TabText = "Slave list";
                this.Text = "ListenersWindow";
                this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ListenersWindow_FormClosing);
                this.Load += new System.EventHandler(this.ListenersWindow_Load);
                this.mainMenuStrip.ResumeLayout(false);
                this.mainMenuStrip.PerformLayout();
                this.treeViewContextMenuStrip.ResumeLayout(false);
                this.statusStrip1.ResumeLayout(false);
                this.statusStrip1.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
            }
            private System.Windows.Forms.ToolStripMenuItem synchronizeOrdersToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem synchronizeAllPositionsToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem synchronizePositionsToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem synchronizeOrdersToolStripMenuItem1;
            private System.Windows.Forms.ToolStripStatusLabel IdLabel;
            private System.Windows.Forms.ToolStripStatusLabel IDtoolStripStatusLabel;
        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip treeViewContextMenuStrip;
        private ToolStripMenuItem startListenerToolStripMenuItem;
        private ToolStripMenuItem stopListenerToolStripMenuItem;
        private ToolStripMenuItem deleteListenerToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel masterToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel masterStatusToolStripStatusLabel;
        public System.Windows.Forms.TreeView FrontEndsTreeView;
        private ToolStripMenuItem showToolStripMenuItem;
        private Timer balanceTimer;
        private Timer timer1;
        private ToolStripMenuItem closeAllOrdersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllPositionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private ToolStripMenuItem startAllToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cancelAllOrdersToolStripMenuItem1;
        private ToolStripMenuItem closeAllPositionsToolStripMenuItem2;
        private ToolStripMenuItem stopAllToolStripMenuItem;
        private ToolStripMenuItem trackerToolStripMenuItem;
        private ToolStripMenuItem startToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
    }
}