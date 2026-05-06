using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace TelegramConsumer
{
    public class TelegramService
    {
        private readonly TelegramBotClient telegramBot;
        private readonly IConfiguration _configuration;
        public TelegramService(IConfiguration configuration)
        {
            var token = configuration.GetSection("Telegram:Token").Get<string>()!;
            telegramBot = new TelegramBotClient(token);
            _configuration = configuration;
        }
        public async Task SendAsync(string message)
        {
            var id = _configuration.GetSection("Telegram:ChatId").Get<string>()!;
            await telegramBot.SendMessage(id,message);
        }

    }
}
