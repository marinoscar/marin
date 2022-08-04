using Luval.StatusLight.Tray.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Luval.StatusLight.Tray
{
    public class LuvalApplicationContext : ApplicationContext
    {
        private NotifyIcon _trayIcon;
        private System.Timers.Timer? _timer;


        public LuvalApplicationContext()
        {
            _trayIcon = new NotifyIcon()
            {
                Icon = Resources.TrayIcon,
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items = { 
                        new ToolStripMenuItem("Exit", null, Exit),
                        new ToolStripMenuItem("Turn On", null, TurnOn),
                        new ToolStripMenuItem("Turn Off", null, TurnOff)
                    }
                },
                Visible = true
            };
            StartMonitoring();
        }

        void Exit(object? sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Application.Exit();
        }

        void TurnOn(object? sender, EventArgs e)
        {
        }

        void TurnOff(object? sender, EventArgs e)
        {
            
        }

        public void StartMonitoring()
        {
            _timer = new System.Timers.Timer(10000);
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Enabled = true;
            _timer.Start();
        }

        private void StopMonitoring()
        {
            if (_timer == null) return;
            _timer.Enabled = false;
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var cam = DeviceStatus.IsCameraInUse();
            var mic = DeviceStatus.IsMicInUse();
            Debug.WriteLine("MIC: {0}           CAM:{1}", mic, cam);
        }
    }
}
