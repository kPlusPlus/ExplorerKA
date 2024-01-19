namespace ExplorerKA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtFileName.Text = "c:\\"; //TEST
            PopulateTreeView();
        }


        private void PopulateTreeView()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                TreeNode driveNode = new TreeNode(drive.Name);
                driveNode.Tag = drive.RootDirectory;
                trvDirs.Nodes.Add(driveNode);
                //PopulateDirectories(drive.RootDirectory, driveNode);
            }
        }

        private void PopulateDirectories(DirectoryInfo directory, TreeNode parentNode)
        {
            try
            {
                foreach (var subDirectory in directory.GetDirectories())
                {
                    TreeNode node = new TreeNode(subDirectory.Name);
                    node.Tag = subDirectory;
                    parentNode.Nodes.Add(node);
                    PopulateDirectories(subDirectory, node);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access (e.g., access denied to a folder)
            }
        }

        private void PopulateFiles(DirectoryInfo directory)
        {
            lstViewDirsFiles.Items.Clear();

            try
            {
                foreach (var file in directory.GetFiles())
                {
                    var item = new ListViewItem(file.Name);
                    item.SubItems.Add(file.Length.ToString());
                    item.SubItems.Add(file.LastWriteTime.ToString());
                    lstViewDirsFiles.Items.Add(item);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access (e.g., access denied to a file)
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void PopulateFilesAndDirectories(DirectoryInfo directory)
        {
            lstViewDirsFiles.Items.Clear();

            try
            {
                foreach (var subDirectory in directory.GetDirectories())
                {
                    var item = new ListViewItem(subDirectory.Name);
                    item.SubItems.Add("Folder"); // Indicate that it's a folder
                    item.SubItems.Add(subDirectory.LastWriteTime.ToString()); // Last write time
                    item.Tag = subDirectory; // Store the DirectoryInfo object for later use
                    lstViewDirsFiles.Items.Add(item);
                }

                foreach (var file in directory.GetFiles())
                {
                    var item = new ListViewItem(file.Name);
                    item.SubItems.Add(file.Length.ToString()); // File size
                    item.SubItems.Add(file.LastWriteTime.ToString()); // Last write time
                    item.Tag = file; // Store the FileInfo object for later use
                    lstViewDirsFiles.Items.Add(item);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access (e.g., access denied to a file or folder)
            }
        }



        private void trvDirs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = e.Node;
            if (selectedNode != null && selectedNode.Tag is DirectoryInfo directory)
            {
                txtFileName.Text = directory.FullName;
                //PopulateFiles(directory);
                PopulateFilesAndDirectories(directory);
            }
        }
    }
}
