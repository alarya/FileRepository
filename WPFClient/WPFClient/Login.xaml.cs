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

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void LoginButton(object sender, RoutedEventArgs e)
        {
            
            if(string.IsNullOrEmpty(userId.Text) || string.IsNullOrEmpty(password.Password))
            {
                MessageBox.Show("Please enter your credentials");
                return;
            }
            var message = new HttpRequestMessage();
            var content = new MultipartFormDataContent();
            //message.Content = null;
  
            String userName = userId.Text;
            String password_ = password.Password;
            //message.Headers.Add("username", userName);
            //message.Headers.Add("password", password_);
            //content.Headers.Add("username", userName);
            //content.Headers.Add("password", password_);
            //message.Content = content;

            //content.Add(content, userName,"username");
            //UserCredentials user = new UserCredentials();
            //user.username = userId.Text;;
            //user.password = password.Text;
            //message.Content = user;

            string baseUri = "http://localhost:46234/api/LoginClient";
            baseUri = baseUri + "?username=" + userName +"&password=" + password_;
            message.Method = HttpMethod.Post;
            HttpClient client = new HttpClient();
            message.RequestUri = new Uri(baseUri);


            String result;
            try
            {
               result = await client.GetStringAsync(message.RequestUri);
               if (result.ToString() == "\"successful\"")
               {
                   MainWindow Home = new MainWindow();
                   Home.Show();
                   this.Close();
               }
                else
                MessageBox.Show("Invalid credentials");
            }
            catch
            {
                MessageBox.Show("Could not get a response from the server...please try later");
            }
                //MessageBox.Show("Invalid credentials");
        }     
    }

    public class UserCredentials
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
