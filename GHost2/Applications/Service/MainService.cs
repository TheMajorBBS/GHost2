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
using System.ServiceProcess;

namespace MajorBBS.GHost
{
    public partial class MainService : ServiceBase
    {
        private GHost _GHost = new GHost();

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            _GHost.Start();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _GHost.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _GHost.Pause();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _GHost.Stop();
            _GHost.Dispose();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _GHost.Stop();
            _GHost.Dispose();
        }
    }
}
