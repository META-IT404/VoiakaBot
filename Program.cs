using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using VoiakaBot;
using System.IO;

namespace VoiakaBotBackEnd
{
    internal class Program
    {
        private static bool IsDelProduct = false;
        private static bool IsSetProduct = false;
        private static List<Product> products;
        private static List<Orders> orders;
        private static ITelegramBotClient _client;
        private static ReceiverOptions _receiverOptions;
        static async Task Main(string[] args)
        {
            _client = new TelegramBotClient("7130742648:AAFvkFe7DgbEGKUSSSWSwyHxgueR5oZtUak");
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                },
                ThrowPendingUpdates = false,

            };
            var cts = new CancellationTokenSource();

            _client.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

            var me = await _client.GetMeAsync();
            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1);
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
            try
            {
                // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
                switch (update.Type)
                {

                    case UpdateType.Message:

                        // эта переменная будет содержать в себе все связанное с сообщениями
                        var message = update.Message;

                        // From - это от кого пришло сообщение
                        var user = message.From;

                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                        Console.WriteLine($"{user.Username} ({user.Id}) написал сообщение: {message.Text}");

                        // Chat - содержит всю информацию о чате
                        var chat = message.Chat;

                        // Добавляем проверку на тип Message
                        switch (message.Type)
                        {
                            // Тут понятно, текстовый тип
                            case MessageType.Text:
                                {
                                    // тут обрабатываем команду /start, остальные аналогично
                                    if (message.Text == "/start" && user.Id == 971061398)
                                    {
                                        // Тут создаем нашу клавиатуру
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                        {
                                            new KeyboardButton[]
                                            {
                                                new KeyboardButton("Покажи видеокарты в наличие"),
                                                new KeyboardButton("Добавить видеокарт"),
                                                new KeyboardButton("Удаление продуктов"),
                                                //todo вернуть когда понадобиться база заказчиков
                                                //new KeyboardButton("Показать всех заказчиков"),
                                            },
                                        })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "Выберите действие!",
                                    replyMarkup: replyKeyboard);
                                    }
                                    else if (message.Text == "/start")
                                    {
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                        {
                                            new KeyboardButton[]
                                            {
                                                new KeyboardButton("Покажи видеокарты в наличие"),
                                                new KeyboardButton("Хочу консультацию"),
                                            },
                                        })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Выберите действие!",
                                        replyMarkup: replyKeyboard);
                                    }
                                    else if (message.Text == "Хочу консультацию")
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            $"{System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "\\ConsultationText.txt")}"
                                            );
                                    }    
                                    else if (message.Text == "Покажи видеокарты в наличие")
                                    {
                                        products = WorkingToDB.GetAllProduct().Result;
                                        List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
                                        for (int i = 0; i < products.Count; i++)
                                        {
                                            buttons.Add(new InlineKeyboardButton[]{ InlineKeyboardButton.WithCallbackData($"{products[i].Name}, {products[i].Price}, " +
                                                $"{products[i].WarrantyProduct}, {products[i].State}", $"{products[i].ID}") });
                                        }
                                        var inlineKeyboard = new InlineKeyboardMarkup(
                                        buttons.ToArray());

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Выберите продукт для заказа",
                                            replyMarkup: inlineKeyboard);

                                        return;
                                    }
                                    else if (message.Text == "Покажи видеокарты в наличие" && user.Id == 971061398)
                                    {
                                        products = WorkingToDB.GetAllProduct().Result;
                                        List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
                                        for (int i = 0; i < products.Count; i++)
                                        {
                                            buttons.Add(new InlineKeyboardButton[]{ InlineKeyboardButton.WithCallbackData($"{products[i].Name}, {products[i].Price}, Наличие: {products[i].QuantityProduct}, " +
                                                $"{products[i].WarrantyProduct}, {products[i].State}", $"{products[i].ID}") });
                                        }
                                        var inlineKeyboard = new InlineKeyboardMarkup(
                                        buttons.ToArray());

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Выберите продукт для заказа",
                                            replyMarkup: inlineKeyboard);

                                        return;
                                    }
                                    //else if (message.Text == "Показать всех заказчиков" && user.Id == 971061398)
                                    //{
                                    //    orders = WorkingToDB.GetOrders().Result;
                                    //    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                                    //    for (int i = 0; i < orders.Count; i++)
                                    //    {
                                    //        buttons.Add(InlineKeyboardButton.WithCallbackData($"{orders[i].Name}, {orders[i].NameProduct}, {orders[i].PriceProduct}, " +
                                    //            $"{orders[i].WarrantyProduct}, {orders[i].State}"
                                    //            , $"{orders[i].ID}"));
                                    //    }
                                    //    var inlineKeyboard = new InlineKeyboardMarkup(
                                    //    new List<InlineKeyboardButton[]>()
                                    //{
                                    //    buttons.ToArray()
                                    //    });

                                    //    await botClient.SendTextMessageAsync(
                                    //        chat.Id,
                                    //        "Выберите заказчика для удаления",
                                    //        replyMarkup: inlineKeyboard);

                                    //    return;
                                    //}
                                    else if (message.Text == "Добавить видеокарты" && user.Id == 971061398)
                                    {
                                        IsSetProduct = true;
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                        {
                                            new KeyboardButton[]
                                            {
                                                new KeyboardButton("Отменить добавление видеокарты")
                                            },
                                        })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "Активирован режим добавление товара отправьте имя продукта,цену,количество,срок гарантии,состояние",
                                    replyMarkup: replyKeyboard);
                                    }
                                    else if (user.Id == 971061398 && IsSetProduct)
                                    {
                                        string[] strings = message.Text.Split(',');
                                        if (strings == null || strings.Length != 5)
                                        {
                                            await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "Не верный формат\nФормат должен быть: имя продукта,цену,количество,срок гарантии,состояние\nс учётом запятых");
                                            return;
                                        }
                                        IsSetProduct = false;
                                        WorkingToDB.SetProduct(strings[0], strings[1], Convert.ToInt32(strings[2]), strings[3], strings[4]);
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
                                            new KeyboardButton[]
                                                {
                                                new KeyboardButton("Покажи видеокарты в наличие"),
                                                new KeyboardButton("Добавить видеокарту"),
                                                 new KeyboardButton("Удаление продуктов"),
                                                //new KeyboardButton("Показать всех заказчиков"),
                                                },
                                            })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        await botClient.SendTextMessageAsync(
                                         chat.Id,
                                        "Выберите действие!",
                                        replyMarkup: replyKeyboard);
                                    }
                                    else if (message.Text == "Отменить добавление видеокарты" && user.Id == 971061398)
                                    {
                                        IsSetProduct = false;
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                                new List<KeyboardButton[]>()
                                                {
                                            new KeyboardButton[]
                                                {
                                                new KeyboardButton("Покажи видеокарты в наличие"),
                                                new KeyboardButton("Добавить видеокарты"),
                                                 new KeyboardButton("Удаление видеокарты"),
                                                 //todo вернуть когда понадобиться база заказчиков
                                                //new KeyboardButton("Показать всех заказчиков"),
                                                },
                                                })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        await botClient.SendTextMessageAsync(
                                         chat.Id,
                                        "Выберите действие!",
                                        replyMarkup: replyKeyboard);
                                    }
                                    else if (message.Text == "Удаление видеокарты" && user.Id == 971061398)
                                    {
                                        IsDelProduct = true;
                                        products = WorkingToDB.GetAllProduct().Result;
                                        List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
                                        for (int i = 0; i < products.Count; i++)
                                        {
                                            buttons.Add(new InlineKeyboardButton[]{ InlineKeyboardButton.WithCallbackData($"{products[i].Name}, {products[i].Price}, {products[i].QuantityProduct}, " +
                                                $"{products[i].WarrantyProduct}, {products[i].State}", $"{products[i].ID}") });
                                        }
                                        var inlineKeyboard = new InlineKeyboardMarkup(
                                        
                                        buttons.ToArray()
                                        );

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Выберите видеокарту для удаления",
                                            replyMarkup: inlineKeyboard);

                                        return;
                                    }

                                }
                                break;

                        }
                        break;
                    case UpdateType.CallbackQuery:
                        var callbackQuery = update.CallbackQuery;

                        // Аналогично и с Message мы можем получить информацию о чате, о пользователе и т.д.
                        var user1 = callbackQuery.From;

                        // Выводим на экран нажатие кнопки
                        Console.WriteLine($"{user1.FirstName} ({user1.Id}) нажал на кнопку: {callbackQuery.Data}");

                        // Вот тут нужно уже быть немножко внимательным и не путаться!
                        // Мы пишем не callbackQuery.Chat , а callbackQuery.Message.Chat , так как
                        // кнопка привязана к сообщению, то мы берем информацию от сообщения.
                        var chat1 = callbackQuery.Message.Chat;
                        if (user1.Id == 971061398 && IsDelProduct)
                        {
                            IsDelProduct = false;
                            for (int i = 0; i < products.Count; i++)
                            {
                                if (callbackQuery.Data == products[i].ID.ToString())
                                {
                                    WorkingToDB.DelProduct(products[i].ID);
                                }
                            }
                            return;
                        }
                        for (int i = 0; i < products.Count; i++)
                        {
                            if (callbackQuery.Data == products[i].ID.ToString())
                            {
                                string order = WorkingToDB.BuyProduct(products[i].ID, user1.Username).Result;
                                await botClient.SendTextMessageAsync(
                                            chat1.Id,
                                            $"{System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "\\MessageOrders.txt").Replace("order", order)}"
                                            );
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
