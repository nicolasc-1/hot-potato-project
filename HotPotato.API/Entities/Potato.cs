namespace HotPotato.API.Entities;

public class Potato
{
    public int TimeToLive { get; set; } // number of throws before it gets dropped
    public int ThrowDelay { get; set; }

    public void Tick()
    {
        this.TimeToLive--;
    }
}