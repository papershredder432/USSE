namespace Unturned_Server_Settings_Editor
{
    partial class USSESplash
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(USSESplash));
            this.pictureBoxUSSE = new System.Windows.Forms.PictureBox();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.panelBack = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUSSE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxUSSE
            // 
            this.pictureBoxUSSE.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxUSSE.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxUSSE.Image")));
            this.pictureBoxUSSE.Location = new System.Drawing.Point(92, 3);
            this.pictureBoxUSSE.Name = "pictureBoxUSSE";
            this.pictureBoxUSSE.Size = new System.Drawing.Size(687, 75);
            this.pictureBoxUSSE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxUSSE.TabIndex = 1;
            this.pictureBoxUSSE.TabStop = false;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.Image")));
            this.pictureBoxLogo.Location = new System.Drawing.Point(11, 3);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(75, 75);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 2;
            this.pictureBoxLogo.TabStop = false;
            // 
            // panelBack
            // 
            this.panelBack.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelBack.Controls.Add(this.pictureBoxLogo);
            this.panelBack.Controls.Add(this.pictureBoxUSSE);
            this.panelBack.Location = new System.Drawing.Point(0, -1);
            this.panelBack.Name = "panelBack";
            this.panelBack.Size = new System.Drawing.Size(790, 88);
            this.panelBack.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // USSESplash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 85);
            this.Controls.Add(this.panelBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "USSESplash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "USSESplash";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUSSE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.panelBack.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBoxUSSE;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Panel panelBack;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}