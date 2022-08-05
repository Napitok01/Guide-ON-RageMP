//!  <--------------------ГАЙД ПО СОЗДАНИЮ КОМАНД----------------------->

/*
    *пишем в классе Commands.cs :
*/


using GTANetworkAPI;

namespace ServerSide : Script
{
    [Command] // ? создание машинки по команде CreateCar(на кординатах игрока, с цветом 131(1) и 131(2))
    public class Commands
    {
        public void CreateCar(Player player) 
        {
            NAPI.Vehicle.CreateVehicle(VehicleHash.Adder, player.position, player.heading, 131, 131);
        } 

            //! Создание Аллиасов 

        [Command("hp")] //? Получение count-количества HP при вводе /hp (кол-во)
        public void SetHealth(Player player, int count)
        {
            player.Health = count;
        }


        [Command("teleport", Alias = "tp" )] //? телепортация по координатам 
        public void TeleportPlayer(Player player, float x, float y, float z)
        {
            player.Position = new Vector3(x, y, z); //? например /tp 123 123 123
        }

        //! необязательные параметры

        [Command("armor")] //? дает игроку 'armor' брони
        public void SetPlayerArmor(Player player, int armor = 100)
        {
            player.Armor = armor;
        }
    }

    //! Greddy Arg (жадный аргумент) 

    [Command("me", GreddyArg = true)] //? для того чтобы RAGE не делил строку на несколько переменных
    public void TypeMe (Player player, string actions)
    {
        player.SendChatMessage(${"player.Name"}) did that: + actions ;
    }

    //! Кастомный Атрибут

    /*
     * чтобы сделать код более красивым можно создать атрибьют для комманд
     * для этого мы создаем дерриторию от ServerSide "CommandAttributes"
     * потом создадим от неё класс "RequiresHealthAttribute"
    */
    //! код для RequiresHealthAttribute :

    using GTANetworkAPI;

    namespace ServerSide : CommandAttributes
    {
        public class RequiresHealthAttribute : CommandConditionAttribute //Делаем метод Check 
        {
            public int Amount { get; set; }

            public RequiresHealthAttribute(int amount = 100)
            {
                Amount = amount;
            }

            public override bool Check(Player player, string cmdName, string cmdText)
            {
                if (player.Health > Amount)
                {
                    player.SendChatMessage("Not Healed, youre heal af!");
                    return false;
                }
                return true;
            }
        }
    }


    //! Теперь возвращаемся к Commands :

    [RequiresHealth(75)]
    [Command ("healme")]
    public void HealPlayer(Player player)
    {
        player.Health = 100;
            player.SendChatMessage("Healed!");
    }
    
}
    
     