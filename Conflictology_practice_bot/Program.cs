using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var client = new TelegramBotClient("-----");

using var cts  = new CancellationTokenSource();
client.StartReceiving(UpdateAsync, ErrorAsync);

// bot launch
Console.WriteLine("Бот запущен");
Console.ReadLine();

async Task SendFirstCase(ITelegramBotClient botClient, long chatId, CancellationToken ct)
{
    var buttons = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("Личный разговор", "case1_private"),
                InlineKeyboardButton.WithCallbackData("Написать жалобу", "case1_claim") }
    });

await botClient.SendMessage(
    chatId: chatId,
    text: "Ваш коллега на общем собрании присвоил себе все заслуги за сложный проект, над которым вы работали вместе. Вы чувствуете острую несправедливость и понимаете, что это может повлиять на вашу будущую премию. В офисе между вами повисло напряжение, которое мешает текущим задачам. Вам нужно выбрать стратегию поведения для разрешения этого профессионального конфликта. Как вы поступите в первую очередь?",
    replyMarkup: buttons,
    cancellationToken: ct);
}

async Task SendSecondCase(ITelegramBotClient botClient, long chatId, CancellationToken ct)
{
    var buttons = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Поблагодарить", "case2_silent"),
                    InlineKeyboardButton.WithCallbackData("Личные границы", "case2_borders")
                }
            });
    await botClient.SendMessage(
        chatId: chatId,
        text: "Ваша мама без предупреждения приехала к вам домой, пока вы были на работе, и навела свой порядок: выкинула \"старые\" вещи, переставила мебель и поменяла шторы на те, что нравятся ей. Когда вы вернулись, она с гордостью сказала: \"У тебя было так неуютно, я решила сделать тебе сюрприз!\". Вы чувствуете гнев от нарушения личного пространства, но мама уверена, что проявила высшую форму любви. Как вы начнете этот непростой разговор: поблагодарите и промолчите или же наоборот, твёрдо заявите о личных границах?",
        replyMarkup: buttons,
        cancellationToken: ct);
}


