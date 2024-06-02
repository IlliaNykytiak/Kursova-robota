using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WebApplication1.Clients;
using WebApplication1.Models;

namespace WebApplication1.Telegram
{
    public class telegram_bot
    {
        private readonly IMemoryCache _cache;
        Dictionary<long, (int start_year, int end_year, int min_imdb, int max_imdb)> chatParameters = new Dictionary<long, (int, int, int, int)>();
        private ITelegramBotClient? botClient;
        private static string? _botApiKey;

        public telegram_bot(IMemoryCache cache)
        {
            _cache = cache;
        }

        public telegram_bot()
        {
            _botApiKey = Constants.TelegramBotApiKey;
            botClient = null;
        }

        public async void InitializeBot()
        {
            botClient = new TelegramBotClient(_botApiKey);
            botClient.StartReceiving(updateHandler: HandleUpdateAsync, pollingErrorHandler: HandlePollingErrorAsync, receiverOptions: receiverOptions);
            var me = await botClient.GetMeAsync();
            Console.ReadLine();
        }
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (update.Message is not { } message1)
                return;
            if (message.Text is not { } messageText)
                return;
            if (message?.Text != null)
            {
                if (message.Text.Equals("/start"))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: "Привіт! Я бот, який допоможе тобі знайти фільми, які ти зможеш додати до списку \"Хочу переглянути\". Щоб почати, введи /enterValues."
                    );
                    return;
                }
                if (message.Text.StartsWith("/enterValues"))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: "Напиши початковий рік випуску фільму, кінцевий рік випуску фільму, мінімальний рейтинг imdb та максимальний рейтинг imdb в форматі: **** **** * *. Щоб продовжити, введіть /searchList"
                    );
                    return;
                }
                if (message.Text.StartsWith("/searchList"))
                {
                    var movieClient = new MovieClient();
                    if (message.Type == MessageType.Text)
                    {
                        var parts = message.Text.Split(' ');
                        if (parts.Length == 5
                            && int.TryParse(parts[1], out int start_year)
                            && int.TryParse(parts[2], out int end_year)
                            && int.TryParse(parts[3], out int min_imdb)
                            && int.TryParse(parts[4], out int max_imdb))
                        {
                            chatParameters[message.Chat.Id] = (start_year, end_year, min_imdb, max_imdb);
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat,
                                text: $"Параметри встановлені на: початковий рік {start_year}, кінцевий рік {end_year}, мінімальний рейтинг IMDB {min_imdb}, максимальний рейтинг IMDB {max_imdb}."
                            );
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat,
                                text: "Введіть команду у форматі: /enterValues start_year end_year min_imdb max_imdb"
                            );
                            return;
                        }
                    }
                    var parameters = chatParameters[message.Chat.Id];
                    var movieList = await movieClient.GetMovieListIMDBRating(parameters.start_year, parameters.end_year, parameters.min_imdb, parameters.max_imdb);
                    _cache.Set("MovieList", movieList);
                    var titles = movieList.results.Select(m => m.title).ToList();
                    var titlesString = string.Join("\n", titles);

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: titlesString
                    );
                    return;
                }
                if (message.Text.StartsWith("/getMovie"))
                {
                    var movieList = _cache.Get<MovieList>("MovieList");
                    var title = message.Text.Substring(10);
                    if (movieList != null && movieList.results != null)
                    {
                        foreach (var result in movieList.results)
                        {
                            if (result.title == title)
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat,
                                    text: $"Назва: {result.title}\nЖанр: {string.Join(", ", result.genre)}\nРік випуску: {result.released}\nРейтинг IMDB: {result.imdbrating}\nСинопсис: {result.synopsis}"
                                );
                                return;
                            }
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat,
                            text: "Movie list is not available."
                        );
                    }
                    return;
                }
            }
        }
        private async Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

    }
    public class TelegramBotHostedService : IHostedService
    {
        private readonly IMemoryCache _cache;
        telegram_bot bot = new telegram_bot();

        public TelegramBotHostedService(IMemoryCache cache)
        {
            _cache = cache;
            bot = new telegram_bot(_cache);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            bot.InitializeBot();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Add code here to handle what should happen when your service stops.
            return Task.CompletedTask;
        }
    }
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddHostedService<TelegramBotHostedService>();
            // Add services to the container.
        }
        public static void Configure(WebApplication app)
        {
            // Configure the HTTP request pipeline.
        }
    }
}
