﻿using DatabaseMigration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Xunkong.Desktop.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[INotifyPropertyChanged]
public sealed partial class WelcomPage : Page
{


    public WelcomPage()
    {
        this.InitializeComponent();
        Loaded += WelcomPage_Loaded;
    }



    private void WelcomPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            needToMigrateDatabase = Migration.NeedToMigrate();
        }
        catch { }
        try
        {
            var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            WebView2State = $"已安装 ({version})";
        }
        catch
        {
            WebView2State = "未检测到 WebView2 Runtime";
        }
    }


    private bool needToMigrateDatabase;


    [ObservableProperty]
    private string _webView2State;



    private string GetXunkongDataPath()
    {
        return XunkongEnvironment.UserDataPath;
    }




    private void _Button_RefreshStats_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            WebView2State = $"已安装 ({version})";
        }
        catch
        {
            WebView2State = "未检测到 WebView2 Runtime";
        }
    }



    private void _Button_Download_Click(object sender, RoutedEventArgs e)
    {
        const string url = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";
        try
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = url,
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            NotificationProvider.Error(ex);
        }
    }



    private async void _Button_InstallFont_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Fonts/Segoe Fluent Icons.ttf"));
            Process.Start(new ProcessStartInfo
            {
                FileName = file.Path,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            NotificationProvider.Error(ex);
        }
    }


    int showIndex = 1;


    private async void _Button_Next_Click(object sender, RoutedEventArgs e)
    {
        showIndex++;
        switch (showIndex)
        {
            case 2:
                Tip_1.Visibility = Visibility.Collapsed;
                Tip_2.Visibility = Visibility.Visible;
                await ClockDown();
                break;
            case 3:
                Tip_2.Visibility = Visibility.Collapsed;
                Tip_3.Visibility = Visibility.Visible;
                await ClockDown();
                break;
            case 4:
                Tip_3.Visibility = Visibility.Collapsed;
                Tip_4.Visibility = Visibility.Visible;
                if (!needToMigrateDatabase)
                {
                    _Button_Next.Content = "已完成";
                    showIndex++;
                }
                await ClockDown();
                break;
            case 5:
                Tip_4.Visibility = Visibility.Collapsed;
                Tip_5.Visibility = Visibility.Visible;
                _Button_Next.Content = "已完成";
                _Button_Next.IsEnabled = false;
                await Task.Delay(100);
                try
                {
                    Migration.Migrate();
                }
                catch (Exception ex)
                {
                    NotificationProvider.Error(ex);
                }
                finally
                {
                    _ProgressRing_MigrateDatabase.IsActive = false;
                    _Button_Next.IsEnabled = true;
                }
                break;
            default:
                try
                {
                    AppSetting.SetValue(SettingKeys.HasShownWelcomePage, true);
                    MainWindow.Current.Navigate(typeof(MainPage));
                }
                catch (Exception ex)
                {
                    NotificationProvider.Error(ex);
                }
                break;
        }
    }



    private async Task ClockDown()
    {
        _Button_Next.IsEnabled = false;
        _TextBlock_ClockDown.Visibility = Visibility.Visible;
        _TextBlock_ClockDown.Text = "3s";
        await Task.Delay(1000);
        _TextBlock_ClockDown.Text = "2s";
        await Task.Delay(1000);
        _TextBlock_ClockDown.Text = "1s";
        await Task.Delay(1000);
        _TextBlock_ClockDown.Text = "";
        _TextBlock_ClockDown.Visibility = Visibility.Collapsed;
        _Button_Next.IsEnabled = true;
    }



    private void _Button_Privacy_Click(object sender, RoutedEventArgs e)
    {
        const string url = "https://go.xunkong.cc/desktop/privacy-policy";
        try
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = url,
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            NotificationProvider.Error(ex);
        }
    }


}
