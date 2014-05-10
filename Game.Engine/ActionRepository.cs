﻿using System;
using System.Collections.Generic;
using System.Linq;
//using System.Reflection;
using System.Reflection;
using Game.Engine.Actions;
using Game.Engine.Interfaces;
using Game.Engine.Interfaces.IActions;
using Game.Engine.Objects;

namespace Game.Engine
{
    class ActionRepository : IActionRepository
    {
        public ActionRepository()
        {
            Assembly.GetExecutingAssembly();
        }
        List<IAction> actions = new List<IAction>()
        {
            new CollectBerriesAction(),
            new CutAction(),
            new PickAction(),
            new CollectBranchAction(),
            new EatAction()
        };

        public IEnumerable<IAction> GetPossibleActions(GameObject gameObject)
        {
            var properties = gameObject.Properties;

            var result = from action in actions
                         from property in properties
                         where action.IsApplicable(property)
                         select action;

            return result.Distinct();

        }
    }
}
