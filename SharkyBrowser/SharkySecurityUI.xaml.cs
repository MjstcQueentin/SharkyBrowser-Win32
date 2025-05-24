using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkyWeb;

namespace SharkyBrowser
{
    public sealed partial class SharkySecurityUI : UserControl
    {
        private VisibleSecurityState currentVisibleSecurityState;

        public SharkySecurityUI()
        {
            InitializeComponent();
        }

        public VisibleSecurityState CurrentVisibleSecurityState
        {
            get => currentVisibleSecurityState;
            set
            {
                currentVisibleSecurityState = value;
                switch (value.SecurityState)
                {
                    case "unknown":
                        ServerSecurityInfoBar.Severity = InfoBarSeverity.Informational;
                        ServerSecurityInfoBar.Message = "The security state of this connection is unknown.";
                        break;
                    case "neutral":
                        ServerSecurityInfoBar.Severity = InfoBarSeverity.Informational;
                        ServerSecurityInfoBar.Message = "The security state of this connection is neutral.";
                        break;
                    case "insecure":
                    case "insecure-broken":
                        ServerSecurityInfoBar.Severity = InfoBarSeverity.Error;
                        ServerSecurityInfoBar.Message = "This connection is insecure.";
                        break;
                    case "secure":
                        ServerSecurityInfoBar.Severity = InfoBarSeverity.Success;
                        ServerSecurityInfoBar.Message = "This connection is secure.";
                        break;
                    case "info":
                        ServerSecurityInfoBar.Severity = InfoBarSeverity.Warning;
                        ServerSecurityInfoBar.Message = "This connection has information.";
                        break;
                }

                if(value.CertificateSecurityState?.Issuer is not null)
                {
                    IssuerTextBlock.Visibility = Visibility.Visible;
                    IssuerTextBlock.Text = $"Issued by: {value.CertificateSecurityState.Issuer}";
                } else
                {
                    IssuerTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
