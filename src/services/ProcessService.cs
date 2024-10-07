using System;
using System.Diagnostics;
using System.IO;

namespace projet_easy_save_v2.src.services
{
    class ProcessService
    {
        public void EncryptOrDecrypteFile(FileInfo file, DirectoryInfo targetDirectory, Stopwatch cryptTimer)
        {
            ProcessStartInfo cryptoSoftInfo = new ProcessStartInfo
            {
                // set directory
                FileName = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"CryptoSoft\bin\Debug\netcoreapp3.1\CryptoSoft.exe"),
                // set args [0], args [1] & args [2]
                Arguments = $"\"{file.FullName}\" {targetDirectory.FullName}",
                UseShellExecute = false,
                CreateNoWindow = true
        };
            // start crypt timer
            cryptTimer.Start();

            // start CryptoSoft
            Process cryptoSoftExe = Process.Start(cryptoSoftInfo);

            //exeCryptoSoft.Kill();
            cryptoSoftExe.WaitForExit();

            // stop crypt timer
            cryptTimer.Stop();

        }
    }
}
