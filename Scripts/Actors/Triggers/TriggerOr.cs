using System.Linq;

public class TriggerOr : TriggeringBase
{
    public ushort[] triggerings = new ushort[1];

    public override void DataLoaded(string s, string beforeEqual)
    {
        triggerings = LevelLoader.CreateVariable(s, beforeEqual, "triggeredIds", triggerings);
        base.DataLoaded(s, beforeEqual);
    }

    public override void TriggerTick()
    {
        if (triggerings.Select(x => IsTargetActive(x)).Where(y => y == true).ToArray().Length > 0)
            SetTargetBoolean(activate);
    }
    public override void ActivatedTick() { return; }
}
