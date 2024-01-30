using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace ExplorerKA
{
    public partial class Form1 : Form
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_SMALLICON = 0x1;
        private const uint SHGFI_LARGEICON = 0x0;

        private ImageList imageList; // Icon of file
        private TreeNode selectedNode; // tree of directory
        private DirectoryInfo directoryInfo;


        public Form1()
        {
            InitializeComponent();
            InitializeImageList();
            InitializeListView();
            PopulateTreeView();
        }

        private void InitializeImageList()
        {
            imageList = new ImageList();
            imageList.ImageSize = new Size(32, 32); // Set the desired icon size
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            lstViewDirsFiles.SmallImageList = imageList;
        }

        private void InitializeListView()
        {
            lstViewDirsFiles.View = View.Details;
            lstViewDirsFiles.Columns.Add("Name", 150); // Column for file/folder namewq
            lstViewDirsFiles.Columns.Add("Size", 100); // Column for file size
            lstViewDirsFiles.Columns.Add("Last Modified", 150); // Column for last modified timestamp

            lstViewDirsFiles.ContextMenuStrip = contextMenuStrip1;
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
                    var icon = GetFileIcon(subDirectory.FullName);// Get the icon for the file
                    imageList.Images.Add(icon);
                    item.ImageIndex = imageList.Images.Count - 1;
                    lstViewDirsFiles.Items.Add(item);
                }

                foreach (var file in directory.GetFiles())
                {
                    var item = new ListViewItem(file.Name);
                    item.SubItems.Add(file.Length.ToString()); // File size
                    item.SubItems.Add(file.LastWriteTime.ToString()); // Last write time
                    item.Tag = file; // Store the FileInfo object for later use
                    var icon = GetFileIcon(file.FullName);// Get the icon for the file
                    imageList.Images.Add(icon);
                    item.ImageIndex = imageList.Images.Count - 1;
                    lstViewDirsFiles.Items.Add(item);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access (e.g., access denied to a file or folder)
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void trvDirs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedNode = e.Node;
            if (selectedNode != null && selectedNode.Tag is DirectoryInfo directory)
            {
                txtFileName.Text = directory.FullName;
                //PopulateFiles(directory);
                PopulateFilesAndDirectories(directory);
                trvDirs.SelectedNode = selectedNode;
                directoryInfo = new DirectoryInfo(directory.FullName); // to variable
            }
        }

        private Icon GetFileIcon(string filePath)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImg = SHGetFileInfo(filePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
            if (hImg == IntPtr.Zero)
            {
                throw new FileNotFoundException("Icon not found");
            }
            Icon icon = Icon.FromHandle(shinfo.hIcon);
            return icon;
        }


        private void lstViewDirsFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lstViewDirsFiles.SelectedItems.Count > 0)
            {
                var selectedItem = lstViewDirsFiles.SelectedItems[0];
                DirectoryInfo di = selectedItem.Tag as DirectoryInfo;

                if (di != null)
                {
                    PopulateFilesAndDirectories(di);
                    PopulateDirectories(di, selectedNode);
                    txtFileName.Text = di.FullName;
                    directoryInfo = di;
                }
            }

        }

        private void tsmOpen_Click(object sender, EventArgs e)
        {
            if (lstViewDirsFiles.SelectedItems.Count > 0)
            {
                ListViewItem item = lstViewDirsFiles.SelectedItems[0];
                object obj = item.Tag;
                FileInfo fileInfo = (FileInfo)obj;
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fileInfo.FullName.ToString(),
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
        }

        private void tsmCompress_Click(object sender, EventArgs e)
        {
            if (lstViewDirsFiles.SelectedItems.Count > 0)
            {
                ListViewItem item = lstViewDirsFiles.SelectedItems[0];
                object obj = item.Tag;
                FileInfo fileInfo = (FileInfo)obj;
                string fileName = fileInfo.FullName.ToString();
                CompressFileZIP(fileName, fileName + ".gzip");
                MessageBox.Show(" File is compressed " + fileName);

                PopulateFilesAndDirectories(directoryInfo);
            }
        }

        public static void CompressFile(string sourceFilePath, string compressedFilePath)
        {
            // Create a new compressed file
            using (FileStream sourceFile = File.OpenRead(sourceFilePath))
            {
                using (FileStream compressedFile = File.Create(compressedFilePath))
                {
                    using (GZipStream compressionStream = new GZipStream(compressedFile, CompressionMode.Compress))
                    {
                        // Copy the source file to the compressed file
                        sourceFile.CopyTo(compressionStream);
                    }
                }
            }
        }

        public static void CompressFileZIP(string sourceFilePath, string compressedFilePath)
        {
            using (ZipArchive archive = new ZipArchive(File.Open(compressedFilePath, FileMode.Create), ZipArchiveMode.Create))
            {
                ZipArchiveEntry entry = archive.CreateEntry(sourceFilePath);

                using (Stream entryStream = entry.Open())
                {
                    using (StreamWriter writer = new StreamWriter(entryStream))
                    {
                        writer.Write(File.ReadAllText(sourceFilePath));
                    }
                }
            }

        }


    }
}
