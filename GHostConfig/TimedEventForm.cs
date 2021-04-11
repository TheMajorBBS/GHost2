using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MajorBBS.GHost
{
    public partial class TimedEventForm : Form
    {
        private Dictionary<string, string> mapPlatforms = new Dictionary<string, string>();

        public TimedEventForm(TimedEvent evt)
        {
            InitializeComponent();
            txtName.Text = evt.Name;
            txtCommand.Text = evt.Command;
            chkGoOffline.Checked = evt.GoOffline.ToString().Equals("True");
            chkFri.Checked = evt.Days.Contains("Friday");
            chkMon.Checked = evt.Days.Contains("Monday");
            chkSat.Checked = evt.Days.Contains("Saturday");
            chkSun.Checked = evt.Days.Contains("Sunday");
            chkThr.Checked = evt.Days.Contains("Thursday");
            chkTue.Checked = evt.Days.Contains("Tuesday");
            chkWed.Checked = evt.Days.Contains("Wednesday");
            numHour.Value = evt.Time != "" ? Int16.Parse(evt.Time.Split(':')[0]) : 0;
            numMinute.Value = evt.Time != "" ? Int16.Parse(evt.Time.Split(':')[1]) : 0;
            InitPlatforms();
            cmbPlatform.Text = evt.Platform;
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
                    PlatformInfo platformInfo = new PlatformInfo(platShort);
                    cmbPlatform.Items.Add(platformInfo.Name);
                    mapPlatforms.Add(platformInfo.Name, platShort);
                }
            }
        }

        public TimedEvent StoreForm()
        {
            TimedEvent evt = new TimedEvent();
            evt.Name = txtName.Text;
            evt.Command = txtCommand.Text;
            evt.GoOffline = chkGoOffline.Checked;

            List<string> dowList = new List<string>();
            if (chkSun.Checked) dowList.Add("Sunday");
            if (chkMon.Checked) dowList.Add("Monday");
            if (chkTue.Checked) dowList.Add("Tuesday");
            if (chkWed.Checked) dowList.Add("Wednesday");
            if (chkThr.Checked) dowList.Add("Thursday");
            if (chkFri.Checked) dowList.Add("Friday");
            if (chkSat.Checked) dowList.Add("Saturday");
            evt.Days = String.Join(",", dowList);

            int mins = (int)numMinute.Value;
            evt.Time = String.Format("{0}:{1,0:D2}", numHour.Value.ToString(), mins);
            evt.Platform = mapPlatforms[cmbPlatform.Text];

            return evt;
        }
    }
}
