
using System.ComponentModel;
using System.Windows.Forms;
namespace NormalRouterApplication
{
    partial class SlavePositions
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
            this.positionsDataGridView = new System.Windows.Forms.DataGridView();
            this.time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.symbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.entryPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.leverage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.liquidationPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.positionAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unrealizedProfit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.positionSide = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.positionsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // positionsDataGridView
            // 
            this.positionsDataGridView.AllowUserToAddRows = false;
            this.positionsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.positionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.positionsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
            {
                this.time,
                this.name,
                this.symbol,
                this.entryPrice,
                this.leverage,
                this.liquidationPrice,
                this.positionAmt,
                this.unrealizedProfit,
                this.positionSide
            });
            this.positionsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.positionsDataGridView.Name = "positionsDataGridView";
            this.positionsDataGridView.RowHeadersVisible = false;
            this.positionsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.positionsDataGridView.Size = new System.Drawing.Size(616, 274);
            this.positionsDataGridView.TabIndex = 0;
            this.positionsDataGridView.VirtualMode = true;
            this.positionsDataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.positionsDataGridView_CellValueNeeded);
            this.positionsDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.positionsDataGridView_ColumnHeaderMouseClick);
            // 
            // time
            // 
            this.time.HeaderText = "Time";
            this.time.Name = "time";
            this.time.ReadOnly = true;
            // 
            // name
            // 
            this.name.HeaderText = "Name";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            // 
            // symbol
            // 
            this.symbol.HeaderText = "Symbol";
            this.symbol.Name = "symbol";
            this.symbol.ReadOnly = true;
            // 
            // entryPrice
            // 
            this.entryPrice.HeaderText = "Entry price";
            this.entryPrice.Name = "entryPrice";
            this.entryPrice.ReadOnly = true;
            // 
            // leverage
            // 
            this.leverage.HeaderText = "Leverage";
            this.leverage.Name = "leverage";
            this.leverage.ReadOnly = true;
            // 
            // liquidationPrice
            // 
            this.liquidationPrice.HeaderText = "Liquidation price";
            this.liquidationPrice.Name = "liquidationPrice";
            this.liquidationPrice.ReadOnly = true;
            // 
            // positionAmt
            // 
            this.positionAmt.HeaderText = "Position amt";
            this.positionAmt.Name = "positionAmt";
            this.positionAmt.ReadOnly = true;
            // 
            // unrealizedProfit
            // 
            this.unrealizedProfit.HeaderText = "Unrealized profit";
            this.unrealizedProfit.Name = "unrealizedProfit";
            this.unrealizedProfit.ReadOnly = true;
            // 
            // positionSide
            // 
            this.positionSide.HeaderText = "Position side";
            this.positionSide.Name = "positionSide";
            this.positionSide.ReadOnly = true;
            // 
            // SlavePositions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 274);
            this.CloseButton = false;
            this.Controls.Add(this.positionsDataGridView);
            this.HideOnClose = true;
            this.Name = "SlavePositions";
            this.TabText = "SlavePositions";
            this.Text = "SlavePositions";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SlavePositions_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.positionsDataGridView)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DataGridView positionsDataGridView;
        private DataGridViewTextBoxColumn time;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn symbol;
        private DataGridViewTextBoxColumn entryPrice;
        private DataGridViewTextBoxColumn leverage;
        private DataGridViewTextBoxColumn liquidationPrice;
        private DataGridViewTextBoxColumn positionAmt;
        private DataGridViewTextBoxColumn unrealizedProfit;
        private DataGridViewTextBoxColumn positionSide;
    }
}