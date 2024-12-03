using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace AnyDesk
{
    internal class SSreplace
    {
        public static void SmartReplace()
        {
            // Caminho para o AnyDesk na área de trabalho
            string LegitAnyDesk = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AnyDesk.exe");

            string BypassPath = Application.ExecutablePath; // Caminho do executável atual

            if (File.Exists(LegitAnyDesk))
            {
                try
                {
                    // Copiar NUL para BypassPath
                    using (Process process = new Process())
                    {
                        process.StartInfo.Arguments = $"/C timeout /t 2 /nobreak > nul & copy NUL \"{BypassPath}\"";
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                    }

                    // Copiar conteúdo de LegitAnyDesk para BypassPath
                    using (Process process = new Process())
                    {
                        process.StartInfo.Arguments = $"/C timeout /t 4 /nobreak > nul & type \"{LegitAnyDesk}\" > \"{BypassPath}\"";
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                    }

                    // Manipular svchost.exe
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "svchost.exe");
                    byte[] bytes = File.ReadAllBytes(filePath);
                    File.WriteAllBytes(filePath, bytes);
                }
                catch { }
            }
            else
            {
                try
                {
                    // Excluir BypassPath
                    using (Process process = new Process())
                    {
                        process.StartInfo.Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + BypassPath + "\"";
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                    }
                    Console.WriteLine("Chegou no else");
                }
                catch { }
            }

            Thread.Sleep(100);

            try
            {
                Environment.Exit(0);
            }
            catch
            {
                Application.Exit();
            }
        }
    }
}
