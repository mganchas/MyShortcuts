using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using MyShortcuts.Classes;

namespace MyShortcuts
{
    public partial class MainWindow : Window
    {
        #region Class paramethers
        private List<ListViewItem> selectedItems = new List<ListViewItem>();
        private Definitions.AddTypes fType;
        private Imagem xImg = new Imagem();
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            SetInitConditions();
            BuildJumpList();
        }
        #endregion

        #region Methods
        private void BuildJumpList()
        {
            List<JumpItem> customCat = new List<JumpItem>();

            if (lstv_atalhos.Items.Count > 0)
            {
                foreach (ListItems listItem in lstv_atalhos.Items)
                {
                    JumpTask jumpItem = new JumpTask()
                    {
                        ApplicationPath = listItem.path,
                        IconResourcePath = listItem.imagePath,
                        Title = listItem.title,
                        Description = listItem.title,
                        CustomCategory = "Shortcuts"
                    };
                    customCat.Add(jumpItem);
                }
            }

            JumpList jumpList = new JumpList();// customCat, true, true);
            jumpList.JumpItems.AddRange(customCat);
            JumpList.SetJumpList(App.Current, jumpList);
        }

        private void SetInitConditions()
        {
            this.DataContext = xImg;
            lstv_atalhos.Items.Clear();

            SetImgSource(Definitions.AddTypes.File);
            Folders.folderChecking();
            dStructToListItem(new Saver().ReadFile());

            txtSearch.Focus();

            this.Loaded += new RoutedEventHandler(setAppPosition);
        }

        private void dStructToListItem(List<Definitions.DataStruct> structList)
        {
            foreach (Definitions.DataStruct structure in structList)
                LoopThroughItems(structure.value, structure.name);
        }

        private void setDropShortcuts(object sender, DragEventArgs e)
        {
            string[] lstCaminhos = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (lstCaminhos != null)
                addItemsListV(lstCaminhos);
            else
                MessageBox.Show("Sorry! Type not supported");
        }

        private bool IsDuplicateItem(string path)
        {
            if (lstv_atalhos.Items.Count > 0)
            {
                foreach (ListItems listItem in lstv_atalhos.Items)
                    if (listItem.path.Equals(path)) { return true; }
            }

            return false;
        }

        private void LoopThroughItems(string xCaminho, string Name, bool saveItems = false)
        {
            if (!IsDuplicateItem(xCaminho))
            {
                lstv_atalhos.Items.Add(new ListItems()
                {
                    title = Name ?? Definitions.getShortcutName(xCaminho),
                    image = Definitions.getImageSource(xCaminho),
                    imagePath = xCaminho,
                    path = xCaminho.ToString(),
                    error = Visibility.Collapsed,
                    nivel = 1.0
                });
            }

            if (saveItems) {
                SaveItems();
            }
        }

        private void addItemsListV(string[] lstCaminhos)
        {
            foreach (string xCaminho in lstCaminhos) { 
                LoopThroughItems(xCaminho, null, true);
            }
        }

        private void setAppPosition(object sender, RoutedEventArgs e)
        {
            //var areaTrab = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            //this.Left = 0; //areaTrab.Right - this.Width * 2;
            //this.Top = areaTrab.Bottom - areaTrab.Top;
        }

