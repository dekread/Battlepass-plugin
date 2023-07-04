using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace battlepassPlugin
{
    public class Config : IRocketPluginConfiguration
    {
        public bool background = false;
        public string goldPassPermission = "goldPass";
        public string collectedRewardMessage = "Succesfully collected reward!";
        public List<playeri> pi = new List<playeri>();
        public List<passFree> passFreei = new List<passFree>();
        public List<passPaid> passPaidi = new List<passPaid>();
        public List<Mission> Missioni = new List<Mission>();


        public void LoadDefaults()
        {

        }
        public void SaveData(UnturnedPlayer player)
        {
            //ArenaPluginV1.Instance.Configuration.Save();
        }

        internal void SaveData()
        {

            battlepassPlugin.Instance.Configuration.Save();
        }

        internal void LoadData()
        {

            battlepassPlugin.Instance.Configuration.Save();
        }


        public void AddRegMission(int number, bool isDaily, bool isWeek)
        {
            if (Missioni.Exists(x => x.number == number)) return;
            Missioni.Add(new Mission { number = number, isDailyMission = isDaily, isWeekMission = isWeek });
            battlepassPlugin.Instance.Configuration.Save();
        }


        public void AddRegFreePass(int number, string imageURL)
        {
            if (passFreei.Exists(x => x.number == number)) return;
            passFreei.Add(new passFree { number = number, iconURL = imageURL });
            battlepassPlugin.Instance.Configuration.Save();
        }
        public void AddRegPaidPass(int number, string imageURL)
        {
            if (passPaidi.Exists(x => x.number == number)) return;
            passPaidi.Add(new passPaid { number = number, iconURL = imageURL });
            battlepassPlugin.Instance.Configuration.Save();
        }


        public class passFree
        {
            public int number = 0;
            public ushort[] itemsReward = { 363, 363, 363 };

            public uint xpToGet = 100;
            public string iconURL;
            //public int choppedTree = 10;
            //public int playerKilledCount = 5;
            //public int zombieKilledCount = 20;

            //public int NORMAL = 10;
            //public int MEGA = 0;
            //public int CRAWLER = 0;
            //public int SPRINTER = 0;
            //public int FLANKER_FRIENDLY = 0;
            //public int FLANKER_STALK = 0;
            //public int BURNER = 0;
            //public int ACID = 0;
            //public int BOSS_ELECTRIC = 0;
            //public int BOSS_WIND = 0;
            //public int BOSS_FIRE = 0;
            //public int BOSS_ALL = 0;
            //public int BOSS_MAGMA = 0;
            //public int SPIRIT = 0;
            //public int BOSS_SPIRIT = 0;
            //public int BOSS_NUCLEAR = 0;
            //public int DL_RED_VOLATILE = 0;
            //public int DL_BLUE_VOLATILE = 0;
            //public int BOSS_ELVER_STOMPER = 0;
            //public int BOSS_KUWAIT = 0;
        }
        public class passPaid
        {
            public int number = 0;
            public ushort[] itemsReward = { 363, 363, 363 };

            public uint xpToGet = 100;
            public string iconURL = "https://art.pixilart.com/a58fcc1787d9144.png";
            //public int choppedTree = 10;
            //public int playerKilledCount = 5;
            //public int zombieKilledCount = 20;

            //public int NORMAL = 10;
            //public int MEGA = 0;
            //public int CRAWLER = 0;
            //public int SPRINTER = 0;
            //public int FLANKER_FRIENDLY = 0;
            //public int FLANKER_STALK = 0;
            //public int BURNER = 0;
            //public int ACID = 0;
            //public int BOSS_ELECTRIC = 0;
            //public int BOSS_WIND = 0;
            //public int BOSS_FIRE = 0;
            //public int BOSS_ALL = 0;
            //public int BOSS_MAGMA = 0;
            //public int SPIRIT = 0;
            //public int BOSS_SPIRIT = 0;
            //public int BOSS_NUCLEAR = 0;
            //public int DL_RED_VOLATILE = 0;
            //public int DL_BLUE_VOLATILE = 0;
            //public int BOSS_ELVER_STOMPER = 0;
            //public int BOSS_KUWAIT = 0;
        }

        public class Mission
        {
            public int number = 0;
            public bool isDailyMission = false;
            public bool isWeekMission = false;
            public uint xpGiveCompete = 100;
            public float timerToResetMissionSec = 604800;

            public List<CSteamID> whoCompleted = new List<CSteamID>();
            public List<playerinfo> playeri = new List<playerinfo>();

            public string text = "Kill 40 zombies and 20 normal zombies";

            public int choppedTree = 0;
            public int playerKilledCount = 0;
            public int zombieKilledCount = 40;

            public int NORMAL = 20;
            public int MEGA = 0;
            public int CRAWLER = 0;
            public int SPRINTER = 0;
            public int FLANKER_FRIENDLY = 0;
            public int FLANKER_STALK = 0;
            public int BURNER = 0;
            public int ACID = 0;
            public int BOSS_ELECTRIC = 0;
            public int BOSS_WIND = 0;
            public int BOSS_FIRE = 0;
            public int BOSS_ALL = 0;
            public int BOSS_MAGMA = 0;
            public int SPIRIT = 0;
            public int BOSS_SPIRIT = 0;
            public int BOSS_NUCLEAR = 0;
            public int DL_RED_VOLATILE = 0;
            public int DL_BLUE_VOLATILE = 0;
            public int BOSS_ELVER_STOMPER = 0;
            public int BOSS_KUWAIT = 0;
        }


        public class playeri
        {
            public CSteamID cSteamID;

            public uint xpPass = 100;
            public List<int> collectedRewardsFreeIndex = new List<int>();
            public List<int> collectedRewardsPaidIndex = new List<int>();
        }

        public class playerinfo
        {
            public CSteamID cSteamID;


            public int choppedTree;
            public int playerKilledCount;
            public int zombieKilledCount;

            public int NORMAL;
            public int MEGA;
            public int CRAWLER;
            public int SPRINTER;
            public int FLANKER_FRIENDLY;
            public int FLANKER_STALK;
            public int BURNER;
            public int ACID;
            public int BOSS_ELECTRIC;
            public int BOSS_WIND;
            public int BOSS_FIRE;
            public int BOSS_ALL;
            public int BOSS_MAGMA;
            public int SPIRIT;
            public int BOSS_SPIRIT;
            public int BOSS_NUCLEAR;
            public int DL_RED_VOLATILE;
            public int DL_BLUE_VOLATILE;
            public int BOSS_ELVER_STOMPER;
            public int BOSS_KUWAIT;
        }
    }
}
