using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
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

namespace adpu
{
    public enum LogMessage
    {
        DomainConnectionFail,
        Authenticated,
        UsernameNotFound,
        PasswordInvalid,
        UnknownError
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(TXT_Username.Text) || string.IsNullOrWhiteSpace(TXT_Password.Password))
            {
                BTN_Check.IsEnabled = false;
            }
            else
            {
                BTN_Check.IsEnabled = true;
            }
        }

        private void BTN_Check_Click(object sender, RoutedEventArgs e)
        {
            ConLogger();
            try
            {
                PrincipalContext _ADCon = new PrincipalContext(ContextType.Domain, "AD");

                using (_ADCon)
                {
                    UserPrincipal usr = UserPrincipal.FindByIdentity(_ADCon,
                                               IdentityType.SamAccountName,
                                               TXT_Username.Text);

                        if (usr != null)
                    {
                        ConLogger(usr);

                        try
                        {
                            if (_ADCon.ValidateCredentials(TXT_Username.Text, TXT_Password.Password,ContextOptions.Negotiate))
                            {
                                ConLogger(LogMessage.Authenticated, TXT_Username.Text);
                            }
                            else
                            {

                                ConLogger(LogMessage.PasswordInvalid, TXT_Password.Password);
                            }
                        }
                        catch (Exception ex)
                        {
                            ConLogger(LogMessage.UnknownError, ex.Message);
                        }
                    }
                    else
                    {
                        ConLogger(LogMessage.UsernameNotFound, TXT_Username.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                ConLogger(LogMessage.DomainConnectionFail);
            }

            
        }


        


        public void ConLogger(UserPrincipal usr)
        {
            Details.Text = $@"{usr.Guid}{Environment.NewLine}KP Broj: {usr.EmployeeId}{Environment.NewLine}Ime i prezime: {usr.DisplayName}{Environment.NewLine}E-mail: {usr.EmailAddress}{Environment.NewLine}";
        }
        public void ConLogger(LogMessage err, string name = null)
        {
            switch(err)
            {
                case LogMessage.Authenticated:
                    Logger.Text = $@"Unesena lozinka za user {name} je ispravna."+Environment.NewLine;
                    Logger.Foreground = Brushes.Green;
                    break;
                case LogMessage.PasswordInvalid:
                    Logger.Text = $@"[{name}] nije ispravna lozinka za ovaj user." + Environment.NewLine;
                    Logger.Foreground = Brushes.Orange;
                    break;
                case LogMessage.UsernameNotFound:
                    Logger.Text = $@"Korisničko ime {name} nije pronađeno." + Environment.NewLine;
                    Logger.Foreground = Brushes.Orange;
                    break;
                case LogMessage.DomainConnectionFail:
                    Logger.Text = $@"Neuspješno spajanje na AD Server." + Environment.NewLine;
                    Logger.Foreground = Brushes.Red;
                    break;
                default:
                    Logger.Text = "";
                    Details.Text = @"Nepoznata greška." + Environment.NewLine + "Detalji u nastavku:"+Environment.NewLine+name+Environment.NewLine;
                    Logger.Foreground = Brushes.Black;
                    break;

            }
        }
        public void ConLogger()
        {
            Logger.Text = "";
        }

        private void PasswordChange(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrWhiteSpace(TXT_Username.Text) || string.IsNullOrWhiteSpace(TXT_Password.Password))
            {
                BTN_Check.IsEnabled = false;
            }
            else
            {
                BTN_Check.IsEnabled = true;
            }
        }

    }
}
