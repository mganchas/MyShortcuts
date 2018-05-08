using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            setInitConditions();
        }
        #endregion

        #region Methods
        private void setInitConditions()
        {
            this.DataContext = xImg;
            lstv_atalhos.Items.Clear();

            setImgSource(Definitions.AddTypes.File);
            Folders.folderChecking();
            dStructToListItem(new Saver().readFile());

            txtSearch.Focus();

            this.Loaded += new RoutedEventHandler(setAppPosition);
        }

        private void dStructToListItem(List<Definitions.DataStruct> structList)
        {
            foreach (Definitions.DataStruct structure in structList)
                loopThroughItems(structure.value, structure.name);
        }

        private void setDropShortcuts(object sender, DragEventArgs e)
        {
            string[] lstCaminhos = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (lstCaminhos != null)
                addItemsListV(lstCaminhos);
            else
                MessageBox.Show("Sorry! Type not supported");
        }

        private bool isDuplicateItem(string path)
        {
            if (lstv_atalhos.Items.Count > 0)
            {
                foreach (ListItems listItem in lstv_atalhos.Items)
                    if (listItem.path.Equals(path)) { return true; }
            }

            return false;
        }

        private void loopThroughItems(string xCaminho, string Name)
        {
            if (!isDuplicateItem(xCaminho))
            {
                lstv_atalhos.Items.Add(new ListItems()
                {
                    title = Name ?? Definitions.getShortcutName(xCaminho),
                    image = Definitions.getImageSource(xCaminho),
                    path = xCaminho.ToString(),
                    error = Visibility.Collapsed,
                    nivel = 1.0
                });
            }
        }

        private void addItemsListV(string[] lstCaminhos)
        {
            foreach (string xCaminho in lstCaminhos)
                loopThroughItems(xCaminho, null);
        }

        private void setAppPosition(object sender, RoutedEventArgs e)
        {
            var areaTrab = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
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

        private void addFiles()
        {
            System.Windows.Forms.OpenFileDialog xDialog = new System.Windows.Forms.OpenFileDialog();
            xDialog.Multiselect = true;

            if (xDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                addItemsListV(xDialog.FileNames);
        }

        private void addFolders()
        {
            System.Windows.Forms.FolderBrowserDialog xDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (xDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                addItemsListV(new string[] { xDialog.SelectedPath.ToString() });
        }

        private void removeItems()
        {
            foreach (ListItems xItem in lstv_atalhos.SelectedItems)
            {
                lstv_atalhos.Items.Remove(xItem);
                removeItems();
                break;
            }
            lstv_atalhos.Items.Refresh();
        }

        private void setImgSource(Definitions.AddTypes type)
        {
            if (type == Definitions.AddTypes.File)
            {
                xImg.addImg = strToImgConverter("/MyShortcuts;component/Imagens/file_add.png");
                fType = Definitions.AddTypes.File;
            }
            else
            {
                fType = Definitions.AddTypes.Folder;
                xImg.addImg = strToImgConverter("/MyShortcuts;component/Imagens/folder_add.png");
            }
        }

        private ImageSource strToImgConverter(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }

        private bool shortcutStatus(ListItems oListItem)
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
       
        private void saveItems()
        {
            List<Definitions.DataStruct> dStruct = new List<Definitions.DataStruct>();

            foreach (ListItems listItem in lstv_atalhos.Items)
            {
                var xxx = (lstv_atalhos.Items[lstv_atalhos.Items.IndexOf(listItem)] as ListViewItem);
                

                dStruct.Add(new Definitions.DataStruct()
                {
                    type = (Definitions.Types)Definitions.getTypeFromString(Definitions.getShortcutType(listItem.path)),
                    value = listItem.path,
                    name = listItem.title
                });
            }

            new Saver().writeFile(dStruct, false);
        }
        #endregion

        #region Controls' Events
        private void clk_sair(object sender, RoutedEventArgs e)
        {
            //saveItems();
            this.Close();
        }

        private void clk_remover(object sender, RoutedEventArgs e)
        {
            removeItems();
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

            if (shortcutStatus(oListItem))
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
                setImgSource(Definitions.AddTypes.Folder);
            else
                setImgSource(Definitions.AddTypes.File);
        }

        private void clk_add(object sender, MouseButtonEventArgs e)
        {
            if (fType == Definitions.AddTypes.File)
                addFiles();
            else
                addFolders();
        }
       
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            saveItems();
        }
        #endregion

        private void clk_save(object sender, RoutedEventArgs e)
        {
            saveItems();
        }

        private void clk_addFile(object sender, RoutedEventArgs e)
        {
            addFiles();
        }

        private void clk_addFolder(object sender, RoutedEventArgs e)
        {
            addFolders();
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
