﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunkong.Core.Hoyolab;
using Xunkong.Desktop.Services;

namespace Xunkong.Desktop.ViewModels
{
    internal partial class UserPanelViewModel : ObservableObject
    {


        const string LastSelectUserInfoUid = "LastSelectUserInfoUid";
        const string LastSelectGameRoleUid = "LastSelectGameRoleUid";


        private readonly UserSettingService _userSettingService;

        private readonly HoyolabService _hoyolabService;

        private readonly ILogger<UserPanelViewModel> _logger;


        public Action HideUserPanelSelectorFlyout { get; set; }


        public UserPanelViewModel() { }



        public UserPanelViewModel(UserSettingService userSettingService, HoyolabService hoyolabService, ILogger<UserPanelViewModel> logger)
        {
            _userSettingService = userSettingService;
            _hoyolabService = hoyolabService;
            _logger = logger;
        }



        private UserPanelModel? _SelectedUserPanelModel;
        public UserPanelModel? SelectedUserPanelModel
        {
            get => _SelectedUserPanelModel;
            set
            {
                SetProperty(ref _SelectedUserPanelModel, value);
                SaveLastSelectSetting(value);
            }
        }


        private async void SaveLastSelectSetting(UserPanelModel? model)
        {
            try
            {
                await _userSettingService.SaveSettingAsync(LastSelectUserInfoUid, model?.UserInfo?.Uid ?? 0);
                await _userSettingService.SaveSettingAsync(LastSelectGameRoleUid, model?.GameRoleInfo?.Uid ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in method {MethodName}.", nameof(SaveLastSelectSetting));
            }
        }



        private ObservableCollection<UserPanelModel> _UserPanelModelList;
        public ObservableCollection<UserPanelModel> UserPanelModelList
        {
            get => _UserPanelModelList;
            set => SetProperty(ref _UserPanelModelList, value);
        }




        public async Task InitlizeUserPanel()
        {
            _logger.LogDebug("Initlize User Panel.");
            try
            {
                var userInfo = await _hoyolabService.GetLastSelectedOrFirstUserInfoAsync();
                if (userInfo == null)
                {
                    _logger.LogDebug("No saved hoyolab userinfo.");
                    return;
                }
                var gameRole = await _hoyolabService.GetLastSelectedOrFirstUserGameRoleInfoAsync();
                var model = new UserPanelModel { UserInfo = userInfo, GameRoleInfo = gameRole };
                SelectedUserPanelModel = model;
                await RefreshAllUserPanelModelAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in method {MethodName}.", nameof(InitlizeUserPanel));
                InfoBarHelper.Error(ex.GetType().Name, ex.Message);
            }
        }


        [ICommand]
        public async Task RefreshAllUserPanelModelAsync(bool showResult = false)
        {
            _logger.LogDebug("Start refresh all hoyolab and genshin user info.");
            HideUserPanelSelectorFlyout?.Invoke();
            try
            {
                var oldUsers = await _hoyolabService.GetUserInfoListAsync();
                // 更新米游社账号数据
                var tmpUsers = new BlockingCollection<UserInfo>();
                await Parallel.ForEachAsync(oldUsers, async (x, _) =>
                {
                    try
                    {
                        // 同时会更新至数据库
                        var user = await _hoyolabService.GetUserInfoAsync(x);
                        if (user is not null)
                        {
                            // cookie 有效的账号才进行下一步
                            tmpUsers.Add(user);
                        }
                    }
                    catch (HoyolabException ex)
                    {
                        _logger.LogError(ex, "Catch HoyolabException when get hoyolab user info (Nickname {Nickname}, Uid {Uid}).", x.Nickname, x.Uid);
                        InfoBarHelper.Error(ex.GetType().Name, $"获取米游社账号信息 (Nickname {x.Nickname}, Uid {x.Uid})\n{ex.Message}");
                    }
                });
                // 更新原神账号数据
                var tmpRoles = new BlockingCollection<UserGameRoleInfo>();
                await Parallel.ForEachAsync(tmpUsers, async (x, _) =>
                {
                    try
                    {
                        // 同时会更新至数据库
                        var roles = await _hoyolabService.GetUserGameRoleInfoListAsync(x.Cookie!);
                        if (roles.Any())
                        {
                            // cookie 有效的账号才进行下一步
                            foreach (var item in roles)
                            {
                                tmpRoles.Add(item);
                            }
                        }
                    }
                    catch (HoyolabException ex)
                    {
                        _logger.LogError(ex, "Catch HoyolabException when get genshin user info (Nickname {Nickname}, Uid {Uid}).", x.Nickname, x.Uid);
                        InfoBarHelper.Error(ex.GetType().Name, $"获取原神账号信息 (Nickname {x.Nickname}, Uid {x.Uid})\n{ex.Message}");
                    }
                });
                // 更新实时便笺数据
                var dailyNotes = new BlockingCollection<DailyNoteInfo>();
                await Parallel.ForEachAsync(tmpRoles, async (x, _) =>
                {
                    try
                    {
                        // 同时会更新至数据库
                        var note = await _hoyolabService.GetDailyNoteInfoAsync(x);
                        if (note is not null)
                        {
                            dailyNotes.Add(note);
                        }
                    }
                    catch (HoyolabException ex)
                    {
                        _logger.LogError(ex, "Catch HoyolabException when get daily note info (Nickname {Nickname}, Uid {Uid}).", x.Nickname, x.Uid);
                        InfoBarHelper.Error(ex.GetType().Name, $"获取实时便笺 (Nickname {x.Nickname}, Uid {x.Uid})\n{ex.Message}");
                    }
                });
                // 重新从数据库获取所有数据，包含cookie过期的账号
                var userInfos = await _hoyolabService.GetUserInfoListAsync();
                var gameRoles = await _hoyolabService.GetUserGameRoleInfoListAsync();
                // UserInfo UserGameRoleInfo DailyNoteInfo 相对应，应该没问题
                var query = from user in userInfos
                            join role in gameRoles
                            on user.Cookie equals role.Cookie into userGroup
                            from r in userGroup.DefaultIfEmpty()
                            join note in dailyNotes
                            on r.Uid equals note.Uid into roleGroup
                            from n in roleGroup.DefaultIfEmpty()
                            select new UserPanelModel { UserInfo = user, GameRoleInfo = r, DailyNoteInfo = n };
                var list = new ObservableCollection<UserPanelModel>(query);
                var pinnedList = await TileService.FindAllAsync();
                // 更新磁贴
                foreach (var item in list)
                {
                    if (pinnedList.Any(x => x == $"DailyNote_{item.GameRoleInfo?.Uid}"))
                    {
                        item.IsPinned = true;
                        if (item.DailyNoteInfo is not null)
                        {
                            TileService.UpdatePinnedTile(item.DailyNoteInfo);
                        }
                    }
                }
                UserPanelModelList = list;
                var lastRoleUid = await _userSettingService.GetSettingAsync<int>(LastSelectGameRoleUid);
                var lastModel = list.FirstOrDefault(x => x?.GameRoleInfo?.Uid == lastRoleUid) ?? list.FirstOrDefault();
                SelectedUserPanelModel = lastModel;
                if (showResult)
                {
                    InfoBarHelper.Success("刷新完成", TimeSpan.FromSeconds(3));
                }
                _logger.LogDebug("Finish refresh all hoyolab and genshin user info.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Catch Exception when refresh all hoyolab and genshin user info.");
                InfoBarHelper.Error(ex.GetType().Name, $"刷新所有账号信息\n{ex.Message}");
            }
        }



        [ICommand]
        public async Task HoyolabLogin_InputCookieAsync()
        {
            _logger.LogDebug("Login Input Cookie button has been pressed.");
            var dialog = new ContentDialog
            {
                Title = "Input cookie",
                Content = new TextBox(),
                PrimaryButtonText = "Confirm",
                SecondaryButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = MainWindow.XamlRoot,
            };
            var result = await dialog.ShowAsync();
            HideUserPanelSelectorFlyout?.Invoke();
            if (result is ContentDialogResult.Primary)
            {
                var cookie = (dialog.Content as TextBox)?.Text;
                if (string.IsNullOrWhiteSpace(cookie))
                {
                    _logger.LogDebug("Input box has nothing.");
                    return;
                }
                try
                {
                    _logger.LogDebug("Start get info by cookie.");
                    var userInfo = await _hoyolabService.GetUserInfoAsync(cookie);
                    var gameRoles = await _hoyolabService.GetUserGameRoleInfoListAsync(cookie);
                    var list = gameRoles.Select(x => new UserPanelModel { UserInfo = userInfo, GameRoleInfo = x }).ToList();
                    var pinnedList = await TileService.FindAllAsync();
                    foreach (var item in list)
                    {
                        if (pinnedList.Any(x => x == $"DailyNote_{item.GameRoleInfo?.Uid}"))
                        {
                            item.IsPinned = true;
                        }
                    }
                    if (UserPanelModelList is null)
                    {
                        UserPanelModelList = new ObservableCollection<UserPanelModel>(list);
                    }
                    else
                    {
                        foreach (var item in list)
                        {
                            var match = UserPanelModelList.FirstOrDefault(x => x.GameRoleInfo?.Uid == item.GameRoleInfo?.Uid);
                            if (match is not null)
                            {
                                UserPanelModelList.Remove(match);
                            }
                            UserPanelModelList.Add(item);
                        }
                    }
                    var firstModel = list.FirstOrDefault();
                    if (firstModel is null)
                    {
                        _logger.LogDebug("No relative game roles.");
                        InfoBarHelper.Warning("没有相关联的角色");
                        return;
                    }
                    SelectedUserPanelModel = firstModel;
                    firstModel.DailyNoteInfo = await _hoyolabService.GetDailyNoteInfoAsync(firstModel.GameRoleInfo!);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in {MethodName}", nameof(HoyolabLogin_InputCookieAsync));
                    InfoBarHelper.Error(ex.GetType().Name, ex.Message);
                }
            }
            else
            {
                _logger.LogDebug("Canceled Input Cookie command.");
            }
        }


        [ICommand]
        public async Task RefreshDailyNoteAsync()
        {
            var model = SelectedUserPanelModel;
            if (model is null || model.GameRoleInfo is null)
            {
                return;
            }
            try
            {
                _logger.LogDebug("Refresh daily note with genshin nickname {Nickname} uid {Uid}", model.GameRoleInfo.Nickname, model.GameRoleInfo.Uid);
                var info = await _hoyolabService.GetDailyNoteInfoAsync(model.GameRoleInfo);
                model.DailyNoteInfo = info;
                if (info is not null && model.IsPinned)
                {
                    TileService.UpdatePinnedTile(info);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {MethodName}", nameof(RefreshDailyNoteAsync));
                InfoBarHelper.Error(ex.GetType().Name, ex.Message);
            }
        }



        public object? InitGroupSource()
        {
            if (UserPanelModelList is null)
            {
                return null;
            }
            return from model in UserPanelModelList group model by model.UserInfo;
        }



        public async Task DeleteUserInfoAsync(IEnumerable<UserPanelModel> models)
        {
            HideUserPanelSelectorFlyout?.Invoke();
            if (models.Any())
            {
                var hoyolabUser = models.FirstOrDefault()?.UserInfo;
                _logger.LogDebug("Delete hoyolab user info with nickname {Nickname} uid {Uid}", hoyolabUser?.Nickname, hoyolabUser?.Uid);
            }
            try
            {
                foreach (var model in models)
                {
                    if (model.UserInfo is not null)
                    {
                        await _hoyolabService.DeleteUserInfoAsync(model.UserInfo.Uid);
                    }
                    if (model.GameRoleInfo is not null)
                    {
                        _logger.LogDebug("Delete genshin user info with nickname {Nickname} uid {Uid}", model.GameRoleInfo.Nickname, model.GameRoleInfo.Uid);
                        await _hoyolabService.DeleteUserGameRoleInfoAsync(model.GameRoleInfo.Uid);
                        if (model.IsPinned)
                        {
                            await TileService.RequestUnpinTileAsync(model.GameRoleInfo.Uid);
                        }
                    }
                    if (UserPanelModelList?.Contains(model) ?? false)
                    {
                        UserPanelModelList.Remove(model);
                    }
                }
                _logger.LogDebug("Finished delete user info.");
                InfoBarHelper.Success("删除成功", TimeSpan.FromSeconds(3));
                if (SelectedUserPanelModel is null)
                {
                    SelectedUserPanelModel = UserPanelModelList?.FirstOrDefault();
                }
                else
                {
                    if (UserPanelModelList?.Contains(SelectedUserPanelModel) ?? false)
                    {
                        return;
                    }
                    else
                    {
                        SelectedUserPanelModel = UserPanelModelList?.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {MethodName}", nameof(DeleteUserInfoAsync));
                InfoBarHelper.Error(ex.GetType().Name, ex.Message);
            }
        }



    }
}
