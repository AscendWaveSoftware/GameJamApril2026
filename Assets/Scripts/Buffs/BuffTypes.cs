using System;
using System.Collections.Generic;

public class BuffTypes
{
    List<Buff> buffTypes = new List<Buff>();

    public void Init()
    {
        buffTypes.Add(new AddDamage());
        buffTypes.Add(new AddMaxHealth());
        buffTypes.Add(new AuraBuff());
    }

    public Buff GetRandom()
    {
        Random rng = new Random();
        int index = rng.Next(buffTypes.Count);

        return buffTypes[index];
    }
}
