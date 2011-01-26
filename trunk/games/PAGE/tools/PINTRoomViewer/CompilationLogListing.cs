using System;
using System.Windows.Forms;
using PINTCompiler.Utilities;
using PINTCompiler.PINTBasic;

namespace PINTBasic.Utilities
{

    public partial class CompilationLogListing : Form
    {
        private System.Windows.Forms.TextBox txtMessage;
		
		public CompilationLogListing()
        {
            InitializeComponent();
        }
		
        private void InitializeComponent()
        {
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Location = new System.Drawing.Point(0, -2);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(328, 227);
            this.txtMessage.TabIndex = 1;
            // 
            // ScrollableMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 229);
            this.Controls.Add(this.txtMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CompilationLogListing";
            this.Text = "Compilation Log";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		
        public void Show (CompilationLog thisLog)
        {
            // populate the text box with the message
			foreach (CompilationLogEntry entry in thisLog.Entries) {
				txtMessage.Text += entry.Source + "> Line: " + entry.LineNumber + " Message: " + entry.Message + Environment.NewLine;
			}
            this.ShowDialog();
        }

    }
}