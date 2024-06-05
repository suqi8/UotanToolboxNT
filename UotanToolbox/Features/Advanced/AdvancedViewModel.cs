﻿using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using ReactiveUI;
using SukiUI.Controls;
using SukiUI.MessageBox;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UotanToolbox.Common;
using UotanToolbox.Features.Components;

namespace UotanToolbox.Features.Advanced;

public partial class AdvancedViewModel : MainPageBase
{
    [ObservableProperty] private string _qcnFile, _superEmptyFile, _formatName, _extractName, _advancedLog;

    public AdvancedViewModel() : base("高级", MaterialIconKind.WrenchCogOutline, -300)
    {
    }

    [RelayCommand]
    private async Task WriteQcn()
    {
        // Write QCN File
        if (QcnFile != null)
        {
            string qcnfilepatch = QcnFile;
            string usbdevices = await CallExternalProgram.Devcon("find usb*");
            if (usbdevices.Contains("901D (") || usbdevices.Contains("9091 ("))
            {
                int com = StringHelper.Onlynum(Global.thisdevice);
                string shell = string.Format("-w -p {0} -f \"{1}\"", com, qcnfilepatch);
                string output = await CallExternalProgram.QCNTool(shell);
                AdvancedLog = output;
                if (output.Contains("error"))
                {
                    SukiHost.ShowDialog(new ConnectionDialog("写入失败"), allowBackgroundClose: true);
                }
                else
                {
                    SukiHost.ShowDialog(new ConnectionDialog("写入成功"), allowBackgroundClose: true);
                }
             }
            else
            {
                SukiHost.ShowDialog(new ConnectionDialog("无可用设备"), allowBackgroundClose: true);
            }
        }
        else
        {
            SukiHost.ShowDialog(new ConnectionDialog("请先选择QCN文件"), allowBackgroundClose: true);
        }
    }
    [RelayCommand]
    private async Task BackupQcn()
    {
        // Backup QCN file
        _ = QcnFile;
        string usbdevices = await CallExternalProgram.Devcon("find usb*");
        if (usbdevices.Contains("901D (") || usbdevices.Contains("9091 ("))
        {
            int com = StringHelper.Onlynum(Global.thisdevice);
            string shell = string.Format("-r -p {0} -f {1}\\backup", com, System.IO.Directory.GetCurrentDirectory());
            string output = await CallExternalProgram.QCNTool(shell);
            AdvancedLog = output;
            if (output.Contains("error"))
            {
                SukiHost.ShowDialog(new ConnectionDialog("备份失败"), allowBackgroundClose: true);
            }
            else
            {
                SukiHost.ShowDialog(new ConnectionDialog("备份成功"), allowBackgroundClose: true);
            }
        }
        else
        {
            SukiHost.ShowDialog(new ConnectionDialog("无可用设备"), allowBackgroundClose: true);
        }
    }
    [RelayCommand]
    private static Task OpenBackup()
    {
        // Open QCN backup file directory
        string filepath = string.Format(@"{0}\backup", System.IO.Directory.GetCurrentDirectory());
        Process.Start("Explorer.exe", filepath);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private static Task Enable901d()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private static Task Enable9091()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task FlashSuperEmpty()
    {
        if (await GetDevicesInfo.SetDevicesInfoLittle())
        {
            MainViewModel sukiViewModel = GlobalData.MainViewModelInstance;
            if (sukiViewModel.Status == "Fastboot")
            {
                if (SuperEmptyFile != null)
                {
                    string output = await CallExternalProgram.Fastboot($"-s {Global.thisdevice} wipe-super \"{SuperEmptyFile}\"");
                    AdvancedLog = output;
                    if (!output.Contains("FAILED") && !output.Contains("error"))
                    {
                        
                    }
                    else
                    {
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            SukiHost.ShowDialog(new ConnectionDialog("刷入失败！"), allowBackgroundClose: true);
                        });
                    }
                }
                else
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        SukiHost.ShowDialog(new ConnectionDialog("请选择SuperEmpty文件！"), allowBackgroundClose: true);
                    });
                }
            }
            else
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    SukiHost.ShowDialog(new ConnectionDialog("请进入Fastboot模式！"), allowBackgroundClose: true);
                });
            }
        }
    }
}