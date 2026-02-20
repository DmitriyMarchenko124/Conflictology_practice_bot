using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var bot = new TelegramBotClient("7532570784:AAGM8K-PWhoXsoscbRD5Kxaa-YGQSHO2hDk");

using var cts  = new CancellationTokenSource();
bot.StartReceiving(UpdateAsync, ErrorAsync, cancellationToken: cts.Token);

Console.WriteLine("Бот запущен");
Console.ReadLine();

async Task UpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
{
    if (update.Message is not { Text: { } messageText } message)
        return;
    {
        Console.WriteLine($"{message.From.Username}  |  {message.Text}");
    }

    if (update.CallbackQuery != null)
    {
        string choice = update.CallbackQuery.Data;
        await botClient.SendMessage(update.CallbackQuery.Message.Chat.Id, $"Вы выбрали решение {choice}");
        return;
    }

    if (update.Message is { Text: not null } message1)
    {
        string situation = "Вы нашли кошелёк на улице, что будете делать?";

        var buttons = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Отдать полиции", "Полиция"),
                InlineKeyboardButton.WithCallbackData("Оставить себе", "Себе")
            }
        });

        await botClient.SendMessage(
            chatId: message.Chat.Id, 
            text: situation,
            replyMarkup: buttons,
            cancellationToken: default
        ); 

    }
}

async Task ErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
{
    Console.WriteLine(exception.ToString());
}