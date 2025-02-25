﻿using CommunityToolkit.WinUI.Notifications;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
using System.Net.Http;
using System.Numerics;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Services.Store;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using Xunkong.ApiClient;
using Xunkong.Desktop.Controls;
using Xunkong.Hoyolab;
using Xunkong.Hoyolab.Account;
using Xunkong.Hoyolab.Activity;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Xunkong.Desktop.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[INotifyPropertyChanged]
public sealed partial class HomePage : Page
{

    private const string FallbackWallpaperUri = "ms-appx:///Assets/Images/102203689_p0.jpg";
    private readonly static WallpaperInfo FallbackWallpaper = new WallpaperInfo
    {
        Title = "原神2周年記念",
        Author = "アナ",
        Description = "おめでとうございます！\r\nこれからも旅人と共に、星と深淵を目指せ！",
        FileName = "[アナ] 原神2周年記念 [102203689_p0].jpg",
        Source = "https://www.pixiv.net/artworks/102203689",
        Url = "https://file.xunkong.cc/wallpaper/102203689_p0.webp"
    };

    private readonly XunkongApiService _xunkongApiService;

    private readonly HoyolabService _hoyolabService;

    private readonly HoyolabClient _hoyolabClient;


    public HomePage()
    {
        this.InitializeComponent();
        _xunkongApiService = ServiceProvider.GetService<XunkongApiService>()!;
        _hoyolabService = ServiceProvider.GetService<HoyolabService>()!;
        _hoyolabClient = ServiceProvider.GetService<HoyolabClient>()!;
        Loaded += HomePage_Loaded;
    }



    [ObservableProperty]
    private WallpaperInfo wallpaperInfo;


    [ObservableProperty]
    private List<GrowthScheduleItem> growthScheduleItems;


    [ObservableProperty]
    private List<Announcement> finishingActivities;


    private void HomePage_Loaded(object sender, RoutedEventArgs e)
    {
        // 推荐图片
        InitializeWallpaperAsync();
        // 实时便笺
        GetDailyNotesAsync();
        // 今天刷什么
        GetCalendarAndGrowthScheduleAsync();
        // 即将结束的活动
        GetFinishingActivityAsync();
        // 通知
        GetNotificationContentAsync();
        // 更新
        CheckUpdateAsync();
    }



    #region Wallpaper


    /// <summary>
    /// 图片最大高度
    /// </summary>
    private double imageMaxHeight;
    /// <summary>
    /// 图片高宽比
    /// </summary>
    private double heightDivWidth;
    /// <summary>
    /// 图片 Visual
    /// </summary>
    private SpriteVisual imageVisual;


