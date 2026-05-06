using Contracts;
using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.DataTransfers;
using InvokerTraining.Infrastructure.Redis;
using InvokerTraining.Models.Entities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
namespace InvokerTraining.Application.GameServices
{
    public class InvokeHub(InvokeService service,
        IServiceScopeFactory scopeFactory) : Hub
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = scopeFactory;
        private readonly InvokeService _Service = service;
        private List<string> Combination = new();
        private readonly Stopwatch _Timer = new();
        private int SpellSCounter = 0;
        public async Task SendKey(string key,string target)
        {
            if (key.ToLower() == "r")
            {
                if (Combination.Count < 3) { return; }
                bool isTrue = _Service.CheckValidSpell(string.Concat(Combination.OrderBy(x => x)), target);
                if (isTrue)
                {
                    Combination.Clear();
                    if(SpellSCounter == 9)
                    {
                        _Timer.Stop();
                        await this.Finish(_Timer.Elapsed.TotalSeconds);
                        return;
                    }
                    SpellSCounter++;
                    string nextTarget = _Service.RandomSpell();
                    await Clients.Caller.SendAsync("NextSpell", nextTarget);
                }
                else
                {
                    await Clients.Caller.SendAsync("ResultReceived", false);
                }
            }
            else
            {
                if (Combination.Count >= 3) Combination.RemoveAt(0);
                Combination.Add(key);
                await Clients.Caller.SendAsync("UpdateVisualSpheres",Combination);
            }
        }
        public async Task StartGame()
        {
            Combination.Clear();
            SpellSCounter = 0;
            _Timer.Restart();
            var firstTarget = _Service.RandomSpell();
            await Clients.Caller.SendAsync("NextSpell",firstTarget);
        }
        public async Task Finish(double time)
        {
            SpellSCounter = 0;
            await Clients.Caller.SendAsync("FinishGame", time);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IPlayerRepository _playerRepository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
                ICurrentUser _currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
                IGameSessionRepository _sessionRepository = scope.ServiceProvider.GetRequiredService<IGameSessionRepository>();
                ICacher cacher = scope.ServiceProvider.GetRequiredService<ICacher>();
                IPublishEndpoint _publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                Guid? guid = _currentUser.CurrentUserID;
                if (guid == null)
                {
                    return;
                }
                var currentPlayer = await _playerRepository.GetByIdAsync((Guid)guid);
                bool isNewRecord = false;
                var currentTime = TimeSpan.FromSeconds(time);
                if (currentPlayer != null)
                {
                    currentPlayer.GameCount++;
                    if(currentPlayer.Record == null || currentTime < currentPlayer.Record)
                    {
                        isNewRecord = true;
                        currentPlayer.Record = currentTime;
                    }
                    var bestRecord = await cacher.Get<TimeSpan?>("best");
                    if(bestRecord == null)
                    {
                        bestRecord = _playerRepository.GetBestByRecord();
                        if (bestRecord == null)
                        {
                            bestRecord = currentTime;
                            isNewRecord = true;
                        }
                        await cacher.Set("best",bestRecord,TimeSpan.FromDays(30));
                    }
                    if (currentTime < bestRecord) 
                    { 
                        isNewRecord = true;
                        await cacher.Set("best", currentTime, TimeSpan.FromDays(30));
                    }
                    var session = new GameSession(currentPlayer.Id, currentTime);
                    await _sessionRepository.AddAsync(session);
                }
                if (isNewRecord)
                {
                    await _publishEndpoint.Publish<RecordSet>(new RecordSet 
                    { 
                        PlayerId = currentPlayer!.Id,
                        AccountName = currentPlayer.PhoneOrEmail,
                        Time = currentPlayer.Record,
                        When = DateTime.UtcNow
                    });
                }
            }
        }
    }
        
}
