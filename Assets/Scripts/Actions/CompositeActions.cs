using System;
using System.Collections.Generic;

namespace Actions
{
    public static class CompositeActions
    {
        private static void runAndActions(List<Action<Action>> actions, int index, Action callback)
        {
            if (index >= actions.Count)
            {
                callback?.Invoke();
                return;
            }

            Action cb = () => runAndActions(actions, index + 1, callback);
            actions[index]?.Invoke(cb);
        }
        public static Action<Action> GetAndAction(List<Action<Action>> actions)
        {
            return (Action cb) => runAndActions(actions, 0, cb);
        }

        private static void runRandomAction(List<Action<Action>> actions, System.Random random, Action callback)
        {
            bool called = false;
            if (actions != null && actions.Count > 0)
            {
                Action<Action> action = actions[random.Next(actions.Count)];
                if (action != null)
                {
                    action.Invoke(callback);
                    called = true;
                }
            }
            if (!called)
            {
                callback?.Invoke();
            }
        }

        public static Action<Action> GetRandomAction(List<Action<Action>> actions, System.Random random)
        {
            return (Action cb) => runRandomAction(actions, random, cb);
        }
    }
}
