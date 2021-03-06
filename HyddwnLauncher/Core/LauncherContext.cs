﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HyddwnLauncher.Util;

namespace HyddwnLauncher.Core
{
    public class LauncherContext
    {
        public static readonly LauncherContext Instance = new LauncherContext();

        private readonly string[] _images =
        {
            "bangor.png",
            "damage.png",
            "duncan.png",
            "leehuga.png",
            "lucifer.png",
            "uniqueflyingpuppet.png",
            "vans.png"
        };

        public void Initialize(string logFileLocation, string version, string betaVersion)
        {
            LogFileLocation = logFileLocation;
            Version = version;
            BetaVersion = betaVersion;
            Log.Info(Properties.Resources.LauncherContextLoadingSettings);
            LauncherSettingsManager = new LauncherSettingsManager();
            Log.Info(Properties.Resources.LauncherContextLoadingComplete);
        }

        public string LogFileLocation { get; protected set; }
        public string LogFileLocationTruncated => Unmanaged.TruncatePath(LogFileLocation, 100);
        public string Version { get; protected set; }
        public string BetaVersion { get; protected set; }

        public LauncherSettingsManager LauncherSettingsManager { get; protected set; }

        public ImageSource HostImage
        {
            get
            {
                var rnd = new Random();
                return GetImage(_images[rnd.Next(_images.Length)]);
            }
        }

        private static ImageSource GetImage(string imageName)
        {
            var sri = Application.GetResourceStream(new Uri("pack://application:,,,/Images/" + imageName));
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = sri.Stream;
            bmp.EndInit();

            return bmp;
        }
    }
}