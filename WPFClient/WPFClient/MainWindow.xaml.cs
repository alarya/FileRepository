using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Web.Http;  // need to add reference
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.IO;
using System.Windows.Forms;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.Hide();
        }

        void addFile(string file)
        {
            filelist.Items.Add(file);
        }
        void addFolder(string file)
        {
            folderlist.Items.Add(file);
        }

        private void LoadFilesForDownload(object sender, RoutedEventArgs e)
        {
            var message = new HttpRequestMessage();
            var content = new MultipartFormDataContent();
            message.Method = HttpMethod.Get;
            message.Content = null;
            message.RequestUri = new Uri("http://localhost:46234/api/FilesDownload");

            //SystemSounds.Beep.Play();

            var client = new HttpClient();

            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response = task.Result;
            HttpContent reply = response.Content;

            Task<string> t = reply.ReadAsStringAsync();
            string str = t.Result;
            Console.Write("\n  Get Response content: {0}\n", str);

            Console.Write("\n  Server has these files for download:");
            List<string> list = ParseArrayOfJsonObjects(str);

            foreach (string file in list)
            {
                if (Dispatcher.CheckAccess())
                    addFile(file);
                else
                    Dispatcher.Invoke(
                      new Action<string>(addFile),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { file }
                    );
            }

        }
        static List<string> ParseArrayOfJsonObjects(string str)
        {
            int end, begin = 0;
            List<string> list = new List<string>();
            do
            {
                begin = str.IndexOf('\"', begin) + 1;
                if (begin == 0)
                    break;
                end = str.IndexOf('\"', begin + 1) - 1;
                string temp = str.Substring(begin, end - begin + 1);
                list.Add(temp);
                begin = end + 2;
            } while (begin < str.Count());
            return list;
        }

        private void downloadFile(object sender, RoutedEventArgs e)
        {

            try
            {
                if (String.IsNullOrEmpty(filelist.SelectedValue.ToString()))
                {
                    System.Windows.MessageBox.Show("Please select a file for download");
                    return;
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Please select a file for download");
                return;
            }
            
            var message = new HttpRequestMessage();
            var content = new MultipartFormDataContent();            
            message.Content = null;
            string baseUri = "http://localhost:46234/api/FilesDownload/";
            message.Method = HttpMethod.Get;

            string downloadfile = filelist.SelectedValue.ToString() ;
            bool? dependencies_ = dependencies.IsChecked ; 

            var client = new HttpClient();

            baseUri = baseUri + "?path=" + downloadfile + "&dependencies=" + dependencies_;
            message.RequestUri = new Uri(baseUri);

            Task<Stream> task2;
            
            try{
                       task2 = client.GetStreamAsync(message.RequestUri);
            }
            catch
            {
                System.Windows.MessageBox.Show("Couldn't connect to server...please try later");
                return;
            }

            string filename = downloadfile.Split('/')[downloadfile.Split('/').Count()-1];
            
            Stream str2; ;
            try
            {
                str2 = task2.Result;
                
                if (str2 != null)
                    System.Windows.MessageBox.Show("File has been saved to received files");

                if (dependencies_ == false)
                {
                    FileStream fs = new FileStream("../../ReceivedFiles/" + filename, FileMode.Create, FileAccess.ReadWrite);
                    str2.CopyTo(fs);
                    fs.Seek(0, SeekOrigin.Begin);


                    StreamReader sr = new StreamReader(fs);
                    string fileText = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                }
                else
                {
                    int index = downloadfile.LastIndexOf('/');
                    string filezipname = downloadfile.Substring(index+1) + ".zip";
                    using (var fileStream = new FileStream("../../ReceivedFiles/" + filezipname, FileMode.Create))
                    {
                        //str2.Seek(0, SeekOrigin.Begin); this throws an exception
                        str2.CopyTo(fileStream);
                    }
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Server did not send any result..");
            }
        }

        private void LoadFoldersForUpload(object sender, RoutedEventArgs e)
        {
            var message = new HttpRequestMessage();
            var content = new MultipartFormDataContent();
            message.Method = HttpMethod.Get;
            message.Content = null;
            message.RequestUri = new Uri("http://localhost:46234/api/FileUpload");

            //SystemSounds.Beep.Play();

            var client = new HttpClient();

            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response = task.Result;
            HttpContent reply = response.Content;

            Task<string> t = reply.ReadAsStringAsync();
            string str = t.Result;
            Console.Write("\n  Get Response content: {0}\n", str);

            Console.Write("\n  Server has these files for download:");
            List<string> list = ParseArrayOfJsonObjects(str);

            foreach (string file in list)
            {
                if (Dispatcher.CheckAccess())
                    addFolder(file);
                else
                    Dispatcher.Invoke(
                      new Action<string>(addFolder),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { file }
                    );
            }

        }

        private void SelectFolder(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fs = new FolderBrowserDialog();

            String folderName;
            if(fs.ShowDialog().ToString() == "OK")
            {
                folderName = fs.SelectedPath;
                //System.Windows.MessageBox.Show(folderName);
                //string[] files = Directory.GetFiles(folderName);
                PathText.Text = folderName;
            }                
        }

        private void Upload(object sender, RoutedEventArgs e)
        {
            String folderName = PathText.Text;
            
            if(string.IsNullOrEmpty(folderName))
            {
                System.Windows.MessageBox.Show("Please select a local folder to upload");
                return;
            }
            try
            {
                String testSelectedfolder = folderlist.SelectedItem.ToString();
            }
            catch
            {
                System.Windows.MessageBox.Show("Please select a server folder for upload location");
                return;
            }
            string[] files = Directory.GetFiles(folderName);

            foreach(string file in files)
            {
                var message = new HttpRequestMessage();
                var content = new MultipartFormDataContent();

                var filestream = new FileStream(file, FileMode.Open);
                var fileName_ = System.IO.Path.GetFileName(file);
                content.Add(new StreamContent(filestream), "file", fileName_);

                message.Method = HttpMethod.Post;
                message.Content = content;
                string baseUri = "http://localhost:46234/api/FileUpload" + "?selectedFolder=" + folderlist.SelectedItem.ToString() + "&" + "selectedFile=" + fileName_;
                message.RequestUri = new Uri(baseUri);

                var client = new HttpClient();
                try
                {
                    client.SendAsync(message).ContinueWith(task =>
                        {
                            if (task.Result.IsSuccessStatusCode)
                            {
                                //PathText.Text = "Uploaded!!";
                            }
                        }
                        );
                    System.Windows.MessageBox.Show("Folder Uploaded");
                }
                catch
                {
                    System.Windows.MessageBox.Show("Could not connect to the server...please try later");
                }
            }
        }
    }
}
