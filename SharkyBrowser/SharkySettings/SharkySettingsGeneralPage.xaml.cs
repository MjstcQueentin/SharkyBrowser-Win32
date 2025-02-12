using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkyWeb;
using System;
using System.Collections.Generic;

namespace SharkyBrowser.SharkySettings
{
    public sealed partial class SharkySettingsGeneralPage : Page
    {
        SharkyUserSettings Settings;
        List<SharkyWebSearchEngine> SearchEngines = SharkyWebSearchEngine.UserSavedEngines;

        public SharkySettingsGeneralPage()
        {
            InitializeComponent();
            Settings = SharkyUserSettings.Instance;
            HomepageTextBox.Text = Settings.Homepage;
            DownloadFolderTextBox.Text = Settings.DownloadLocation;
            DefaultSearchEngineComboBox.SelectedItem = SearchEngines.Find(s => s.UrlPattern == Settings.SearchUrl);
        }

        private void HomepageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.Homepage = HomepageTextBox.Text;
            Settings.WriteToFile();
        }

        private void HomepageSuggestionButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            HyperlinkButton button = (HyperlinkButton)sender;
            HomepageTextBox.Text = (string)button.Tag;
        }

        private void DefaultSearchEngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox castedSender = (ComboBox)sender;
            if (castedSender.SelectedItem is null) return;
            SharkyWebSearchEngine castedItem = (SharkyWebSearchEngine)castedSender.SelectedItem;
            Settings.SearchUrl = castedItem.UrlPattern;
            Settings.WriteToFile();
        }

        private async void AddSearchEngineButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            SharkySettingsGeneralPageAddSearchEngineDialog dialogContent = new();

            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Add a search engine to the list";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = dialogContent;

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                SearchEnginesListView.ItemsSource = DefaultSearchEngineComboBox.ItemsSource = SearchEngines = SharkyWebSearchEngine.AddEngineToList(dialogContent.NewSearchEngine.Name, dialogContent.NewSearchEngine.UrlPattern);
            }
        }

        private void RemoveSearchEngineButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (SearchEnginesListView.SelectedIndex < 0) return;
            SearchEnginesListView.ItemsSource = DefaultSearchEngineComboBox.ItemsSource = SearchEngines = SharkyWebSearchEngine.RemoveEngineFromList(SearchEnginesListView.SelectedIndex);
        }

        private void DownloadFolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.DownloadLocation = DownloadFolderTextBox.Text;
            Settings.WriteToFile();
        }

        private void CloseWarningCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Settings.MultipleTabWarningOnClose = true;
            Settings.WriteToFile();
        }

        private void CloseWarningCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Settings.MultipleTabWarningOnClose = false;
            Settings.WriteToFile();
        }
    }
}
