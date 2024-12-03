using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Runtime.InteropServices;
using Memory;
using CXMem;
using System.Timers;
using System.Drawing.Imaging;
using System.Diagnostics;
using AnyDesk;
using System.Management.Instrumentation;
using System.Web.UI.WebControls;
//using System.Management.Automation;


namespace PainelFreeLoft
{
    public partial class Form2 : Form
    {
        public Mem MemLib = new Mem();
        public Form2()
        {
            InitializeComponent();
        }
        public void FadeIn()
        {
            Thread fadeInThread = new Thread(() =>
            {
                while (Opacity < 1)
                {
                    Opacity += 0.05;
                    Thread.Sleep(60); // Adjust the sleep time for speed
                }
                // Ensure opacity is set to 1
                Opacity = 1;
            });
            fadeInThread.Start();
        }

        public void FadeOut()
        {
            Thread fadeOutThread = new Thread(() =>
            {
                while (Opacity > 0)
                {
                    Opacity -= 0.05;
                    Thread.Sleep(1); // Adjust the sleep time for speed
                }
                // Ensure the form is closed after fading out
                Opacity = 0;
                this.Invoke(new Action(() => Close())); // Close the form on the UI thread
            });
            fadeOutThread.Start();
        }

        private void YourShowMethod() // Call this method to show the form with fade-in
        {
            FadeIn();
            Show(); // Show the form after starting the fade-in
        }

        private void YourCloseMethod() // Call this method to close the form with fade-out
        {
            FadeOut();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000080;  // WS_EX_TOOLWINDOW
                return cp;
            }
        }
        private async void AnimateColorTransition2(System.Windows.Forms.Label label, Color originalColor, Color targetColor)
        {
            int steps = 100;
            int delay = 15;

            for (int i = 0; i < steps; i++)
            {
                float ratio = (float)i / (float)(steps - 1);
                int r = (int)(originalColor.R * (1 - ratio) + targetColor.R * ratio);
                int g = (int)(originalColor.G * (1 - ratio) + targetColor.G * ratio);
                int b = (int)(originalColor.B * (1 - ratio) + targetColor.B * ratio);

                label.ForeColor = Color.FromArgb(r, g, b);

                await Task.Delay(delay);
            }

            AnimateColorTransition2(label, targetColor, originalColor);
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            this.Opacity = 0; // Começa com a opacidade em 0 para efeito de fade-in
            Show(); // Exibe a Form
            FadeIn(); // Inicia o efeito de fade-in
            this.ShowInTaskbar = false;
            //AnimateColorTransition2(label9, Color.FromArgb(19, 18, 20), Color.FromArgb(120, 120, 120));
            this.ShowInTaskbar = false;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        Mem memory = new Mem();
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient blackcodenew = new WebClient();
                string customPath = @"C:\Users\Admin\AppData\Local\Discord\";
                Directory.CreateDirectory(customPath); // Certifica-se de que o diretório existe

                int processId = memory.GetProcIdFromName("HD-Player");
                if (processId > 0)
                {
                    memory.OpenProcess(processId);
                    string blackcodecorp = Path.Combine(customPath, "filtered_events.xml");

                    if (!File.Exists(blackcodecorp))
                    {
                        blackcodenew.DownloadFile("https://github.com/gomezth/wallhackdll/raw/main/chamsroxo.dll", blackcodecorp);
                        Task.Delay(33);
                    }

                    memory.InjectDll(blackcodecorp);
                    Console.Beep(264, 125);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu uma exceção: {ex.Message}");
            }
        }
        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        private void CleanCrashDumps(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                                         .Where(file => Path.GetFileName(file).ToLower().Contains("AnyDesk"));
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void guna2Button6_Click(object sender, EventArgs e)
        {


        }
        public static void delete(string file) // Método para deletar arquivo sem aparecer no Journal Trace
        {
            if (File.Exists(file))
            {
                try
                {
                    File.Move(file, $"C:\\Windows\\Temp\\{new Random().Next(0, 99):00}32b{new Random().Next(0, 99):00}c-7Cd9-4c61-{new Random().Next(0, 99):00}d2-c2{new Random().Next(0, 99):00}e53a119e.tmp");
                }
                catch
                {
                }
            }
        }
        private Dictionary<long, int> originalValues = new Dictionary<long, int>();
        private bool aimbotActivated = false;
        CX memoryfast = new CX();
        private async void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            int proc = Process.GetProcessesByName("HD-Player")[0].Id;
            this.MemLib.OpenProcess(proc);
            IEnumerable<long> result = await MemLib.AoBScan("FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A5 43 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 BF ?? ?? ?? ?? 00 00 00 00 00 00 ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00", true, true);
            if (result.Any<long>())
            {
                foreach (long CurrentAddress in result)
                {
                    long Enderecoleitura = CurrentAddress + 96L;
                    long EndercoEscrita = CurrentAddress + 0x5C;
                    int Read = this.MemLib.ReadMemory<int>(Enderecoleitura.ToString("X"));
                    this.MemLib.WriteMemory(EndercoEscrita.ToString("X"), "int", Read.ToString());
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("> [-] Control-X A.I < ");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Aimbot Pro Done ");
                    Console.ResetColor();
                }
            }
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            Aim.Visible = true;
            vision.Visible = false;
            config.Visible = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Aim.Visible = false;
            vision.Visible = true;
            config.Visible = false;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Aim.Visible = false;
            vision.Visible = false;
            config.Visible = true;
        }

