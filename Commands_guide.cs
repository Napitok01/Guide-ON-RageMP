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


    [Command("teleport", Alias = "tp")] //? телепортация по координатам 
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
public void TypeMe(Player player, string actions)
{
    player.SendChatMessage(${ "player.Name"}) did that: +actions;
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
[Command("healme")]
public void HealPlayer(Player player)
{
    player.Health = 100;
    player.SendChatMessage("Healed!");
}

}




//! Про маркеры и чекпоинты :
/*
    * Маркеры - это просто доп. элементы которые не несут ничего, а служат просто для
    * того чтобы указать игроку что в этом месте что-то происходит обычно
    * они работают с колшейпами
    * чекпоинты - это как маркеры, ток в гонках GTA5 и можно указать направление
*/
//! Создание маркеров :
[Command("marker")] //? комманда для создания красного маркера на позиции игрока : /marker (id маркера)

public void Marker(Player player, uint markerType)
{
    NAPI.Marker.CreateMarker(markerType, player.Position, new Vector3(), new Vector3(), 1f, new Color(255, 0, 0, 100), false, player.Dimension); //! 100 -- это прозрачность

}

//! Создание чекпоинтов :
[Command("checkpoint")] //? Команда для создания красного чекпоинта /checkpoint (id чекпоинта)

public void Checkpoint(Player player, uint checkpointType) //? в этом примере предусмотрено запоминание предыдущего чекпоинта из за этого каждый чекпоинт будет указывать на предыдущий
{
    var direction = _prevCheckpoint?.Position ?? player.Position;
    _prevCheckpoint = NAPI.Checkpoint.CreateCheckpoint(checkpointType, player.Position + new Vector3(0f, 0f, -1f), direction, 1f, new Color(255, 0, 0, 100), player.Dimension);
}


//! Теперь про Блипы :
/*
    * Блипы это все точки на карте(и миникарте)
    * они могут работать так : 1)видны всегда; 2) видны только когда игрок рядом; 
*/

//! Создание блипов :
[Command("blip")] //? создание блипа на координатах игрока // sprite - id блипа, byte - непрозрачность(0-255)

public void Blip(Player player, uint sprite, byte color, string name, bool shortRange) //? /blip 1 1 Blip1 true
{
    NAPI.Blip.CreateBlip(sprite, player.Position, 1f, color, name, 255, 0f, shortRange, 0, player.Dimension)
    }


//! Теперь про колшейпы
/*
    * это зона которая определяет вошел или вышел ли кто-то из неё
*/

//! Создание колшейпов:
[Command("colshape")] //? Создание колшейпа на позиции игрока с размером "scale" /colshape (радиус)
public void Colshape(Player player, float scale)
{
    var colShape = NAPI.ColShape.CreateCylinderColShape(player.Position + new Vector3(0f, 0f, -1f), scale, 2f, player.Dimension);
    //* colShape принимает 2 занчения OnEntityEnterColShape(вОшел) и OnEntityExitColShape(вЫшел) 
    colShape.OnEntityEnterColShape += OnEntityEnterColShape;
    colShape.OnEntityExitColShape += OnEntityExitColShape
    }

private void OnEntityEnterColShape(ColShape colShape, Player player)
{
    player.Armor = 100;
}

private void OnEntityExitColShape(ColShape colShape, Player player)
{
    player.Armor = 0;
}


//! чтобы сделать колшейп видимым можно написать так:
/*
!заменить часть public void Colshape на : 

    * public void Colshape(Player player, float scale)
    * {
    *     var position = player.Position + new Vector3(0f, 0f, -1f);
    * 
    *     var colShape = NAPI.ColShape.CreateCylinderColShape(position, scale, 2f, player.Dimension);
    *     colShape.SetData(nameof(GTANetworkAPI.Marker), NAPI.Marker.CreateMarker(1, position, new Vector3(), new Vector3(), new Color(255, 0, 0, 100), false, player.Dimension)); 
    *     colShape.OnEntityEnterColShape += OnEntityEnterColShape;
    *     colShape.OnEntityExitColShape += OnEntityExitColShape;
    * }
*/



//! Команда для кастомного ивента с сервера на клиент: 

[Command("randomizeme")]
private void RandomizeMe(Player player)
{
    player.TriggerEvent("SERVER:CLIENT:RandomizePLayer"); //* тригерим ивент
} //* теперь переходим на ClientSide(ExampleEvents(т.к ивент от игрока)), для реализации ивента

