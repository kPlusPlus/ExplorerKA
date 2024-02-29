using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip;
using SevenZip;
using SevenZip.Sdk.Compression.Lzma;
using CompressionLevel = System.IO.Compression.CompressionLevel;



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

            //lstViewDirsFiles.ContextMenuStrip = contextMenuStrip1;
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
                for(int i=0;i<lstViewDirsFiles.SelectedItems.Count;i++)
                {
                    ListViewItem item = lstViewDirsFiles.SelectedItems[i];
                    object obj = item.Tag;
                    FileInfo fileInfo = obj as FileInfo;
                    if (fileInfo != null)
                    {
                        string fileName = fileInfo.FullName.ToString();

                        Compress("TESLAAA.zip", fileName);
                    }
                    if (obj != null)
                    {
                        DirectoryInfo di = obj as DirectoryInfo;
                        if(di != null)  Compress(di.FullName, "TESTAAA.zip");
                     
                    }
                }

                PopulateFilesAndDirectories(directoryInfo);
            }
        }


        static void Compress(string inputPath, string outputPath)
        {
            if (Directory.Exists(inputPath))
            {
                // If input is a folder, use SharpZipLib to compress
                using (FileStream outputStream = File.Create(outputPath))
                using (ZipOutputStream zipStream = new ZipOutputStream(outputStream))
                {
                    zipStream.SetLevel(9); // Set compression level to maximum

                    CompressFolder(inputPath, zipStream, "");
                }
            }
            else if (File.Exists(inputPath))
            {
                // If input is a file, use built-in .NET compression
                using (FileStream inputStream = File.OpenRead(inputPath))
                using (FileStream outputStream = File.Create(outputPath)) //TODO: create or append
                using (GZipStream compressionStream = new GZipStream(outputStream, CompressionLevel.SmallestSize))
                {
                    inputStream.CopyTo(compressionStream);
                }
            }
            else
            {
                Console.WriteLine("Input path does not exist.");
            }
        }

        static void CompressFolder(string folderPath, ZipOutputStream zipStream, string parentFolder)
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (string filePath in files)
            {
                string relativePath = Path.Combine(parentFolder, Path.GetFileName(filePath));
                AddFileToZip(zipStream, filePath, relativePath);
            }

            string[] folders = Directory.GetDirectories(folderPath);
            foreach (string folder in folders)
            {
                string folderName = Path.GetFileName(folder);
                string relativePath = Path.Combine(parentFolder, folderName + "/");
                CompressFolder(folder, zipStream, relativePath);
            }
        }

        static void AddFileToZip(ZipOutputStream zipStream, string filePath, string relativePath)
        {
            ZipEntry entry = new ZipEntry(relativePath);
            zipStream.PutNextEntry(entry);

            using (FileStream fileStream = File.OpenRead(filePath))
            {
                fileStream.CopyTo(zipStream);
            }

            zipStream.CloseEntry();
        }


    }
}
