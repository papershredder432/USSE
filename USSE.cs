using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Unturned_Server_Settings_Editor
{
    public partial class USSE : Form
    {
        // Curved Corners
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRec,
            int nRightRect,
            int nLeftBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public USSE()
        {
            // Splash Screen
            Thread t = new Thread(new ThreadStart(StartForm));
            t.Start();
            Thread.Sleep(5000);
            InitializeComponent();
            t.Abort();

            // Curved Corners Cont
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        // Splash Screen Cont
        public void StartForm()
        {
            Application.Run(new USSESplash());
        }



        // Moveable Window
        int mouseX = 0, mouseY = 0;
        bool mouseDown;

        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                mouseX = MousePosition.X - 400;
                mouseY = MousePosition.Y - 14;

                this.SetDesktopLocation(mouseX, mouseY);
            }
        }

        private void pictureBoxUSSE_Click(object sender, EventArgs e) { }



        // Top Buttons --- Close, Minimize, Discord
        private void pictureBoxClose_Click(object sender, EventArgs e) => Close();

        private void pictureBoxMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

        private void pictureBoxDiscord_Click(object sender, EventArgs e) => System.Diagnostics.Process.Start("Chrome", "https://discord.gg/2XXvBWt");



        // Save, Load, Open Buttons
        private void buttonFileExplore_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON|*.json|XML|*.xml";
            ofd.Title = "Open File";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DialogResult dialog = MessageBox.Show("Successfully loaded " + ofd.SafeFileName + " from " + ofd.FileName, "File Loaded", MessageBoxButtons.OK);

                StreamReader read = new StreamReader(File.OpenRead(ofd.FileName));

                richConfigFile.Text = read.ReadToEnd();
                read.Dispose();



                string jsonString = richConfigFile.Text;

                dynamic configjson = JsonConvert.DeserializeObject(jsonString);

                foreach (var data in configjson)
                {
                    foreach (var data1 in data.Browser)
                    {
                        Console.Write("Icon: " + data1.Icon);
                    }
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {

            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Save File";
            save.Filter = "JSON| *.json|XML|*.xml";

            if (save.ShowDialog() == DialogResult.OK)
            {
                StreamWriter write = new StreamWriter(File.Create(save.FileName));

                write.Write(richConfigFile.Text);
                write.Dispose();
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "Favorites");
        }



        // Checks
        private void checkBoxEasy_CheckedChanged(object sender, EventArgs e)
        {
            ToolTip easyTip = new ToolTip();
            easyTip.SetToolTip(checkBoxEasy, "Saves the config to the Easy section");
        }

        private void checkBoxNormal_CheckedChanged(object sender, EventArgs e)
        {
            ToolTip normalTip = new ToolTip();
            normalTip.SetToolTip(checkBoxNormal, "Saves the config to the Normal section");
        }

        private void checkBoxHard_CheckedChanged(object sender, EventArgs e)
        {
            ToolTip hardTip = new ToolTip();
            hardTip.SetToolTip(checkBoxHard, "Saves the config to the Hard section");
        }



        // Side Buttons --- Info
        private void buttonInfo_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("This application was made by papershredder432#0883." + Environment.NewLine + "And thanks to Pustalorc#8489 for helping with some of the code." + Environment.NewLine + "Go to the USSE Discord for more information", "Information", MessageBoxButtons.OK);
            if (dialog == DialogResult.OK)
            {
                DialogResult next = MessageBox.Show("Would you like to go to the USSE Discord?", "Discord", MessageBoxButtons.YesNo);
                if (next == DialogResult.Yes) { System.Diagnostics.Process.Start("Chrome", "https://discord.gg/2XXvBWt"); }
                if (next == DialogResult.No) { }
            }

        }

        /* Json Loader: Thx Pusta :
        private void SaveJson(ConfigData instance) =>
            File.WriteAllText( Definitely need the path to the file here, you can get it with OpenFileDialog with the Stream you get from opening the file. Simply cast the stream to FileStream and get the Name property , JsonConvert.SerializeObject( You have to generate an instance of the class here, you can use LoadJson(), but you need to modify the details , Formatting.Indented));
        private ConfigData LoadJson()
        {
            if (FileActions.VerifyFile(Some path to the file here, you can get it with OpenFileDialog, though you can ignore this if you use OpenFileDialog, since the dialog can verify that the file exists and then give you a Stream class when loading the file itself, so you could just use the return part with the stream ))
                return JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(configjson));
            else
                return null;
        } */
    }
}
