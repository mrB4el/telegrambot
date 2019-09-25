using DotNetTelegramBot.DataBase;
using DotNetTelegramBot.DataBase.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace DotNetTelegramBot
{
    public class Program
    {

        private static readonly TelegramBotClient Bot = new TelegramBotClient("token");
        public static Dictionary<long, int> dictionary = new Dictionary<long, int>();
        private static GraphityClass adminprod = new GraphityClass();
        private static LocationClass adminlocation = new LocationClass();

        public static void Main(string[] args)
        {
            var me = Bot.GetMeAsync().Result;
            Console.Title = me.Username;

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());

            Console.WriteLine($"Start listening for @{me.Username}");


            DataBaseClass database = new DataBaseClass("address", 3306, "login", "table", "password");
            database.StartConnection();

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.Text)
            {
                ShowMenuMain(message);
                return;
            }
            else
                MenuLogic(message);
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Bot.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"Received {callbackQuery.Data}");

            await Bot.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"Received test {callbackQuery.Data}");
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                new InlineQueryResultLocation(
                    id: "1",
                    latitude: 40.7058316f,
                    longitude: -74.2581888f,
                    title: "New York")   // displayed result
                    {
                        InputMessageContent = new InputLocationMessageContent(
                            latitude: 40.7058316f,
                            longitude: -74.2581888f)    // message if result is selected
                    },

                new InlineQueryResultLocation(
                    id: "2",
                    latitude: 13.1449577f,
                    longitude: 52.507629f,
                    title: "Berlin") // displayed result
                    {
                        InputMessageContent = new InputLocationMessageContent(
                            latitude: 13.1449577f,
                            longitude: 52.507629f)   // message if result is selected
                    }
            };

            await Bot.AnswerInlineQueryAsync(
                inlineQueryEventArgs.InlineQuery.Id,
                results,
                isPersonal: true,
                cacheTime: 0);
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }

        // LOGIC
        private static async void MenuLogic(Telegram.Bot.Types.Message message)
        {
            if (!dictionary.ContainsKey(message.Chat.Id))
                dictionary[message.Chat.Id] = 0;

            switch (message.Text.Split(' ').First())
            {
                // send inline keyboard
                case "Меню":
                    dictionary[message.Chat.Id] = 0;
                    ShowMenuMain(message);
                    break;

                case "/admin":
                    if (message.Chat.Username == "PotatOS")
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Hello, senpai ^_^");
                        dictionary[message.Chat.Id] = 80;
                    }
                    else Console.WriteLine("baka");
                    break;
                default:
                    break;
            }

            if (dictionary[message.Chat.Id] >= 0 && dictionary[message.Chat.Id] < 3)
            {
                switch (message.Text)
                {
                    case "Графити":
                        dictionary[message.Chat.Id] = 1;
                        ShowMenuBuy(message);
                        break;
                    case "Район 1":
                        dictionary[message.Chat.Id] = 2;
                        string[] graphits = new string[] { "1", "2", "3", "4", "5", "0"};

                        ShowMenugraphits(message, graphits);

                        break;
                    case "Обратная связь":
                        ShowTBA(message);
                        break;
                    default:
                        break;
                }
            }

            if (dictionary[message.Chat.Id] >= 80 && dictionary[message.Chat.Id] < 500)
            {
                switch (message.Text)
                {
                    case "/admin":
                        dictionary[message.Chat.Id] = 80;
                        ShowMenuAdmin(message);
                        break;

                    case "Добавить графити":
                        ShowAdminAddProd(message);
                        dictionary[message.Chat.Id] = 81;
                        adminprod = new GraphityClass();
                        break;

                    case "Удалить графити":
                        ShowAdminDelProd(message);
                        dictionary[message.Chat.Id] = 91;
                        break;

                    case "Добавить район":
                        ShowAdminAddRegion(message);
                        dictionary[message.Chat.Id] = 101;
                        break;

                    case "Удалить район":
                        ShowAdminDelRegion(message);
                        dictionary[message.Chat.Id] = 111;
                        break;

                    case "Статистика":
                        ShowTBA(message);
                        break;

                    case "Закрыть бота":
                        ShowTBA(message);
                        break;

                    case "Открыть бота":
                        ShowTBA(message);
                        break;

                    default:
                        break;
                }

                if (dictionary[message.Chat.Id] > 80 && dictionary[message.Chat.Id] < 500)
                {
                    string answer = "";
                    int localid = dictionary[message.Chat.Id];

                    switch (localid)
                    {
                        case 81:
                            answer = "[Название]: ";
                            dictionary[message.Chat.Id] += 1;
                            break;
                        case 82:
                            adminprod.title = message.Text;
                            adminlocation = new LocationClass();
                            answer = "[Размер]: ";
                            dictionary[message.Chat.Id] += 1;
                            break;
                        case 83:
                            adminprod.size = Convert.ToInt32(message.Text);
                            answer = "[Широта]: ";
                            dictionary[message.Chat.Id] += 1;
                            break;
                        case 84:
                            adminlocation.latitude = float.Parse(message.Text, CultureInfo.InvariantCulture.NumberFormat);
                            answer = "[Долгота]: ";
                            dictionary[message.Chat.Id] += 1;
                            break;
                        case 85:
                            adminlocation.longitude = float.Parse(message.Text, CultureInfo.InvariantCulture.NumberFormat);
                            answer = "[Радиус]: ";
                            dictionary[message.Chat.Id] += 1;
                            break;
                        case 86:
                            adminlocation.radius = float.Parse(message.Text, CultureInfo.InvariantCulture.NumberFormat);
                            answer = "[Район]: ";
                            ShowTBA(message);
                            dictionary[message.Chat.Id] += 1;
                            break;
                        case 87:
                            adminprod.region = Convert.ToInt32(message.Text);
                            adminlocation.author = 1;
                            answer = JsonConvert.SerializeObject(adminlocation);
                            answer += JsonConvert.SerializeObject(adminprod);
                            answer += "Всё верно?";
                            ShowFinishDialog(message);
                            dictionary[message.Chat.Id] += 1;
                            break;
                        case 88:
                            if(message.Text == "Да")
                            {
                                // To-Do
                                ShowMenuAdmin(message);
                            }

                            else if(message.Text == "Нет")
                            {
                                adminlocation = new LocationClass();
                                adminprod = new GraphityClass();
                                answer = "Отменено";
                            }
                            else
                            {
                                answer = JsonConvert.SerializeObject(adminlocation);
                                answer += JsonConvert.SerializeObject(adminprod);
                                answer += "Всё верно?";
                                ShowFinishDialog(message);
                            }
                            break;
                        default:
                            break;
                    }

                    if (dictionary[message.Chat.Id] == 191)
                    {
                    }
                    await Bot.SendTextMessageAsync(message.Chat.Id, answer);
                }
            }

            switch (message.Text.Split(' ').First())
            {

                case "111":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                    await Task.Delay(500); // simulate longer running task

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("1.1"),
                            InlineKeyboardButton.WithCallbackData("1.2"),
                        },
                        new [] // second row
                        {
                            InlineKeyboardButton.WithCallbackData("Купить"),
                        }
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: inlineKeyboard);
                    break;


                // send a photo
                case "/photo":
                    //await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                    const string file = @"Files/tux.jpg";

                    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await Bot.SendPhotoAsync(
                            message.Chat.Id,
                            fileStream,
                            "Nice Picture");
                    }
                    break;

                // request location or contact
                case "/request":
                    var RequestReplyKeyboard1 = new ReplyKeyboardMarkup(new[]
                    {
                        KeyboardButton.WithRequestLocation("Location"),
                        KeyboardButton.WithRequestContact("Contact"),
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Who or Where are you?",
                        replyMarkup: RequestReplyKeyboard1);
                    break;

                default:
                    break;
            }
        }


        // MENUS
        private static async void ShowMenuMain(Telegram.Bot.Types.Message message)
        {
            ReplyKeyboardMarkup ReplyKeyboard = new[]
                    {
                        new[] { "Подтверждение графити", "Инструкция", "Обратная связь"},
                        new[] { "Меню" },
                    };
            ReplyKeyboard.ResizeKeyboard = true;
            await Bot.SendTextMessageAsync(message.Chat.Id, "Выбери пункт меню", replyMarkup: ReplyKeyboard);
        }
                
        private static async void ShowMenuBuy(Telegram.Bot.Types.Message message)
        {
            ReplyKeyboardMarkup ReplyKeyboard = new[]
                    {
                        new[] { "Район 1", "Район 2", "Район 3"},
                        new[] { "Меню" },
                    };
            ReplyKeyboard.ResizeKeyboard = true;
            await Bot.SendTextMessageAsync(message.Chat.Id, "Выбери район", replyMarkup: ReplyKeyboard);
        }

        static async void ShowMenugraphits(Telegram.Bot.Types.Message message, string[] graphits)
        {            
            ReplyKeyboardMarkup ReplyKeyboard = new[]
                    {
                        graphits,
                        new[] { "Меню" },
                    };
            ReplyKeyboard.ResizeKeyboard = true;
            await Bot.SendTextMessageAsync(message.Chat.Id, "Найти графити", replyMarkup: ReplyKeyboard);
        }

        private static async void ShowMenuAdmin(Telegram.Bot.Types.Message message)
        {
            ReplyKeyboardMarkup ReplyKeyboard = new[]
                                {
                        new[] { "Найти графити", "Удалить графити"},
                        new[] { "Добавить район", "Удалить район"},
                        new[] { "Статистика" },
                        new[] { "Открыть бота", "Закрыть бота" },
                        new[] { "/admin" },
                        new[] { "Меню" },
                    };
            ReplyKeyboard.ResizeKeyboard = true;
            await Bot.SendTextMessageAsync(message.Chat.Id, "Панель администратора", replyMarkup: ReplyKeyboard);
        }

        private static async void ShowAdminAddProd(Telegram.Bot.Types.Message message)
        {
            string answer = "Добавление графити следующим образом: \n" +
                @"/graphit add [название] [краткое описание] [размер] [широта] [долгота] [радиус] [район]";

            await Bot.SendTextMessageAsync(message.Chat.Id, answer);
        }
        private static async void ShowAdminAddRegion(Telegram.Bot.Types.Message message)
        {
            string answer = "Добавление района следующим образом: \n" +
                @"/region add [название] [краткое описание]";

            await Bot.SendTextMessageAsync(message.Chat.Id, answer);
        }

        private static async void ShowAdminDelProd(Telegram.Bot.Types.Message message)
        {
            string answer = "Удаление графити следующим образом: \n" +
                @"/graphit del [id]";

            await Bot.SendTextMessageAsync(message.Chat.Id, answer);
        }
        private static async void ShowAdminDelRegion(Telegram.Bot.Types.Message message)
        {
            string answer = "Удаление района следующим образом: \n" +
                @"/region del [id]";

            await Bot.SendTextMessageAsync(message.Chat.Id, answer);
        }

        private static async void ShowFinishDialog(Telegram.Bot.Types.Message message)
        {
            ReplyKeyboardMarkup ReplyKeyboard = new[]
            {
                new[] { "Да", "Нет"},
            };
            ReplyKeyboard.ResizeKeyboard = true;
            await Bot.SendTextMessageAsync(message.Chat.Id, "Подтвердите действие", replyMarkup: ReplyKeyboard);
        }

        private static async void ShowTBA(Telegram.Bot.Types.Message message)
        {
            string answer = "В разработке";

            await Bot.SendTextMessageAsync(message.Chat.Id, answer);
        }
    }
}
