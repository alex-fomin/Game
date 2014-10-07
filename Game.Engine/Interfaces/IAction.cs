﻿namespace Game.Engine.Interfaces.IActions
{
    using System.Collections.Generic;
    using Heros;
    using Objects;
    using Wrapers;
    
    public interface IAction
    {
        string Name { get; }

        bool IsApplicable(Property property);

        bool Do(Hero hero, IEnumerable<GameObject>objects);

        bool CanDo(Hero hero, IEnumerable<GameObject> objects);
        IEnumerable<List<GameObject>> GetActionsWithNecessaryObjects(IEnumerable<GameObject> objects, Hero hero);
    }
}
