﻿using Engine.Interfaces;

namespace Engine.Behaviors
{
    public class EatableBehavior : IBehavior
    {
        public int SatietyCoefficient { get; set; }

        public int Poisoness { get; set; }
        public int Time { get; set; }

      //  public Dictionary<ObjectStates.ObjectStates, EatableProps> EatableByStates;

        public EatableBehavior(int eatingCoefficient)
        {
            SatietyCoefficient = eatingCoefficient;
        }

        public EatableBehavior(int eatingCoefficient, int poisoness, int time)
        {
            SatietyCoefficient = eatingCoefficient;
            Poisoness = poisoness;
            Time = time;
        }
    }
    /*
    public class EatableProps
    {
        public int SatietyCoefficient { get; set; }
        public int Poisoness { get; set; }
        public int Time { get; set; }
    }*/
}
