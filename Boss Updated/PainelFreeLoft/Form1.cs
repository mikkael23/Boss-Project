using AntiCmdApp;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PainelFreeLoft
{
    public partial class Form1 : Form
    {
        private CmdDetector cmdDetector;
        public Form1()
        {
            InitializeComponent();
            cmdDetector = new CmdDetector();
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
        private async void AnimateColorTransition2(Label label, Color originalColor, Color targetColor)
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
        private async void Form1_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Opacity = 0; // Começa com a opacidade em 0 para efeito de fade-in
            Show(); // Exibe a Form
            FadeIn(); // Inicia o efeito de fade-in
            this.ShowInTaskbar = false;
            guna2CircleProgressBar1.Visible = false;
            AnimateColorTransition2(label4, Color.FromArgb(19, 18, 20), Color.FromArgb(120, 120, 120));
            cmdDetector.StartDetection();
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
        private async void guna2Button6_Click(object sender, EventArgs e)
        {
            {
                guna2CircleProgressBar1.Visible = true;
                guna2ControlBox2.Visible = false;
                timer1.Start();
                await Task.Delay(3500);

                {
                    Form2 form = new Form2();
                    form.Show();
                    this.Hide();
                    return;
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            if (Program.auth_sample.login(txtuser.Text, txtuser.Text))
            {
                // Iniciar o fade out
                await Task.Run(() => FadeIn());

                // Tornar os elementos invisíveis e exibir o carregador
                txtuser.Visible = false;
                label2.Visible = false;
                guna2CircleProgressBar1.Visible = true;
                guna2Button1.Visible = false;

                await Task.Delay(3000); // Espera 3 segundos

                // Abre o Form2 após a animação de fade out
                Form2 form = new Form2();
                form.Show();
                this.Hide();
            }
        }
        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);
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
    }
}
