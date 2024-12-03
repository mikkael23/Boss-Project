using authguard;
using PainelFreeLoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CILIX_V3;


namespace PainelFreeLoft
{
    internal static class Program
    {
        // Importa a função SetErrorMode da kernel32.dll
        [DllImport("kernel32.dll")]
        static extern ErrorModes SetErrorMode(ErrorModes uMode);

        // Enumera os modos de erro que podem ser configurados
        [Flags]
        public enum ErrorModes : uint
        {
            SEM_FAILCRITICALERRORS = 0x0001,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOOPENFILEERRORBOX = 0x8000
        }

        // Application Settings
        public static API auth_sample = new API(
            "1.0",
            "Scs3fjOuE1q3mDDYHJlXi84TApSKSYwomkB",
            "cec2849b1643f5824498ed40481d3b01",
            show_messages: false
        );

        [STAThread]
        static void Main()
        {
            // Configura o modo de erro para evitar caixas de erro do sistema
            SetErrorMode(ErrorModes.SEM_NOGPFAULTERRORBOX);

            // Adiciona manipulador de exceções para evitar logs no Event Viewer
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                // Captura exceções não tratadas
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    // Você pode registrar a exceção em um arquivo de log ou apenas ignorar
                    // Por exemplo, você pode comentar a linha abaixo para não registrar:
                    // File.AppendAllText("log.txt", exception.ToString());
                }
                // Finaliza o aplicativo silenciosamente
                Application.Exit();
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Inicialização da autenticação
            auth_sample.init();
            Application.Run(new Form1());
        }
    }
}
