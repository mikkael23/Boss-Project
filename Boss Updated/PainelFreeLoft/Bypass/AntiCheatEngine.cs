using System;
using System.Diagnostics;

public class AntiCheatEngine
{
    public static bool IsCheatEngineRunning()
    {
        // Detecta se o sistema é 32 ou 64 bits
        string architecture = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
        Console.WriteLine($"Sistema operacional: {architecture}");

        // Lista todos os processos em execução
        Process[] processes = Process.GetProcesses();

        // Verifica cada processo
        foreach (Process process in processes)
        {
            if (IsCheatProcess(process.ProcessName) || IsCheatModuleLoaded(process.Id))
            {
                return true; // Cheat Engine detectado
            }
        }

        return false; // Nenhum processo relacionado ao Cheat Engine foi detectado
    }

    private static bool IsCheatProcess(string processName)
    {
        // Verifica se o nome do processo contém "cheat" ou "engine"
        return processName.ToLower().Contains("cheat") || processName.ToLower().Contains("engine");
    }

    private static bool IsCheatModuleLoaded(int processId)
    {
        try
        {
            // Obtém o processo pelo ID e verifica se algum módulo contém "cheat" ou "engine"
            Process process = Process.GetProcessById(processId);
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName.ToLower().Contains("cheat") || module.ModuleName.ToLower().Contains("engine"))
                {
                    return true; // Módulo do Cheat Engine detectado
                }
            }
        }
        catch (Exception)
        {
            // Ignora exceções para processos protegidos que não podem ser acessados
        }

        return false; // Nenhum módulo relacionado ao Cheat Engine foi detectado
    }
}
