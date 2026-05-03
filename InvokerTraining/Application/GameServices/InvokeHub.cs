using InvokerTraining.Application.Abstractions;
using InvokerTraining.Models.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
namespace InvokerTraining.Application.GameServices
{
    public class InvokeHub(InvokeService service,IServiceScopeFactory scopeFactory) : Hub
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
                Guid? guid = _currentUser.CurrentUserID;
                if (guid == null)
                {
                    return;
                }
                var currentPlayer = await _playerRepository.GetByIdAsync((Guid)guid);
                if (currentPlayer != null)
                {
                    currentPlayer.GameCount++;
                    if(currentPlayer.Record == null)
                    {
                        currentPlayer.Record = TimeSpan.FromSeconds(time);
                    }
                    else if (currentPlayer.Record > TimeSpan.FromSeconds(time))
                    {
                        currentPlayer.Record = TimeSpan.FromSeconds(time);
                    }
                    var session = new GameSession(currentPlayer.Id, TimeSpan.FromSeconds(time));
                    await _sessionRepository.AddAsync(session);
                }
            }
        }
    }
        
}
