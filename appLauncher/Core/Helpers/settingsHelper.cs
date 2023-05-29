﻿using appLauncher.Core.Model;

using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;

namespace appLauncher.Core.Helpers
{
    public static class SettingsHelper
    {
        public static GlobalAppSettings totalAppSettings { get; set; } = new GlobalAppSettings();
        public static async Task<bool> IsFilePresent(string fileName, string folderpath = "")
        {
            IStorageItem item;
            if (folderpath == "")
            {
                item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderpath);
                item = await folder.TryGetItemAsync(fileName);
            }
            return item != null;
        }
        public static async Task LoadAppSettingsAsync()
        {
            if (await SettingsHelper.IsFilePresent("globalappsettings.json"))
            {
                try
                {
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("globalappsettings.json");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    totalAppSettings = JsonConvert.DeserializeObject<GlobalAppSettings>(apps);
                    if (!totalAppSettings.AppSettings)
                    {
                        totalAppSettings = new GlobalAppSettings();
                    }
                    totalAppSettings.AppColors = GetStaticPropertyBag(typeof(Colors));
                }
                catch (Exception e)
                {
                }
            }
            else
            {
                totalAppSettings = new GlobalAppSettings();
                totalAppSettings.AppColors = GetStaticPropertyBag(typeof(Colors));
            }
        }
        public static async Task SaveAppSettingsAsync()
        {
            try
            {
                var te = JsonConvert.SerializeObject(totalAppSettings, Formatting.Indented);
                if (!totalAppSettings.AppSettings)
                {
                    te = JsonConvert.SerializeObject(new GlobalAppSettings(), Formatting.Indented);
                }
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("globalappsettings.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(item, te);
            }
            catch (Exception es)
            {
            }
        }
        public static List<ColorComboItem> GetStaticPropertyBag(Type t)
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            List<ColorComboItem> map = new List<ColorComboItem>();
            foreach (var prop in t.GetProperties(flags))
            {
                ColorComboItem colorItem = new ColorComboItem();
                colorItem.ColorName = prop.Name;
                colorItem.ColorBrush = new Windows.UI.Xaml.Media.SolidColorBrush(prop.Name.ToColor());
                map.Add(colorItem);
            }
            return map;
        }
        public static void SetApplicationResources()
        {
            Application.Current.Resources["AppBarButtonForegroundPointerOver"] = SettingsHelper.totalAppSettings.AppForegroundColorBrush;
            Application.Current.Resources["AppBarButtonBackgroundPointerOver"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxBackground"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxBackgroundPointerOver"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxForeground"] = SettingsHelper.totalAppSettings.AppForegroundColorBrush;
            Application.Current.Resources["ComboBoxDropDownBackground"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxPlaceHolderForeground"] = SettingsHelper.totalAppSettings.AppForegroundColorBrush;
        }
    }
}
