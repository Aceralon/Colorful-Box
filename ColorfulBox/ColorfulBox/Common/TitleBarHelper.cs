﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Autofac;

namespace ColorfulBox
{
    public class TitleBarHelper : INotifyPropertyChanged
    {
        private static TitleBarHelper _instance = new TitleBarHelper();
        private static CoreApplicationViewTitleBar _coreTitleBar;
        private Thickness _titlePosition;
        private Visibility _titleVisibility;

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleBarHelper"/> class.
        /// </summary>
        public TitleBarHelper()
        {
            _coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            _coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            _titlePosition = CalculateTilebarOffset(_coreTitleBar.SystemOverlayLeftInset, _coreTitleBar.Height);
            _titleVisibility = Visibility.Visible;
            using (var scope = App.Container.BeginLifetimeScope())
            {
                //   var navigationRoot = App.Container.Resolve<INavigationRoot>();
                var navigationRoot = scope.Resolve<INavigationRoot>();
                navigationRoot.IsPaneOpenChanged += OnNavigationRootIsPaneOpenChanged;
            }
        }

      

        public event PropertyChangedEventHandler PropertyChanged;

        public static TitleBarHelper Instance
        {
            get
            {
                return _instance;
            }
        }

        public CoreApplicationViewTitleBar TitleBar
        {
            get
            {
                return _coreTitleBar;
            }
        }

        public Thickness TitlePosition
        {
            get
            {
                return _titlePosition;
            }

            set
            {
                if (value.Left != _titlePosition.Left || value.Top != _titlePosition.Top)
                {
                    _titlePosition = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitlePosition)));
                }
            }
        }

        public Visibility TitleVisibility
        {
            get
            {
                return _titleVisibility;
            }
            set
            {
                _titleVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitleVisibility)));
            }
        }

        public void ExitFullscreen()
        {
            TitleVisibility = Visibility.Visible;
        }

        public void GoFullscreen()
        {
            TitleVisibility = Visibility.Collapsed;
        }

        private void OnNavigationRootIsPaneOpenChanged(object sender, EventArgs e)
        {
            TitlePosition = CalculateTilebarOffset(_coreTitleBar.SystemOverlayLeftInset, _coreTitleBar.Height);
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            TitlePosition = CalculateTilebarOffset(_coreTitleBar.SystemOverlayLeftInset, _coreTitleBar.Height);
        }

        private Thickness CalculateTilebarOffset(double leftPosition, double height)
        {
            using (var scope = App.Container.BeginLifetimeScope())
            {
                //   var navigationRoot = App.Container.Resolve<INavigationRoot>();
                var navigationRoot = scope.Resolve<INavigationRoot>();



                // top position should be 6 pixels for a 32 pixel high titlebar hence scale by actual height
                var correctHeight = height / 32 * 6;
                var left = leftPosition + 12;
                if (navigationRoot.IsPaneOpen == false && leftPosition == 0)
                    left += navigationRoot.CompactPaneLength;

                return new Thickness(left, correctHeight, 0, 0);
            }
        }
    }
}
