# EJournal Telegram Bot

#### Для чего?
Бот для просотра расписания на следующий день в ЭлЖур через Telegram Bot с возможностью запланированной отправки расписания.

## Использование

### Требования:
* .NET 10.0.100 или новее.

### Как собрать проект:

1. Клонируйте репозиторий.
2. Соберите проект.
3. Заполните файл `appsettings.json`.
4. Запустите проект.

### Заполнение файла `appsettings.json`:

#### BotConfiguration:
`BotToken` - токен Telegram бота из BotFather.

#### ElJurApiConfiguration:

`BaseUrl` - URL к API ЭлЖур `https://{vendor}.eljur.ru/apiv3`. Заместо `{vendor}` - код учебного заведения. 

`DevKey` - ключ разработчика. Необходим для работы с API. Получить его можно через тех. поддержку.

`AuthToken` - API токен от аккаунта.

`Vendor` - код учебного заведения.

#### ScheduleConfiguration:

`chatIds` - массив ID чатов в Telegram, по которым будет осуществляться рассылка расписания

## Документация API ЭлЖур

Подробную информацию о API можно найти на официальном сайте [ЭлЖура](https://eljur.ru/api/).

## Используемые библиотеки

Этот проект использует следующие библиотеки с открытым исходным кодом:

- **Telegram.Bot** - лицензируется под MIT License. См. [Telegram.Bot License](https://github.com/TelegramBots/Telegram.Bot/tree/master?tab=MIT-1-ov-file).
- **Quartz.NET** - лицензируется под Apache License 2.0. См. [Quartz.NET License](https://github.com/quartznet/quartznet?tab=Apache-2.0-1-ov-file).

## Лицензия
Этот проект находится под лицензией MIT. Подробности см. в файле [LICENSE](LICENSE).
