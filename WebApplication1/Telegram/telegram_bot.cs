using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebApplication1.Telegram
{
    public class telegram_bot
    {
        private ITelegramBotClient botClient;
        private static string _botApiKey;
        public telegram_bot()
        {
            _botApiKey = Constants.TelegramBotApiKey;
        }
        public async void InitializeBot()
        {
            botClient = new TelegramBotClient(_botApiKey);
            var me = await botClient.GetMeAsync();
            botClient.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        private async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                await botClient.SendTextMessageAsync(
                  chatId: message.Chat,
                  text: "You said:\n" + message.Text
                );
            }
        }

        private async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        //private async void Bot_OnMessage(object sender, MessageEventArgs e)
        //{
        //    if (e.Message.Text != null)
        //    {
        //        Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

        //        await botClient.SendTextMessageAsync(
        //          chatId: e.Message.Chat,
        //          text: "You said:\n" + e.Message.Text
        //        );
        //    }
        //}
    }
    public class TelegramBotHostedService : IHostedService
    {
        telegram_bot bot = new telegram_bot();
        public Task StartAsync(CancellationToken cancellationToken)
        {
            bot.InitializeBot();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // If you have any cleanup to do when your application stops, do it here.
            return Task.CompletedTask;
        }
    }
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<TelegramBotHostedService>();
            // Add services to the container.
        }
        public static void Configure(WebApplication app)
        {
            // Configure the HTTP request pipeline.
        }
    }
}