    /// <summary>
    /// 初始化推荐图片，并且下载新的
    /// </summary>
    private async void InitializeWallpaperAsync()
    {
        try
        {
            if (!AppSetting.GetValue(SettingKeys.EnableHomePageWallpaper, true))
            {
                return;
            }
            _Grid_Image.Visibility = Visibility.Visible;
            string? file = null;
            bool skipDownload = false;
            if (AppSetting.GetValue<bool>(SettingKeys.UseCustomWallpaper))
            {
                var path = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "CustomWallpaper.png");
                if (File.Exists(path))
                {
                    file = path;
                    skipDownload = true;
                    WallpaperInfo = new WallpaperInfo { Url = path, FileName = "CustomWallpaper.png" };
                }
            }
            if (string.IsNullOrWhiteSpace(file))
            {
                var wallpaper = _xunkongApiService.GetPreparedWallpaper();
                if (wallpaper is null)
                {
                    WallpaperInfo = FallbackWallpaper;
                    file = (await StorageFile.GetFileFromApplicationUriAsync(new(FallbackWallpaperUri))).Path;
                }
                else
                {
                    var cachedFile = await XunkongCache.Instance.GetFileFromCacheAsync(new(wallpaper.Url));
                    if (cachedFile != null)
                    {
                        WallpaperInfo = wallpaper;
                        file = cachedFile.Path;
                    }
                    else
                    {
                        WallpaperInfo = FallbackWallpaper;
                        file = (await StorageFile.GetFileFromApplicationUriAsync(new(FallbackWallpaperUri))).Path;
                    }
                }
            }
            imageMaxHeight = MainWindow.Current.Height * 0.75 / MainWindow.Current.UIScale;
            _Grid_Image.MaxHeight = imageMaxHeight;
            await LoadBackgroundImage(file);
            if (NetworkHelper.IsInternetOnMeteredConnection() && !skipDownload)
            {
                if (!AppSetting.GetValue<bool>(SettingKeys.DownloadWallpaperOnMeteredInternet))
                {
                    // 使用按流量计费的网络时，不下载新图片
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "加载推荐图片");
        }
        finally
        {
            c_StackPanel_QuickAction.Visibility = Visibility.Visible;
        }
        Task.Run(() =>
        {
            try
            {
                _xunkongApiService.PrepareNextWallpaperAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "下载新图片");
            }
        }).ConfigureAwait(false).GetAwaiter();
    }

    /// <summary>
    /// 拖入图片
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _Grid_Image_DragOver(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems) || e.DataView.Contains(StandardDataFormats.Bitmap))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
        }
    }


    /// <summary>
    /// 拖入图片
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void _Grid_Image_Drop(object sender, DragEventArgs e)
    {
        try
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                var item = items.FirstOrDefault() as StorageFile;
                if (item != null)
                {
                    using var fs = await item.OpenReadAsync();
                    var decoder = await BitmapDecoder.CreateAsync(fs);
                    heightDivWidth = (double)decoder.PixelHeight / decoder.PixelWidth;
                    await LoadBackgroundImage(fs.AsStream());
                    WallpaperInfo = null!;
                    var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("CustomWallpaper.png", CreationCollisionOption.ReplaceExisting);
                    await item.CopyAndReplaceAsync(file);
                }
            }
            if (e.DataView.Contains(StandardDataFormats.Bitmap))
            {
                var r = await e.DataView.GetBitmapAsync();
                using var stream = await r.OpenReadAsync();
                var decoder = await BitmapDecoder.CreateAsync(stream);
                heightDivWidth = (double)decoder.PixelHeight / decoder.PixelWidth;
                await LoadBackgroundImage(stream.AsStream());
                WallpaperInfo = null!;
                var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("CustomWallpaper.png", CreationCollisionOption.ReplaceExisting);
                using var fs = await file.OpenStreamForWriteAsync();
                var s = stream.AsStream();
                s.Position = 0;
                await s.CopyToAsync(fs);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "拖入图片文件");
            NotificationProvider.Error("无法识别文件");
        }
    }



    /// <summary>
    /// 加载背景图片
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private async Task LoadBackgroundImage(string file)
    {
        try
        {
            using var stream = File.OpenRead(file);
            var decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
            heightDivWidth = (double)decoder.PixelHeight / decoder.PixelWidth;
            var compositor = ElementCompositionPreview.GetElementVisual(_Border_BackgroundImage).Compositor;
            await LoadBackgroundImage(stream);
        }
        catch (COMException ex)
        {
            // 部分设备会因为缺少 Webp 解码器而出现 COMExeption 组件初始化失败。（0x88982F8B）
            // 还有可能会出现其他不知道的错误
            // 使用无透明度效果的图片代替，这个代替方法也没有用
            // 应该去应用商店更新解码组件
            Logger.Error(ex, "使用 Win2D 加载背景图片");
            if (ex.HResult == unchecked((int)0x88982F8B))
            {
                NotificationProvider.ShowWithButton(InfoBarSeverity.Warning, "出错了", "缺少 Webp 图像解码组件，请在应用商店中下载「Webp 图像扩展」", "打开商店",
                    async () => await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?productid=9PG2DK419DRG&mode=mini")));
            }
        }
    }


    private async Task LoadBackgroundImage(Stream stream)
    {
        var ms = new MemoryStream();
        stream.Position = 0;
        await stream.CopyToAsync(ms);
        ms.Position = 0;
        var imageSurface = LoadedImageSurface.StartLoadFromStream(ms.AsRandomAccessStream());
        var compositor = ElementCompositionPreview.GetElementVisual(_Border_BackgroundImage).Compositor;
        var imageBrush = compositor.CreateSurfaceBrush();
        imageBrush.Surface = imageSurface;
        imageBrush.Stretch = CompositionStretch.UniformToFill;
        imageBrush.VerticalAlignmentRatio = 0;

        var linearGradientBrush1 = compositor.CreateLinearGradientBrush();
        linearGradientBrush1.StartPoint = Vector2.Zero;
        linearGradientBrush1.EndPoint = Vector2.UnitY;
        linearGradientBrush1.ColorStops.Add(compositor.CreateColorGradientStop(0, Colors.White));
        linearGradientBrush1.ColorStops.Add(compositor.CreateColorGradientStop(0.95f, Colors.Black));

        var linearGradientBrush2 = compositor.CreateLinearGradientBrush();
        linearGradientBrush2.StartPoint = new Vector2(0.5f, 0);
        linearGradientBrush2.EndPoint = Vector2.UnitY;
        linearGradientBrush2.ColorStops.Add(compositor.CreateColorGradientStop(0, Colors.White));
        linearGradientBrush2.ColorStops.Add(compositor.CreateColorGradientStop(1, Colors.Black));

        var blendEffect = new BlendEffect
        {
            Mode = BlendEffectMode.Multiply,
            Background = new CompositionEffectSourceParameter("Background"),
            Foreground = new CompositionEffectSourceParameter("Foreground"),
        };

        var blendEffectFactory = compositor.CreateEffectFactory(blendEffect);
        var gradientEffectBrush = blendEffectFactory.CreateBrush();
        gradientEffectBrush.SetSourceParameter("Background", linearGradientBrush2);
        gradientEffectBrush.SetSourceParameter("Foreground", linearGradientBrush1);


        var invertEffect = new LuminanceToAlphaEffect
        {
            Source = new CompositionEffectSourceParameter("Source"),
        };

        var invertEffectFactory = compositor.CreateEffectFactory(invertEffect);
        var opacityEffectBrush = invertEffectFactory.CreateBrush();
        opacityEffectBrush.SetSourceParameter("Source", gradientEffectBrush);

        var maskEffect = new AlphaMaskEffect
        {
            AlphaMask = new CompositionEffectSourceParameter("Mask"),
            Source = new CompositionEffectSourceParameter("Source"),
        };

        var maskFactory = compositor.CreateEffectFactory(maskEffect);
        var maskEffectBrush = maskFactory.CreateBrush();
        maskEffectBrush.SetSourceParameter("Mask", opacityEffectBrush);
        maskEffectBrush.SetSourceParameter("Source", imageBrush);

        imageVisual = compositor.CreateSpriteVisual();
        imageVisual.Brush = maskEffectBrush;

        var width = _Border_BackgroundImage.ActualWidth;
        var height = Math.Clamp(width * heightDivWidth, 0, imageMaxHeight);
        imageVisual.Size = new Vector2((float)width, (float)height);
        _Border_BackgroundImage.Height = height;
        _Border_BackgroundImage.Visibility = Visibility.Visible;
        ElementCompositionPreview.SetElementChildVisual(_Border_BackgroundImage, imageVisual);
    }


    /// <summary>
    /// 改变背景图片大小
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _Border_BackgroundImage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        try
        {
            if (e.NewSize == e.PreviousSize)
            {
                return;
            }
            var width = _Border_BackgroundImage.ActualWidth;
            var height = Math.Clamp(width * heightDivWidth, 0, imageMaxHeight);
            if (imageVisual is not null)
            {
                imageVisual.Size = new Vector2((float)width, (float)height);
                _Border_BackgroundImage.Height = height;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "改变背景图片大小");
        }
    }


    /// <summary>
    /// 浏览图片大图
    /// </summary>
    [RelayCommand]
    private void OpenImageViewer()
    {
        if (WallpaperInfo is not null)
        {
            if (WallpaperInfo == FallbackWallpaper)
            {
                MainWindow.Current.SetFullWindowContent(new ImageViewer { Source = FallbackWallpaperUri });
            }
            else
            {
                MainWindow.Current.SetFullWindowContent(new ImageViewer { Source = WallpaperInfo.Url, DecodeFromStream = true });
            }
        }
    }


    /// <summary>
    /// 复制图片
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task CopyImageAsync()
    {
        try
        {
            StorageFile? file = null;
            if (WallpaperInfo == FallbackWallpaper)
            {
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(FallbackWallpaperUri));
            }
            else
            {
                file = await XunkongCache.GetFileFromUriAsync(WallpaperInfo?.Url);
            }
            if (file != null)
            {
                ClipboardHelper.SetBitmap(file);
                _Button_Copy.Content = "\xE8FB";
                await Task.Delay(3000);
                _Button_Copy.Content = "\xE8C8";
            }
        }
        catch (Exception ex)
        {
            NotificationProvider.Error(ex, "复制图片");
            Logger.Error(ex, "复制图片");
        }
    }



    /// <summary>
    /// 保存图片
    /// </summary>
    [RelayCommand]
    private async Task SaveWallpaper()
    {
        if (string.IsNullOrWhiteSpace(WallpaperInfo?.Url))
        {
            return;
        }
        try
        {
            StorageFile? file = null;
            if (WallpaperInfo == FallbackWallpaper)
            {
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(FallbackWallpaperUri));
            }
            else
            {
                file = await XunkongCache.GetFileFromUriAsync(WallpaperInfo?.Url);
            }
            if (file is null)
            {
                NotificationProvider.Warning("找不到缓存的文件", 3000);
                return;
            }
            var destFolder = AppSetting.GetValue<string>(SettingKeys.WallpaperSaveFolder) ?? Path.Combine(XunkongEnvironment.UserDataPath, "Wallpaper");
            var fileName = WallpaperInfo?.FileName ?? Path.GetFileName(WallpaperInfo?.Url)!;
            var destPath = Path.Combine(destFolder, fileName);
            Action openImageAction = () => Process.Start(new ProcessStartInfo { FileName = destPath, UseShellExecute = true });
            if (File.Exists(destPath))
            {
                Action overwriteAction = () =>
                {
                    Directory.CreateDirectory(destFolder);
                    File.Copy(file.Path, destPath, true);
                    NotificationProvider.ShowWithButton(InfoBarSeverity.Success, "已保存", fileName, "打开文件", openImageAction, null, 3000);
                };
                NotificationProvider.ShowWithButton(InfoBarSeverity.Warning, "文件已存在", null, "覆盖文件", overwriteAction, null, 3000);
                return;
            }
            else
            {
                Directory.CreateDirectory(destFolder);
                File.Copy(file.Path, destPath, true);
                NotificationProvider.ShowWithButton(InfoBarSeverity.Success, "已保存", fileName, "打开文件", openImageAction, null, 3000);
            }
        }
        catch (Exception ex)
        {
            NotificationProvider.Error(ex, "保存图片");
            Logger.Error(ex, "保存图片");
        }
    }


    /// <summary>
    /// 打开图源
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task OpenImageSourceAsync()
    {
        if (string.IsNullOrWhiteSpace(WallpaperInfo?.Source))
        {
            return;
        }
        try
        {
            await Launcher.LaunchUriAsync(new Uri(WallpaperInfo.Source));
        }
        catch (Exception ex)
        {
            NotificationProvider.Error(ex, "打开图源");
            Logger.Error(ex, "打开图源");
        }
    }


    #endregion




    #region Content


    /// <summary>
    /// 实时便笺
    /// </summary>
    /// <returns></returns>
    private async void GetDailyNotesAsync()
    {
        try
        {
            var users = _hoyolabService.GetHoyolabUserInfoList();
            var roles = _hoyolabService.GetGenshinRoleInfoList();
            foreach (var role in roles)
            {
                if (users.FirstOrDefault(x => x.Cookie == role.Cookie) is HoyolabUserInfo user)
                {
                    DailyNoteThumbCard card;
                    try
                    {
                        var dailynote = await _hoyolabClient.GetDailyNoteAsync(role);
                        var travelnote = await _hoyolabClient.GetTravelNotesSummaryAsync(role);
                        card = new DailyNoteThumbCard { HoyolabUserInfo = user, GenshinRoleInfo = role, DailyNoteInfo = dailynote, TravelNotesDayData = travelnote.DayData };
                    }
                    catch (HoyolabException ex)
                    {
                        card = new DailyNoteThumbCard { HoyolabUserInfo = user, GenshinRoleInfo = role, Error = true, ErrorMessage = ex.Message };
                    }
                    c_TextBlock_DailyNote.Visibility = Visibility.Visible;
                    c_GridView_DailyNotes.Visibility = Visibility.Visible;
                    c_GridView_DailyNotes.Items.Add(card);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "实时便笺");
        }
    }


    /// <summary>
    /// 养成计划
    /// </summary>
    /// <returns></returns>
    private async void GetCalendarAndGrowthScheduleAsync()
    {
        try
        {
            using var docDb = DatabaseProvider.CreateDocDb();
            var col = docDb.GetCollection<GrowthScheduleItem>();
            var growthItems = col.FindAll().OrderBy(x => x.Order).ToList();
            if (!growthItems.Any())
            {
                return;
            }
            var todayTitles = new List<string>();
            try
            {
                var calendars = await _hoyolabClient.GetCalendarInfosAsync();
                var day = (int)DateTimeOffset.UtcNow.AddHours(4).DayOfWeek;
                day = day == 0 ? 7 : day;
                var dayString = day.ToString();
                var todayCalendars = calendars.Where(x => x.Kind == "2" && x.DropDay.Contains(dayString)).ToList();
                todayTitles = todayCalendars.Select(x => x.Title).ToList();
                foreach (var growItem in growthItems)
                {
                    if (todayCalendars.FirstOrDefault(x => x.Title == growItem.Name) is CalendarInfo info)
                    {
                        var materials = info.ContentInfos.Select(x => x.Title).ToList();
                        if (growItem.LevelCostItems?.Any() ?? false)
                        {
                            foreach (var costItem in growItem.LevelCostItems)
                            {
                                if (materials.Contains(costItem.Name))
                                {
                                    costItem.IsToday = true;
                                }
                            }
                        }
                        if (growItem.TalentCostItems?.Any() ?? false)
                        {
                            foreach (var costItem in growItem.TalentCostItems)
                            {
                                if (materials.Contains(costItem.Name))
                                {
                                    costItem.IsToday = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Logger.Error(ex, "养成计划");
            }
            GrowthScheduleItems = growthItems.OrderBy(x => x.HasTodayMaterial()).ThenBy(x => x.Order).ToList();
            c_TextBlock_GrowthSchedule.Visibility = Visibility.Visible;
            c_GridView_GrowthSchedule.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "养成计划");
        }
    }


    /// <summary>
    /// 单个材料完成
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void c_Button_CostItem_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button)
            {
                if (button.Content is TextBlock textBlock)
                {
                    if (textBlock.DataContext is GrowthScheduleCostItem costItem)
                    {
                        costItem.IsFinish = !costItem.IsFinish;
                    }
                    if (VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(button))) is StackPanel stackPanel)
                    {
                        if (stackPanel.DataContext is GrowthScheduleItem item)
                        {
                            using var docDb = DatabaseProvider.CreateDocDb();
                            var col = docDb.GetCollection<GrowthScheduleItem>();
                            col.Upsert(item);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "单个材料完成");
        }
    }


    /// <summary>
    /// 即将结束的活动
    /// </summary>
    /// <returns></returns>
    private async void GetFinishingActivityAsync()
    {
        try
        {
            var announces = await _hoyolabClient.GetGameAnnouncementsAsync();
            var activities = announces.Where(x => x.Type == 1 && x.IsFinishing).OrderBy(x => x.EndTime).ToList();
            if (activities.Any())
            {
                c_TextBlock_FinishingActivity.Visibility = Visibility.Visible;
                c_GridView_FinishingActivity.Visibility = Visibility.Visible;
                FinishingActivities = activities;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "即将结束的活动");
        }
    }


    /// <summary>
    /// 显示活动内容
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void c_Grid_FinishingActivity_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        try
        {
            if (sender is Grid grid)
            {
                if (grid.DataContext is Announcement announce)
                {
                    c_AnnouncementContentViewer.Announce = announce;
                    FlyoutBase.ShowAttachedFlyout(c_Grid_Base);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "显示活动内容");
        }
    }


    #endregion




    #region 通知和检查更新

    /// <summary>
    /// 初始化通知栏内容
    /// </summary>
    /// <returns></returns>
    private async void GetNotificationContentAsync()
    {
        try
        {
            var list = await _xunkongApiService.GetInfoBarContentListAsync();
            if (list?.Any() ?? false)
            {
                _Grid_InfoBar.Visibility = Visibility.Visible;
                foreach (var item in list)
                {
                    InfoBar infoBar;
                    if (!string.IsNullOrWhiteSpace(item.ButtonContent) && !string.IsNullOrWhiteSpace(item.ButtonUri))
                    {
                        infoBar = NotificationProvider.Create((InfoBarSeverity)item.Severity, item.Title, item.Message, item.ButtonContent, async () => await Launcher.LaunchUriAsync(new Uri(item.ButtonUri)));
                    }
                    else
                    {
                        infoBar = NotificationProvider.Create((InfoBarSeverity)item.Severity, item.Title, item.Message);
                    }
                    _StackPanel_InfoBar.Children.Add(infoBar);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "主页通知栏内容");
        }
    }


    /// <summary>
    /// 检查更新
    /// </summary>
    /// <returns></returns>
    private async void CheckUpdateAsync()
    {
        try
        {
            if (XunkongEnvironment.IsStoreVersion)
            {
                // 商店版检查更新
                var context = StoreContext.GetDefault();
                // 调用需要在UI线程运行的函数前
                WinRT.Interop.InitializeWithWindow.Initialize(context, MainWindow.Current.HWND);
                var updates = await context.GetAppAndOptionalStorePackageUpdatesAsync();
                if (updates.Any())
                {
                    _Grid_InfoBar.Visibility = Visibility.Visible;
                    Action action;
                    if (context.CanSilentlyDownloadStorePackageUpdates)
                    {
                        action = () =>
                        {
                            var operation = context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(updates);
                            DownloadProgressHandle(operation);
                        };
                    }
                    else
                    {
                        action = () =>
                        {
                            var operation = context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);
                            DownloadProgressHandle(operation);
                        };
                    }
                    if (updates[0].Mandatory)
                    {
                        var stack1 = new StackPanel { Spacing = 8 };
                        stack1.Children.Add(new TextBlock { Text = "这是一个强制更新版本" });
                        var stack2 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
                        stack1.Children.Add(stack2);
                        var button1 = new Button { Content = "下载并安装" };
                        button1.Click += (_, _) =>
                        {
                            try
                            {
                                action();
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex, "下载并安装更新");
                            }
                        };
                        stack2.Children.Add(button1);
                        var button2 = new Button { Content = "打开商店" };
                        button2.Click += async (_, _) => await Launcher.LaunchUriAsync(new(XunkongEnvironment.StoreProtocolUrl));
                        stack2.Children.Add(button2);
                        var dialog = new ContentDialog
                        {
                            Title = "有新版本",
                            Content = stack1,
                            XamlRoot = MainWindow.Current.XamlRoot,
                            RequestedTheme = MainWindow.Current.ActualTheme,
                        };
                        await dialog.ShowWithZeroMarginAsync();
                    }
                    else
                    {
                        var infoBar = NotificationProvider.Create(InfoBarSeverity.Success, "有新版本", "商店版无法获取新版本的信息", "下载并安装", action);
                        _StackPanel_InfoBar.Children.Insert(0, infoBar);
                    }
                }
            }
            else
            {
                // 侧载版检查更新
                var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Xunkong"));
                var release = await client.Repository.Release.GetLatest("xunkong", "xunkong");
                if (Version.TryParse(release.TagName, out var version))
                {
                    if (version > XunkongEnvironment.AppVersion)
                    {
                        _Grid_InfoBar.Visibility = Visibility.Visible;
                        var infoBar = NotificationProvider.Create(InfoBarSeverity.Success, $"新版本 {version}", release.Name, "详细信息", async () => await Launcher.LaunchUriAsync(new Uri(release.HtmlUrl)));
                        _StackPanel_InfoBar.Children.Insert(0, infoBar);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "主页检查更新");
        }
    }


    /// <summary>
    /// 下载并安装新版本的进度条
    /// </summary>
    /// <param name="operation"></param>
    private static void DownloadProgressHandle(IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> operation)
    {
        const string tag = "download new version";
        const string group = "download";
        uint index = 0;
        var content = new ToastContentBuilder().AddText("别点我！").AddVisualChild(new AdaptiveProgressBar()
        {
            Title = "下载中",
            Value = new BindableProgressBarValue("progressValue"),
            ValueStringOverride = new BindableString("progressValueString"),
            Status = new BindableString("progressStatus")
        }).AddToastActivationInfo("DownloadNewVersion", ToastActivationType.Background).GetToastContent();

        var toast = new ToastNotification(content.GetXml());
        toast.Tag = tag;
        toast.Group = group;
        toast.Data = new NotificationData();
        toast.Data.Values["progressValue"] = "0";
        toast.Data.Values["progressValueString"] = "0%";
        toast.Data.Values["progressStatus"] = "0MB / 0MB";
        toast.Data.SequenceNumber = ++index;

        var manager = ToastNotificationManager.CreateToastNotifier();
        operation.Progress = (_, status) =>
        {
            if (status.PackageUpdateState == StorePackageUpdateState.Pending)
            {
                manager.Show(toast);
            }
            if (status.PackageUpdateState == StorePackageUpdateState.Downloading)
            {
                var progress = status.PackageDownloadProgress;
                var data = new NotificationData { SequenceNumber = ++index };
                data.Values["progressValue"] = $"{status.PackageDownloadProgress / 0.95}";
                data.Values["progressValueString"] = $"{status.PackageDownloadProgress / 0.95:P0}";
                data.Values["progressStatus"] = $"{status.PackageBytesDownloaded / (double)(1 << 20):F1}MB / {status.PackageDownloadSizeInBytes / (double)(1 << 20):F1}MB";
                manager.Update(data, tag, group);
            }
            if (status.PackageUpdateState == StorePackageUpdateState.Deploying)
            {
                manager.Hide(toast);
                Vanara.PInvoke.Kernel32.RegisterApplicationRestart(null, 0);
            }
        };
    }

    #endregion




    #region 快捷操作


    [RelayCommand]
    private async Task StartGameAsync()
    {
        if (await InvokeService.StartGameAsync(true))
        {
            await InvokeService.CheckTransformerReachedAndHomeCoinFullAsync(true);
        }
    }










    #endregion


}
