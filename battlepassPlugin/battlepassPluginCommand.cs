using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace battlepassPlugin
{
    public class battlepassPluginCommand : IRocketCommand
    {


        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "battlepass";

        public string Help => "battlepass.";

        public string Syntax => "/battlepass";

        public List<string> Aliases => new List<string>() { "battlepass" }; // Чтобы можно было использовать комманду /back и /b

        public List<string> Permissions => new List<string>() { "battlepassPlugin.battlepass" }; // чтобы использовать комманду - права либо back либо vi2g.back



        private UnturnedPlayer player; // Получаем игрока который вызвал комманду



        public void Execute(IRocketPlayer caller, string[] command)
        {
            // Получаем экземпляр игрока и его баланс
            player = (UnturnedPlayer)caller;

            battlepassPlugin.Instance.UI(player);
        }
    }
}
