
using System.ComponentModel;
using System.Windows.Forms;
namespace NormalRouterApplication
{
    partial class MasterOrderForm
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
            this.orderDataGridView = new System.Windows.Forms.DataGridView();
            this.time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.symbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.orderSide = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clientOrderId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.orderDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // orderDataGridView
            // 
            this.orderDataGridView.AllowUserToAddRows = false;
            this.orderDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.orderDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.orderDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.time,
            this.name,
            this.symbol,
            this.orderSide,
            this.status,
            this.Id,
            this.clientOrderId,
            this.price,
            this.qty});
            this.orderDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.orderDataGridView.Location = new System.Drawing.Point(0, 0);
            this.orderDataGridView.Name = "orderDataGridView";
            this.orderDataGridView.RowHeadersVisible = false;
            this.orderDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.orderDataGridView.Size = new System.Drawing.Size(800, 450);
            this.orderDataGridView.TabIndex = 0;
            this.orderDataGridView.VirtualMode = true;
            this.orderDataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.orderDataGridView_CellValueNeeded);
            this.orderDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.orderDataGridView_ColumnHeaderMouseClick);
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
            // orderSide
            // 
            this.orderSide.HeaderText = "Side";
            this.orderSide.Name = "orderSide";
            this.orderSide.ReadOnly = true;
            // 
            // status
            // 
            this.status.HeaderText = "Status";
            this.status.Name = "status";
            this.status.ReadOnly = true;
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // clientOrderId
            // 
            this.clientOrderId.HeaderText = "Client Id";
            this.clientOrderId.Name = "clientOrderId";
            this.clientOrderId.ReadOnly = true;
            // 
            // price
            // 
            this.price.HeaderText = "Price";
            this.price.Name = "price";
            this.price.ReadOnly = true;
            // 
            // qty
            // 
            this.qty.HeaderText = "Qty";
            this.qty.Name = "qty";
            this.qty.ReadOnly = true;
            // 
            // MasterOrderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.CloseButton = false;
            this.Controls.Add(this.orderDataGridView);
            this.HideOnClose = true;
            this.Name = "MasterOrderForm";
            this.TabText = "MasterOrderForm";
            this.Text = "MasterOrderForm";
            ((System.ComponentModel.ISupportInitialize)(this.orderDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView orderDataGridView;
        private DataGridViewTextBoxColumn time;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn symbol;
        private DataGridViewTextBoxColumn orderSide;
        private DataGridViewTextBoxColumn status;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn clientOrderId;
        private DataGridViewTextBoxColumn price;
        private DataGridViewTextBoxColumn qty;
    }
}