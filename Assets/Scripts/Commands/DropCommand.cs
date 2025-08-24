using System.Threading.Tasks;
using UnityEngine;
using Lua;

namespace Commands
{
    public class DropCommand : Command
    {
        protected override ValueTask<int> ExecuteCommand(Robot robot, LuaFunctionExecutionContext context)
        {
            robot.pickedUpItem.transform.SetParent(GameObject.Find("ItemManager").transform);
            robot.pickedUpItem = null;
            return new ValueTask<int>(0);
        }

        protected override bool CanExecute(Robot robot, LuaFunctionExecutionContext context)
        {
            if (!base.CanExecute(robot, context))
                return false;
            return robot.pickedUpItem != null;
        }
    }
}