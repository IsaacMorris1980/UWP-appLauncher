﻿using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace appLauncher.Core.Model
{
    [Serializable]
    public class AppPaginationObservableCollection : ObservableCollection<AppTiles>
    {
        private ObservableCollection<AppTiles> originalCollection;
        [NonSerialized]
        private int _page;
        [NonSerialized]
        private int _countPerPage;
        private int _startIndex;
        private int _endIndex;

        public AppPaginationObservableCollection(IEnumerable<AppTiles> collection) : base(collection)
        {

            _page = SettingsHelper.totalAppSettings.LastPageNumber;
            _countPerPage = SettingsHelper.totalAppSettings.AppsPerPage;
            _startIndex = _page * _countPerPage;
            _endIndex = _startIndex + _countPerPage;
            originalCollection = new ObservableCollection<AppTiles>(collection);
            RecalculateThePageItems();
            GlobalVariables.PageNumChanged += PageChanged;
            GlobalVariables.NumofApps += SizedChanged;
        }

        private void RecalculateThePageItems()
        {
            ClearItems();


            for (int i = _startIndex; i < _endIndex; i++)
            {
                if (originalCollection.Count > i)
                    base.InsertItem(i - _startIndex, originalCollection[i]);
            }
        }

        public int GetIndexApp(AppTiles app)
        {
            return originalCollection.IndexOf(app);
        }
        public void MoveApp(int initailindex, int newindex)
        {
            originalCollection.Move(initailindex, newindex);
            RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));

        }
        public void GetFilteredApps(string selected)
        {

            List<AppTiles> orderList;
            switch (selected)
            {
                case "AppAZ":
                    orderList = originalCollection.OrderBy(y => y.Name).ToList();
                    originalCollection = new ObservableCollection<AppTiles>(orderList);
                    break;
                case "AppZA":
                    orderList = originalCollection.OrderByDescending(y => y.Name).ToList();
                    originalCollection = new ObservableCollection<AppTiles>(orderList);
                    break;
                case "DevAZ":
                    orderList = originalCollection.OrderBy(x => x.Developer).ToList();
                    originalCollection = new ObservableCollection<AppTiles>(orderList);
                    break;
                case "DevZA":
                    orderList = originalCollection.OrderByDescending(x => x.Developer).ToList();
                    originalCollection = new ObservableCollection<AppTiles>(orderList);
                    break;
                case "InstalledNewest":
                    orderList = originalCollection.OrderByDescending(x => x.InstalledDate).ToList();
                    originalCollection = new ObservableCollection<AppTiles>(orderList);
                    break;
                case "InstalledOldest":
                    orderList = originalCollection.OrderBy(x => x.InstalledDate).ToList();
                    originalCollection = new ObservableCollection<AppTiles>(orderList);
                    break;
                default:
                    return;
            }
            RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        protected override void InsertItem(int index, AppTiles item)
        {
            //Check if the Index is with in the current Page then add to the collection as bellow. And add to the originalCollection also
            if ((index >= _startIndex) && (index < _endIndex))
            {
                base.InsertItem(index - _startIndex, item);
                if (Count > _countPerPage)
                    base.RemoveItem(_endIndex);
            }
            if (index >= Count)
                originalCollection.Add(item);
            else
                originalCollection.Insert(index, item);
        }
        protected override void RemoveItem(int index)
        {
            int startIndex = _page * _countPerPage;
            int endIndex = startIndex + _countPerPage;
            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startIndex) && (index < endIndex))
            {
                this.RemoveAt(index - startIndex);
                if (Count <= _countPerPage)
                    base.InsertItem(endIndex - 1, originalCollection[index + 1]);
            }
            originalCollection.RemoveAt(index);
        }
        public ObservableCollection<AppTiles> GetOriginalCollection()
        {
            return originalCollection;
        }
        public void PageChanged(PageChangedEventArgs e)
        {
            _page = e.PageIndex;
            _startIndex = _page * _countPerPage;
            _endIndex = _startIndex + _countPerPage;
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        public void SizedChanged(AppPageSizeChangedEventArgs e)
        {
            _countPerPage = e.AppPageSize;
            _startIndex = _page * _countPerPage;
            _endIndex = _startIndex + _countPerPage;
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
    }
    public static class ExtensionMethods
    {
        public static int Remove<T>(
            this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }
            return itemsToRemove.Count;
        }
    }
}
