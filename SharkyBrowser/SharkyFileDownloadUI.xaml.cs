using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.Web.WebView2.Core;
using SharkyBrowser.SharkySettings;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharkyBrowser
{
    public sealed partial class SharkyFileDownloadUI : UserControl
    {
        public event EventHandler<SharkyFileDownloadUIEventArgs> DownloadRetryRequested;
        private List<CoreWebView2DownloadOperation> DownloadOperations;

        public SharkyFileDownloadUI()
        {
            InitializeComponent();
            DownloadOperations = new List<CoreWebView2DownloadOperation>();
        }

        public void AddDownloadItem(CoreWebView2DownloadOperation downloadOperation)
        {
            // Cacher l'élément par défaut
            NoDownloadInfoBar.Visibility = Visibility.Collapsed;
            downloadOperation.Pause();

            Button DownloadItemAction = new()
            {
                Content = new FontIcon()
                {
                    Glyph = "\uE769",
                    FontSize = 16
                },
                Tag = downloadOperation
            };
            DownloadItemAction.Click += DownloadItemAction_Click;

            Button DownloadStopButton = new()
            {
                Content = new FontIcon()
                {
                    Glyph = "\uE71A",
                    FontSize = 16
                },
                Tag = downloadOperation
            };
            DownloadStopButton.Click += DownloadStopButton_Click;

            Grid DownloadItemMiddle = new()
            {
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition(),
                    new RowDefinition()
                },
                Children =
                {
                    new TextBlock() { Text = downloadOperation.ResultFilePath.Substring(downloadOperation.ResultFilePath.LastIndexOf("\\") + 1) },
                    new ProgressBar() { IsIndeterminate = true },
                    new TextBlock() { Text = $"0 o/{downloadOperation.TotalBytesToReceive.ToString()} o", FontSize = 10 }
                },
                RowSpacing = 1
            };
            Grid.SetRow((FrameworkElement)DownloadItemMiddle.Children[0], 0);
            Grid.SetRow((FrameworkElement)DownloadItemMiddle.Children[1], 1);
            Grid.SetRow((FrameworkElement)DownloadItemMiddle.Children[2], 2);

            Grid DownloadItem = new()
            {
                Background = Application.Current.Resources["CardBackgroundFillColorDefaultBrush"] as Brush,
                BorderBrush = Application.Current.Resources["ControlElevationBorderBrush"] as Brush,
                Padding = new Thickness(8),
                CornerRadius = new CornerRadius(5),
                BorderThickness = new Thickness(1),
                ColumnSpacing = 12,
                ColumnDefinitions = {
                    new ColumnDefinition() { Width = GridLength.Auto },
                    new ColumnDefinition(),
                    new ColumnDefinition() { Width = GridLength.Auto },
                    new ColumnDefinition() { Width = GridLength.Auto }
                },
                Children =
                {
                    new FontIcon() { Glyph = "\uEC50", FontSize = 16 },
                    DownloadItemMiddle,
                    DownloadItemAction,
                    DownloadStopButton
                }
            };

            Grid.SetColumn((FrameworkElement)DownloadItem.Children[0], 0);
            Grid.SetColumn((FrameworkElement)DownloadItem.Children[1], 1);
            Grid.SetColumn((FrameworkElement)DownloadItem.Children[2], 2);
            Grid.SetColumn((FrameworkElement)DownloadItem.Children[3], 3);

            DownloadUIStackPanel.Children.Add(DownloadItem);
            DownloadOperations.Add(downloadOperation);

            downloadOperation.BytesReceivedChanged += DownloadOperation_BytesReceivedChanged;
            downloadOperation.StateChanged += DownloadOperation_StateChanged;
            downloadOperation.EstimatedEndTimeChanged += DownloadOperation_EstimatedEndTimeChanged;
            downloadOperation.Resume();
        }

        private void DownloadStopButton_Click(object sender, RoutedEventArgs e)
        {
            CoreWebView2DownloadOperation downloadOperation = (CoreWebView2DownloadOperation)((Button)sender).Tag;
            downloadOperation.Cancel();
        }

        private void DownloadItemAction_Click(object sender, RoutedEventArgs e)
        {
            CoreWebView2DownloadOperation downloadOperation = (CoreWebView2DownloadOperation)((Button)sender).Tag;
            if (downloadOperation.State == CoreWebView2DownloadState.Completed)
            {
                try
                {
                    // Completed operation: open the file
                    Process.Start(new ProcessStartInfo(downloadOperation.ResultFilePath)
                    {
                        UseShellExecute = true
                    });
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (downloadOperation.State == CoreWebView2DownloadState.InProgress)
            {
                // In progress = Pause it
                downloadOperation.Pause();
            }
            else
            {
                if (downloadOperation.CanResume)
                {
                    downloadOperation.Resume();
                }
                else
                {
                    DownloadRetryRequested.Invoke(this, new()
                    {
                        DownloadURI = downloadOperation.Uri
                    });
                }
            }
        }

        private void DownloadOperation_EstimatedEndTimeChanged(CoreWebView2DownloadOperation sender, object args)
        {
            RefreshAllDownloadItemsUI();
        }

        private void DownloadOperation_StateChanged(CoreWebView2DownloadOperation sender, object args)
        {
            RefreshAllDownloadItemsUI();
        }

        private void DownloadOperation_BytesReceivedChanged(CoreWebView2DownloadOperation sender, object args)
        {
            RefreshAllDownloadItemsUI();
        }

        private void RefreshAllDownloadItemsUI()
        {
            for (int i = 0; i < DownloadOperations.Count; i++)
            {
                Grid downloadItem = (Grid)DownloadUIStackPanel.Children[i + 1];
                Grid downloadItemDetails = (Grid)downloadItem.Children[1];
                ProgressBar progressBar = (ProgressBar)downloadItemDetails.Children[1];
                TextBlock subText = (TextBlock)downloadItemDetails.Children[2];
                Button actionBtn = (Button)downloadItem.Children[2];
                Button stopBtn = (Button)downloadItem.Children[3];

                if (DownloadOperations[i].State == CoreWebView2DownloadState.InProgress)
                {
                    long progress = (DownloadOperations[i].BytesReceived / DownloadOperations[i].TotalBytesToReceive) * 100;
                    progressBar.Visibility = Visibility.Visible; 
                    progressBar.IsIndeterminate = false;
                    progressBar.Value = progress;
                    subText.Text = $"{DownloadOperations[i].BytesReceived}/{DownloadOperations[i].TotalBytesToReceive}";
                    actionBtn.Content = new FontIcon()
                    {
                        Glyph = "\uE769",
                        FontSize = 16
                    };
                }
                else if (DownloadOperations[i].State == CoreWebView2DownloadState.Interrupted)
                {
                    subText.Text = $"{DownloadOperations[i].InterruptReason}";
                    progressBar.Visibility = Visibility.Collapsed;
                    actionBtn.Content = new FontIcon()
                    {
                        Glyph = "\uE768",
                        FontSize = 16
                    };

                    if (!DownloadOperations[i].CanResume)
                    {
                        stopBtn.Visibility = Visibility.Collapsed;
                        if (downloadItem.ColumnDefinitions.Count >= 4)
                        {
                            downloadItem.ColumnDefinitions.RemoveAt(3);
                        }
                    } 
                }
                else
                {
                    subText.Text = "Download completed.";
                    progressBar.Visibility = Visibility.Collapsed;
                    actionBtn.Content = new FontIcon()
                    {
                        Glyph = "\uE8A7",
                        FontSize = 16
                    };
                    stopBtn.Visibility = Visibility.Collapsed;
                    if(downloadItem.ColumnDefinitions.Count >= 4)
                    {
                        downloadItem.ColumnDefinitions.RemoveAt(3);
                    }
                }
            }
        }
    }

    public class SharkyFileDownloadUIEventArgs : EventArgs
    {
        public string DownloadURI;
    }
}
