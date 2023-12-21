using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SP_exam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cancel;

        public MainWindow()
        {
            InitializeComponent();
        }

        DirectoryInfo directoryTo;
        DirectoryInfo directoryFrom;
        Task[] tasks;

        private void Button_Click_WhereFrom(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();
                WhereFromTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void Button_Click_WhereTo(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();
                WhereToTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void Button_Click_Copy(object sender, RoutedEventArgs e)
        {
            try
            {
               

                TextBlock[] textBlocksFileNames = { tb_fileName_1, tb_fileName_2, tb_fileName_3, tb_fileName_4, tb_fileName_5, tb_fileName_6, tb_fileName_7, tb_fileName_8 };

                var size = GetDirectorySize(WhereFromTextBox.Text);
                tb_FullSize.Text = size.ToString();
                

                directoryFrom = new DirectoryInfo(WhereFromTextBox.Text);
                directoryTo = new DirectoryInfo(WhereToTextBox.Text);

                FileInfo[] filesFrom = directoryFrom.GetFiles();
                FileInfo[] filesTo = directoryTo.GetFiles();

                if (directoryFrom.GetFiles().Length > 0)
                {
                    buttonCopy.Visibility = Visibility.Collapsed;
                    buttonStop.Visibility = Visibility.Visible;
                }
                
                progressBarCommon.Maximum = size;

                for (int i = 0; i < filesFrom.Length; i++)
                {
                    textBlocksFileNames[i].Text = filesFrom[i].Name;
                }

                tasks = new Task[filesFrom.Length];

                cancel = new CancellationTokenSource();
                var token = cancel.Token;
                try
                {
                    for (int i = 0; i < filesFrom.Length; i++)
                    {
                        int iCopy = i;

                        string[] createPathTo = { WhereToTextBox.Text, filesFrom[iCopy].Name };
                        string pathTo = System.IO.Path.Combine(createPathTo);

                        tasks[iCopy] = Copy(filesFrom[iCopy].FullName, pathTo, iCopy, token);
                    }
                    await Task.WhenAll(tasks);
                    System.Windows.MessageBox.Show("Всё успешно скопировано");
                }
                catch (OperationCanceledException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                
            }         
        }

        private async Task Copy(string fromWhere, string whereTo, int j, CancellationToken token)
        {
            

            TextBlock[] textBlocksFileSize = { tb_fileSize_1, tb_fileSize_2, tb_fileSize_3, tb_fileSize_4, tb_fileSize_5, tb_fileSize_6, tb_fileSize_7, tb_fileSize_8 };
            System.Windows.Controls.ProgressBar[] progressBars = { progressBar1, progressBar2, progressBar3, progressBar4, progressBar5, progressBar6, progressBar7, progressBar8 };

            byte[] buffer = new byte[1024];
            long copied = 0;
            long length = new FileInfo(fromWhere).Length;

            using Stream from = File.OpenRead(fromWhere);
            using Stream to = File.OpenWrite(whereTo);


            progressBars[j].Maximum = new FileInfo(fromWhere).Length;


            while (copied < length)
            {
                token.ThrowIfCancellationRequested();

                int read = await from.ReadAsync(buffer, 0, buffer.Length);
                await to.WriteAsync(buffer, 0, read);
                copied += read;
                progressBars[j].Visibility = Visibility.Visible;
                progressBars[j].Dispatcher.Invoke(() => progressBars[j].Value += read);
                progressBarCommon.Dispatcher.Invoke(() => progressBarCommon.Value += read);
                textBlocksFileSize[j].Text = copied.ToString() + " / " + length;

            }
        }
        static long GetDirectorySize(string p)
        {
            string[] a = Directory.GetFiles(p, "*.*");
            long b = 0;
            foreach (string name in a)
            {
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            return b;
        }

        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {

            cancel.Cancel();
            buttonCopy.IsEnabled = false;

           //System.Windows.MessageBox.Show("operation was declined");

            DeleteFiles();

            buttonStop.Visibility = Visibility.Collapsed;
            buttonCopy.Visibility = Visibility.Visible;

        }

        private async Task DeleteFiles()
        {
            foreach (FileInfo item in directoryTo.EnumerateFiles())
            {
                await Task.Run(() => item.Delete());
            }
        }

        
    }
}
