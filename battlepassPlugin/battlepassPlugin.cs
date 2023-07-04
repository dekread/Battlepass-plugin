using Microsoft.SqlServer.Server;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.UI;
using static battlepassPlugin.Config;
using static Rocket.Unturned.Events.UnturnedEvents;
using static Rocket.Unturned.Events.UnturnedPlayerEvents;
using static UnityEngine.UI.GridLayoutGroup;

//using fr34kyn01535.Uconomy;

namespace battlepassPlugin
{
    public class battlepassPlugin : RocketPlugin<Config>
    {
        public static battlepassPlugin Instance; // Ссылка на наш плагин
        //public static Uconomy uconomy;

        protected override void Load()
        {
            Instance = this;
            base.Load();
            DamageTool.damageZombieRequested += damageZombieRequested;
            EffectManager.onEffectButtonClicked += onButton;
            U.Events.OnPlayerConnected += onPlayerConnected;
            Instance.Configuration.Instance.LoadData();

            WorkshopDownloadConfig.getOrLoad().File_IDs.Add(2996880686); // can bug ui
            //https://steamcommunity.com/sharedfiles/filedetails/?id=2996880686
            StartCoroutine(missionResetCheck(300));
            if (Instance.Configuration.Instance.passFreei.Count < 1)
            {
                Instance.Configuration.Instance.AddRegFreePass(0, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegFreePass(1, "https://art.pixilart.com/a58fcc1787d9144.png");
                Instance.Configuration.Instance.AddRegFreePass(2, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegFreePass(3, "https://art.pixilart.com/a58fcc1787d9144.png");
                Instance.Configuration.Instance.AddRegFreePass(4, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegFreePass(5, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegFreePass(6, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegFreePass(7, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegFreePass(8, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegFreePass(9, "https://i.imgur.com/2fga1Qz.png");
            }
            if (Instance.Configuration.Instance.passPaidi.Count < 1)
            {
                Instance.Configuration.Instance.AddRegPaidPass(0, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegPaidPass(1, "https://art.pixilart.com/a58fcc1787d9144.png");
                Instance.Configuration.Instance.AddRegPaidPass(2, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegPaidPass(3, "https://art.pixilart.com/a58fcc1787d9144.png");
                Instance.Configuration.Instance.AddRegPaidPass(4, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegPaidPass(5, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegPaidPass(6, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegPaidPass(7, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegPaidPass(8, "https://i.imgur.com/2fga1Qz.png");
                Instance.Configuration.Instance.AddRegPaidPass(9, "https://i.imgur.com/2fga1Qz.png");
            }
            if (Instance.Configuration.Instance.Missioni.Count < 1)
            {
                Instance.Configuration.Instance.AddRegMission(0, false, true);
                Instance.Configuration.Instance.AddRegMission(1, false, true);
                Instance.Configuration.Instance.AddRegMission(2, false, true);
                Instance.Configuration.Instance.AddRegMission(3, true, false);
                Instance.Configuration.Instance.AddRegMission(4, true, false);
                Instance.Configuration.Instance.AddRegMission(5, true, false);
            }
        }

        protected override void Unload()
        {
            Instance = null;
            base.Unload();
            DamageTool.damageZombieRequested -= damageZombieRequested;
            EffectManager.onEffectButtonClicked -= onButton;
            U.Events.OnPlayerConnected -= onPlayerConnected;
            Instance.Configuration.Instance.SaveData();
        }
        IEnumerator missionResetCheck(float sec)
        {
            foreach (var i in Instance.Configuration.Instance.Missioni)
            {
                i.timerToResetMissionSec -= sec;
                if (i.timerToResetMissionSec < 0)
                {
                    i.playeri.Clear();
                    i.whoCompleted.Clear();
                }
            }
            Instance.Configuration.Save();
            yield return new WaitForSeconds(sec);
            StartCoroutine(missionResetCheck(sec));
        }
        private void onButton(Player player, string buttonName)
        {
            UnturnedPlayer uplayer = UnturnedPlayer.FromPlayer(player);

            if (buttonName == "ExitButton")
            {
                RemoveUI(uplayer);
                return;
            }
            if (buttonName == "ToRewards")
            {
                UItoRewards(uplayer);
                return;
            }
            if (buttonName == "ToMissions")
            {
                UItoMissions(uplayer);
                return;
            }
            if (buttonName == "UpRow")
            {
                if (playerRow.ContainsKey(uplayer))
                {
                    if (playerRow[uplayer] > 1)
                    {
                        playerRow[uplayer]--;
                    }
                }
                SetrowRewards(uplayer);
                return;
            }
            if (buttonName == "DownRow")
            {
                if (playerRow.ContainsKey(uplayer))
                {
                    //int numberStart = (5 * playerRow[uplayer]) - 5;
                    if (Instance.Configuration.Instance.passFreei.Count / 5 > playerRow[uplayer])
                    {
                        playerRow[uplayer]++;
                    }
                }
                SetrowRewards(uplayer);
                return;
            }

            if (buttonName == "UpRowMission")
            {
                if (playerRowMission.ContainsKey(uplayer))
                {
                    if (playerRowMission[uplayer] > 1)
                    {
                        playerRowMission[uplayer]--;
                    }
                }
                else
                {
                    playerRowMission.Add(uplayer, 1);
                }
                UItoMissions(uplayer);
                return;
            }
            if (buttonName == "DownRowMission")
            {
                if (playerRowMission.ContainsKey(uplayer))
                {
                    if ((Instance.Configuration.Instance.Missioni.Count - 1) / 3 > playerRowMission[uplayer])
                    {
                        playerRowMission[uplayer]--;
                    }
                }
                else
                {
                    playerRowMission.Add(uplayer, 1);
                }
                UItoMissions(uplayer);
                return;
            }

            for (int i = 0; i < 100; i++)
            {
                if (buttonName == "getFreePassItem" + i)
                {
                    collectReward(true, 5 * playerRow[uplayer] - 5 + i - 1, uplayer);
                    //collectReward(true, (5 * playerRow[uplayer]) - playerRow[uplayer] - 5 + i, uplayer);
                    break;
                }
                if (buttonName == "getPaidPassItem" + i)
                {
                    collectReward(false, 5 * playerRow[uplayer] - 5 + i - 1, uplayer);
                    //collectReward(false, (5 * playerRow[uplayer]) - playerRow[uplayer] - 5 + i, uplayer);
                    break;
                }
            }
        }

        private void damageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            if (parameters.zombie.GetHealth() <= parameters.damage * parameters.times)
            {
                UnturnedPlayer whoDamageZombie = null;
                if (parameters.instigator is Player || parameters.instigator is CSteamID)
                {
                    whoDamageZombie = UnturnedPlayer.FromPlayer((Player)parameters.instigator);
                    if (whoDamageZombie == null) UnturnedPlayer.FromCSteamID((CSteamID)parameters.instigator);
                }
                if (whoDamageZombie != null)
                {
                    foreach (var k in Instance.Configuration.Instance.Missioni)
                    {
                        foreach (var i in k.playeri)
                        {
                            if (i.cSteamID == whoDamageZombie.CSteamID)
                            {
                                if(k.zombieKilledCount > 0) i.zombieKilledCount++;
                                if (k.NORMAL > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.NORMAL) i.NORMAL++;
                                }
                                if(k.MEGA > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.MEGA) i.MEGA++;
                                }
                                if (k.CRAWLER > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.CRAWLER) i.CRAWLER++;
                                }
                                if (k.SPRINTER > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.SPRINTER) i.SPRINTER++;
                                }
                                if (k.FLANKER_FRIENDLY > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.FLANKER_FRIENDLY) i.FLANKER_FRIENDLY++;
                                }
                                if (k.FLANKER_STALK > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.FLANKER_STALK) i.FLANKER_STALK++;
                                }
                                if (k.BURNER > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BURNER) i.BURNER++;
                                }
                                if (k.ACID > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.ACID) i.ACID++;
                                }
                                if (k.BOSS_ELECTRIC > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BOSS_ELECTRIC) i.BOSS_ELECTRIC++;
                                }
                                if (k.BOSS_WIND > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BOSS_WIND) i.BOSS_WIND++;
                                }
                                if (k.BOSS_FIRE > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BOSS_FIRE) i.BOSS_FIRE++;
                                }
                                if (k.BOSS_ALL > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BOSS_ALL) i.BOSS_ALL++;
                                }
                                if (k.BOSS_MAGMA > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BOSS_MAGMA) i.BOSS_MAGMA++;
                                }
                                if (k.BOSS_SPIRIT > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BOSS_SPIRIT) i.BOSS_SPIRIT++;
                                }
                                if (k.BOSS_NUCLEAR > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BOSS_NUCLEAR) i.BOSS_NUCLEAR++;
                                }
                                if (k.DL_RED_VOLATILE > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.DL_RED_VOLATILE) i.DL_RED_VOLATILE++;
                                }
                                if (k.DL_BLUE_VOLATILE > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.DL_BLUE_VOLATILE) i.DL_BLUE_VOLATILE++;
                                }
                                if(k.BOSS_ELVER_STOMPER > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BOSS_ELVER_STOMPER) i.BOSS_ELVER_STOMPER++;
                                }
                                if (k.BOSS_KUWAIT > 0)
                                {
                                    if (parameters.zombie.speciality == EZombieSpeciality.BOSS_KUWAIT) i.BOSS_KUWAIT++;
                                }
                                Instance.Configuration.Save();
                                updatedPlayer(whoDamageZombie);
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void updatedPlayer(UnturnedPlayer player)
        {
            foreach (var k in Instance.Configuration.Instance.Missioni)
            {
                foreach (var i in k.playeri)
                {
                    if (i.cSteamID == player.CSteamID)
                    {
                        foreach (var j in Instance.Configuration.Instance.Missioni)
                        {
                            if (j.whoCompleted.Contains(player.CSteamID) == false)
                            {
                                if (i.choppedTree >= j.choppedTree &&
                i.playerKilledCount >= j.playerKilledCount &&
                i.zombieKilledCount >= j.zombieKilledCount &&
                i.NORMAL >= j.NORMAL &&
                i.MEGA >= j.MEGA &&
                i.CRAWLER >= j.CRAWLER &&
                i.SPRINTER >= j.SPRINTER &&
                i.FLANKER_FRIENDLY >= j.FLANKER_FRIENDLY &&
                i.FLANKER_STALK >= j.FLANKER_STALK &&
                i.BURNER >= j.BURNER &&
                i.ACID >= j.ACID &&
                i.BOSS_ELECTRIC >= j.BOSS_ELECTRIC &&
                i.BOSS_WIND >= j.BOSS_WIND &&
                i.BOSS_FIRE >= j.BOSS_FIRE &&
                i.BOSS_ALL >= j.BOSS_ALL &&
                i.BOSS_MAGMA >= j.BOSS_MAGMA &&
                i.SPIRIT >= j.SPIRIT &&
                i.BOSS_SPIRIT >= j.BOSS_SPIRIT &&
                i.BOSS_NUCLEAR >= j.BOSS_NUCLEAR &&
                i.DL_RED_VOLATILE >= j.DL_RED_VOLATILE &&
                i.DL_BLUE_VOLATILE >= j.DL_BLUE_VOLATILE &&
                i.BOSS_ELVER_STOMPER >= j.BOSS_ELVER_STOMPER &&
                i.BOSS_KUWAIT >= j.BOSS_KUWAIT)
                                {
                                    j.whoCompleted.Add(player.CSteamID);
                                    foreach (var o in Instance.Configuration.Instance.pi)
                                    {
                                        if (o.cSteamID == player.CSteamID)
                                        {
                                            o.xpPass += j.xpGiveCompete;
                                            break;
                                        }
                                    }
                                    battlepassPlugin.Instance.Configuration.Save();
                                }
                            }
                        }
                    }
                }
            }
        }
        public Dictionary<UnturnedPlayer, int> playerRow = new Dictionary<UnturnedPlayer, int>();
        public Dictionary<UnturnedPlayer, int> playerRowMission = new Dictionary<UnturnedPlayer, int>();
        public void UI(UnturnedPlayer player)
        {
            EffectManager.sendUIEffect(4710, 4710, player.CSteamID, true);

            if (playerRow.ContainsKey(player) == false)
            {
                playerRow.Add(player, 1);
            }
            else if (playerRow.ContainsKey(player))
            {
                playerRow[player] = 1;
            }
            SetrowRewards(player);

            SetLevelRewards(player);
            if (Instance.Configuration.Instance.background)
            {
                EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "ImageBackGround", false);
            }
            EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "MissionsMenu", false);
            EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "RewardsMenu", true);
            player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Default, false);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, false);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowLifeMeters, false);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowUseableGunStatus, false);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVehicleStatus, false);

            player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
        }
        public void RemoveUI(UnturnedPlayer player)
        {
            EffectManager.askEffectClearByID(4710, player.CSteamID);
            player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Default, true);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, true);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, true);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowLifeMeters, true);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowUseableGunStatus, true);
            //player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVehicleStatus, true);
            player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }
        private void UItoMissions(UnturnedPlayer player)
        {
            EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "MissionsMenu", true);
            EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "RewardsMenu", false);
            int intn = 1;
            int intn1 = 0;
            if (playerRowMission.ContainsKey(player))
            {
                foreach (var pl in Instance.Configuration.Instance.pi)
                {
                    if (pl.cSteamID == player.CSteamID)
                    {
                        foreach (var o in Instance.Configuration.Instance.Missioni)
                        {
                            if (o.isDailyMission) intn1++;
                            if (o.isDailyMission && (intn1 == 3 * playerRowMission[player] || intn1 == 3 * playerRowMission[player] - 1 || intn1 == 3 * playerRowMission[player] - 2))
                            {
                                EffectManager.sendUIEffectText(4710, player.CSteamID, true, "DailyText" + intn.ToString(), o.text);
                                EffectManager.sendUIEffectText(4710, player.CSteamID, true, "DailyXP" + intn.ToString(), "+" + o.xpGiveCompete.ToString() + " xp");
                                if (o.whoCompleted.Contains(player.CSteamID))
                                {
                                    EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "DailyCompleted" + intn.ToString(), true);
                                }
                                intn++;
                            }
                            if (intn > 3) break;
                        }
                        int inttn = 1;
                        int inttn1 = 0;
                        foreach (var o in Instance.Configuration.Instance.Missioni)
                        {
                            if (o.isWeekMission) inttn1++;
                            if (o.isWeekMission && (inttn1 == 3 * playerRowMission[player] || inttn1 == 3 * playerRowMission[player] - 1 || inttn1 == 3 * playerRowMission[player] - 2))
                            {
                                EffectManager.sendUIEffectText(4710, player.CSteamID, true, "WeeklyText" + inttn.ToString(), o.text);
                                EffectManager.sendUIEffectText(4710, player.CSteamID, true, "WeeklyXP" + inttn.ToString(), "+" + o.xpGiveCompete.ToString() + " xp");
                                if (o.whoCompleted.Contains(player.CSteamID))
                                {
                                    EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "WeeklyCompleted" + inttn.ToString(), true);
                                }
                                inttn++;
                            }
                            if (inttn > 3) break;
                        }
                        break;
                    }
                }
            }

        }
        private void UItoRewards(UnturnedPlayer player)
        {
            SetrowRewards(player);
            EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "MissionsMenu", false);
            EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "RewardsMenu", true);
        }
        private void SetLevelRewards(UnturnedPlayer player)
        {
            uint xpminus = 0;
            int lvl = 0;
            uint lastj = 0;
            foreach (var i in Instance.Configuration.Instance.pi)
            {
                if (i.cSteamID == player.CSteamID)
                {
                    foreach (var j in Instance.Configuration.Instance.passFreei)
                    {
                        if (i.xpPass - xpminus >= j.xpToGet)
                        {
                            lvl++;
                            lastj = j.xpToGet;
                            xpminus += j.xpToGet;
                        }
                        else
                        {
                            EffectManager.sendUIEffectText(4710, player.CSteamID, true, "Level", "LEVEL " + lvl.ToString());
                            EffectManager.sendUIEffectText(4710, player.CSteamID, true, "XpToNextLevel", (i.xpPass - xpminus).ToString() + " / " + j.xpToGet.ToString());
                            return;
                        }
                    }
                    EffectManager.sendUIEffectText(4710, player.CSteamID, true, "Level", "LEVEL " + lvl.ToString());
                    EffectManager.sendUIEffectText(4710, player.CSteamID, true, "XpToNextLevel", (i.xpPass - xpminus).ToString() + " / " + lastj.ToString());
                    break;
                }
            }
        }
        private void SetrowRewards(UnturnedPlayer player)
        {
            if (playerRow.ContainsKey(player) == false) playerRow.Add(player, 1);
            if (playerRow.ContainsKey(player))
            {
                for (int o = 0; o < 2; o++)
                {
                    if (o == 0)
                    {
                        int numberStart = (5 * playerRow[player]) - 5;
                        int numberEnd = 5 * playerRow[player];
                        int numJust = 1;
                        foreach (var j in Instance.Configuration.Instance.pi)
                        {
                            if (j.cSteamID == player.CSteamID)
                            {
                                int endIndex = Math.Min(numberEnd, Instance.Configuration.Instance.passFreei.Count);
                                for (int i = numberStart; i < endIndex; i++)
                                {
                                    if (j.xpPass >= Instance.Configuration.Instance.passFreei[i].xpToGet && player.HasPermission(Instance.Configuration.Instance.goldPassPermission))
                                    {
                                        EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "getFreePassItem" + numJust.ToString() + "Blocked", false);
                                    }
                                    EffectManager.sendUIEffectImageURL(4710, player.CSteamID, true, "getFreePassItem" + numJust.ToString() + "Image", Instance.Configuration.Instance.passFreei[i].iconURL);
                                    numJust++;
                                }
                                break;
                            }
                        }

                    }
                    if (o == 1)
                    {
                        int numberStart = (5 * playerRow[player]) - 5;
                        int numberEnd = 5 * playerRow[player];
                        int numJust = 1;
                        foreach (var j in Instance.Configuration.Instance.pi)
                        {
                            if (j.cSteamID == player.CSteamID)
                            {
                                int endIndex = Math.Min(numberEnd, Instance.Configuration.Instance.passFreei.Count);
                                for (int i = numberStart; i < endIndex; i++)
                                {
                                    if (j.xpPass >= Instance.Configuration.Instance.passFreei[i].xpToGet && player.HasPermission(Instance.Configuration.Instance.goldPassPermission))
                                    {
                                        EffectManager.sendUIEffectVisibility(4710, player.CSteamID, true, "getPaidPassItem" + numJust.ToString() + "Blocked", false);
                                    }
                                    EffectManager.sendUIEffectImageURL(4710, player.CSteamID, true, "getPaidPassItem" + numJust.ToString() + "Image", Instance.Configuration.Instance.passFreei[i].iconURL);
                                    numJust++;
                                }
                                break;
                            }
                        }

                    }
                }
            }
        }
        
        private void onPlayerConnected(UnturnedPlayer player)
        {
            if (playerRow.ContainsKey(player) == false) playerRow.Add(player, 1);
            if (playerRowMission.ContainsKey(player) == false) playerRowMission.Add(player, 1);
            if (Instance.Configuration.Instance.pi.Exists(x => x.cSteamID == player.CSteamID) == false)
            {
                Instance.Configuration.Instance.pi.Add(new playeri { cSteamID = player.CSteamID });
                battlepassPlugin.Instance.Configuration.Save();
            }
            foreach (var k in Instance.Configuration.Instance.Missioni)
            {
                if (k.playeri.Exists(x => x.cSteamID == player.CSteamID) == false)
                {
                    k.playeri.Add(new playerinfo { cSteamID = player.CSteamID });
                    battlepassPlugin.Instance.Configuration.Save();
                }
            }
        }

        private bool HaveXPtoGetFreePassI(UnturnedPlayer player, playeri i, passFree j)
        {
            uint xpminus = 0;
            foreach (var k in Instance.Configuration.Instance.passFreei)
            {
                if (i.xpPass - xpminus >= k.xpToGet)
                {
                    xpminus += k.xpToGet;
                    if (j == k)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool HaveXPtoGetPaidPassI(UnturnedPlayer player, playeri i, passPaid j)
        {
            uint xpminus = 0;
            foreach (var k in Instance.Configuration.Instance.passPaidi)
            {
                if (i.xpPass - xpminus >= k.xpToGet)
                {
                    xpminus += k.xpToGet;
                    if (j == k)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void collectReward(bool isFree, int index, UnturnedPlayer player)
        {
            if (isFree)
            {
                foreach (var i in Instance.Configuration.Instance.pi)
                {
                    if (i.cSteamID == player.CSteamID)
                    {
                        if (i.collectedRewardsFreeIndex.Contains(index) == false)
                        {
                            foreach (var j in Instance.Configuration.Instance.passFreei)
                            {
                                if (j.number == index)
                                {
                                    if (HaveXPtoGetFreePassI(player, i, j))
                                    {
                                        foreach (ushort k in j.itemsReward)
                                        {
                                            player.GiveItem(k, 1);
                                        }
                                        i.collectedRewardsFreeIndex.Add(index);
                                        battlepassPlugin.Instance.Configuration.Save();
                                        UnturnedChat.Say(player, Instance.Configuration.Instance.collectedRewardMessage, Color.green);
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            if (isFree == false)
            {
                if (player.HasPermission(Instance.Configuration.Instance.goldPassPermission) || player.IsAdmin)
                {
                    foreach (var i in Instance.Configuration.Instance.pi)
                    {
                        if (i.cSteamID == player.CSteamID)
                        {
                            if (i.collectedRewardsPaidIndex.Contains(index) == false)
                            {
                                foreach (var j in Instance.Configuration.Instance.passPaidi)
                                {
                                    if (j.number == index)
                                    {
                                        if (HaveXPtoGetPaidPassI(player, i, j))
                                        {
                                            foreach (ushort k in j.itemsReward)
                                            {
                                                player.GiveItem(k, 1);
                                            }
                                            i.collectedRewardsPaidIndex.Add(index);
                                            battlepassPlugin.Instance.Configuration.Save();
                                            UnturnedChat.Say(player, Instance.Configuration.Instance.collectedRewardMessage, Color.green);
                                        }
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