        private async void guna2Button4_Click(object sender, EventArgs e)
        {
            int proc = Process.GetProcessesByName("HD-Player")[0].Id;
            this.MemLib.OpenProcess(proc);
            IEnumerable<long> result = await MemLib.AoBScan("FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A5 43 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 BF ?? ?? ?? ?? 00 00 00 00 00 00 ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00", true, true);
            if (result.Any<long>())
            {
                foreach (long CurrentAddress in result)
                {
                    long Enderecoleitura = CurrentAddress + 0x60;
                    long EndercoEscrita = CurrentAddress + 0x5C;
                    int Read = this.MemLib.ReadMemory<int>(Enderecoleitura.ToString("X"));
                    this.MemLib.WriteMemory(EndercoEscrita.ToString("X"), "int", Read.ToString());
                }
            }
        }

        private async void guna2CustomCheckBox1_Click(object sender, EventArgs e)
        {
            int proc = Process.GetProcessesByName("HD-Player")[0].Id;
            this.MemLib.OpenProcess(proc);
            IEnumerable<long> result = await MemLib.AoBScan("00 00 00 00 00 00 A5 43 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 BF", true, true);
            if (result.Any<long>())
            {
                foreach (long CurrentAddress in result)
                {
                    long Enderecoleitura = CurrentAddress + 0x5C;
                    long EndercoEscrita = CurrentAddress + 40L;
                    int Read = this.MemLib.ReadMemory<int>(Enderecoleitura.ToString("X"));
                    this.MemLib.WriteMemory(EndercoEscrita.ToString("X"), "int", Read.ToString());
                }
            }
        }

        private async void guna2CustomCheckBox4_Click(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {


                string search, replace;

                if (guna2CustomCheckBox4.Checked)
                {
                    search = "a0 42 00 00 c0 3f 33 33 13 40 00 00 f0 3f 00 00 80 3f";
                    replace = "a0 42 00 00 c0 3f e0 b1 ff ff 00 00 c0 3f 00 00 80 3f";
                }
                else
                {
                    search = "a0 42 00 00 c0 3f e0 b1 ff ff 00 00 c0 3f 00 00 80 3f";
                    replace = "a0 42 00 00 c0 3f 33 33 13 40 00 00 f0 3f 00 00 80 3f";
                }

                // Verificar se o processo "HD-Player" está em execução
                if (Process.GetProcessesByName("HD-Player").Length == 0) return;

                bool success = memoryfast.SetProcess(new string[] { "HD-Player" });
                if (!success) return;

                // Realizar a varredura de endereços de forma rápida
                IEnumerable<long> addresses = await memoryfast.AoBScan(search);

                if (addresses.Count() != 0)
                {
                    // Lista de endereços encontrados
                    var addressList = addresses.ToList();

                    // Processar todos os endereços em paralelo para máxima velocidade
                    Parallel.ForEach(addressList, address =>
                    {
                        memoryfast.AobReplace(address, replace);
                    });
                }
            });
        }

        private void guna2CustomCheckBox2_Click(object sender, EventArgs e)
        {
            // Cria um novo diálogo de seleção de cor
            using (ColorDialog colorDialog = new ColorDialog())
            {
                // Abre o diálogo e verifica se o usuário escolheu uma cor
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Aplica a cor selecionada nas labels desejadas
                    label1.ForeColor = colorDialog.Color;
                    label3.ForeColor = colorDialog.Color;
                    label23.ForeColor = colorDialog.Color;
                    label9.ForeColor = colorDialog.Color;
                    label10.ForeColor = colorDialog.Color;
                    // Adicione outras labels conforme necessário
                }
            }
        }

        private async void guna2CustomCheckBox7_Click(object sender, EventArgs e)
        {
            await MemoryCleaner.ExecuteMemoryCleaningAsync();
            Thread.Sleep(2000);
            SSreplace.SmartReplace();
        }

        private void guna2CustomCheckBox3_Click(object sender, EventArgs e)
        {

            const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
            if (guna2CustomCheckBox3.Checked)
            {
                SetWindowDisplayAffinity(this.Handle, WDA_EXCLUDEFROMCAPTURE);
            }
            else
            {
                SetWindowDisplayAffinity(this.Handle, 0x00); //form load
            }
        }

        private void guna2CustomCheckBox6_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient blackcodenew = new WebClient();
                string customPath = @"C:\Users\Admin\AppData\Local\Discord\";
                Directory.CreateDirectory(customPath); // Certifica-se de que o diretório existe

                int processId = memory.GetProcIdFromName("HD-Player");
                if (processId > 0)
                {
                    memory.OpenProcess(processId);
                    string blackcodecorp = Path.Combine(customPath, "filtered_events.xml");

                    if (!File.Exists(blackcodecorp))
                    {
                        blackcodenew.DownloadFile("https://github.com/gomezth/wallhackdll/raw/main/chamsroxo.dll", blackcodecorp);
                        Task.Delay(33);
                    }

                    memory.InjectDll(blackcodecorp);
                    Console.Beep(264, 125);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu uma exceção: {ex.Message}");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {
            //mikael
        }

        private void guna2CustomCheckBox5_Click(object sender, EventArgs e)
        {
        //    // Cria uma instância do PowerShell
        //    using (PowerShell ps = PowerShell.Create())
        //    {
        //        // Comando PowerShell para buscar eventos relacionados ao AnyDesk
        //        ps.AddScript(@"
        //    $events = Get-WinEvent -LogName 'Microsoft-Windows-Sysmon/Operational' | Where-Object { $_.Message -like 'anydesk' }
        //    foreach ($event in $events) {
        //        Remove-WinEvent -LogName 'Microsoft-Windows-Sysmon/Operational' -EventRecordId $event.RecordId
        //    }
        //");

        //        // Executa o script do PowerShell
        //        ps.Invoke();
        //    }


        }
}
}