using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace ProjMarduk.AppPackagesViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PackageManager packageManager = new Windows.Management.Deployment.PackageManager();
        private List<Package> packages;
        private CollectionView colView;

        public MainWindow()
        {
            InitializeComponent();
        }
        async Task<List<Package>> GetPackages()
        {
            return packageManager.FindPackages().ToList();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            listView.ItemsSource = null;

            progressBar.IsIndeterminate = true;

            packages = await GetPackages();
            listView.ItemsSource = packages;

            colView = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);

            switch (comboBox.SelectedIndex)
            {
                case 0:
                    colView.Filter = FullNameFilter;
                    break;
                case 1:
                    colView.Filter = PublisherFilter;
                    break;
            }

            progressBar.IsIndeterminate = false;
        }

        private bool FullNameFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((item as Package).Id.FullName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool PublisherFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((item as Package).Id.Publisher.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (colView != null)
            {
                switch (comboBox.SelectedIndex)
                {
                    case 0:
                        colView.Filter = FullNameFilter;
                        break;
                    case 1:
                        colView.Filter = PublisherFilter;
                        break;
                }

                CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();
            }
        }

        private void menuItemOpenInstallDir_OnClick(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1)
            {
                try
                {
                    var item = packages[listView.SelectedIndex];


                    if (!string.IsNullOrEmpty(item.InstalledLocation?.Path))
                    {
                        Process process = new Process
                        {
                            StartInfo =
                            {
                                UseShellExecute = true,
                                FileName = item.InstalledLocation.Path
                            }
                        };
                        process.Start();

                        return;
                    }

                    MessageBox.Show("Installation folder is not found!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void menuItemOpenLocalDir_OnClick(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1)
            {
                try
                {
                    var item = packages[listView.SelectedIndex];
                    var fullpath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                   @"\Packages\" + item.Id.FamilyName;

                    if (Directory.Exists(fullpath))
                    {
                        Process process = new Process
                        {
                            StartInfo =
                            {
                                UseShellExecute = true,
                                FileName = fullpath
                            }
                        };
                        process.Start();
                    }
                    else
                    {
                        MessageBox.Show("Local storage folder is not found!");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void menuItemStatus_OnClick(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1)
            {
                try
                {
                    var item = packages[listView.SelectedIndex];

                    MessageBox.Show(
                        $"DataOffline: {item.Status.DataOffline}\nDependencyIssue: {item.Status.DependencyIssue}\nDeploymentInProgress: {item.Status.DeploymentInProgress}\nDisabled: {item.Status.Disabled}\nLicenseIssue: {item.Status.LicenseIssue}\nModified: {item.Status.Modified}\nNeedsRemediation: {item.Status.NeedsRemediation}\nNotAvailable: {item.Status.NotAvailable}\nPackageOffline: {item.Status.PackageOffline}\nServicing: {item.Status.Servicing}\nTampered: {item.Status.Tampered}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (colView != null)
            {
                switch (comboBox.SelectedIndex)
                {
                    case 0:
                        colView.Filter = FullNameFilter;
                        break;
                    case 1:
                        colView.Filter = PublisherFilter;
                        break;
                }

                CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();
            }
        }
    }
}
