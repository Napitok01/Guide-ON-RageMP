/* 
  !---------START---------!
  * Установить RageMP, Файлы из папки "ServerFiles" скопировать в
  * новую папку для удобства например : 'Rage'
  * после запусти Ragemp-server.exe от имени админ.

  * в файле conf.json поменять "csharp : 'disabled'" на 'enabled'

  * после запусти Ragemp-server.exe от имени админ. --
  * будет ошибка мол папка ресурсы - не найдена

  * после в папке "dotnet" создаем папку "resources"

  * заходим в Visual Studio
  !ДОЛЖЕН СТОЯТЬ .NET

  *Создаем проект с "ClassLibrary" имя файла : 'ServerSide' Solution name = "Project"
  *location = "rage\dotnet\resources"
  *после выбрать NETCORE 3.1

  *'Class1.cs' поменять на 'Main.cs'
  убрать using
  *справа -- ServerSide-> ПКМ-> Add-> project_Reference -- Solution -> browse --
  *rage\dotnet\runtime\bootstrapper.dll -> OK
  ? (создаем зависимость)
*/


//! КОД ДЛЯ Main.cs в ServerSide

using GTANetworkAPI;

namespace ServerSide
{
    public class Main : Script
    // ? ДЛЯ ПРОВЕРКИ :
    {
        [ServerEvent(Event.ResourceStart)]

        public void OnResourceStart()
        {
            NAPI.Util.ConsoleOutput("Server started!")
        }
    }
}

    /*
        *Двойной клик по ServerSide(справа)
        ? Заходим в конфигурацию проекта
        * после "<TargetFramework>...</TargetFramework> добавить(на след строку)
        *"<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>" !ОБЯЗАТЕЛЬНО!
        *чтобы потом не исать и не скачивать dll ки всякие

        *Заходим в настройки запуска(Возле кнопки запуска стрелочка вниз) ->
        *ServerSide Debug Properties -> (В верхнем левом углу) нажать создать новый профиль(Executable)
        *после переименовать его в "Rage" например, В Executable -> browse: \rage\ragemp-server.exe
        *после Working Derictory -> browse: \rage
        * Выйти
        * теперь изменить выбор запуска на имя которое ты указал (например Rage)
        ? Теперь при запуске решения у нас будет автоматически запускаться Сервер

        *рядом с кнопкой запуска -- anyCPU -> Configuration Manager -> Active Solution Platform ->
        *AnyCPU -> NEW -> выбрать " x64 "

        *Запускаем решение (будет ошибка)
        *Заходим dotnet\resources\Project\ServerSide\bin(удалить Debug)
        Поздравляю ты не бесполезен
        
    */

    /*
    *Заходим Rage\dotnet\Settings.xml
    *Делаем так:
    ! Acl - false
    ! logConsole - True
    ! LogChat - False
    *и добвляем после строку :
    *"<resource src="Project(Название проекта)" />, ctr + S


    *теперь заходим в dotnet/resources/Project(название проета)
    *(создать файл) - meta.xml
    */


    //! Код для meta.xml
    < meta >
    < info name = "/*другое название проекта*/" description = "/*Описание*/" />

    < script src = "ServerSide\bin\x64\Debug\netcoreapp3.1\ServerSide.dll" /> //ctr + S
    </ meta >

     /*
       *Запускаем решение
       *Если в консоли есть 'Server started!' - значит всё норм 

        *заходим rage\client_packages создаем папку "cs_packages"

        *заходим в VS, нажимаем ПКМ на Solution(на решение) -> Add -> new project ->
        * -> Class Library -> next -> ClientSide(Project Name) -> location : rage\client_packages\cs_packages -> OK
        * -> next -> NETCORE 3.1 -> create
        * убираем using
        *Class1.cs переименовать в Main.cs
        ? добавляем зависимость
        * как в прошлый раз но путь теперь другой,(к самому RAGEMP ):
        *\RAGEMP\dotnet\rage-sharpN.dll -> OK
    */


    //! КОД ДЛЯ Main.cs в ClientSide
    
using Rage;

namespace ClientSide
{
    public class Main : Events.Script
    {
        public Main()
        {
            Events.OnPlayerReady += PlayerReady;
        }
        private void PlayerReady()
        {
            RAGE.Chat.Output("Hello, you joined the server!"); //? Для примера
        }
    }
}//? ClienSide Собирать не нужно, RAGE сам подгрузит файлы т.к они и так уже там есть



/* 
!Взять с Github Файл IGNORE_LIST и поместить его в папку client_packages
? Для того чтобы в этой папке не появлялись не нужные побочные папки
*/






//! <------------------------Теперь тут будет дальнейшее выполнение и направление в другие файлы-------------------------->



/*
    * Чтобы добавить команды нужно создать новый класс "Commands.cs" в ServerSide 
    !Дальше про команды в файле Commands_guide.cs
*/


/*
! колшейпы, блипы, маркеры и тд. разобраны в Commands_guide.cs
*/



/*
! ИвЕНТЫ
* Список можно найти на GTAnet.work wiki - ServrSide Events
* Список можно найти на - Rage.mp ClientSide Events

* Серверные Ивенты прописываются в классе Events.cs от ServerSide - гайд по ним будет в файле EventsGuide.cs 

* Клиентские ивенты прописываются в папке Player от ClientSide
* В эту папку нужно складывать все скрипты относящиеся к игроку
* создаем в папке новый класс ExampleEvents.cs - туда пишем ивенты
*/