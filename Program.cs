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

namespace VoiakaBotBackEnd
{
    internal class Program
    {
        private static List<Product> products;
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

        [Obsolete]
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
                                                new KeyboardButton("Покажи всё что в наличие"),
                                                new KeyboardButton("Добавить товар"),
                                                new KeyboardButton("Показать всех заказчиков"),
                                                new KeyboardButton("Удалить заказчика")
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
                                                new KeyboardButton("Покажи всё что в наличие"),
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
                                    else if (message.Text == "Покажи всё что в наличие")
                                    {
                                        products = WorkingToDB.GetAllProduct().Result;
                                        List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                                        for (int i = 0; i < products.Count; i++)
                                        {
                                            buttons.Add(InlineKeyboardButton.WithCallbackData($"{products[i].Name} {products[i].Price} {products[i].QuantityProduct}" +
                                                $"{products[i].WarrantyProduct}", $"{products[i].ID}"));
                                        }
                                        var inlineKeyboard = new InlineKeyboardMarkup(
                                    new List<InlineKeyboardButton[]>()
                                    {
                                        buttons.ToArray()
                                    });

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Выберите продукт для заказа",
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
                        for (int i = 0; i < products.Count; i++)
                        {
                            if (callbackQuery.Data == products[i].ID.ToString())
                            {
                                string order = WorkingToDB.BuyProduct(products[i].ID, user1.Username).Result;
                                await botClient.SendTextMessageAsync(
                                            chat1.Id,
                                            $"Заказ создан на {order}\nДля уточнения статуса заказа пишите хозяину бота @VoiakaGM\nУкажите что вы заказали"
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
