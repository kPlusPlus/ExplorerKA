namespace ExplorerKA
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            txtFileName = new TextBox();
            trvDirs = new TreeView();
            lstViewDirsFiles = new ListView();
            panel1 = new Panel();
            contextMenuStrip1 = new ContextMenuStrip(components);
            tsmOpen = new ToolStripMenuItem();
            panel1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtFileName
            // 
            txtFileName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFileName.Location = new Point(3, 3);
            txtFileName.Name = "txtFileName";
            txtFileName.Size = new Size(1263, 23);
            txtFileName.TabIndex = 0;
            // 
            // trvDirs
            // 
            trvDirs.AllowDrop = true;
            trvDirs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            trvDirs.Location = new Point(3, 32);
            trvDirs.Name = "trvDirs";
            trvDirs.Size = new Size(381, 807);
            trvDirs.TabIndex = 1;
            trvDirs.AfterSelect += trvDirs_AfterSelect;
            // 
            // lstViewDirsFiles
            // 
            lstViewDirsFiles.AllowColumnReorder = true;
            lstViewDirsFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstViewDirsFiles.Location = new Point(390, 32);
            lstViewDirsFiles.Name = "lstViewDirsFiles";
            lstViewDirsFiles.Size = new Size(876, 807);
            lstViewDirsFiles.TabIndex = 2;
            lstViewDirsFiles.UseCompatibleStateImageBehavior = false;
            lstViewDirsFiles.View = View.List;
            lstViewDirsFiles.DoubleClick += lstViewDirsFiles_DoubleClick;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = Color.Brown;
            panel1.Controls.Add(lstViewDirsFiles);
            panel1.Controls.Add(txtFileName);
            panel1.Controls.Add(trvDirs);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(1269, 842);
            panel1.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { tsmOpen });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(181, 48);
            // 
            // tsmOpen
            // 
            tsmOpen.Name = "tsmOpen";
            tsmOpen.Size = new Size(180, 22);
            tsmOpen.Text = "Open";
            tsmOpen.Click += tsmOpen_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1293, 866);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Form1";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        public TextBox txtFileName;
        private TreeView trvDirs;
        private ListView lstViewDirsFiles;
        private Panel panel1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem tsmOpen;
    }
}