async Task UpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
{ 
    // console writing: whom and what text in message 
    if (update.Message is { } msg) Console.WriteLine($"{msg.From?.Username} [MSG] | {msg.Text}");
    if (update.CallbackQuery is { } cb) Console.WriteLine($"{cb.From?.Username} [BTN] | {cb.Data}");
    var menuButton = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("В меню", "меню"));

    if (update.Message is { Text: { } messageText } message)
    {
        // first launch
        if (messageText.ToLower() == "/start")
        {
            await botClient.SendMessage(
                chatId: update.Message.Chat.Id,
                text: "Здравствуйте! \nВас приветствует чат - бот по вопросам конфликтологии.Данный ресурс разработан для ознакомления с практическими аспектами разрешения споров и анализа конфликтных ситуаций. \nДля начала работы воспользуйтесь следующими функциями: \nПервый кейс — разбор и анализ первой практической ситуации. \nВторой кейс — разбор и анализ второй практической ситуации. \nКлиника СПбГУ — получение ссылки на официальный сайт клиники медиации СПбГУ, на котором вы можете подробнее посмтреть про процессы медиации \nЕсли вам необходимо вернуться в начало, отправьте команду «Меню».",
                cancellationToken: ct);
            await botClient.SendMessage(update.Message.Chat.Id, "Хотите перейти в главное меню?", replyMarkup: menuButton);
            return;
        }

        //menu
        else if (update.Message.Text.ToLower().Contains("/menu") || update.Message.Text.ToLower().Contains("меню"))
        {
            await botClient.SendMessage(
                chatId: update.Message.Chat.Id,
                text: "Меню: \nПервый кейс - /first\nВторой кейс - /second \nКлиника СПбГУ - /SPBu_clinic\nИнформация о конфликтологии - /info",
                cancellationToken: ct);
        }
        //SPbU clinic 
        else if (update.Message.Text.ToLower().Contains("clinic") || update.Message.Text.ToLower().Contains("клиника"))
        {
            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Конечно, вот ссылка на клинику СПбГУ: https://mediation.spbu.ru/",
                cancellationToken: ct);
            await botClient.SendMessage(update.Message.Chat.Id, "Хотите вернуться в главное меню?", replyMarkup: menuButton);
        }

        // info about conflictology
        else if (update.Message.Text.ToLower().Contains("/info"))
        {
            await botClient.SendMessage(update.Message.Chat.Id, "Конфликтология - это наука о том, почему возникают споры и как превратить их из разрушительной ссоры в полезное решение. Она изучает не только открытые скандалы, но и скрытое напряжение между людьми в семье, на работе или в обществе. Вместо того чтобы просто искать виноватых, эта дисциплина предлагает конкретные инструменты: переговоры, посредничество и поиск общих интересов. \n\nПрактическая ценность заключается в следующем: \n\n Экономия ресурсов: предотвращение затяжных судов и сохранение рабочего времени. \n\nСохранение отношений: возможность решить проблему, не разрывая связи с близкими или коллегами. \n\nЛичный рост: умение сохранять самообладание и превращать «тупиковые» ситуации в точки развития.");
            await botClient.SendMessage(update.Message.Chat.Id, "Хотите вернуться в главное меню?", replyMarkup: menuButton);
        }

        // first case launch
        else if (update.Message.Text.ToLower().Contains("/first") || update.Message.Text.ToLower().Contains("первый"))
        { await SendFirstCase(botClient, message.Chat.Id, ct); }

        // second case launch
        else if (update.Message.Text.ToLower().Contains("/second") || update.Message.Text.ToLower().Contains("второй"))
        { await SendSecondCase(botClient, message.Chat.Id, ct); }

        // text to error messages
        else
        {
            await botClient.SendMessage(update.Message.Chat.Id, "Для работы бота нажиме /start");
        }
    }

    // first case 
    if (update.CallbackQuery is { } callbackQuery)
    {
        long chatId1 = callbackQuery.Message.Chat.Id;
        string choice = callbackQuery.Data;

        await botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);

        // first case & menu buttons
        var firstCaseButtons = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Первый кейс", "first_case_launch"),
                InlineKeyboardButton.WithCallbackData("Меню", "menu")
                }
            });

        // second case & menu buttons
        var secondCaseButtons = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Второй кейс", "second_case_launch"),
                    InlineKeyboardButton.WithCallbackData("Меню", "menu")
            }
        });

        // menu
        if (choice == "меню")
        {
            await botClient.SendMessage(
                chatId: chatId1,
                text: "Меню: \n/first - первый кейс\n/second - второй кейс\n/SPBu_clinic - клиника СПбГУ\n/info - информация о конфликтологии",
                cancellationToken: ct);
        }

        // first case launch
        else if (choice == "first_case_launch")
        {
            SendFirstCase(botClient, chatId1, ct);
        }

        // second case launch
        else if (choice == "second_case_launch")
        {
            SendSecondCase(botClient, chatId1, ct);
        }

        // first choice in first case & buttons
        else if (choice == "case1_private")
        {
            var retryButtons = new InlineKeyboardMarkup(new[]
            {
                new[] {InlineKeyboardButton.WithCallbackData("Просить исправиться", "case1_private1"),
                InlineKeyboardButton.WithCallbackData("Требовать письмо", "case1_claim")}
            });

            await botClient.SendMessage(
                chatId: chatId1,
                text: "Коллега начал оправдываться, что „просто разволновался и забыл упомянуть вас“. Что сделаете: обяжете его написать письмо-извинение или попросите коллегу исправиться?",
                replyMarkup: retryButtons,
                cancellationToken: ct);
        }

        else if (choice == "case1_private1")
        {
            await botClient.SendMessage(
                chatId: chatId1,
                text: "Вы решили поверить коллеге. Он действительно сдержал слово и более не доспускал подобных ситуаций.",
                cancellationToken: ct);
            await botClient.SendMessage(
                chatId: chatId1,
                text: "Вы завершили один из кейсов, Вы - молодец! Теперь предлагаю Вам отправиться в главное меню или начать следующий кейс.",
                replyMarkup: secondCaseButtons,
                cancellationToken: ct);
        }

        else if (choice == "case1_claim")
        {
            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Настаивать", "case1_end"),
                    InlineKeyboardButton.WithCallbackData("Извиниться", "case1_appologise")
                }
            });
            await botClient.SendMessage(
                chatId: chatId1,
                text: "Начальник вызвал вас обоих к себе. Коллега в шоке и теперь считает вас доносчиком. Что выберете: настаивать на своём или принесёте извинения вместе с коллегой публично?",
                replyMarkup: buttons,
                cancellationToken: ct);
        }

        else if (choice == "case1_appologise")
        {
            await botClient.SendMessage(
                chatId: chatId1,
                text: "В конце концов вы, вместе с коллегой, стоите перед всем коллективом и извиняетесь за саботаж рабочего процесса. Можно ли было сделать это менее заметно для всех? Да, конечно! Можете потренировать своё умение выходить из конфликтных ситуаций на следующем кейсе",
                cancellationToken: ct);
            await botClient.SendMessage(
                chatId: chatId1,
                text: "Поздравляю с разрешением первого кейса! А теперь можно перейти в меню по кнопке ниже",
                replyMarkup: secondCaseButtons,
                cancellationToken: ct);
        }

        else if (choice == "case1_end")
        {
            await botClient.SendMessage(
                chatId: chatId1,
                text: "В конце концов вы, вместе с коллегой, стоите перед всем коллективом и извиняетесь за саботаж рабочего процесса, но у вас также осталось сильное чувство неудовлетворённости, ведь правда так и не была раскрыта. Можно ли было сделать это менее заметно для всех? Да, конечно! Можете потренировать своё умение выходить из конфликтных ситуаций на следующем кейсе",
                cancellationToken: ct);
            await botClient.SendMessage(
                chatId: chatId1,
                text: "Поздравляю с разрешением первого кейса! А теперь можно перейти в меню по кнопке ниже",
                replyMarkup: secondCaseButtons,
                cancellationToken: ct);
        }

        // first choice in second case
        else if (choice == "case2_silent")
        {
            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Сменить замки", "case2_locks"),
                        InlineKeyboardButton.WithCallbackData("Разговор", "case2_talk_later")}
            });
            await botClient.SendMessage(
                chatId: chatId1,
                text: "Мама обрадовалась и теперь планирует приходить каждую неделю, чтобы \"помогать\" дальше. Вы чувствуете себя гостем в собственном доме. Что предпримете?",
                replyMarkup: buttons,
                cancellationToken: ct);
        }

        else if (choice == "case2_locks")
        {
            await botClient.SendMessage(chatId1, "Вы сменили замки. Мама в шоке, конфликт перерос в холодную войну. Проблема границ так и не решена конструктивно.\"", cancellationToken: ct);

            await botClient.SendMessage(chatId1, "Поздравляем, пусть и с не самым успешным, но завершением кейса. по кнопке ниже можете перейти в меню, или начать первый кейс.", replyMarkup: firstCaseButtons, cancellationToken: ct);
        }

        else if (choice == "case2_talk_later")
        {
            await botClient.SendMessage(chatId1, "Напряжение растет. Вы просто тянете время, и следующий взрыв будет гораздо болезненнее для обоих.", cancellationToken: ct);

            await botClient.SendMessage(chatId1, "Поздравляю с успешным завершением кейса! Теперь можете перейти в меню по кнопке ниже.", replyMarkup: menuButton, cancellationToken: ct);
        }

        else if (choice == "case2_borders")
        {
            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Настоять на своём", "case2_insist"),
                        InlineKeyboardButton.WithCallbackData("Оправдываться", "case2_surrender") }
            });
            await botClient.SendMessage(
                 chatId1,
                "Мама расплакалась и обвинила вас в неблагодарности: \"Я для тебя всё, а ты из-за тряпок скандал устраиваешь!\". Атмосфера накалена. Ваш следующий шаг?", replyMarkup: buttons);
        }

        else if (choice == "case2_insist")
        {
            await botClient.SendMessage(chatId1, "Вы обняли маму и сказали: 'Я ценю твою заботу, но мой дом - моё пространство'. Вы сохранили и отношения, и свои границы.", cancellationToken: ct);

            await botClient.SendMessage(chatId1, "Поздравляю с успешным завершением кейса! ниже будет кнопка для выхода в меню", replyMarkup: firstCaseButtons);
        }

        else if (choice == "case2_surrender")
        {
            await botClient.SendMessage(chatId1, "Вы сдались под эмоциональным давлением. Теперь ваша квартира — её площадка, а ваше мнение больше не учитывается.\"");

            await botClient.SendMessage(chatId1, "Данный кейс можно быть завершить успешнее. Можете попробовать ещё раз, или выйти в меню по кнопке ниже", replyMarkup: firstCaseButtons);
        }
    }   
}

async Task ErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            Console.WriteLine(exception.ToString());
        }
