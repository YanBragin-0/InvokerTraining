using Contracts;
using MassTransit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TelegramConsumer
{
    public class TelegramConsumer(TelegramService service) : IConsumer<RecordSet>
    {
        private readonly TelegramService telegramService = service;
        public async Task Consume(ConsumeContext<RecordSet> context)
        {
            var msg = context.Message;
            string message = @$"*Новый рекорд!*
                👤 Игрок: `{msg.AccountName}`
                ⏱ Время: *{msg.Time:mm\:ss\.ff}*
                📅 Дата: {msg.When:dd.MM.yyyy} в {msg.When:HH:mm}";
            await telegramService.SendAsync(message);
        }
    }
}
