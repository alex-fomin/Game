﻿using System.Collections.Generic;
using System.Linq;
using Game.Engine.Heros;
using Game.Engine.Interfaces;
using Game.Engine.Interfaces.IActions;
using Game.Engine.Objects;

namespace Game.Engine.Actions
{
    internal class EatAction : IAction
    {
        public string Name {
            get { return "Eat"; }
        }

        public string GetName(IEnumerable<GameObject> objects)
        {
            return Name;
        }

        public bool IsApplicable(Property property)
        {
            return property == Property.Eatable;
        }

        public bool Do(Hero hero, IEnumerable<GameObject> objects)
        {
            foreach (var removableObject in objects.OfType<IEatable>())
            {
                hero.Eat(removableObject.Satiety);
                (removableObject as GameObject).RemoveFromContainer();
            }

            return true;
        }

        public bool CanDo(Hero hero, IEnumerable<GameObject> objects)
        {
            return objects.All(x => x.Properties.Contains(Property.Eatable));
        }

        public IEnumerable<List<GameObject>> GetActionsWithNecessaryObjects(IEnumerable<GameObject> objects, Hero hero)
        {
            yield return objects.Where(x => x.Properties.Contains(Property.Eatable)).ToList();
        }
    }
}
