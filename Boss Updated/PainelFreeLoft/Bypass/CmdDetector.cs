using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace AntiCmdApp
{
    public class CmdDetector
    {
        // Lista de processos indesejados
        private readonly string[] unwantedProcesses = { "cmd", "SystemInformer", "ProcessHacker", "dnSpy", "Process Hacker" };
        private const int detectionInterval = 1000;

        public bool CmdDetected { get; private set; } = false;

        public void StartDetection()
        {
            Thread detectionThread = new Thread(new ThreadStart(DetectProcesses));
            detectionThread.IsBackground = true;
            detectionThread.Start();
        }

        private void DetectProcesses()
        {
            while (true)
            {
                foreach (string processName in unwantedProcesses)
                {
                    Process[] processes = Process.GetProcessesByName(processName);

                    if (processes.Length > 0)
                    {
                        foreach (Process process in processes)
                        {
                            try
                            {
                                process.Kill();
                                CmdDetected = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error ao fechar o processo " + processName + ": " + ex.Message,
                                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

                Thread.Sleep(detectionInterval);
            }
        }
    }
}