        private void setFilter(object sender, TextChangedEventArgs e)
        {
            ICollectionView xView = CollectionViewSource.GetDefaultView(lstv_atalhos.Items);
            TextBox xTxt = (sender as TextBox);

            if (String.IsNullOrEmpty(xTxt.Text))
                xView.Filter = null;
            else
            {
                xView.Filter = x =>
                {
                    var xItems = x as ListItems;
                    return (bool)xItems.title.ToUpper().Contains(xTxt.Text.ToUpper());
                };
            }

            btnClearSearch.Visibility = String.IsNullOrEmpty(xTxt.Text) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AddFiles()
        {
            System.Windows.Forms.OpenFileDialog xDialog = new System.Windows.Forms.OpenFileDialog();
            xDialog.Multiselect = true;

            if (xDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                addItemsListV(xDialog.FileNames);
        }

        private void AddFolders()
        {
            System.Windows.Forms.FolderBrowserDialog xDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (xDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                addItemsListV(new string[] { xDialog.SelectedPath.ToString() });
        }

        private void RemoveItems()
        {
            foreach (ListItems xItem in lstv_atalhos.SelectedItems)
            {
                lstv_atalhos.Items.Remove(xItem);
                RemoveItems();
                //break;
            }
            SaveItems();
            lstv_atalhos.Items.Refresh();
        }

        private void SetImgSource(Definitions.AddTypes type)
        {
            if (type == Definitions.AddTypes.File)
            {
                xImg.addImg = StrToImgConverter("/MyShortcuts;component/Imagens/file_add.png");
                fType = Definitions.AddTypes.File;
            }
            else
            {
                fType = Definitions.AddTypes.Folder;
                xImg.addImg = StrToImgConverter("/MyShortcuts;component/Imagens/folder_add.png");
            }
        }

        private ImageSource StrToImgConverter(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }

        private bool ShortcutStatus(ListItems oListItem)
        {
            bool exists = false;

            switch ((Definitions.Types)Definitions.getTypeFromString(Definitions.getShortcutType(oListItem.path)))
            {
                case Definitions.Types.File:
                    exists = Files.fileExists(oListItem.path);
                    break;
                case Definitions.Types.Folder:
                case Definitions.Types.Drive:
                    exists = Folders.folderExists(oListItem.path);
                    break;
            }

            return exists;
        }
       
        private void SaveItems()
        {
            List<Definitions.DataStruct> dStruct = new List<Definitions.DataStruct>();

            foreach (ListItems listItem in lstv_atalhos.Items)
            {
                dStruct.Add(new Definitions.DataStruct()
                {
                    type = (Definitions.Types)Definitions.getTypeFromString(Definitions.getShortcutType(listItem.path)),
                    value = listItem.path,
                    name = listItem.title
                });
            }

            Saver.WriteFile(dStruct, false);
            BuildJumpList();
        }
        #endregion

        #region Controls' Events
        private void clk_sair(object sender, RoutedEventArgs e)
        {
            SaveItems();
            this.Close();
        }

        private void clk_remover(object sender, RoutedEventArgs e)
        {
            RemoveItems();
        }

        private void clk_minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void clk_showOptions(object sender, RoutedEventArgs e)
        {
            popOptions.IsOpen = true;
        }

        private void clk_openShortcut(object sender, MouseButtonEventArgs e)
        {
            ListItems oListItem = (lstv_atalhos.SelectedItem as ListItems);

            if (ShortcutStatus(oListItem))
            {
                Process.Start(oListItem.path);
                oListItem.error = Visibility.Collapsed;
                oListItem.nivel = 1.0;
            }
            else
            {
                oListItem.error = Visibility.Visible;
                oListItem.nivel = 0.5;
            }

            lstv_atalhos.Items.Refresh();
        }
        
        private void clk_clearSearch(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = String.Empty;
            txtSearch.Focus();
        }

        private void clk_changeType(object sender, MouseButtonEventArgs e)
        {
            if (fType == Definitions.AddTypes.File)
                SetImgSource(Definitions.AddTypes.Folder);
            else
                SetImgSource(Definitions.AddTypes.File);
        }

        private void clk_add(object sender, MouseButtonEventArgs e)
        {
            if (fType == Definitions.AddTypes.File)
                AddFiles();
            else
                AddFolders();
        }
       
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SaveItems();
        }
        #endregion

        //private void clk_save(object sender, RoutedEventArgs e)
        //{
        //    SaveItems();
        //}

        private void clk_addFile(object sender, RoutedEventArgs e)
        {
            AddFiles();
        }

        private void clk_addFolder(object sender, RoutedEventArgs e)
        {
            AddFolders();
        }
    }

    public class Imagem : INotifyPropertyChanged
    {
        private ImageSource _addImg;

        public ImageSource addImg
        {
            get { return _addImg; }
            set { _addImg = value; OnPropertyChanged("addImg"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
