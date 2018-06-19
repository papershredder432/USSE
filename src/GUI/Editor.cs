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
                    SpawnCZ.Value = (decimal)zombies.Spawn_Chance;
                    LootC.Value = (decimal)zombies.Loot_Chance;
                    Crawler.Value = (decimal)zombies.Crawler_Chance;
                    Sprinter.Value = (decimal)zombies.Sprinter_Chance;
                    Flanker.Value = (decimal)zombies.Flanker_Chance;
                    Burner.Value = (decimal)zombies.Burner_Chance;
                    Acid.Value = (decimal)zombies.Acid_Chance;
                    Spirit.Value = (decimal)zombies.Spirit_Chance;
                    Electric.Value = (decimal)zombies.Boss_Electric_Chance;
                    Wind.Value = (decimal)zombies.Boss_Wind_Chance;
                    Fire.Value = (decimal)zombies.Boss_Fire_Chance;
                    RespawnD.Value = (decimal)zombies.Respawn_Day_Time;
                    RespawnN.Value = (decimal)zombies.Respawn_Night_Time;
                    RespawnB.Value = (decimal)zombies.Respawn_Beacon_Time;
                    DamageM.Value = (decimal)zombies.Damage_Multiplier;
                    ArmorM.Value = (decimal)zombies.Armor_Multiplier;
                    ExperienceBM.Value = (decimal)zombies.Beacon_Experience_Multiplier;
                    ExperienceFMM.Value = (decimal)zombies.Full_Moon_Experience_Multiplier;
                    MinDrops.Value = zombies.Min_Drops;
                    MaxDrops.Value = zombies.Max_Drops;
                    MinDropsM.Value = zombies.Min_Mega_Drops;
                    MaxDropsM.Value = zombies.Max_Mega_Drops;
                    MinDropsB.Value = zombies.Min_Boss_Drops;
                    MaxDropsB.Value = zombies.Max_Boss_Drops;
                    Slow.Checked = zombies.Slow_Movement;
                    Stun.Checked = zombies.Can_Stun;
                    break;
                case "animals":
                    // Load Animals configuration
                    var animals = Config.Config.Normal.Animals;
                    RespawnA.Value = (decimal)animals.Respawn_Time;
                    DamageMA.Value = (decimal)animals.Damage_Multiplier;
                    InstancesIA.Value = animals.Max_Instances_Insane;
                    InstancesLA.Value = animals.Max_Instances_Large;
                    InstancesMA.Value = animals.Max_Instances_Medium;
                    InstancesSA.Value = animals.Max_Instances_Small;
                    InstancesTA.Value = animals.Max_Instances_Tiny;
                    break;
                case "barricadesstructures":
                    // Load Barricades configuration
                    var barricades = Config.Config.Normal.Barricades;
                    DecayTB.Value = barricades.Decay_Time;
                    ArmorMB.Value = (decimal)barricades.Armor_Multiplier;
                    GunDMB.Value = (decimal)barricades.Gun_Damage_Multiplier;
                    MeleeDMB.Value = (decimal)barricades.Melee_Damage_Multiplier;

                    // Load Structures configuration
                    var structures = Config.Config.Normal.Structures;
                    DecayTS.Value = structures.Decay_Time;
                    ArmorMS.Value = (decimal)structures.Armor_Multiplier;
                    GunDMS.Value = (decimal)structures.Gun_Damage_Multiplier;
                    MeleeDMS.Value = (decimal)structures.Melee_Damage_Multiplier;
                    break;
                case "players":
                    // Load Players configuration
                    var players = Config.Config.Normal.Players;
                    HealthRegenMF.Value = players.Health_Regen_Min_Food;
                    HealthRegenMW.Value = players.Health_Regen_Min_Water;
                    HealthRegenT.Value = players.Health_Regen_Ticks;
                    FoodT.Value = players.Food_Use_Ticks;
                    FoodDT.Value = players.Food_Damage_Ticks;
                    WaterT.Value = players.Water_Use_Ticks;
                    WaterDT.Value = players.Water_Damage_Ticks;
                    VirusI.Value = players.Virus_Infect;
                    VirusT.Value = players.Virus_Use_Ticks;
                    VirusDT.Value = players.Virus_Damage_Ticks;
                    LegT.Value = players.Leg_Regen_Ticks;
                    BleedDT.Value = players.Bleed_Damage_Ticks;
                    BleedRT.Value = players.Bleed_Regen_Ticks;
                    ArmorMP.Value = (decimal)players.Armor_Multiplier;
                    ExperienceM.Value = (decimal)players.Experience_Multiplier;
                    DetectRM.Value = (decimal)players.Detect_Radius_Multiplier;
                    RayAD.Value = (decimal)players.Ray_Aggressor_Distance;
                    LoseSP.Value = (decimal)players.Lose_Skills_PvP;
                    LoseSE.Value = (decimal)players.Lose_Skills_PvE;
                    LoseIP.Value = (decimal)players.Lose_Items_PvP;
                    LoseIE.Value = (decimal)players.Lose_Items_PvE;
                    LoseCP.Checked = players.Lose_Clothes_PvP;
                    LoseCE.Checked = players.Lose_Clothes_PvE;
                    LegsH.Checked = players.Can_Hurt_Legs;
                    LegsB.Checked = players.Can_Break_Legs;
                    LegsF.Checked = players.Can_Fix_Legs;
                    BleedStart.Checked = players.Can_Start_Bleeding;
                    BleedStop.Checked = players.Can_Stop_Bleeding;
                    MaxSkills.Checked = players.Spawn_With_Max_Skills;
                    StaminaS.Checked = players.Spawn_With_Stamina_Skills;
                    break;
                case "gameplayobjects":
                    // Load Gameplay configuration
                    var gameplay = Config.Config.Normal.Gameplay;
                    RepairLM.Value = gameplay.Repair_Level_Max;
                    ExitT.Value = gameplay.Timer_Exit;
                    RespawnT.Value = gameplay.Timer_Respawn;
                    HomeT.Value = gameplay.Timer_Home;
                    GroupMM.Value = gameplay.Max_Group_Members;
                    Hitmarkers.Checked = gameplay.Hitmarkers;
                    Crosshair.Checked = gameplay.Crosshair;
                    Ballistics.Checked = gameplay.Ballistics;
                    Chart.Checked = gameplay.Chart;
                    Satellite.Checked = gameplay.Satellite;
                    Compass.Checked = gameplay.Compass;
                    GroupM.Checked = gameplay.Group_Map;
                    GroupHUD.Checked = gameplay.Group_HUD;
                    StaticGroups.Checked = gameplay.Allow_Static_Groups;
                    DynamicGroups.Checked = gameplay.Allow_Dynamic_Groups;
                    ShoulderC.Checked = gameplay.Allow_Shoulder_Camera;
                    Suicide.Checked = gameplay.Can_Suicide;

                    // Load Objects configuration
                    var objects = Config.Config.Normal.Objects;
                    BSRM.Value = (decimal)objects.Binary_State_Reset_Multiplier;
                    FuelRM.Value = (decimal)objects.Fuel_Reset_Multiplier;
                    WaterRM.Value = (decimal)objects.Water_Reset_Multiplier;
                    ResourceRM.Value = (decimal)objects.Resource_Reset_Multiplier;
                    RubbleRM.Value = (decimal)objects.Rubble_Reset_Multiplier;
                    break;
                case "events":
                    var events = Config.Config.Normal.Events;
                    MinRF.Value = (decimal)events.Rain_Frequency_Min;
                    MaxRF.Value = (decimal)events.Rain_Frequency_Max;
                    MinRD.Value = (decimal)events.Rain_Duration_Min;
                    MaxRD.Value = (decimal)events.Rain_Duration_Max;
                    MinSF.Value = (decimal)events.Snow_Frequency_Min;
                    MaxSF.Value = (decimal)events.Snow_Frequency_Max;
                    MinSD.Value = (decimal)events.Snow_Duration_Min;
                    MaxSD.Value = (decimal)events.Snow_Duration_Max;
                    MinAF.Value = (decimal)events.Airdrop_Frequency_Min;
                    MaxAF.Value = (decimal)events.Airdrop_Frequency_Max;
                    AirdropS.Value = (decimal)events.Airdrop_Speed;
                    AirdropF.Value = (decimal)events.Airdrop_Force;
                    MinAP.Value = events.Arena_Min_Players;
                    CircleDMG.Value = events.Arena_Compactor_Damage;
                    ArenaCT.Value = events.Arena_Clear_Timer;
                    TimerF.Value = events.Arena_Finale_Timer;
                    TimerR.Value = events.Arena_Restart_Timer;
                    CircleDT.Value = events.Arena_Compactor_Delay_Timer;
                    CirclePT.Value = events.Arena_Compactor_Pause_Timer;
                    CircleSI.Value = (decimal)events.Arena_Compactor_Speed_Insane;
                    CircleSL.Value = (decimal)events.Arena_Compactor_Speed_Large;
                    CircleSM.Value = (decimal)events.Arena_Compactor_Speed_Medium;
                    CircleSS.Value = (decimal)events.Arena_Compactor_Speed_Small;
                    CircleST.Value = (decimal)events.Arena_Compactor_Speed_Tiny;
                    ArenaAirdrops.Checked = events.Arena_Use_Airdrops;
                    PauseCircle.Checked = events.Arena_Use_Compactor_Pause;
                    break;
            }
        }
        #endregion
    }
}