using papershredder.Programs.USSE.Constants;
using papershredder.Programs.USSE.Memory;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using SDG.Unturned;
using Newtonsoft.Json;

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
                LoadedFile();
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

        #region Load Config When Changing Tabs
        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDetails();
        }
        #endregion

        #region Custom Methods
        private void LoadedFile()
        {
            OpenFold.Enabled = true;
            SaveFile.Enabled = true;
            TXT_Load.Visible = false;
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
                    var items = new ItemsConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        items = Config.Config.Normal.Items;
                    else if (Easy.Checked)
                        items = Config.Config.Easy.Items;
                    else if (Hard.Checked)
                        items = Config.Config.Hard.Items;

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
                    var vehicles = new VehiclesConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        vehicles = Config.Config.Normal.Vehicles;
                    else if (Easy.Checked)
                        vehicles = Config.Config.Easy.Vehicles;
                    else if (Hard.Checked)
                        vehicles = Config.Config.Hard.Vehicles;

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
                    var zombies = new ZombiesConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        zombies = Config.Config.Normal.Zombies;
                    else if (Easy.Checked)
                        zombies = Config.Config.Easy.Zombies;
                    else if (Hard.Checked)
                        zombies = Config.Config.Hard.Zombies;

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
                    var animals = new AnimalsConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        animals = Config.Config.Normal.Animals;
                    else if (Easy.Checked)
                        animals = Config.Config.Easy.Animals;
                    else if (Hard.Checked)
                        animals = Config.Config.Hard.Animals;

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
                    var barricades = new BarricadesConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        barricades = Config.Config.Normal.Barricades;
                    else if (Easy.Checked)
                        barricades = Config.Config.Easy.Barricades;
                    else if (Hard.Checked)
                        barricades = Config.Config.Hard.Barricades;

                    DecayTB.Value = barricades.Decay_Time;
                    ArmorMB.Value = (decimal)barricades.Armor_Multiplier;
                    GunDMB.Value = (decimal)barricades.Gun_Damage_Multiplier;
                    MeleeDMB.Value = (decimal)barricades.Melee_Damage_Multiplier;

                    // Load Structures configuration
                    var structures = new StructuresConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        structures = Config.Config.Normal.Structures;
                    else if (Easy.Checked)
                        structures = Config.Config.Easy.Structures;
                    else if (Hard.Checked)
                        structures = Config.Config.Hard.Structures;

                    DecayTS.Value = structures.Decay_Time;
                    ArmorMS.Value = (decimal)structures.Armor_Multiplier;
                    GunDMS.Value = (decimal)structures.Gun_Damage_Multiplier;
                    MeleeDMS.Value = (decimal)structures.Melee_Damage_Multiplier;
                    break;
                case "players":
                    // Load Players configuration
                    var players = new PlayersConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        players = Config.Config.Normal.Players;
                    else if (Easy.Checked)
                        players = Config.Config.Easy.Players;
                    else if (Hard.Checked)
                        players = Config.Config.Hard.Players;

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
                    var gameplay = new GameplayConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        gameplay = Config.Config.Normal.Gameplay;
                    else if (Easy.Checked)
                        gameplay = Config.Config.Easy.Gameplay;
                    else if (Hard.Checked)
                        gameplay = Config.Config.Hard.Gameplay;

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
                    var objects = new ObjectConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        objects = Config.Config.Normal.Objects;
                    else if (Easy.Checked)
                        objects = Config.Config.Easy.Objects;
                    else if (Hard.Checked)
                        objects = Config.Config.Hard.Objects;

                    BSRM.Value = (decimal)objects.Binary_State_Reset_Multiplier;
                    FuelRM.Value = (decimal)objects.Fuel_Reset_Multiplier;
                    WaterRM.Value = (decimal)objects.Water_Reset_Multiplier;
                    ResourceRM.Value = (decimal)objects.Resource_Reset_Multiplier;
                    RubbleRM.Value = (decimal)objects.Rubble_Reset_Multiplier;
                    break;
                case "events":
                    // Load Events configuration
                    var events = new EventsConfigData(EGameMode.ANY);
                    if (Normal.Checked)
                        events = Config.Config.Normal.Events;
                    else if (Easy.Checked)
                        events = Config.Config.Easy.Events;
                    else if (Hard.Checked)
                        events = Config.Config.Hard.Events;

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

        #region Prevent all modes from being unchecked.
        private void PreventAllUnchecked(string sent)
        {
            if (!Easy.Checked && !Normal.Checked && !Hard.Checked)
            {
                switch (sent.ToLowerInvariant())
                {
                    case "easy":
                        Easy.Checked = true;
                        break;
                    case "normal":
                        Normal.Checked = true;
                        break;
                    case "hard":
                        Hard.Checked = true;
                        break;
                }
            }
        }
        private void Easy_CheckedChanged(object sender, EventArgs e) => PreventAllUnchecked("easy");
        private void Normal_CheckedChanged(object sender, EventArgs e) => PreventAllUnchecked("normal");
        private void Hard_CheckedChanged(object sender, EventArgs e) => PreventAllUnchecked("hard");
        #endregion

        #region Save all data when anything changes (Yes, it's overkill, but whatevs).
        #region Server & Browser
        private void BrowserIcon_TextChanged(object sender, EventArgs e) => Config.Config.Browser.Icon = BrowserIcon.Text;
        private void Thumbnail_TextChanged(object sender, EventArgs e) => Config.Config.Browser.Thumbnail = Thumbnail.Text;
        private void DescriptionH_TextChanged(object sender, EventArgs e) => Config.Config.Browser.Desc_Hint = DescriptionH.Text;
        private void DescriptionL_TextChanged(object sender, EventArgs e) => Config.Config.Browser.Desc_Server_List = DescriptionL.Text;
        private void DescriptionF_TextChanged(object sender, EventArgs e) => Config.Config.Browser.Desc_Full = DescriptionF.Text;
        private void Ping_ValueChanged(object sender, EventArgs e) => Config.Config.Server.Max_Ping_Milliseconds = (uint)Ping.Value;
        private void TimeoutQ_ValueChanged(object sender, EventArgs e) => Config.Config.Server.Timeout_Queue_Seconds = (float)TimeoutQ.Value;
        private void TimeoutG_ValueChanged(object sender, EventArgs e) => Config.Config.Server.Timeout_Game_Seconds = (float)TimeoutG.Value;
        private void Packets_ValueChanged(object sender, EventArgs e) => Config.Config.Server.Max_Packets_Per_Second = (float)Packets.Value;
        private void BattlEye_CheckedChanged(object sender, EventArgs e) => Config.Config.Server.BattlEye_Secure = BattlEye.Checked;
        private void VAC_CheckedChanged(object sender, EventArgs e) => Config.Config.Server.VAC_Secure = VAC.Checked;
        #endregion

        #region Items
        private void SpawnChance_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Spawn_Chance = (float)SpawnChance.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Spawn_Chance = (float)SpawnChance.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Spawn_Chance = (float)SpawnChance.Value;
        }
        private void DespawnD_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Despawn_Dropped_Time = (float)DespawnD.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Despawn_Dropped_Time = (float)DespawnD.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Despawn_Dropped_Time = (float)DespawnD.Value;
        }
        private void DespawnN_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Despawn_Natural_Time = (float)DespawnN.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Despawn_Natural_Time = (float)DespawnN.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Despawn_Natural_Time = (float)DespawnN.Value;
        }
        private void RespawnI_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Spawn_Chance = (float)RespawnI.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Spawn_Chance = (float)RespawnI.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Spawn_Chance = (float)RespawnI.Value;
        }
        private void QualityF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Quality_Full_Chance = (float)QualityF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Quality_Full_Chance = (float)QualityF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Quality_Full_Chance = (float)QualityF.Value;
        }
        private void QualityM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Quality_Multiplier = (float)QualityM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Quality_Multiplier = (float)QualityM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Quality_Multiplier = (float)QualityM.Value;
        }
        private void Durability_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Has_Durability = Durability.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Items.Has_Durability = Durability.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Items.Has_Durability = Durability.Checked;
        }
        private void CrateBM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Crate_Bullets_Multiplier = (float)CrateBM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Crate_Bullets_Multiplier = (float)CrateBM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Crate_Bullets_Multiplier = (float)CrateBM.Value;
        }
        private void CrateBF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Crate_Bullets_Full_Chance = (float)CrateBF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Crate_Bullets_Full_Chance = (float)CrateBF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Crate_Bullets_Full_Chance = (float)CrateBF.Value;
        }
        private void MagBM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Magazine_Bullets_Multiplier = (float)MagBM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Magazine_Bullets_Multiplier = (float)MagBM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Magazine_Bullets_Multiplier = (float)MagBM.Value;
        }
        private void MagBF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Magazine_Bullets_Full_Chance = (float)MagBF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Magazine_Bullets_Full_Chance = (float)MagBF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Magazine_Bullets_Full_Chance = (float)MagBF.Value;
        }
        private void GunBM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Gun_Bullets_Multiplier = (float)GunBM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Gun_Bullets_Multiplier = (float)GunBM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Gun_Bullets_Multiplier = (float)GunBM.Value;
        }
        private void GunBF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Items.Gun_Bullets_Full_Chance = (float)GunBF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Items.Gun_Bullets_Full_Chance = (float)GunBF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Items.Gun_Bullets_Full_Chance = (float)GunBF.Value;
        }
        #endregion

        #region Vehicles
        private void Battery_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Has_Battery_Chance = (float)Battery.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Has_Battery_Chance = (float)Battery.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Has_Battery_Chance = (float)Battery.Value;
        }
        private void Unlock_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Unlocked_After_Seconds_In_Safezone = (float)Unlock.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Unlocked_After_Seconds_In_Safezone = (float)Unlock.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Unlocked_After_Seconds_In_Safezone = (float)Unlock.Value;
        }
        private void RespawnV_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Respawn_Time = (float)RespawnV.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Respawn_Time = (float)RespawnV.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Respawn_Time = (float)RespawnV.Value;
        }
        private void Tire_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Has_Tire_Chance = (float)Tire.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Has_Tire_Chance = (float)Tire.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Has_Tire_Chance = (float)Tire.Value;
        }
        private void MaxBattery_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Max_Battery_Charge = (float)MaxBattery.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Max_Battery_Charge = (float)MaxBattery.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Max_Battery_Charge = (float)MaxBattery.Value;
        }
        private void MinBattery_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Min_Battery_Charge = (float)MinBattery.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Min_Battery_Charge = (float)MinBattery.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Min_Battery_Charge = (float)MinBattery.Value;
        }
        private void ArmorV_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Armor_Multiplier = (float)ArmorV.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Armor_Multiplier = (float)ArmorV.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Armor_Multiplier = (float)ArmorV.Value;
        }
        private void InstancesI_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Max_Instances_Insane = (uint)InstancesI.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Max_Instances_Insane = (uint)InstancesI.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Max_Instances_Insane = (uint)InstancesI.Value;
        }
        private void InstancesL_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Max_Instances_Large = (uint)InstancesL.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Max_Instances_Large = (uint)InstancesL.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Max_Instances_Large = (uint)InstancesL.Value;
        }
        private void InstancesM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Max_Instances_Medium = (uint)InstancesM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Max_Instances_Medium = (uint)InstancesM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Max_Instances_Medium = (uint)InstancesM.Value;
        }
        private void InstancesS_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Max_Instances_Small = (uint)InstancesS.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Max_Instances_Small = (uint)InstancesS.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Max_Instances_Small = (uint)InstancesS.Value;
        }
        private void InstancesT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Max_Instances_Tiny = (uint)InstancesT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Max_Instances_Tiny = (uint)InstancesT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Max_Instances_Tiny = (uint)InstancesT.Value;
        }
        private void MeleeD_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Melee_Damage_Multiplier = (float)MeleeD.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Melee_Damage_Multiplier = (float)MeleeD.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Melee_Damage_Multiplier = (float)MeleeD.Value;
        }
        private void GunD_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Vehicles.Gun_Damage_Multiplier = (float)GunD.Value;
            if (Normal.Checked)
                Config.Config.Normal.Vehicles.Gun_Damage_Multiplier = (float)GunD.Value;
            if (Hard.Checked)
                Config.Config.Hard.Vehicles.Gun_Damage_Multiplier = (float)GunD.Value;
        }
        #endregion

        #region Zombies
        private void SpawnCZ_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Spawn_Chance = (float)SpawnCZ.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Spawn_Chance = (float)SpawnCZ.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Spawn_Chance = (float)SpawnCZ.Value;
        }
        private void Spirit_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Spirit_Chance = (float)Spirit.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Spirit_Chance = (float)Spirit.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Spirit_Chance = (float)Spirit.Value;
        }
        private void Acid_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Acid_Chance = (float)Acid.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Acid_Chance = (float)Acid.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Acid_Chance = (float)Acid.Value;
        }
        private void Burner_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Burner_Chance = (float)Burner.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Burner_Chance = (float)Burner.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Burner_Chance = (float)Burner.Value;
        }
        private void Flanker_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Flanker_Chance = (float)Flanker.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Flanker_Chance = (float)Flanker.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Flanker_Chance = (float)Flanker.Value;
        }
        private void Sprinter_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Sprinter_Chance = (float)Sprinter.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Sprinter_Chance = (float)Sprinter.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Sprinter_Chance = (float)Sprinter.Value;
        }
        private void Crawler_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Crawler_Chance = (float)Crawler.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Crawler_Chance = (float)Crawler.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Crawler_Chance = (float)Crawler.Value;
        }
        private void LootC_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Loot_Chance = (float)LootC.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Loot_Chance = (float)LootC.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Loot_Chance = (float)LootC.Value;
        }
        private void Electric_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Boss_Electric_Chance = (float)Electric.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Boss_Electric_Chance = (float)Electric.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Boss_Electric_Chance = (float)Electric.Value;
        }
        private void Wind_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Boss_Wind_Chance = (float)Wind.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Boss_Wind_Chance = (float)Wind.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Boss_Wind_Chance = (float)Wind.Value;
        }
        private void MinDrops_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Min_Drops = (uint)MinDrops.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Min_Drops = (uint)MinDrops.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Min_Drops = (uint)MinDrops.Value;
        }
        private void MaxDrops_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Max_Drops = (uint)MaxDrops.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Max_Drops = (uint)MaxDrops.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Max_Drops = (uint)MaxDrops.Value;
        }
        private void MinDropsM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Min_Mega_Drops = (uint)MinDropsM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Min_Mega_Drops = (uint)MinDropsM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Min_Mega_Drops = (uint)MinDropsM.Value;
        }
        private void MaxDropsM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Max_Mega_Drops = (uint)MaxDropsM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Max_Mega_Drops = (uint)MaxDropsM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Max_Mega_Drops = (uint)MaxDropsM.Value;
        }
        private void MinDropsB_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Min_Boss_Drops = (uint)MinDropsB.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Min_Boss_Drops = (uint)MinDropsB.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Min_Boss_Drops = (uint)MinDropsB.Value;
        }
        private void MaxDropsB_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Max_Boss_Drops = (uint)MaxDropsB.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Max_Boss_Drops = (uint)MaxDropsB.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Max_Boss_Drops = (uint)MaxDropsB.Value;
        }
        private void Fire_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Boss_Fire_Chance = (long)Fire.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Boss_Fire_Chance = (long)Fire.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Boss_Fire_Chance = (long)Fire.Value;
        }
        private void RespawnD_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Respawn_Day_Time = (long)RespawnD.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Respawn_Day_Time = (long)RespawnD.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Respawn_Day_Time = (long)RespawnD.Value;
        }
        private void RespawnN_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Respawn_Night_Time = (long)RespawnN.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Respawn_Night_Time = (long)RespawnN.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Respawn_Night_Time = (long)RespawnN.Value;
        }
        private void RespawnB_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Respawn_Beacon_Time = (long)RespawnB.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Respawn_Beacon_Time = (long)RespawnB.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Respawn_Beacon_Time = (long)RespawnB.Value;
        }
        private void DamageM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Damage_Multiplier = (long)DamageM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Damage_Multiplier = (long)DamageM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Damage_Multiplier = (long)DamageM.Value;
        }
        private void ArmorM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Armor_Multiplier = (long)ArmorM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Armor_Multiplier = (long)ArmorM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Armor_Multiplier = (long)ArmorM.Value;
        }
        private void ExperienceBM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Beacon_Experience_Multiplier = (long)ExperienceBM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Beacon_Experience_Multiplier = (long)ExperienceBM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Beacon_Experience_Multiplier = (long)ExperienceBM.Value;
        }
        private void ExperienceFMM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Full_Moon_Experience_Multiplier = (long)ExperienceFMM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Full_Moon_Experience_Multiplier = (long)ExperienceFMM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Full_Moon_Experience_Multiplier = (long)ExperienceFMM.Value;
        }
        private void Stun_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Can_Stun = Stun.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Can_Stun = Stun.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Can_Stun = Stun.Checked;
        }
        private void Slow_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Zombies.Slow_Movement = Slow.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Zombies.Slow_Movement = Slow.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Zombies.Slow_Movement = Slow.Checked;
        }
        #endregion

        #region Animals
        private void RespawnA_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Animals.Respawn_Time = (long)RespawnA.Value;
            if (Normal.Checked)
                Config.Config.Normal.Animals.Respawn_Time = (long)RespawnA.Value;
            if (Hard.Checked)
                Config.Config.Hard.Animals.Respawn_Time = (long)RespawnA.Value;
        }
        private void InstancesLA_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Animals.Max_Instances_Large = (uint)InstancesLA.Value;
            if (Normal.Checked)
                Config.Config.Normal.Animals.Max_Instances_Large = (uint)InstancesLA.Value;
            if (Hard.Checked)
                Config.Config.Hard.Animals.Max_Instances_Large = (uint)InstancesLA.Value;
        }
        private void InstancesMA_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Animals.Max_Instances_Medium = (uint)InstancesMA.Value;
            if (Normal.Checked)
                Config.Config.Normal.Animals.Max_Instances_Medium = (uint)InstancesMA.Value;
            if (Hard.Checked)
                Config.Config.Hard.Animals.Max_Instances_Medium = (uint)InstancesMA.Value;
        }
        private void InstancesSA_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Animals.Max_Instances_Small = (uint)InstancesSA.Value;
            if (Normal.Checked)
                Config.Config.Normal.Animals.Max_Instances_Small = (uint)InstancesSA.Value;
            if (Hard.Checked)
                Config.Config.Hard.Animals.Max_Instances_Small = (uint)InstancesSA.Value;
        }
        private void InstancesTA_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Animals.Max_Instances_Tiny = (uint)InstancesTA.Value;
            if (Normal.Checked)
                Config.Config.Normal.Animals.Max_Instances_Tiny = (uint)InstancesTA.Value;
            if (Hard.Checked)
                Config.Config.Hard.Animals.Max_Instances_Tiny = (uint)InstancesTA.Value;
        }
        private void ArmorMA_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Animals.Armor_Multiplier = (long)ArmorMA.Value;
            if (Normal.Checked)
                Config.Config.Normal.Animals.Armor_Multiplier = (long)ArmorMA.Value;
            if (Hard.Checked)
                Config.Config.Hard.Animals.Armor_Multiplier = (long)ArmorMA.Value;
        }
        private void DamageMA_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Animals.Damage_Multiplier = (long)DamageMA.Value;
            if (Normal.Checked)
                Config.Config.Normal.Animals.Damage_Multiplier = (long)DamageMA.Value;
            if (Hard.Checked)
                Config.Config.Hard.Animals.Damage_Multiplier = (long)DamageMA.Value;
        }
        private void InstancesIA_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Animals.Max_Instances_Insane = (uint)InstancesIA.Value;
            if (Normal.Checked)
                Config.Config.Normal.Animals.Max_Instances_Insane = (uint)InstancesIA.Value;
            if (Hard.Checked)
                Config.Config.Hard.Animals.Max_Instances_Insane = (uint)InstancesIA.Value;
        }
        #endregion

        #region Barricades & Structures
        private void DecayTB_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Barricades.Decay_Time = (uint)DecayTB.Value;
            if (Normal.Checked)
                Config.Config.Normal.Barricades.Decay_Time = (uint)DecayTB.Value;
            if (Hard.Checked)
                Config.Config.Hard.Barricades.Decay_Time = (uint)DecayTB.Value;
        }
        private void GunDMB_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Barricades.Gun_Damage_Multiplier = (long)GunDMB.Value;
            if (Normal.Checked)
                Config.Config.Normal.Barricades.Gun_Damage_Multiplier = (long)GunDMB.Value;
            if (Hard.Checked)
                Config.Config.Hard.Barricades.Gun_Damage_Multiplier = (long)GunDMB.Value;
        }
        private void ArmorMB_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Barricades.Armor_Multiplier = (long)ArmorMB.Value;
            if (Normal.Checked)
                Config.Config.Normal.Barricades.Armor_Multiplier = (long)ArmorMB.Value;
            if (Hard.Checked)
                Config.Config.Hard.Barricades.Armor_Multiplier = (long)ArmorMB.Value;
        }
        private void MeleeDMB_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Barricades.Melee_Damage_Multiplier = (long)MeleeDMB.Value;
            if (Normal.Checked)
                Config.Config.Normal.Barricades.Melee_Damage_Multiplier = (long)MeleeDMB.Value;
            if (Hard.Checked)
                Config.Config.Hard.Barricades.Melee_Damage_Multiplier = (long)MeleeDMB.Value;
        }
        private void MeleeDMS_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Structures.Melee_Damage_Multiplier = (long)MeleeDMS.Value;
            if (Normal.Checked)
                Config.Config.Normal.Structures.Melee_Damage_Multiplier = (long)MeleeDMS.Value;
            if (Hard.Checked)
                Config.Config.Hard.Structures.Melee_Damage_Multiplier = (long)MeleeDMS.Value;
        }
        private void GunDMS_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Structures.Gun_Damage_Multiplier = (long)GunDMS.Value;
            if (Normal.Checked)
                Config.Config.Normal.Structures.Gun_Damage_Multiplier = (long)GunDMS.Value;
            if (Hard.Checked)
                Config.Config.Hard.Structures.Gun_Damage_Multiplier = (long)GunDMS.Value;
        }
        private void ArmorMS_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Structures.Armor_Multiplier = (long)ArmorMS.Value;
            if (Normal.Checked)
                Config.Config.Normal.Structures.Armor_Multiplier = (long)ArmorMS.Value;
            if (Hard.Checked)
                Config.Config.Hard.Structures.Armor_Multiplier = (long)ArmorMS.Value;
        }
        private void DecayTS_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Structures.Decay_Time = (uint)DecayTS.Value;
            if (Normal.Checked)
                Config.Config.Normal.Structures.Decay_Time = (uint)DecayTS.Value;
            if (Hard.Checked)
                Config.Config.Hard.Structures.Decay_Time = (uint)DecayTS.Value;
        }
        #endregion

        #region Players
        private void LoseIE_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Lose_Items_PvE = (long)LoseIE.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Lose_Items_PvE = (long)LoseIE.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Lose_Items_PvE = (long)LoseIE.Value;
        }
        private void LoseIP_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Lose_Items_PvP = (long)LoseIP.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Lose_Items_PvP = (long)LoseIP.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Lose_Items_PvP = (long)LoseIP.Value;
        }
        private void LoseSE_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Lose_Skills_PvE = (long)LoseSE.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Lose_Skills_PvE = (long)LoseSE.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Lose_Skills_PvE = (long)LoseSE.Value;
        }
        private void LoseSP_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Lose_Skills_PvP = (long)LoseSP.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Lose_Skills_PvP = (long)LoseSP.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Lose_Skills_PvP = (long)LoseSP.Value;
        }
        private void RayAD_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Ray_Aggressor_Distance = (long)RayAD.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Ray_Aggressor_Distance = (long)RayAD.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Ray_Aggressor_Distance = (long)RayAD.Value;
        }
        private void DetectRM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Detect_Radius_Multiplier = (long)DetectRM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Detect_Radius_Multiplier = (long)DetectRM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Detect_Radius_Multiplier = (long)DetectRM.Value;
        }
        private void ExperienceM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Experience_Multiplier = (long)ExperienceM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Experience_Multiplier = (long)ExperienceM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Experience_Multiplier = (long)ExperienceM.Value;
        }
        private void ArmorMP_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Armor_Multiplier = (long)ArmorMP.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Armor_Multiplier = (long)ArmorMP.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Armor_Multiplier = (long)ArmorMP.Value;
        }
        private void BleedRT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Bleed_Regen_Ticks = (uint)BleedRT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Bleed_Regen_Ticks = (uint)BleedRT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Bleed_Regen_Ticks = (uint)BleedRT.Value;
        }
        private void BleedDT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Bleed_Damage_Ticks = (uint)BleedDT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Bleed_Damage_Ticks = (uint)BleedDT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Bleed_Damage_Ticks = (uint)BleedDT.Value;
        }
        private void LegT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Leg_Regen_Ticks = (uint)LegT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Leg_Regen_Ticks = (uint)LegT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Leg_Regen_Ticks = (uint)LegT.Value;
        }
        private void StaminaS_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Spawn_With_Stamina_Skills = StaminaS.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Players.Spawn_With_Stamina_Skills = StaminaS.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Players.Spawn_With_Stamina_Skills = StaminaS.Checked;
        }
        private void MaxSkills_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Spawn_With_Max_Skills = MaxSkills.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Players.Spawn_With_Max_Skills = MaxSkills.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Players.Spawn_With_Max_Skills = MaxSkills.Checked;
        }
        private void BleedStart_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Can_Start_Bleeding = BleedStart.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Players.Can_Start_Bleeding = BleedStart.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Players.Can_Start_Bleeding = BleedStart.Checked;
        }
        private void BleedStop_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Can_Stop_Bleeding = BleedStop.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Players.Can_Stop_Bleeding = BleedStop.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Players.Can_Stop_Bleeding = BleedStop.Checked;
        }
        private void LegsF_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Can_Fix_Legs = LegsF.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Players.Can_Fix_Legs = LegsF.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Players.Can_Fix_Legs = LegsF.Checked;
        }
        private void LegsB_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Can_Break_Legs = LegsB.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Players.Can_Break_Legs = LegsB.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Players.Can_Break_Legs = LegsB.Checked;
        }
        private void LegsH_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Can_Hurt_Legs = LegsH.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Players.Can_Hurt_Legs = LegsH.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Players.Can_Hurt_Legs = LegsH.Checked;
        }
        private void LoseCE_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Lose_Clothes_PvE = LoseCE.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Players.Lose_Clothes_PvE = LoseCE.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Players.Lose_Clothes_PvE = LoseCE.Checked;
        }
        private void LoseCP_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Lose_Clothes_PvP = LoseCP.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Players.Lose_Clothes_PvP = LoseCP.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Players.Lose_Clothes_PvP = LoseCP.Checked;
        }
        private void VirusDT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Virus_Damage_Ticks = (uint)VirusDT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Virus_Damage_Ticks = (uint)VirusDT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Virus_Damage_Ticks = (uint)VirusDT.Value;
        }
        private void VirusT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Virus_Use_Ticks = (uint)VirusT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Virus_Use_Ticks = (uint)VirusT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Virus_Use_Ticks = (uint)VirusT.Value;
        }
        private void VirusI_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Virus_Infect = (uint)VirusI.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Virus_Infect = (uint)VirusI.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Virus_Infect = (uint)VirusI.Value;
        }
        private void WaterDT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Water_Damage_Ticks = (uint)WaterDT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Water_Damage_Ticks = (uint)WaterDT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Water_Damage_Ticks = (uint)WaterDT.Value;
        }
        private void WaterT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Water_Use_Ticks = (uint)WaterT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Water_Use_Ticks = (uint)WaterT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Water_Use_Ticks = (uint)WaterT.Value;
        }
        private void FoodDT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Food_Damage_Ticks = (uint)FoodDT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Food_Damage_Ticks = (uint)FoodDT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Food_Damage_Ticks = (uint)FoodDT.Value;
        }
        private void FoodT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Food_Use_Ticks = (uint)FoodT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Food_Use_Ticks = (uint)FoodT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Food_Use_Ticks = (uint)FoodT.Value;
        }
        private void HealthRegenT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Health_Regen_Ticks = (uint)HealthRegenT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Health_Regen_Ticks = (uint)HealthRegenT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Health_Regen_Ticks = (uint)HealthRegenT.Value;
        }
        private void HealthRegenMW_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Health_Regen_Min_Water = (uint)HealthRegenMW.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Health_Regen_Min_Water = (uint)HealthRegenMW.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Health_Regen_Min_Water = (uint)HealthRegenMW.Value;
        }
        private void HealthRegenMF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Players.Health_Regen_Min_Food = (uint)HealthRegenMF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Players.Health_Regen_Min_Food = (uint)HealthRegenMF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Players.Health_Regen_Min_Food = (uint)HealthRegenMF.Value;
        }
        #endregion

        #region Gameplay & Objects
        private void RepairLM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Repair_Level_Max = (uint)RepairLM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Repair_Level_Max = (uint)RepairLM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Repair_Level_Max = (uint)RepairLM.Value;
        }
        private void Compass_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Compass = Compass.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Compass = Compass.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Compass = Compass.Checked;
        }
        private void GroupHUD_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Group_HUD = GroupHUD.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Group_HUD = GroupHUD.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Group_HUD = GroupHUD.Checked;
        }
        private void DynamicGroups_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Allow_Dynamic_Groups = DynamicGroups.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Allow_Dynamic_Groups = DynamicGroups.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Allow_Dynamic_Groups = DynamicGroups.Checked;
        }
        private void Ballistics_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Ballistics = Ballistics.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Ballistics = Ballistics.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Ballistics = Ballistics.Checked;
        }
        private void Chart_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Chart = Chart.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Chart = Chart.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Chart = Chart.Checked;
        }
        private void Satellite_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Satellite = Satellite.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Satellite = Satellite.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Satellite = Satellite.Checked;
        }
        private void GroupM_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Group_Map = GroupM.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Group_Map = GroupM.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Group_Map = GroupM.Checked;
        }
        private void StaticGroups_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Allow_Static_Groups = StaticGroups.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Allow_Static_Groups = StaticGroups.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Allow_Static_Groups = StaticGroups.Checked;
        }
        private void ShoulderC_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Allow_Shoulder_Camera = ShoulderC.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Allow_Shoulder_Camera = ShoulderC.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Allow_Shoulder_Camera = ShoulderC.Checked;
        }
        private void Suicide_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Can_Suicide = Suicide.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Can_Suicide = Suicide.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Can_Suicide = Suicide.Checked;
        }
        private void Hitmarkers_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Hitmarkers = Hitmarkers.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Hitmarkers = Hitmarkers.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Hitmarkers = Hitmarkers.Checked;
        }
        private void GroupMM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Max_Group_Members = (uint)GroupMM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Max_Group_Members = (uint)GroupMM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Max_Group_Members = (uint)GroupMM.Value;
        }
        private void HomeT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Timer_Home = (uint)HomeT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Timer_Home = (uint)HomeT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Timer_Home = (uint)HomeT.Value;
        }
        private void RespawnT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Timer_Respawn = (uint)RespawnT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Timer_Respawn = (uint)RespawnT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Timer_Respawn = (uint)RespawnT.Value;
        }
        private void ExitT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Timer_Exit = (uint)ExitT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Timer_Exit = (uint)ExitT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Timer_Exit = (uint)ExitT.Value;
        }
        private void Crosshair_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Gameplay.Crosshair = Crosshair.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Gameplay.Crosshair = Crosshair.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Gameplay.Crosshair = Crosshair.Checked;
        }
        private void BSRM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Objects.Binary_State_Reset_Multiplier = (long)BSRM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Objects.Binary_State_Reset_Multiplier = (long)BSRM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Objects.Binary_State_Reset_Multiplier = (long)BSRM.Value;
        }
        private void FuelRM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Objects.Fuel_Reset_Multiplier = (long)FuelRM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Objects.Fuel_Reset_Multiplier = (long)FuelRM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Objects.Fuel_Reset_Multiplier = (long)FuelRM.Value;
        }
        private void WaterRM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Objects.Water_Reset_Multiplier = (long)WaterRM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Objects.Water_Reset_Multiplier = (long)WaterRM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Objects.Water_Reset_Multiplier = (long)WaterRM.Value;
        }
        private void ResourceRM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Objects.Resource_Reset_Multiplier = (long)ResourceRM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Objects.Resource_Reset_Multiplier = (long)ResourceRM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Objects.Resource_Reset_Multiplier = (long)ResourceRM.Value;
        }
        private void RubbleRM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Objects.Rubble_Reset_Multiplier = (long)RubbleRM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Objects.Rubble_Reset_Multiplier = (long)RubbleRM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Objects.Rubble_Reset_Multiplier = (long)RubbleRM.Value;
        }
        #endregion

        #region Events
        private void CircleSI_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Compactor_Speed_Insane = (long)CircleSI.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Compactor_Speed_Insane = (long)CircleSI.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Compactor_Speed_Insane = (long)CircleSI.Value;
        }
        private void CircleSL_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Compactor_Speed_Large = (long)CircleSL.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Compactor_Speed_Large = (long)CircleSL.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Compactor_Speed_Large = (long)CircleSL.Value;
        }
        private void CircleSM_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Compactor_Speed_Medium = (long)CircleSM.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Compactor_Speed_Medium = (long)CircleSM.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Compactor_Speed_Medium = (long)CircleSM.Value;
        }
        private void CircleSS_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Compactor_Speed_Small = (long)CircleSS.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Compactor_Speed_Small = (long)CircleSS.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Compactor_Speed_Small = (long)CircleSS.Value;
        }
        private void CircleST_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Compactor_Speed_Tiny = (long)CircleST.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Compactor_Speed_Tiny = (long)CircleST.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Compactor_Speed_Tiny = (long)CircleST.Value;
        }
        private void CirclePT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Compactor_Pause_Timer = (uint)CirclePT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Compactor_Pause_Timer = (uint)CirclePT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Compactor_Pause_Timer = (uint)CirclePT.Value;
        }
        private void CircleDT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Compactor_Delay_Timer = (uint)CircleDT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Compactor_Delay_Timer = (uint)CircleDT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Compactor_Delay_Timer = (uint)CircleDT.Value;
        }
        private void TimerR_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Restart_Timer = (uint)TimerR.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Restart_Timer = (uint)TimerR.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Restart_Timer = (uint)TimerR.Value;
        }
        private void TimerF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Finale_Timer = (uint)TimerF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Finale_Timer = (uint)TimerF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Finale_Timer = (uint)TimerF.Value;
        }
        private void ArenaCT_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Clear_Timer = (uint)ArenaCT.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Clear_Timer = (uint)ArenaCT.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Clear_Timer = (uint)ArenaCT.Value;
        }
        private void CircleDMG_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Compactor_Damage = (uint)CircleDMG.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Compactor_Damage = (uint)CircleDMG.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Compactor_Damage = (uint)CircleDMG.Value;
        }
        private void MinAP_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Min_Players = (uint)MinAP.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Min_Players = (uint)MinAP.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Min_Players = (uint)MinAP.Value;
        }
        private void PauseCircle_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Use_Compactor_Pause = PauseCircle.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Use_Compactor_Pause = PauseCircle.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Use_Compactor_Pause = PauseCircle.Checked;
        }
        private void ArenaAirdrops_CheckedChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Arena_Use_Airdrops = ArenaAirdrops.Checked;
            if (Normal.Checked)
                Config.Config.Normal.Events.Arena_Use_Airdrops = ArenaAirdrops.Checked;
            if (Hard.Checked)
                Config.Config.Hard.Events.Arena_Use_Airdrops = ArenaAirdrops.Checked;
        }
        private void AirdropF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Airdrop_Force = (long)AirdropF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Airdrop_Force = (long)AirdropF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Airdrop_Force = (long)AirdropF.Value;
        }
        private void AirdropS_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Airdrop_Speed = (long)AirdropS.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Airdrop_Speed = (long)AirdropS.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Airdrop_Speed = (long)AirdropS.Value;
        }
        private void MaxAF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Airdrop_Frequency_Max = (long)MaxAF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Airdrop_Frequency_Max = (long)MaxAF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Airdrop_Frequency_Max = (long)MaxAF.Value;
        }
        private void MinAF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Airdrop_Frequency_Min = (long)MinAF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Airdrop_Frequency_Min = (long)MinAF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Airdrop_Frequency_Min = (long)MinAF.Value;
        }
        private void MaxSD_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Snow_Duration_Max = (long)MaxSD.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Snow_Duration_Max = (long)MaxSD.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Snow_Duration_Max = (long)MaxSD.Value;
        }
        private void MinSD_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Snow_Duration_Min = (long)MinSD.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Snow_Duration_Min = (long)MinSD.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Snow_Duration_Min = (long)MinSD.Value;
        }
        private void MaxSF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Snow_Frequency_Max = (long)MaxSF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Snow_Frequency_Max = (long)MaxSF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Snow_Frequency_Max = (long)MaxSF.Value;
        }
        private void MinSF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Snow_Frequency_Min = (long)MinSF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Snow_Frequency_Min = (long)MinSF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Snow_Frequency_Min = (long)MinSF.Value;
        }
        private void MaxRD_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Rain_Duration_Max = (long)MaxRD.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Rain_Duration_Max = (long)MaxRD.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Rain_Duration_Max = (long)MaxRD.Value;
        }
        private void MinRD_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Rain_Duration_Min = (long)MinRD.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Rain_Duration_Min = (long)MinRD.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Rain_Duration_Min = (long)MinRD.Value;
        }
        private void MaxRF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Rain_Frequency_Max = (long)MaxRF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Rain_Frequency_Max = (long)MaxRF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Rain_Frequency_Max = (long)MaxRF.Value;
        }
        private void MinRF_ValueChanged(object sender, EventArgs e)
        {
            if (Easy.Checked)
                Config.Config.Easy.Events.Rain_Frequency_Min = (long)MinRF.Value;
            if (Normal.Checked)
                Config.Config.Normal.Events.Rain_Frequency_Min = (long)MinRF.Value;
            if (Hard.Checked)
                Config.Config.Hard.Events.Rain_Frequency_Min = (long)MinRF.Value;
        }
        #endregion

        #endregion

        #region File editor
        // When the button is pressed, it will actually verify the written contents
        private void Verification_Click(object sender, EventArgs e)
        {
            try
            {
                Config.Config = JsonConvert.DeserializeObject<ConfigData>(FileOutput.Text);
                MessageBox.Show("Text is valid JSON for the format config.data uses, saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("The text is not valid. Error: " + ex.Message);
            }
        }
        #endregion
    }
}