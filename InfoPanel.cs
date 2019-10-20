using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("InfoPanel", "Mevent", "0.1.6")]
    public class InfoPanel : RustPlugin
    {
        #region Init
        [PluginReference] private readonly Plugin ImageLibrary;
        public static string Layer = "UI_InfoPanel";
        private List<BasePlayer> MenuUsers;
        private static InfoPanel instance;
        #endregion

        #region Config
        private static ConfigData config;
        private class ConfigData
        {
            [JsonProperty("Server name")]
            public serverName ServerName;
            [JsonProperty("Panel display type (Overlay/Hud)")]
            public string LayerType;
            [JsonProperty("Color for panels (background)")]
            public string LabelColor;
            [JsonProperty("Color for panels (closing)")]
            public string CloseColor;
            [JsonProperty("Menu Panel")]
            public Menu menuCfg;
            [JsonProperty("Player Panel")]
            public Panel UsersIcon;
            [JsonProperty("Time Panel")]
            public Panel TimeIcon;
            [JsonProperty("Sleeping Panel")]
            public Panel SleepersIcon;
            [JsonProperty("Coordinate Panel")]
            public Panel CoordsPanel;
            [JsonProperty("Logo customization")]
            public LogoSettings logosettings;
            [JsonProperty("Economy Settings")]
            public Economy economy;
            [JsonProperty("Customize Event Icons")]
            public SettingsEvents eventsettings;
            [JsonProperty("Menu buttons")]
            public Buttons BTN;
        }
        private class LogoSettings : Panel
        {
            [JsonProperty(PropertyName = "Command [EXAMPLE] chat command: chat.say /store OR console command: map.open")]
            public string LogoCmd;
        }
        private class Panel
        {
            [JsonProperty("Enable display?")]
            public bool display;
            [JsonProperty(PropertyName = "Image URL")]
            public string icon;
            [JsonProperty(PropertyName = "Offset Min")]
            public string OffMin;
            [JsonProperty(PropertyName = "Offset Max")]
            public string OffMax;
        }
        private class Economy : Panel
        {
            [JsonProperty(PropertyName = "Call function")]
            public string hook;
            [JsonProperty(PropertyName = "Name of plugin")]
            public string plugin;
        }
        private class SettingsEvents
        {
            [JsonProperty(PropertyName = "Bradley")]
            public EventSetting EventBradley;
            [JsonProperty(PropertyName = "Helicopter")]
            public EventSetting EventHelicopter;
            [JsonProperty(PropertyName = "CargoPlane")]
            public EventSetting EventAirdrop;
            [JsonProperty(PropertyName = "CargoShip")]
            public EventSetting EventCargoship;
            [JsonProperty(PropertyName = "CH47 Helicopter")]
            public EventSetting EventCH47;
        }
        private class EventSetting : Panel
        {
            [JsonProperty(PropertyName = "Activated Event Color")]
            public string OnColor;
            [JsonProperty(PropertyName = "Deactivated Event Color")]
            public string OffColor;
        }
        private class Buttons
        {
            [JsonProperty(PropertyName = "Indent from the edge of the screen")]
            public float IndentStart;
            [JsonProperty(PropertyName = "Button Length")]
            public float btnLenght;
            [JsonProperty(PropertyName = "Button Height")]
            public float btnHeight;
            [JsonProperty(PropertyName = "Indent between buttons")]
            public float btnMargin;
            [JsonProperty(PropertyName = "Close Menu Button")]
            public CloseMenuBTN closeMenuBTN;
            [JsonProperty(PropertyName = "Buttons Settings")]
            public List<BTN> btns;
        }
        private class BTN
        {
            [JsonProperty(PropertyName = "Image URL")]
            public string URL;
            [JsonProperty(PropertyName = "Command for the button")]
            public string CMD;
            [JsonProperty(PropertyName = "Button header")]
            public string Title;
        }
        private class CloseMenuBTN
        {
            [JsonProperty(PropertyName = "Offset Min")]
            public string OffMin;
            [JsonProperty(PropertyName = "Offset Max")]
            public string OffMax;
        }
        private class Menu
        {
            [JsonProperty("Enable display?")]
            public bool display;
            [JsonProperty(PropertyName = "Button header")]
            public string Title;
            [JsonProperty(PropertyName = "Offset Min")]
            public string OffMin;
            [JsonProperty(PropertyName = "Offset Max")]
            public string OffMax;
        }
        private class serverName
        {
            [JsonProperty("Enable display?")]
            public bool display;
            [JsonProperty(PropertyName = "Title")]
            public string name;
            [JsonProperty(PropertyName = "Offset Min")]
            public string OffMin;
            [JsonProperty(PropertyName = "Offset Max")]
            public string OffMax;
        }
        private ConfigData GetDefaultConfig()
        {
            return new ConfigData
            {
                ServerName = new serverName
                {
                    display = true,
                    name = "<b>BY MEVENT</b>",
                    OffMin = "43 -21",
                    OffMax = "183 -5"
                },
                LayerType = "Overlay",
                LabelColor = "#A7A7A725",
                CloseColor = "#FF00003B",
                menuCfg = new Menu
                {
                    display = true,
                    Title = "/MENU",
                    OffMin = "5 -55",
                    OffMax = "40 -43"
                },
                UsersIcon = new Panel
                {
                    display = true,
                    icon = "https://i.imgur.com/MUkpWFA.png",
                    OffMin = "138 -40",
                    OffMax = "183 -24"
                },
                TimeIcon = new Panel
                {
                    display = true,
                    icon = "https://i.imgur.com/c5AW7sO.png",
                    OffMin = "186 -21",
                    OffMax = "231 -5"
                },
                SleepersIcon = new Panel
                {
                    display = true,
                    icon = "https://i.imgur.com/UvLItA7.png",
                    OffMin = "186 -40",
                    OffMax = "231 -24"
                },
                CoordsPanel = new Panel
                {
                    display = true,
                    icon = "https://i.imgur.com/VicmD9Q.png",
                    OffMin = "234 -21",
                    OffMax = "344 -5"
                },
                logosettings = new LogoSettings
                {
                    display = true,
                    icon = "https://i.imgur.com/UFmy9HT.png",
                    LogoCmd = "chat.say /store",
                    OffMin = "5 -40",
                    OffMax = "40 -5"
                },
                economy = new Economy
                {
                    display = false,
                    hook = "Balance",
                    plugin = "Economics",
                    icon = "https://i.imgur.com/K4dCGkQ.png",
                    OffMin = "234 -40",
                    OffMax = "294 -24"
                },
                eventsettings = new SettingsEvents
                {
                    EventHelicopter = new EventSetting
                    {
                        display = true,
                        icon = "https://i.imgur.com/Y0rVkt8.png",
                        OnColor = "#0CF204FF",
                        OffColor = "#FFFFFFFF",
                        OffMin = "43 -40",
                        OffMax = "59 -24",
                    },
                    EventAirdrop = new EventSetting
                    {
                        display = true,
                        icon = "https://i.imgur.com/GcQKlg2.png",
                        OnColor = "#0CF204FF",
                        OffColor = "#FFFFFFFF",
                        OffMin = "62 -40",
                        OffMax = "78 -24",
                    },
                    EventCargoship = new EventSetting
                    {
                        display = true,
                        icon = "https://i.imgur.com/3jigtJS.png",
                        OnColor = "#0CF204FF",
                        OffColor = "#FFFFFFFF",
                        OffMin = "81 -40",
                        OffMax = "97 -24",
                    },
                    EventBradley = new EventSetting
                    {
                        display = true,
                        icon = "https://i.imgur.com/6Vtl3NG.png",
                        OnColor = "#0CF204FF",
                        OffColor = "#FFFFFFFF",
                        OffMin = "100 -40",
                        OffMax = "116 -24",
                    },
                    EventCH47 = new EventSetting
                    {
                        display = true,
                        icon = "https://i.imgur.com/6U5ww9g.png",
                        OnColor = "#0CF204FF",
                        OffColor = "#FFFFFFFF",
                        OffMin = "119 -40",
                        OffMax = "135 -24",
                    },
                },
                BTN = new Buttons
                {
                    IndentStart = -58,
                    btnHeight = 20,
                    btnLenght = 130,
                    btnMargin = 3,
                    closeMenuBTN = new CloseMenuBTN
                    {
                        OffMin = "43 -55",
                        OffMax = "53 -43"
                    },
                    btns = new List<BTN>
                    {
                        new BTN
                        {
                            URL = "https://i.imgur.com/WeHYCni.png",
                            CMD = "chat.say /store",
                            Title = "SHOP",
                        },
                        new BTN
                        {
                            URL = "https://i.imgur.com/buPPBW9.png",
                            CMD = "chat.say /menu",
                            Title = "MENU",
                        },
                        new BTN
                        {
                             URL = "https://i.imgur.com/oFhPHky.png",
                            CMD = "chat.say /map",
                            Title = "MAP",
                        }
                    }
                }
            };
        }
        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                config = Config.ReadObject<ConfigData>();
                if (config == null)
                {
                    LoadDefaultConfig();
                }
            }
            catch
            {
                LoadDefaultConfig();
            }
            SaveConfig();
        }
        protected override void LoadDefaultConfig()
        {
            PrintError("Configuration file is corrupt(or not exists), creating new one!");
            config = GetDefaultConfig();
        }
        protected override void SaveConfig() => Config.WriteObject(config);
        #endregion

        #region Hooks
        void OnServerInitialized()
        {
            if (!ImageLibrary)
            {
                PrintError("Please setup ImageLibrary plugin!");
                Interface.Oxide.UnloadPlugin(Title);
                return;
            }
            else if (config.economy.display && !plugins.Find(config.economy.plugin))
            {
                PrintError("Please setup Economy plugin!");
                Interface.Oxide.UnloadPlugin(Title);
                return;
            }
            else
            {
                instance = this;
                MenuUsers = new List<BasePlayer>();
                for (int i = 0; i < config.BTN.btns.Count; i++)
                    ImageLibrary.Call("AddImage", config.BTN.btns[i].URL, config.BTN.btns[i].URL);
                if (config.logosettings.display) ImageLibrary.Call("AddImage", config.logosettings.icon, config.logosettings.icon);
                if (config.UsersIcon.display) ImageLibrary.Call("AddImage", config.UsersIcon.icon, config.UsersIcon.icon);
                if (config.TimeIcon.display) ImageLibrary.Call("AddImage", config.TimeIcon.icon, config.TimeIcon.icon);
                if (config.SleepersIcon.display) ImageLibrary.Call("AddImage", config.SleepersIcon.icon, config.SleepersIcon.icon);
                if (config.CoordsPanel.display) ImageLibrary.Call("AddImage", config.CoordsPanel.icon, config.CoordsPanel.icon);
                if (config.eventsettings.EventAirdrop.display) ImageLibrary.Call("AddImage", config.eventsettings.EventAirdrop.icon, config.eventsettings.EventAirdrop.icon);
                if (config.eventsettings.EventBradley.display) ImageLibrary.Call("AddImage", config.eventsettings.EventBradley.icon, config.eventsettings.EventBradley.icon);
                if (config.eventsettings.EventCargoship.display) ImageLibrary.Call("AddImage", config.eventsettings.EventCargoship.icon, config.eventsettings.EventCargoship.icon);
                if (config.eventsettings.EventHelicopter.display) ImageLibrary.Call("AddImage", config.eventsettings.EventHelicopter.icon, config.eventsettings.EventHelicopter.icon);
                if (config.eventsettings.EventCH47.display) ImageLibrary.Call("AddImage", config.eventsettings.EventCH47.icon, config.eventsettings.EventCH47.icon);
                if (config.economy.display) ImageLibrary.Call("AddImage", config.economy.icon, config.economy.icon);

                for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
                    InitializeUI(BasePlayer.activePlayerList[i]);

                timer.In(3, () =>
                {
                    foreach (var entity in BaseNetworkable.serverEntities)
                    {
                        if (entity is CargoPlane && config.eventsettings.EventAirdrop.display)
                            BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "air", config.eventsettings.EventAirdrop.OnColor));
                        if (entity is BradleyAPC && config.eventsettings.EventBradley.display)
                            BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "bradley", config.eventsettings.EventBradley.OnColor));
                        if (entity is BaseHelicopter && config.eventsettings.EventHelicopter.display)
                            BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "heli", config.eventsettings.EventHelicopter.OnColor));
                        if (entity is CargoShip && config.eventsettings.EventCargoship.display)
                            BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "cargo", config.eventsettings.EventCargoship.OnColor));
                        if (entity is CH47Helicopter && config.eventsettings.EventCH47.display)
                            BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "ch47", config.eventsettings.EventCH47.OnColor));
                    }
                });

                timer.Every(5, () =>
                {
                    for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
                    {
                        var player = BasePlayer.activePlayerList[i];
                        if (player.IsNpc || player.IsSleeping() || player.IsReceivingSnapshot) return;
                        if (config.TimeIcon.display) RefreshUI(BasePlayer.activePlayerList[i], "time");
                        if (config.economy.display) RefreshUI(BasePlayer.activePlayerList[i], "balance");
                        if (config.CoordsPanel.display) RefreshUI(BasePlayer.activePlayerList[i], "coords");
                    }
                });
            }
        }

        void OnEntitySpawned(BaseNetworkable entity)
        {
            if (entity == null || entity.net == null) return;
            try
            {
                if (entity is CargoPlane && config.eventsettings.EventAirdrop.display)
                    BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "air", config.eventsettings.EventAirdrop.OnColor));
                if (entity is BradleyAPC && config.eventsettings.EventBradley.display)
                    BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "bradley", config.eventsettings.EventBradley.OnColor));
                if (entity is BaseHelicopter && config.eventsettings.EventHelicopter.display)
                    BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "heli", config.eventsettings.EventHelicopter.OnColor));
                if (entity is CargoShip && config.eventsettings.EventCargoship.display)
                    BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "cargo", config.eventsettings.EventCargoship.OnColor));
                if (entity is CH47Helicopter && config.eventsettings.EventCH47.display)
                    BasePlayer.activePlayerList.ForEach(player => RefreshUI(player, "ch47", config.eventsettings.EventCH47.OnColor));
            }
            catch (NullReferenceException)
            {

            }
        }

        void OnPlayerInit(BasePlayer player)
        {
            if (player.IsReceivingSnapshot || player.IsSleeping())
            {
                timer.In(1, () =>
                {
                    OnPlayerInit(player);
                });
                return;
            }

            InitializeUI(player);
            for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
            {
                if (config.UsersIcon.display) RefreshUI(BasePlayer.activePlayerList[i], "online");
                if (config.SleepersIcon.display) RefreshUI(BasePlayer.activePlayerList[i], "sleepers");
            }

            foreach (var entity in BaseNetworkable.serverEntities)
            {
                if (entity is CargoPlane && config.eventsettings.EventAirdrop.display)
                    RefreshUI(player, "air", config.eventsettings.EventAirdrop.OnColor);
                if (entity is BradleyAPC && config.eventsettings.EventBradley.display)
                    RefreshUI(player, "bradley", config.eventsettings.EventBradley.OnColor);
                if (entity is BaseHelicopter && config.eventsettings.EventHelicopter.display)
                    RefreshUI(player, "heli", config.eventsettings.EventHelicopter.OnColor);
                if (entity is CargoShip && config.eventsettings.EventCargoship.display)
                    RefreshUI(player, "cargo", config.eventsettings.EventCargoship.OnColor);
                if (entity is CH47Helicopter && config.eventsettings.EventCH47.display)
                    RefreshUI(player, "ch47", config.eventsettings.EventCH47.OnColor);
            }
        }
        void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            timer.In(1, () =>
            {
                for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
                {
                    if (config.UsersIcon.display) RefreshUI(BasePlayer.activePlayerList[i], "online");
                    if (config.SleepersIcon.display) RefreshUI(BasePlayer.activePlayerList[i], "sleepers");
                }
            });
        }
        private void Unload()
        {
            BasePlayer.activePlayerList.ForEach(p => CuiHelper.DestroyUi(p, Layer));
        }
        #endregion

        #region Commands
        [ChatCommand("menu")]
        private void CmdChatMenu(BasePlayer player)
        {
            if (MenuUsers.Contains(player))
            {
                CuiHelper.DestroyUi(player, Layer + ".Menu.Opened");
                MenuUsers.Remove(player);
            }
            else
            {
                ButtonsUI(player);
                MenuUsers.Add(player);
            }
            return;
        }
        #endregion

        #region Interface
        private void InitializeUI(BasePlayer player)
        {
            CuiElementContainer container = new CuiElementContainer();

            container.Add(new CuiPanel
            {
                RectTransform = { AnchorMin = "0 1", AnchorMax = "0 1" },
                Image = { Color = "0 0 0 0" }
            }, config.LayerType, Layer);
            if (config.logosettings.display)
            {
                container.Add(new CuiButton
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.logosettings.OffMin, OffsetMax = config.logosettings.OffMax },
                    Button = { Color = HexToCuiColor(config.LabelColor), Command = config.logosettings.LogoCmd },
                    Text = { Text = "" }
                }, Layer, Layer + ".Logo");
                UI.LoadImage(ref container, ".Logo.Icon", ".Logo", oMin: "2 2", oMax: "-2 -2", image: config.logosettings.icon);
            }
            if (config.TimeIcon.display)
            {
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.TimeIcon.OffMin, OffsetMax = config.TimeIcon.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".Time.Label");
                UI.LoadImage(ref container, ".Time.Icon", ".Time.Label", aMin: "0 0", aMax: "0 1", oMin: "1 1", oMax: "13 -1", image: config.TimeIcon.icon);
            }
            if (config.SleepersIcon.display)
            {
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.SleepersIcon.OffMin, OffsetMax = config.SleepersIcon.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".Sleepers.Label");
                UI.LoadImage(ref container, ".Sleepers.Icon", ".Sleepers.Label", aMin: "0 0", aMax: "0 1", oMin: "1 1", oMax: "13 -1", image: config.SleepersIcon.icon);
            }
            if (config.CoordsPanel.display)
            {
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.CoordsPanel.OffMin, OffsetMax = config.CoordsPanel.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".Coords.Label");
                UI.LoadImage(ref container, ".Coords.Icon", ".Coords.Label", aMin: "0 0", aMax: "0 1", oMin: "1 1", oMax: "13 -1", image: config.CoordsPanel.icon);
            }
            if (config.ServerName.display)
            {
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.ServerName.OffMin, OffsetMax = config.ServerName.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".ServerName");
                container.Add(new CuiLabel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1" },
                    Text = { FadeIn = 1f, Color = "1 1 1 1", Text = config.ServerName.name, Align = TextAnchor.MiddleCenter, Font = "robotocondensed-bold.ttf", FontSize = 11 }
                }, Layer + ".ServerName");
            }
            if (config.eventsettings.EventHelicopter.display)
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.eventsettings.EventHelicopter.OffMin, OffsetMax = config.eventsettings.EventHelicopter.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".Helicopter");
            if (config.eventsettings.EventAirdrop.display)
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.eventsettings.EventAirdrop.OffMin, OffsetMax = config.eventsettings.EventAirdrop.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".Air");
            if (config.eventsettings.EventCargoship.display)
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.eventsettings.EventCargoship.OffMin, OffsetMax = config.eventsettings.EventCargoship.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".Cargo");
            if (config.eventsettings.EventBradley.display)
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.eventsettings.EventBradley.OffMin, OffsetMax = config.eventsettings.EventBradley.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".Bradley");
            if (config.eventsettings.EventCH47.display)
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.eventsettings.EventCH47.OffMin, OffsetMax = config.eventsettings.EventCH47.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".CH47");
            if (config.UsersIcon.display)
            {
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.UsersIcon.OffMin, OffsetMax = config.UsersIcon.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".Online.Label");
                UI.LoadImage(ref container, ".Online.Icon", ".Online.Label", aMin: "0 0", aMax: "0 1", oMin: "1 1", oMax: "13 -1", image: config.UsersIcon.icon);
            }
            if (config.economy.display)
            {
                container.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.economy.OffMin, OffsetMax = config.economy.OffMax },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer, Layer + ".Balance.Label");
                UI.LoadImage(ref container, ".Balance.Icon", ".Balance.Label", aMin: "0 0", aMax: "0 1", oMin: "1 1", oMax: "14 -1", image: config.economy.icon);
            }
            if (config.menuCfg.display)
                container.Add(new CuiButton
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.menuCfg.OffMin, OffsetMax = config.menuCfg.OffMax },
                    Button = { Color = HexToCuiColor(config.LabelColor), Command = "chat.say /menu" },
                    Text = { Color = "1 1 1 1", Text = config.menuCfg.Title, Align = TextAnchor.MiddleCenter, Font = "robotocondensed-bold.ttf", FontSize = 9 }
                }, Layer, Layer + ".Menu");

            CuiHelper.DestroyUi(player, Layer);
            CuiHelper.AddUi(player, container);

            RefreshUI(player, "all");
        }
        void OnEntityKill(BaseNetworkable entity)
        {
            if (entity == null) return;
            try
            {
                var tag = entity is CargoPlane && config.eventsettings.EventAirdrop.display ? "air" : entity is BradleyAPC && config.eventsettings.EventBradley.display ? "bradley" : entity is BaseHelicopter && config.eventsettings.EventHelicopter.display ? "heli" : entity is CargoShip && config.eventsettings.EventCargoship.display ? "cargo" : entity is CH47Helicopter && config.eventsettings.EventCH47.display ? "ch47" : "";
                if (tag == string.Empty) return;
                var color = entity is CargoPlane ? config.eventsettings.EventAirdrop.OffColor : entity is BradleyAPC ? config.eventsettings.EventBradley.OffColor : entity is BaseHelicopter ? config.eventsettings.EventHelicopter.OffColor : entity is CargoShip ? config.eventsettings.EventCargoship.OffColor : entity is CH47Helicopter ? config.eventsettings.EventCH47.OffColor : "";
                timer.In(1, () => BasePlayer.activePlayerList.ForEach(p => RefreshUI(p, tag, color)));
            }
            catch (NullReferenceException)
            {

            }
        }
        private void ButtonsUI(BasePlayer player)
        {
            CuiElementContainer ButtonsContainer = new CuiElementContainer();
            float ySwitch = config.BTN.IndentStart;

            ButtonsContainer.Add(new CuiPanel
            {
                RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1" },
                Image = { Color = "0 0 0 0" }
            }, Layer, Layer + ".Menu.Opened");

            ButtonsContainer.Add(new CuiPanel
            {
                RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = config.BTN.closeMenuBTN.OffMin, OffsetMax = config.BTN.closeMenuBTN.OffMax },
                Image = { Color = HexToCuiColor(config.CloseColor) }
            }, Layer + ".Menu.Opened", Layer + ".Menu.Opened.Close");
            ButtonsContainer.Add(new CuiButton
            {
                RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1" },
                Button = { Color = "0 0 0 0", Command = "chat.say /menu" },
                Text = { Text = "X", Align = TextAnchor.MiddleCenter, Font = "robotocondensed-bold.ttf", FontSize = 9 }
            }, Layer + ".Menu.Opened.Close");

            for (int i = 0; i < config.BTN.btns.Count; i++)
            {
                var button = config.BTN.btns[i];
                ButtonsContainer.Add(new CuiPanel
                {
                    RectTransform = { AnchorMin = "0 1", AnchorMax = "0 1", OffsetMin = $"5 {ySwitch - config.BTN.btnHeight}", OffsetMax = $"{config.BTN.btnLenght + 5} {ySwitch}" },
                    Image = { Color = HexToCuiColor(config.LabelColor) }
                }, Layer + ".Menu.Opened", Layer + $".Menu.Opened.{button.URL}");
                ySwitch -= (config.BTN.btnHeight + config.BTN.btnMargin);
                UI.LoadImage(ref ButtonsContainer, $".Menu.Opened.{button.URL}.Img", $".Menu.Opened.{button.URL}", aMin: "0 0", aMax: "0 1", oMin: "3 1", oMax: "21 -1", image: button.URL);
                ButtonsContainer.Add(new CuiLabel
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMin = $"{config.BTN.btnHeight + 2} 0" },
                    Text = { Text = $"<b>{button.Title}</b>", Align = TextAnchor.MiddleCenter, Font = "robotocondensed-bold.ttf", FontSize = 12 }
                }, Layer + $".Menu.Opened.{button.URL}");

                ButtonsContainer.Add(new CuiButton
                {
                    RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1" },
                    Button = { Color = "0 0 0 0", Command = $"{button.CMD}" },
                    Text = { Text = "" }
                }, Layer + $".Menu.Opened.{button.URL}");
            }
            CuiHelper.DestroyUi(player, Layer + ".Menu.Opened");
            CuiHelper.AddUi(player, ButtonsContainer);
        }
        private void RefreshUI(BasePlayer player, string Type, string color = "#FFFFFF")
        {
            CuiElementContainer RefreshContainer = new CuiElementContainer();
            switch (Type)
            {
                case "coords":
                    UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Coords", parent: ".Coords.Label", text: $"X: {player.transform.position.x.ToString("0")} Z: {player.transform.position.z.ToString("0")}");
                    break;
                case "online":
                    UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Online", parent: ".Online.Label", text: $"{BasePlayer.activePlayerList.Count}");
                    break;
                case "balance":
                    UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Balance", parent: ".Balance.Label", oMin: "14 1", text: plugins.Find(config.economy.plugin).Call(config.economy.hook, player.userID).ToString(), fontsize: 12);
                    break;
                case "time":
                    UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Time", parent: ".Time.Label", text: TOD_Sky.Instance.Cycle.DateTime.ToString("HH:mm"));
                    break;
                case "sleepers":
                    UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Sleepers", parent: ".Sleepers.Label", text: $"{BasePlayer.sleepingPlayerList.Count}");
                    break;
                case "heli":
                    CuiHelper.DestroyUi(player, Layer + ".Events.Helicopter");
                    UI.LoadImage(ref RefreshContainer, ".Events.Helicopter", ".Helicopter", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventHelicopter.icon);
                    break;
                case "air":
                    CuiHelper.DestroyUi(player, Layer + ".Events.Air");
                    UI.LoadImage(ref RefreshContainer, ".Events.Air", ".Air", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventAirdrop.icon);
                    break;
                case "cargo":
                    CuiHelper.DestroyUi(player, Layer + ".Events.Cargo");
                    UI.LoadImage(ref RefreshContainer, ".Events.Cargo", ".Cargo", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventCargoship.icon);
                    break;
                case "bradley":
                    CuiHelper.DestroyUi(player, Layer + ".Events.Bradley");
                    UI.LoadImage(ref RefreshContainer, ".Events.Bradley", ".Bradley", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventBradley.icon);
                    break;
                case "ch47":
                    CuiHelper.DestroyUi(player, Layer + ".Events.CH47");
                    UI.LoadImage(ref RefreshContainer, ".Events.CH47", ".CH47", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventCH47.icon);
                    break;
                case "all":
                    CuiHelper.DestroyUi(player, Layer + ".Events.Helicopter");
                    CuiHelper.DestroyUi(player, Layer + ".Events.Air");
                    CuiHelper.DestroyUi(player, Layer + ".Events.Cargo");
                    CuiHelper.DestroyUi(player, Layer + ".Events.Bradley");
                    CuiHelper.DestroyUi(player, Layer + ".Events.CH47");
                    if (config.CoordsPanel.display) UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Coords", parent: ".Coords.Label", text: $"X: {player.transform.position.x.ToString("0")} Z: {player.transform.position.z.ToString("0")}");
                    if (config.UsersIcon.display) UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Online", parent: ".Online.Label", text: $"{BasePlayer.activePlayerList.Count}");
                    if (config.economy.display) UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Balance", parent: ".Balance.Label", oMin: "14 1", text: plugins.Find(config.economy.plugin).Call(config.economy.hook, player.userID).ToString(), fontsize: 12);
                    if (config.TimeIcon.display) UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Time", parent: ".Time.Label", text: TOD_Sky.Instance.Cycle.DateTime.ToString("HH:mm"));
                    if (config.SleepersIcon.display) UI.CreateLabel(ref RefreshContainer, player, name: ".Refresh.Sleepers", parent: ".Sleepers.Label", text: $"{BasePlayer.sleepingPlayerList.Count}");
                    if (config.eventsettings.EventHelicopter.display) UI.LoadImage(ref RefreshContainer, ".Events.Helicopter", ".Helicopter", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventHelicopter.icon);
                    if (config.eventsettings.EventAirdrop.display) UI.LoadImage(ref RefreshContainer, ".Events.Air", ".Air", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventAirdrop.icon);
                    if (config.eventsettings.EventCargoship.display) UI.LoadImage(ref RefreshContainer, ".Events.Cargo", ".Cargo", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventCargoship.icon);
                    if (config.eventsettings.EventBradley.display) UI.LoadImage(ref RefreshContainer, ".Events.Bradley", ".Bradley", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventBradley.icon);
                    if (config.eventsettings.EventCH47.display) UI.LoadImage(ref RefreshContainer, ".Events.CH47", ".CH47", oMin: "1 1", oMax: "-1 -1", color: HexToCuiColor(color), image: config.eventsettings.EventCH47.icon);
                    break;
            }
            CuiHelper.AddUi(player, RefreshContainer);
        }
        #endregion

        #region Helpers
        class UI
        {
            static public void LoadImage(ref CuiElementContainer container, string name, string parent, string aMin = "0 0", string aMax = "1 1", string oMin = "13 1", string oMax = "0 -1", string color = "1 1 1 1", string image = "")
            {
                container.Add(new CuiElement
                {
                    Name = Layer + name,
                    Parent = Layer + parent,
                    Components =
                    {
                        new CuiRawImageComponent { Png = (string) instance.ImageLibrary.Call("GetImage", $"{image}"), Color = color },
                        new CuiRectTransformComponent { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax },
                    }
                });
            }
            static public void CreateLabel(ref CuiElementContainer container, BasePlayer player, string name, string parent, string aMin = "0 0", string aMax = "1 1", string oMin = "13 1", string oMax = "0 -1", string color = "1 1 1 1", string text = "", TextAnchor align = TextAnchor.MiddleCenter, int fontsize = 11, string font = "robotocondensed-bold.ttf")
            {
                CuiHelper.DestroyUi(player, Layer + name);
                container.Add(new CuiLabel
                {
                    RectTransform = { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax },
                    Text = { Text = text, Color = color, Align = align, Font = font, FontSize = fontsize }
                }, Layer + parent, Layer + name);
            }
        }
        private static string HexToCuiColor(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                hex = "#FFFFFFFF";
            }

            var str = hex.Trim('#');

            if (str.Length == 6)
                str += "FF";

            if (str.Length != 8)
            {
                throw new Exception(hex);
                throw new InvalidOperationException(" Cannot convert a wrong format.");
            }

            var r = byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber);
            var a = byte.Parse(str.Substring(6, 2), NumberStyles.HexNumber);

            Color color = new Color32(r, g, b, a);

            return $"{color.r:F2} {color.g:F2} {color.b:F2} {color.a:F2}";
        }
        #endregion
    }
}