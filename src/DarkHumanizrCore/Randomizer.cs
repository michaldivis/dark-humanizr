namespace DarkHumanizrCore;
internal class Randomizer
{
    private readonly Random _random;

    public Randomizer(Random random)
    {
        _random = random;
    }

    public int RandomizeVelocity(int initial, int subtractLimit, int addLimit)
    {
        var randomized = _random.Next(initial - subtractLimit, initial + addLimit);

        if (randomized < MidiVelocity.Min)
        {
            return MidiVelocity.Min;
        }

        if (randomized > MidiVelocity.Max)
        {
            return MidiVelocity.Max;
        }

        return randomized;
    }
}
