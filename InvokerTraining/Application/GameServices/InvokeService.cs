namespace InvokerTraining.Application.GameServices
{
    public class InvokeService
    {
        private readonly Dictionary<string, string> _Spells = new Dictionary<string,string>{
            ["qqq"] = "Cold Snap",
            ["qqw"] = "Ghost Walk",
            ["eqq"] = "Ice Wall",
            ["eee"] = "Sun Strike",
            ["eeq"] = "Forge Spirit",
            ["eew"] = "Chaos Meteor",
            ["www"] = "E.M.P",
            ["eww"] = "Alacrity",
            ["qww"] = "Tornado",
            ["eqw"] = "Deafening Blast"
        };
        private readonly Random _Random = new Random();
        public string RandomSpell()
        {
            var val = _Random.Next(0,10);
            var spell = (_Spells.ToList())[val];
            return spell.Value;
        }
        public bool CheckValidSpell(string combination,string expectedSpell)
        {
            var spell = _Spells.FirstOrDefault(s => s.Key == combination);
            if(spell.Value == expectedSpell)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
        
}
