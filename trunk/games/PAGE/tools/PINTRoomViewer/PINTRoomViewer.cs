using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PINTCompiler.Utilities;
using PINTCompiler.PINTBasic;

namespace PINTBasic.Utilities {
    public class PINTRoomViewer {
        [STAThread]
        public static int Main(String[] args) {
            Application.Run(new PINTRoomViewForm(args));
            return 0;
        }
    }

    public class PINTRoomViewForm : Form {   
		private System.ComponentModel.Container components = null;
		PINTBasicApplication thisApplication = null;
		
        private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.Label labelMouseInfo;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hotspotsToolStripMenuItem;		
		private PictureBox pictureBox1;
		
        public PINTRoomViewForm(String[] args) {	
			InitializeForm();
		}
		
       protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }		
		
		private void InitializeForm() {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotspotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pictureBox1 = new PictureBox();
            this.menuStrip1.SuspendLayout();
			this.labelMouseInfo = new Label();
            this.SuspendLayout();
			
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);	
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(292, 26);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hotspotsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(49, 22);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadToolStripMenuItem.Text = "Load";
			this.loadToolStripMenuItem.Click += new EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // hotspotsToolStripMenuItem
            // 
            this.hotspotsToolStripMenuItem.Name = "hotspotsToolStripMenuItem";
			this.hotspotsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hotspotsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hotspotsToolStripMenuItem.Text = "Hotspots";
			this.hotspotsToolStripMenuItem.Click += new EventHandler(this.hotspotsToolStripMenuItem_Click);
			//
			// pictureBox1
			//
			// Dock the PictureBox to the form and set its background to white.
			this.pictureBox1.BackColor = Color.White;
			this.pictureBox1.Location = new System.Drawing.Point(2,27);
			this.pictureBox1.Width = 480;
			this.pictureBox1.Height = 240;
			this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);			
			this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
			//
			// labelMouseInfo
			//
			this.labelMouseInfo.Name = "labelMouseInfo";
			this.labelMouseInfo.BackColor = Color.White;
			this.labelMouseInfo.Location = new System.Drawing.Point(4,275);
			this.labelMouseInfo.Size = new System.Drawing.Size(180,25);
			this.labelMouseInfo.Text = "";
			this.labelMouseInfo.ForeColor = Color.Black;
			this.labelMouseInfo.Font = new Font("Georgia", 8); 
            // 
            // PINTRoomViewForm
            // 
			this.BackColor = Color.White;
			this.Text = "PINTBasic Room Viewer";
			this.MaximizeBox = false;
			this.MinimizeBox = true;
			this.ClientSize = new System.Drawing.Size(487, 340);
			this.MaximumSize = new Size(487,340);
            this.MinimumSize = new Size(487,340);
			this.Name = "PINTRoomViewForm";		
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.ShowInTaskbar = true;
			this.StartPosition = FormStartPosition.CenterParent;
			this.Controls.Add(menuStrip1);
			this.Controls.Add(pictureBox1);
			this.Controls.Add(labelMouseInfo);
			this.MainMenuStrip = this.menuStrip1;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();	
		}
		
		private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e){
			RedrawBackdrop();
		}
		
		private void RedrawBackdrop() {
			if (thisApplication != null) {
				string fileName = thisApplication.Rooms[0].Backdrop.FileName.Replace("\"", ""); 
				Bitmap scaledBitmap = new Bitmap(fileName);
				if (hotspotsToolStripMenuItem.Checked) scaledBitmap = DrawHotspots(scaledBitmap);
				scaledBitmap = ResizeBitmap(scaledBitmap, 480, 240);
				pictureBox1.Image = scaledBitmap;
			}
		}
		
		private Bitmap ResizeBitmap( Bitmap b, int nWidth, int nHeight ){
			Bitmap result = new Bitmap( nWidth, nHeight );
			using( Graphics g = Graphics.FromImage( (Image) result ) ) {
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				g.DrawImage( b, 0, 0, nWidth, nHeight ); 
				return result;
			}	
		}	

		private Bitmap DrawHotspots( Bitmap b){
			Bitmap returnBitmap = b;
			using( Graphics g = Graphics.FromImage( (Image) returnBitmap ) ) {
				Pen orangePen = new Pen(Color.Orange, 1);
				foreach (PINTBasicHotspot thisHotspot in thisApplication.Rooms[0].Hotspots) {
					Rectangle rect = new Rectangle(thisHotspot.X, thisHotspot.Y, thisHotspot.Width, thisHotspot.Height);
					g.DrawRectangle(orangePen, rect);
				}
				orangePen = null;
				
				return returnBitmap;
			}	
		}	
		
		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Application.Exit();
		}
		
		private void pictureBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e){
				if (thisApplication != null) {
					Point LocalMousePosition  = pictureBox1.PointToClient(Cursor.Position);
					int pointX = LocalMousePosition.X/6;
					int pointY = LocalMousePosition.Y/3;
					labelMouseInfo.Text = "X: " + pointX + " Y: " + pointY;	
					this.Refresh();
				}
		}
		
		private void hotspotsToolStripMenuItem_Click(object sender, EventArgs e) {
			if (this.hotspotsToolStripMenuItem.Checked) {
				this.hotspotsToolStripMenuItem.Checked = false;
			}else{
				this.hotspotsToolStripMenuItem.Checked = true;
			}
			RedrawBackdrop();
			pictureBox1.Refresh();
		}
		
		private void loadToolStripMenuItem_Click (object sender, EventArgs e) {
		    OpenFileDialog ofn = new OpenFileDialog ();
            ofn.Filter = "PINTBasic Source Files (*.bas)|*.bas|All Types (*.*)|*.*";
            ofn.Title = "Open PINTBasic Source File";
			
			if (ofn.ShowDialog () == DialogResult.Cancel) {
                    return;
			} else {
                try{
					string directoryPath = Path.GetDirectoryName(ofn.FileName) + "\\";
					 
					CompilationLog thisLog = new CompilationLog();
					SourceLineList lines = Preprocessor.Preprocess(Path.GetFileName(ofn.FileName), directoryPath, thisLog);
					PINTCompiler.PINTBasic.Parser parser = new PINTCompiler.PINTBasic.Parser();
					thisApplication = parser.Parse(thisLog, directoryPath, lines);
					parser = null;		
					if (!thisLog.CanContinue) {
						thisApplication = null;
						
						DialogResult result = MessageBox.Show ("There was an error processing this room file.\nWould you like to see the compilation log?", "Room File Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
						if (result == DialogResult.Yes) {
							CompilationLogListing thisListing = new CompilationLogListing();
							thisListing.Show(thisLog);
						}
					}
				
                }catch (Exception){
                    MessageBox.Show ("Error opening file", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

			}
		}
		
	}
	
}