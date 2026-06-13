using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using SharkyBrowser.SharkyFilter;
using SharkyBrowser.SharkyWeb;
using System.Collections.Generic;

namespace SharkyBrowser
{
    public sealed partial class SharkySecurityUI : UserControl
    {
        private VisibleSecurityState currentVisibleSecurityState;

        private int FilteredResourcesCount = 0;
        private readonly List<SharkyFilteredResource> FilteredResources = [];

        private readonly ResourceLoader resourceLoader = new(ResourceLoader.GetDefaultResourceFilePath(), "SecurityUIResources");

        public SharkySecurityUI()
        {
            InitializeComponent();
            FilteredResourcesListView.ItemsSource = FilteredResources;
        }

        public string DomainName
        {
            set
            {
                SecurityUIHeader.Text = string.Format(resourceLoader.GetString("SecurityUIHeader"), value);
            }
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
                    case "neutral":
                    default:
                        ConnectionSecurityStateBadge.IconSource = new SymbolIconSource() { Symbol = Symbol.Help };
                        ConnectionSecurityStateHeaderTextBlock.Text = resourceLoader.GetString("SecurityStateNeutral");
                        break;
                    case "insecure":
                    case "insecure-broken":
                        ConnectionSecurityStateBadge.IconSource = new SymbolIconSource() { Symbol = Symbol.Cancel };
                        ConnectionSecurityStateHeaderTextBlock.Text = resourceLoader.GetString("SecurityStateInsecure");
                        break;
                    case "secure":
                        ConnectionSecurityStateBadge.IconSource = new SymbolIconSource() { Symbol = Symbol.Accept };
                        ConnectionSecurityStateHeaderTextBlock.Text = resourceLoader.GetString("SecurityStateSecure");
                        break;
                    case "info":
                        ConnectionSecurityStateBadge.IconSource = new SymbolIconSource() { Symbol = Symbol.Help };
                        ConnectionSecurityStateHeaderTextBlock.Text = resourceLoader.GetString("SecurityStateInfo");
                        break;
                }

                if (value.CertificateSecurityState is not null)
                {
                    if (value.CertificateSecurityState.Issuer == "sharky")
                    {
                        CertificateTextBlock.Text = resourceLoader.GetString("SecurityStateSharky");
                    }
                    else
                    {
                        CertificateTextBlock.Text = string.Format(resourceLoader.GetString("SecurityProtocol"), value.CertificateSecurityState?.Protocol);
                        CertificateTextBlock.Text += "\n" + string.Format(resourceLoader.GetString("CertificateIssuer"), value.CertificateSecurityState?.Issuer ?? resourceLoader.GetString("UnknownIssuer"));
                        CertificateTextBlock.Text += "\n" + string.Format(resourceLoader.GetString("CertificateValidFrom"), SharkyUtils.UnixTimestampToDateTime((long)value.CertificateSecurityState?.ValidFrom));
                        CertificateTextBlock.Text += "\n" + string.Format(resourceLoader.GetString("CertificateValidUntil"), SharkyUtils.UnixTimestampToDateTime((long)value.CertificateSecurityState?.ValidTo));
                    }
                }
                else
                {
                    CertificateTextBlock.Text = "";
                }
            }
        }

        public void ResetFilteredResourcesCount()
        {
            FilteredResourcesCount = 0;
            FilteredResources.Clear();
            FilterStateBadge.IconSource = new SymbolIconSource() { Symbol = Symbol.Accept };
            FilterStateHeaderTextBlock.Text = resourceLoader.GetString("NoRequestBlocked");
        }

        public void PushFilteredResource(SharkyFilteredResource resource)
        {
            FilteredResourcesCount++;
            FilteredResources.Add(resource);
            FilterStateBadge.IconSource = new SymbolIconSource() { Symbol = Symbol.Important };
            FilterStateHeaderTextBlock.Text = string.Format(resourceLoader.GetString("SharkyBlockedRequests"), FilteredResourcesCount);
        }
    }
}
