using papershredder.Programs.USSE.Constants;
using papershredder.Programs.USSE.Memory;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace papershredder.Programs.USSE.GUI
{
    public partial class Editor : Form
    {
        private ConfigJson Config;

        #region DLLImports
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
        #endregion

        #region Instance Creators
        public Editor()
        {
            new Splash().ShowDialog();

            InitializeComponent();

            // Make corners curved.
            FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
        #endregion

        #region Allow Moving Form
        bool mouseDown;
        private void TopPanel_MouseDown(object sender, MouseEventArgs e) => mouseDown = true;
        private void TopPanel_MouseUp(object sender, MouseEventArgs e) => mouseDown = false;
        private void TopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
                SetDesktopLocation(MousePosition.X - 400, MousePosition.Y - 14);
        }
        #endregion

        #region Buttons
        private void Exit_Click(object sender, EventArgs e) => Close();
        private void SaveFile_Click(object sender, EventArgs e) => Config.Save();
        private void Minimize_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;
        private void Discord_Click(object sender, EventArgs e) => Process.Start("Chrome", Information.Discord);
        private void OpenFile_Click(object sender, EventArgs e)
        {
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                Config = new ConfigJson(OFD.FileName);
                EnableButtons();
                LoadDetails();
            }
        }
        private void OpenFold_Click(object sender, EventArgs e)
        {
            if (Config != null)
                Process.Start(Path.GetDirectoryName(Config.Location));
        }
        private void Info_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show(Information.Credits, "Information", MessageBoxButtons.OK);
            if (dialog == DialogResult.OK)
            {
                DialogResult next = MessageBox.Show("Would you like to go to the USSE Discord?", "Discord", MessageBoxButtons.YesNo);
                if (next == DialogResult.Yes)
                    Process.Start("Chrome", Information.Discord);
            }
        }
        #endregion

        #region View Config When Changing
        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDetails();
        }
        #endregion

        #region Custom Methods
        private void EnableButtons()
        {
            OpenFold.Enabled = true;
            SaveFile.Enabled = true;
            Easy.Enabled = true;
            Normal.Enabled = true;
            Hard.Enabled = true;
            Tabs.Enabled = true;
        }
        private void LoadDetails()
        {
            // Load text into rich text box.
            FileOutput.Text = Config.ToString();

            switch (Tabs.SelectedTab.Name.ToLowerInvariant())
            {
                case "serverbrowser":
                    // Load Server configuration
                    var server = Config.Config.Server;
                    Ping.Value = server.Max_Ping_Milliseconds;
                    TimeoutQ.Value = (decimal)server.Timeout_Queue_Seconds;
                    TimeoutG.Value = (decimal)server.Timeout_Game_Seconds;
                    Packets.Value = (decimal)server.Max_Packets_Per_Second;
                    VAC.Checked = server.VAC_Secure;
                    BattlEye.Checked = server.BattlEye_Secure;

                    // Load Browser configuration
                    var browser = Config.Config.Browser;
                    BrowserIcon.Text = browser.Icon;
                    Thumbnail.Text = browser.Thumbnail;
                    DescriptionH.Text = browser.Desc_Hint;
                    DescriptionF.Text = browser.Desc_Full;
                    DescriptionL.Text = browser.Desc_Server_List;
                    break;
                case "items":
                    // Load Items configuration
                    var items = Config.Config.Normal.Items;
                    SpawnChance.Value = (decimal)items.Spawn_Chance;
                    DespawnD.Value = (decimal)items.Despawn_Dropped_Time;
                    DespawnN.Value = (decimal)items.Despawn_Natural_Time;
                    CrateBF.Value = (decimal)items.Crate_Bullets_Full_Chance;
                    CrateBM.Value = (decimal)items.Crate_Bullets_Multiplier;
                    GunBF.Value = (decimal)items.Gun_Bullets_Full_Chance;
                    GunBM.Value = (decimal)items.Gun_Bullets_Multiplier;
                    Durability.Checked = items.Has_Durability;
                    MagBF.Value = (decimal)items.Magazine_Bullets_Full_Chance;
                    MagBM.Value = (decimal)items.Magazine_Bullets_Multiplier;
                    QualityF.Value = (decimal)items.Quality_Full_Chance;
                    QualityM.Value = (decimal)items.Quality_Multiplier;
                    RespawnI.Value = (decimal)items.Respawn_Time;
                    break;
                case "vehicles":
                    // Load Vehicles configuration
                    var vehicles = Config.Config.Normal.Vehicles;
                    ArmorV.Value = (decimal)vehicles.Armor_Multiplier;
                    GunD.Value = (decimal)vehicles.Gun_Damage_Multiplier;
                    Battery.Value = (decimal)vehicles.Has_Battery_Chance;
                    Tire.Value = (decimal)vehicles.Has_Tire_Chance;
                    MaxBattery.Value = (decimal)vehicles.Max_Battery_Charge;
                    InstancesI.Value = vehicles.Max_Instances_Insane;
                    InstancesL.Value = vehicles.Max_Instances_Large;
                    InstancesM.Value = vehicles.Max_Instances_Medium;
                    InstancesS.Value = vehicles.Max_Instances_Small;
                    InstancesT.Value = vehicles.Max_Instances_Tiny;
                    MeleeD.Value = (decimal)vehicles.Melee_Damage_Multiplier;
                    MinBattery.Value = (decimal)vehicles.Min_Battery_Charge;
                    RespawnV.Value = (decimal)vehicles.Respawn_Time;
                    Unlock.Value = (decimal)vehicles.Unlocked_After_Seconds_In_Safezone;
                    break;
                case "zombies":
                    // Load Zombies configuration
                    var zombies = Config.Config.Normal.Zombies;
                    break;
            }
        }
        #endregion
    }
}