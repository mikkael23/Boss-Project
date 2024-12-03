using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CILIX_V3
{
    public class AntiCmdAndExternalApps
    {
        public static void ProtectApp()
        {
            // Verificar se o cmd está em execução
            if (IsProcessRunning("cmd"))
            {
                MessageBox.Show("Se ha detectado la ejecución del símbolo del sistema (cmd). La aplicación se cerrará.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }

            // Verificar se o aplicativo foi iniciado externamente
            Process currentProcess = Process.GetCurrentProcess();
            Process[] runningProcesses = Process.GetProcesses();
            foreach (Process process in runningProcesses)
            {
                if (process.Id != currentProcess.Id && process.MainModule.FileName == currentProcess.MainModule.FileName)
                {
                    MessageBox.Show($"Se ha detectado un intento de abrir la aplicación desde otra aplicación ({process.ProcessName}). La aplicación se cerrará.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Environment.Exit(0);
                }
            }
        }

        private static bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
    }
}
