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



//! Кастомный ивент с Сервера на Клиент