
using System.ComponentModel;
using System.Windows.Forms;
namespace NormalRouterApplication
{
    partial class NewLogForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.log_dataGridView = new System.Windows.Forms.DataGridView();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LogType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.log_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // log_dataGridView
            // 
            this.log_dataGridView.AllowUserToAddRows = false;
            this.log_dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.log_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            this.log_dataGridView.Columns.AddRange(
                new System.Windows.Forms.DataGridViewColumn[]
                {
                    this.Time, this.LogType, this.Source, this.Message
                }
            );

            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.log_dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.log_dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.log_dataGridView.Location = new System.Drawing.Point(0, 0);
            this.log_dataGridView.Name = "log_dataGridView";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.log_dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.log_dataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.log_dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.log_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.log_dataGridView.Size = new System.Drawing.Size(597, 295);
            this.log_dataGridView.TabIndex = 0;
            this.log_dataGridView.VirtualMode = true;
            this.log_dataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.log_dataGridView_CellFormatting);
            this.log_dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.log_dataGridView_CellValueNeeded);
            // 
            // Time
            // 
            this.Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Time.FillWeight = 10F;
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            // 
            // LogType
            // 
            this.LogType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LogType.FillWeight = 10F;
            this.LogType.HeaderText = "LogType";
            this.LogType.Name = "LogType";
            this.LogType.ReadOnly = true;
            // 
            // Source
            // 
            this.Source.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Source.DefaultCellStyle = dataGridViewCellStyle1;
            this.Source.FillWeight = 20F;
            this.Source.HeaderText = "Source";
            this.Source.Name = "Source";
            this.Source.ReadOnly = true;
            // 
            // Message
            // 
            this.Message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Message.FillWeight = 60F;
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // NewLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 295);
            this.Controls.Add(this.log_dataGridView);
            this.HideOnClose = true;
            this.Name = "NewLogForm";
            this.TabText = "Application log";
            this.Text = "NewLogForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewLogForm_FormClosing);
            this.Load += new System.EventHandler(this.NewLogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.log_dataGridView)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView log_dataGridView;
        private DataGridViewTextBoxColumn Time;
        private DataGridViewTextBoxColumn LogType;
        private DataGridViewTextBoxColumn Source;
        private DataGridViewTextBoxColumn Message;
        private Timer timer1;
    }
}