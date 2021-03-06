﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HyddwnLauncher.Util;
using Newtonsoft.Json;

namespace HyddwnLauncher.Core
{
    public class ProfileManager
    {
        private readonly string _clientProfileJson =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Hyddwn Launcher\\clientprofiles.json";

        private readonly string _serverProfileJson =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Hyddwn Launcher\\serverprofiles.json";

        public ProfileManager()
        {
            if (!Directory.Exists(Path.GetDirectoryName(_clientProfileJson)))
                Directory.CreateDirectory(Path.GetDirectoryName(_clientProfileJson));

            if (!Directory.Exists(Path.GetDirectoryName(_serverProfileJson)))
                Directory.CreateDirectory(Path.GetDirectoryName(_serverProfileJson));

            Load();
        }

        public ObservableCollection<ClientProfile> ClientProfiles { get; private set; }
        public ObservableCollection<ServerProfile> ServerProfiles { get; private set; }

        public async Task UpdateProfiles()
        {
            foreach (var serverProfile in ServerProfiles)
                await serverProfile.GetUpdates();
            SaveServerProfiles();
        }

        public void Load()
        {
            ClientProfiles = LoadClientProfiles();
            ServerProfiles = LoadServerProfiles();

            // Attempt to correct existing profiles with no localization set
            foreach (var clientProfile in ClientProfiles.Where(x => string.IsNullOrWhiteSpace(x.Localization)))
                clientProfile.Localization = "North America";
        }

        public ObservableCollection<ClientProfile> LoadClientProfiles()
        {
            try
            {
                var fs = new FileStream(_clientProfileJson, FileMode.Open);
                var sr = new StreamReader(fs);

                var result = JsonConvert.DeserializeObject<ObservableCollection<ClientProfile>>(sr.ReadToEnd());

                sr.Dispose();
                fs.Dispose();

                return result ?? new ObservableCollection<ClientProfile>();
            }
            catch (Exception ex)
            {
                Log.Exception(ex, Properties.Resources.UnableToLoadClientProfileData);
                MessageBox.Show(
                    string.Format(Properties.Resources.UnableToLoadClientProfileDataMessage,
                        ex.Message), Properties.Resources.Error);
                return new ObservableCollection<ClientProfile>();
            }
        }

        public ObservableCollection<ServerProfile> LoadServerProfiles()
        {
            try
            {
                var fs = new FileStream(_serverProfileJson, FileMode.Open);
                var sr = new StreamReader(fs);

                var result = JsonConvert.DeserializeObject<ObservableCollection<ServerProfile>>(sr.ReadToEnd());

                sr.Dispose();
                fs.Dispose();

                return result ?? new ObservableCollection<ServerProfile>();
            }
            catch (Exception ex)
            {
                Log.Exception(ex, Properties.Resources.UnableToLoadServerProfileData);
                MessageBox.Show(
                    string.Format(Properties.Resources.UnableToLoadServerProfileDataMessage,
                        ex.Message), Properties.Resources.Error);
                return new ObservableCollection<ServerProfile>();
            }
        }

        public void SaveClientProfiles()
        {
            try
            {
                var jsonFile = JsonConvert.SerializeObject(ClientProfiles, Formatting.Indented);
                jsonFile.WriteAllTextWithBackup(_clientProfileJson);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, Properties.Resources.UnableToSaveClientProfileData);
                MessageBox.Show(
                    string.Format(
                        Properties.Resources.UnableToSaveClientProfileDataMessage,
                        ex.Message), Properties.Resources.Error);
            }
        }

        public void SaveServerProfiles()
        {
            try
            {
                var jsonFile = JsonConvert.SerializeObject(ServerProfiles, Formatting.Indented);
                jsonFile.WriteAllTextWithBackup(_serverProfileJson);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, Properties.Resources.UnableToSaveServerProfileData);
                MessageBox.Show(
                    string.Format(
                        Properties.Resources.UnableToSaveServerProfileDataMessage,
                        ex.Message), Properties.Resources.Error);
            }
        }

        public void ResetClientProfiles()
        {
            ClientProfiles.Clear();
            SaveClientProfiles();
        }

        public void ResetServerProfiles()
        {
            ServerProfiles.Clear();
            ServerProfiles.Add(ServerProfile.OfficialProfile);
            SaveServerProfiles();
        }
    }
}