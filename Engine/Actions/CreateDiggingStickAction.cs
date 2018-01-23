﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Heros;
using Engine.Interfaces.IActions;
using Engine.Objects;
using Engine.Objects.Tool;
using Engine.Resources;
using Engine.Tools;

namespace Engine.Actions
{
    public class CreateDiggingStickAction : IAction
    {
        public string Name => ActionsResource.CreateDiggingStick;

        public Knowledges GetKnowledge()
        {
            return Knowledges.Nothing;
        }

        public string GetName(IEnumerable<GameObject> objects)
        {
            return Name;
        }

        public bool IsApplicable(Property property)
        {
            return Property.Cutter == property || Property.Branch == property;
        }

        public IActionResult Do(Hero hero, IEnumerable<GameObject> objects)
        {
            var branch = objects.SingleOrDefault(o => o is Branch);
            var stone = objects.SingleOrDefault(ao => ao.Properties.Contains(Property.Cutter));

            if (branch == null || stone == null)
                return new FinishedActionResult();

            branch.RemoveFromContainer();
            var diggingStick = new DiggingStick();

            Game.AddToGame(hero, diggingStick);

            return new FinishedActionResult();
        }

        public bool CanDo(Hero hero, IEnumerable<GameObject> objects)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<List<GameObject>> GetActionsWithNecessaryObjects(IEnumerable<GameObject> objects, Hero hero)
        {
            var allObjects =
                objects.Union(hero.GetContainerItems()).Distinct();

            var branch = allObjects.FirstOrDefault(ao => ao is Branch);
            var stone = allObjects.FirstOrDefault(ao => ao.Properties.Contains(Property.Cutter));

            if (branch != null && stone != null)
            {
                yield return new List<GameObject> { branch, stone };
            }
        }

        public double GetTiredness()
        {
            return 2;
        }

        public Point GetDestination(Point destination, FixedObject destObject, Hero hero)
        {
            return destination;
        }
    }
}
