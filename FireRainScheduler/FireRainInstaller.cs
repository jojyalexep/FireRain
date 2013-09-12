using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace FireRainScheduler
{
    [RunInstaller(true)]
    public partial class FireRainInstaller : System.Configuration.Install.Installer
    {
        public FireRainInstaller()
        {
            InitializeComponent();
        }
    }
}
