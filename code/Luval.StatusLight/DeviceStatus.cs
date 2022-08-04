using Microsoft.Win32;

namespace Luval.StatusLight
{
    public class DeviceStatus
    {
        public static bool IsCameraInUse()
        {
            return IsDeviceInUse(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged");
        }
        public static bool IsMicInUse()
        {
            return IsDeviceInUse(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone\NonPackaged");
        }

        private static bool IsDeviceInUse(string regKey)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(regKey))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                        {
                            var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                            if (endTime <= 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }

    
}