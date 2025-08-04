using System.Threading.Tasks;
using UnityEngine;
using Lua;

namespace Commands
{
    public class DropCommand : Command
    {
        public override ValueTask<int> Execute(Robot robot, LuaFunctionExecutionContext context)
        {
            if (robot.pickedUpItem != null)
            {
                robot.pickedUpItem.transform.SetParent(GameObject.Find("Items").transform);
                robot.pickedUpItem = null;
            }
            return new ValueTask<int>(0);
        }
    }
}