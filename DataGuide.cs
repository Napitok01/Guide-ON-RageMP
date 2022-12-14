/*
    *Есть всего 3 способа хранения информации внутри ENTITY :
    *1) LOCAL DATA - обычний словарь (GetData, SetData, GetExternalData, SetExtarnalData)
    *2) SHARED DATA - получает её и сервер, и клиент(+ могут получать все клиенты не зависимо от того кому она принадлежит)
    *3) OWN SHARED DATA - получает только клиент которому она была предоставлена
*/



//! LOCAL DATA :

/*
    * Допустим создадим приватную машину в которую сможет сесть только владелец

    * Создаем класс PrivateVehicle.cs на ServerSide
*/

//* Private Vehicle :

using GTANetworkAPI;

namespace ServerSide
{
    public class PrivateVehicle : Script
    {
        private static string _vehicleKey = ":OwnVehicle"; //? создаем переменную которая будет содержать в себе ключ


        [Command] //? команда для создания приватной машины
        private void MyVeh(Player player)
        {
            var veh = NAPI.Vehicle.CreateVehicle(VehicleHash.Adder, player.Position, player.Rotation, 131, 131);
            veh.SetData(_vehicleKey, player.SocialClubId); //? доставляем информацию самой машине
        }

        [ServerEvent(Events.PlayerEnterVehicle)]
        private void OnPlayerEnterVehicle(Player player, Vehicle vehicle, SByte seatID)
        {
            if (vehicle.HasData(_vehicleKey) && vehicle.GetData<ulong>(_vehicleKey) != player.SocialClubId) // has чтобы проверить есть ли информация(если она есть и не совподает то человека выбрасывает из машины)
            {
                player.SendChatMessage("Its not your car!");
                player.WarpOutOfVehicle();
            }
        }
    }
}




//!  SHARED DATA :  
/*
    *сделаем выдачу рандомного уровня игроку при заходе на сервер и возможность просмотретьэтот уровень

    * создаем класс Level.cs на ServerSide
*/

using GTANetworkAPI;

namespace ServerSide
{
    public class Level : Script
    {
        private static string _levelKey = nameof(_levelKey);

        [ServerEvent(Event.PlayerConnected)]
        private void OnPlayerConnected(Player player)
        {
            player.SetSharedData(_levelKey, new Random().Next(1, 100));
        }
    }
}

//* теперь переходим в класс Client от ClientSide :


using RAGE;
using System;
using RAGE.Elements;
using RAGE.Ui;



namespace ClientSide
{

    public class Client : Events.Script
    {
        private const string _levelKey = nameof(_levelKey);
        private const string _walletKey = nameof(_walletKey);
        public class Client()
        {
            RAGE.Chat.Output("Hello  World, Client");
            RAGE.Input.Bind(VirtualKeys.F4, true, LevelNotifier);
            RAGE.Events.AddDataHandler(_walletKey, MoneyHandler);
    }

    private void MoneyHandler(Entity entity, object arg, object oldarg) //* когда команда будет введена HANDLER сам скажет что произошло
    {
        if (entity is RAGE.Elements.Player == false) return; //? проверяем является ли entity игроком
        int money = (int)arg;
        int oldMoney = (int)oldarg;

        if (money > oldMoney)
        {
            Chat.Output("You earned some money. Amount: " + (money - oldMoney));
        }
        else
        {
            Chat.Output("You spend some Money. Amount: " + (oldMoney - money));
        }
    }

    private void LevelNotifier() //* Сделаем так чтобы при нажатии F4 выводился в чат весь список игроков и их уровни
    {
        foreach (var player in RAGE.Elements.Entities.Players.All)
        {
            Chat.Output(player.name + "-----------" + player._GetSharedData<int>(_levelKey));
        }
    }
}
}



//! OWN SHARED DATA : 

/*
    * Пример : Система денег :

    * в OwnSharedData мы не можем использовать метод "HasOwnSharedData" - он никогда не вернёт нам настоящее значение
    * он сейчас на платформе поломан поэтому пользуемся костылём

    * Создаем класс Wallet в ServerSide :
*/

using GTANetworkAPI;

namespace ServerSide
{
    public class Wallet : Script
    {
        private const string _walletKey = nameof(_walletKey);

        [Command]
        private void GetMoney(Player player, int amount)
        {
            if (player.GetOwnSharedData<int?>(_walletKey) == null) player.SetOwnSharedData(_walletKey, 0);
            player.SetOwnSharedData(_walletKey, player.GetOwnSharedData<int>(_walletKey) + amount);
        }

        [Command]
        private void SpendMoney(Player player, int amount)
        {
            if (player.GetOwnSharedData<int?>(_walletKey) == null) player.SetOwnSharedData(_walletKey, 0);

            if (player.GetOwnSharedData<int?>(_walletKey) < amount)
            {
                player.SendChatMessage("you don't have Enought money!");
                return;
            }

            player.SetOwnSharedData(_walletKey, player.GetOwnSharedData<int>(_walletKey) - amount);
        }
    }
}
//* Теперь переходим в ClientSide в Client :

//! смотри ВЫШЕ !!!!



