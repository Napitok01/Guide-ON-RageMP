//! ------------------ServerSide Events------------------

//файл Events.cs : 
using GTANetworkAPI;

namespace ServerSide
{
    public class Events : Script
    {
        [ServerEvent(Event.PlayerSpawn)] //! Обязательно нужно учитывать все параметры которые передаёт этот ивент ибо будет краш
        private void OnPlayerSpawn(Player player) //* чтобы увидеть параметры можно навести на 'Event.PlayerSpawn'
        {
            player.Armor = 100;
        }


        [ServerEvent(Events.PlayerEnterVehicle)]
        private void OnPlayerEnterVehicle(Player player, Vehicle vehicle, SByte SeatID)
        {
            if (vehicle == null) return;//? Проверка 'Существует ли машина ?'
            vehicle.PrimaryColor = 12; //?  если машина существует красим её в черный цвет
        }
        [ServerEvent(Event.PlayerExitVehicle)]
        private void OnPlayerExitVehicle(Player player, Vehicle vehicle)
        {
            if (vehicle == null) return; //? если игрко вышел то машина красится в белый
            vehicle.PrimaryColor = 131;
        }

        [RemoteEvent("CLIENT:SERVER:RepairCar")] //* для получения с клиента 
        private void RepairCar(Player player)
        {
            if (player.Vehicle == null)
            {
                player.SendChatMessage("You're not in a vehicle! From Server!");
                return;
            }
            player.Vehicle.Repair();
        }
    }
}



//! ------------------ClientSide Events------------------

//ExampleEvents.cs :
using RAGE;

namespace ClientSide.Player
{
    public class ExampleEvents : Events.Script
    {
        public ExampleEvents()
        {
            Events.OnPlayerEnterColshape += OnPlayerEnterColshape;
            Events.OnPlayerExitColshape += OnPlayerExitColshape;
            Events.Add("SERVER:CLIENT:RandomizePLayer", RandomizePlayer);//* Реализуем Кастомный ивент
            RAGE.Input.Bind(VirtualKeys.F5, true, () => //! С клиента на сервер 
            {
                if (RAGE.Elements.Player.LocalPlayer.Vehicle == null)
                    RAGE.Chat.Output("You're not in car! From Client! ");
                return;
                Events.CallRemote("CLIENT:SERVER:RepairCar"); //? тригерим ивента на сервер 
            }); //* теперь на ServerSide в Events
        }


        //! С сервера на клиент
        private void RandomizePLayer(object[] args)
        {
            Random rand = new Random();
            RAGE.Elements.Player.LocalPlayer.SetHeadBlendData(
            rand.next(0, 3),
            rand.next(0, 3),
            rand.next(0, 3),
            rand.next(0, 3),                    //* тут создаем рандом 
            rand.next(0, 3),
            rand.next(0, 3),
            0.5f,
            0.5f,
            0.5f,
            false);
        }


        private void OnPlayerExitColshape(Colshape colshape, Events.CancelEventArgs cancel)
        {
            RAGE.Chat.Output("you exited the colshape");
        }


        private void OnPlayerEnterColshape(Colshape colshape, Events.CancelEventArgs cancel)
        {
            RAGE.Chat.Output("you entered the colshape");
        }
    }
}



//! Кастомный ивент с Сервера на Клиент:
/*
    *Создаем команду для кастомного ивента, (Например /RandomizeMe) 
*/


//! Кастомный ивент с Клиента на Сервер: 
/*
    * Сначало на ClientSide создаем ивент(Пример,"При нажатии F5 чинится машина" )
*/




//! Всё про DATA будет в DataGuide.cs



//! Теперь про процедуры будет в EF_Guide.cs
