﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    internal class runner
    {
       public static void StartCommand()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                Arguments = "/C cd C:\\Users\\Lawrence\\Desktop\\ " +
               "& start vlc.exe --fullscreen 1.mp4 " +
               "& timeout /T 10" +
               "& taskkill /IM vlc.exe /F",

                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            };
            var process = new Process { StartInfo = startInfo };

            process.Start();
            

            process.CloseMainWindow();
            
        }
    }
}
