/*
  GHost/2: Door Server
  Copyleft 2021 Major BBS (GPL3)
    original: Rick Parrish, R&M Software

  This file is part of GHost/2.

  GHost/2 is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  any later version.

  GHost/2 is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with GHost/2.  If not, see <http://www.gnu.org/licenses/>.
*/
using MajorBBS.GHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace DoorEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DoorInfo info;
        private Dictionary<string, string> platformMap = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void InitPlatforms()
        {
            string[] platformList = Directory.GetFiles("platforms");
            //cbPlatforms.Items = platformList;
            foreach (string platformFile in platformList)
            {
                if (platformFile.EndsWith(".ini"))
                {
                    string platShort = platformFile.Substring(10, platformFile.Length - 14);
                    System.Console.WriteLine("platformFile: {0}", platShort);
                    PlatformInfo platformInfo = new PlatformInfo(platShort);
                    cbPlatforms.Items.Add(platformInfo.Name);
                    platformMap.Add(platformInfo.Name, platShort);
                }
            }
        }

        private void Init()
        {
            string[] arguments = Environment.GetCommandLineArgs();

            InitPlatforms();

            if (arguments.Length > 1)
            {
                info = new DoorInfo(arguments[1]);
                textAuthorization.Text = info.Authorization;
                textCommand.Text = info.Command;
                textName.Text = info.Name;
                textParameters.Text = info.Parameters;
                cbPlatforms.Text = GetPlatformTitle(info.Platform);
                chkClearAfter.IsChecked = info.ClearScreenAfter;
                chkClearBefore.IsChecked = info.ClearScreenBefore;
                textDoorNode.Text = info.MaxDoorNodes > 0 ? info.MaxDoorNodes.ToString() : "5";
            }
            //info = new DoorInfo();
        }

        private string GetPlatformTitle(string name)
        {
            foreach (KeyValuePair<string, string> entry in platformMap)
            {
                if (entry.Value == name)
                {
                    return entry.Key;
                }
            }

            return "";
        }

        private string GetPlatformName(string title)
        {
            return platformMap[title];
        }

        private void Save()
        {
            int maxNodes = int.Parse(textDoorNode.Text) > 0 ? int.Parse(textDoorNode.Text) : 5;
            info.Authorization = textAuthorization.Text;
            info.Command = textCommand.Text;
            info.Name = textName.Text;
            info.Parameters = textParameters.Text;
            info.Platform = GetPlatformName(cbPlatforms.Text);
            info.ClearScreenAfter = (bool)chkClearAfter.IsChecked;
            info.ClearScreenBefore = (bool)chkClearBefore.IsChecked;
            info.MaxDoorNodes = maxNodes;
            info.Save();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
