﻿using System.Linq;
using System.Windows;
using System.Windows.Input;
using ReactiveUI;
using SimpleMusicPlayer.Core;
using SimpleMusicPlayer.Core.Interfaces;
using SimpleMusicPlayer.Core.Player;
using TinyIoC;

namespace SimpleMusicPlayer.ViewModels
{
    public class MainViewModel : ReactiveObject, IKeyHandler
    {
        private ICommand showOnGitHubCmd;

        public MainViewModel()
        {
            var container = TinyIoCContainer.Current;

            this.PlayerSettings = container.Resolve<PlayerSettings>().Update();
            this.CustomWindowPlacementSettings = new CustomWindowPlacementSettings(this.PlayerSettings.MainWindow);

            this.PlayerEngine = container.Resolve<PlayerEngine>().Configure();

            this.PlayListsViewModel = new PlayListsViewModel();

            this.PlayControlInfoViewModel = new PlayControlInfoViewModel(this);
        }

        public CustomWindowPlacementSettings CustomWindowPlacementSettings { get; private set; }

        public PlayerEngine PlayerEngine { get; private set; }

        public PlayerSettings PlayerSettings { get; private set; }

        public PlayControlInfoViewModel PlayControlInfoViewModel { get; private set; }

        public PlayListsViewModel PlayListsViewModel { get; private set; }

        public ICommand ShowOnGitHubCmd
        {
            get { return this.showOnGitHubCmd ?? (this.showOnGitHubCmd = new DelegateCommand(this.ShowOnGitHub, () => true)); }
        }

        private void ShowOnGitHub()
        {
            System.Diagnostics.Process.Start("https://github.com/punker76/simple-music-player");
        }

        public void ShutDown()
        {
            foreach (var w in Application.Current.Windows.OfType<Window>())
            {
                w.Close();
            }
            this.PlayerSettings.Save();
            this.PlayerEngine.CleanUp();
            this.PlayListsViewModel.SavePlayList();
        }

        public bool HandleKeyDown(Key key)
        {
            if (this.PlayControlInfoViewModel.PlayControlViewModel.HandleKeyDown(key))
            {
                return true;
            }
            return false;
        }
    }
}