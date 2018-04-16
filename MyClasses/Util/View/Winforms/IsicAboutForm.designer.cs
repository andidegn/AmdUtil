namespace Isic.Util.Winforms {
	partial class IsicAboutForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IsicAboutForm));
			this.btn_ok = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.rtb_description = new System.Windows.Forms.RichTextBox();
			this.lbl_version = new System.Windows.Forms.Label();
			this.lbl_text2 = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// btn_ok
			// 
			this.btn_ok.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_ok.Location = new System.Drawing.Point(175, 225);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new System.Drawing.Size(75, 23);
			this.btn_ok.TabIndex = 0;
			this.btn_ok.Text = "OK";
			this.btn_ok.UseVisualStyleBackColor = true;
			this.btn_ok.Click += new System.EventHandler(this.btn_close_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.ErrorImage = null;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.InitialImage = null;
			this.pictureBox1.Location = new System.Drawing.Point(12, 83);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(34, 34);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// rtb_description
			// 
			this.rtb_description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtb_description.Location = new System.Drawing.Point(12, 123);
			this.rtb_description.Name = "rtb_description";
			this.rtb_description.ReadOnly = true;
			this.rtb_description.Size = new System.Drawing.Size(401, 96);
			this.rtb_description.TabIndex = 2;
			this.rtb_description.Text = "";
			this.rtb_description.WordWrap = false;
			// 
			// lbl_version
			// 
			this.lbl_version.AutoSize = true;
			this.lbl_version.Location = new System.Drawing.Point(51, 84);
			this.lbl_version.Name = "lbl_version";
			this.lbl_version.Size = new System.Drawing.Size(0, 13);
			this.lbl_version.TabIndex = 3;
			// 
			// lbl_text2
			// 
			this.lbl_text2.AutoSize = true;
			this.lbl_text2.Location = new System.Drawing.Point(51, 101);
			this.lbl_text2.Name = "lbl_text2";
			this.lbl_text2.Size = new System.Drawing.Size(0, 13);
			this.lbl_text2.TabIndex = 4;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
			this.pictureBox2.Location = new System.Drawing.Point(11, 12);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(402, 61);
			this.pictureBox2.TabIndex = 5;
			this.pictureBox2.TabStop = false;
			// 
			// IsicAboutForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(424, 256);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.lbl_text2);
			this.Controls.Add(this.lbl_version);
			this.Controls.Add(this.rtb_description);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.btn_ok);
			this.Name = "IsicAboutForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "AboutForm";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btn_ok;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.RichTextBox rtb_description;
		private System.Windows.Forms.Label lbl_version;
		private System.Windows.Forms.Label lbl_text2;
		private System.Windows.Forms.PictureBox pictureBox2;
	}
}