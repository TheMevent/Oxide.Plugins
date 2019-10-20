using Newtonsoft.Json;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("ItemBreaking", "Mevent", "0.0.2")]
    [Description("Неломаемые предметы")]
    class ItemBreaking : RustPlugin
    {
        void Loaded() => RegisterPermissions();
        private void RegisterPermissions()
        {
            permission.RegisterPermission("itembreaking.all.items", this);
            permission.RegisterPermission("itembreaking.attire", this);
            permission.RegisterPermission("itembreaking.weapon", this);
            permission.RegisterPermission("itembreaking.tool", this);
            permission.RegisterPermission("itembreaking.exception", this);
        }
        private bool HasPerm(BasePlayer player, string perm)
        {
            if (permission.UserHasPermission(player.UserIDString, perm)) return true;
            return false;
        }
        void OnLoseCondition(Item item, ref float amount)
        {
            if (item != null)
            {
                BasePlayer player;
                if (item.GetOwnerPlayer() == null)
                {
                    if (item?.info == null) return;
                    if (!item.info.shortname.Contains("mod")) return;
                    player = item?.GetRootContainer()?.GetOwnerPlayer();
                    if (player == null)
                        return;
                }
                else player = item.GetOwnerPlayer();
                if (player != null)
                {
                    var def = ItemManager.FindItemDefinition(item.info.itemid);
                    if ((HasPerm(player, "itembreaking.all.items") && !configData.ExceptionList.ContainsKey(def.shortname))
                        || (def.category == ItemCategory.Weapon && !configData.ExceptionList.ContainsKey(def.shortname) && configData.useWeapons && HasPerm(player, "itembreaking.weapon"))
                        || (def.category == ItemCategory.Attire && !configData.ExceptionList.ContainsKey(def.shortname) && configData.useAttire && HasPerm(player, "itembreaking.attire"))
                        || (def.category == ItemCategory.Tool && !configData.ExceptionList.ContainsKey(def.shortname) && configData.useTools && HasPerm(player, "itembreaking.tool")))
                            item.RepairCondition(amount);
                    if (configData.ExceptionList.ContainsValue(item.skin) && configData.ExceptionList.ContainsKey(def.shortname) && HasPerm(player, "itembreaking.exception"))
                        if (item.hasCondition)
                            amount *= 1 / configData.ExceptionCondition;
                }
            }
            return;
        }

        #region Config        
        private static ConfigData configData;
        private class ConfigData
        {
            [JsonProperty(PropertyName = "Отключить износ на предметах из категории ОРУЖИЕ?")]
            public bool useWeapons;

            [JsonProperty(PropertyName = "Отключить износ на предметах из категории ИНСТРУМЕНТЫ?")]
            public bool useTools;

            [JsonProperty(PropertyName = "Отключить износ на предметах из категории БРОНЯ?")]
            public bool useAttire;

            [JsonProperty(PropertyName = "Во сколько раз уменьшать износ предметов из списка исключений?")]
            public float ExceptionCondition;

            [JsonProperty(PropertyName = "Список исключений (SHORTNAME: SKINID)")]
            public Dictionary<string, ulong> ExceptionList;
        }

        private ConfigData GetDefaultConfig()
        {
            return new ConfigData
            {
                useTools = true,
                useAttire = true,
                useWeapons = true,
                ExceptionCondition = 3f,
                ExceptionList = new Dictionary<string, ulong>
                {
                    {"chainsaw", 0},
                    {"jackhammer", 0},
                }
            };
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                configData = Config.ReadObject<ConfigData>();
            }
            catch
            {
                LoadDefaultConfig();
            }

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Файл конфигурации поврежден(или не существует), создаю новый!");
            configData = GetDefaultConfig();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(configData);
        }
        #endregion
    }
}