using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Shared.Interfaces;
using Shared.Logger;
using WeifenLuo.WinFormsUI.Docking;
namespace NormalRouterApplication
{
    public partial class NewLogForm : DockContent, IAppender
    {
        public enum MsgColumnType
        {
            Time,
            LogType,
            Source,
            Message
        }
        private readonly List<LogMessage> messages_ = new List<LogMessage>();
        private readonly int maxMsgCount_ = 500;
        private bool update_;
        public NewLogForm()
            => InitializeComponent();

        public void DoAppend(LoggingEvent loggingEvent)
        {
            try
            {
                var logMessage = new LogMessage(loggingEvent);
                if ((int)logMessage.Severity > 2)
                {
                    if (InvokeRequired)
                    {
                        BeginInvoke(new Action<LogMessage>(DoAppend), logMessage);
                    }
                    else
                    {
                        DoAppend(logMessage);
                    }
                }
            }
            catch (Exception) { }
        }

        public void DoAppend(LogMessage logMessage)
        {
            messages_.Insert(0, logMessage);
            if (messages_.Count > maxMsgCount_)
            {
                messages_.RemoveAt(maxMsgCount_);
            }
            update_ = true;
        }
        private void SetRowColor(LogMessage log, int rowIndex) 
        {
            switch (log.Severity) 
            {
                case LogPriority.Error:
                    log_dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
                    break;
                case LogPriority.Warning:
                    log_dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Yellow;
                    break;
                case LogPriority.AccountTracker:
                    log_dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.MediumVioletRed;
                    break;
                default:
                    log_dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    break;
            }
        }
        private void log_dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= messages_.Count)
            {
                return;
            }

            var logMessage = messages_[e.RowIndex];
            switch (e.ColumnIndex)
            {
                case (int)MsgColumnType.Time:
                    e.Value = logMessage.Time;
                    break;
                case (int)MsgColumnType.LogType:
                    e.Value = logMessage.Severity.ToString();
                    break;
                case (int)MsgColumnType.Source:
                    e.Value = logMessage.Source;
                    break;
                case (int)MsgColumnType.Message:
                    e.Value = logMessage.Message;
                    break;
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (update_ == false)
            {
                return;
            }

            log_dataGridView.RowCount = messages_.Count;
            log_dataGridView.Refresh();
            update_ = false;
        }

        private void NewLogForm_Load(object sender, EventArgs e)
        {
            ((Hierarchy)LogManager.GetRepository()).Root.AddAppender(this);
            timer1.Enabled = true;
            timer1.Start();
        }

        private void NewLogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            timer1.Dispose();
        }
        private void log_dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var logMessage = messages_[e.RowIndex];
            SetRowColor(logMessage, e.RowIndex);
        }

    }
}
