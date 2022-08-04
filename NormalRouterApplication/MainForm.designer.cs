
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
namespace RouterApplication
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
            this.dockPanel = new DockPanel();
            this.SuspendLayout();
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = DockStyle.Fill;
            this.dockPanel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.World);
            this.dockPanel.Location = new Point(0, 0);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new Size(800, 450);
            this.dockPanel.TabIndex = 1;
            this.dockPanel.Paint += new PaintEventHandler(this.dockPanel_Paint);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 450);
            this.Controls.Add(this.dockPanel);
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.Text = "Binance Trade Router";
            this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DockPanel dockPanel;
    }
}

