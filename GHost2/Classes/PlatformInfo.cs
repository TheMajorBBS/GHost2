using RandM.RMLib;

namespace MajorBBS.GHost
{
    public class PlatformInfo : ConfigHelper
    {
        public string Name { get; set; }
        public string Shell { get; set; } // dosbox.exe; command.com; cmd.exe; bash; powershell
        public string Arguments { get; set; } // arguments to load bootstrap
        public string BootstrapName { get; set; } // name of the bootstrap
        public string Bootstrap { get; set; } // bootstrap batch
        public bool IsLoaded { get; set; } // whether or not the platform ini loaded or not.

        public PlatformInfo(string platform)
            : base(ConfigSaveLocation.Relative, StringUtils.PathCombine("platforms", platform.ToLower() + ".ini"))
        {
            Name = "";
            Shell = "";
            Arguments = "";
            BootstrapName = "";
            Bootstrap = "";
            IsLoaded = false;

            if (Load("PLATFORM"))
            {
                // platform ini found
                IsLoaded = true;
            }
            else
            {
                // platform not specified
            }
        }

        public new void Save()
        {
            base.Save("PLATFORM");
        }
    }
}
