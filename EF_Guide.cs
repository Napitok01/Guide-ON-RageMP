/*
    * процедуры облегчают работу с передачей информации с сервера на клиент и наоборот
*/

/*
    * 1) Всегда при вызове процедуры всё должно быть асинхронно т.е нужно await т.е дожидаться ответа
    * 2) обработчик процедуры он может быть и синхронным и асинхронным  
*/

//! создаем 2 класса на ClientSide и ServerSide называем их RpcExample(наследуем от Script)

//! Client - Server Узнать серверное время

//! ClientSide RpcExample : 

using RAGE;

namespace ClientSide
{
    public class RpcExample : Events.Script
    {
        private const string GetTimeKey = "RPC::CLIENT::SERVER:GetServerTime";

        public RpcExample()
        {
            Input.Bind(VirtualKeys.F6, true, GetServerTime); //? будем получать время
        }

        private async void GetServerTime()
        {
            var response = (string)[пробел]await Events.CallRemoteProc(GetTimeKey);//? кастим строку чтобы потом реализовать в DateTime
            var date = RAGE.Util.Json.Deserialize<DateTime>(response);
            ChatOutput($"Current server time = {date.ToShortTimeString()}"); //? выводим эти данные //теперь на сервер сайд
        }
    }
}

//! ServerSide RpcExample : 

using GTANetworkAPI;

namespace ServerSide
{
    public class RpcExample : Script
    {
        private const string GetTimeKey = "RPC::CLIENT::SERVER:GetServerTime";

        [RemoteProc(GetTimeKey)]

        private string GetTime(Player player)
        {
            return NAPI.Util.ToJson(DateTime.Now);// *всё можно проверять
        }
    }
    /* Можно сделать так с асинхронностью, но в данном случае это не нужно
         public class RpcExample : Script
     {
         private const string GetTimeKey = "RPC::CLIENT::SERVER:GetServerTime";

         [RemoteProc(GetTimeKey, async: true)]

         private async Task<string> GetTime(Player player)
         {
             await NAPI.Task.WaitForMainThread(5000);
             return NAPI.Util.ToJson(DateTime.Now);
         }
     }
     */
}








//! Server - Client УЗНАТЬ ФПС ИГРОКА : 

//! ServerSide RpcExample : 

using System;
using System.Threading.Tasks;
using GTANetworkAPI;

namespace ServerSide
{
    public class RpcExample : Script
    {
        private const string GetFpsKey = "RPC::SERVER:CLIENT:GetFps"

        [Command]
        private void GetPlayerFps(Player player, int id)
        {
            var target = NAPI.Pools.GetAllPlayer().FirstOrDefault(x => x.Id == id);//? pools - это пул всех обьектов(игроки, таргеты и тд)
            if (target == null) //? проверяем существует ли игрок
            {
                player.SendChatMessage("Player not found");
                return;
            } //* теперь на клиент сайд
            //* не забываем что любые процедуры должны быть async
            NAPI.Task.Run(async () =>
            {
                var response = (float)await target.TriggerProcedure(GetFpsKey);
                player.SendChatMessage($"Player {target.Name} fps: {response}");
            }); // ВСЁ 
        }
    }
}


//! ClientSide RpcExample : 

using RAGE;

namespace ClientSide
{
    public class RpcExample : Events.Script
    {
        private const string GetFpsKey = "RPC::SERVER:CLIENT:GetFps"
        public RpcExample()
        {
            Events.AddProc(GetFpsKey, args =>
            {
                return 1 / RAGE.Geme.Misc.GetFrameTime(); //ВСЁ идём опять на сервер сайд
            });
        }
    }
}