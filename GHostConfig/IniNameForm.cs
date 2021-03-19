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
using System;
using System.Windows.Forms;

namespace MajorBBS.GHost
{
    public partial class IniNameForm : Form
    {
        public string iniFileName = "";

        public IniNameForm(string title = "Creating new INI.", string caption = "Enter the name for a new INI file.")
        {
            InitializeComponent();
            this.Text = title;
            lblCaption.Text = caption;
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            iniFileName = txtIniName.Text;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            iniFileName = "";
            Close();
        }
    }
}
