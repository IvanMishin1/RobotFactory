using System;
using System.Threading.Tasks;
using UnityEngine;
using Lua;

namespace Commands
{
    public class PickupCommand : Command
    {
        protected override ValueTask<int> ExecuteCommand(Robot robot, LuaFunctionExecutionContext context)
        {
            string itemType = null;
            if (context.ArgumentCount > 0)
                itemType = context.GetArgument<string>(0);
            if (Physics2D.OverlapCircle(robot.transform.position, 0.5f, LayerMask.GetMask("Items")) is Collider2D itemCollider)
            {
                Item item = itemCollider.GetComponent<Item>();
                if (!String.IsNullOrEmpty(itemType) && item.type != itemType && !item.transform.parent.CompareTag("Machine"))
                {
                    return new ValueTask<int>(1);
                }
                if (item != null)
                {
                    robot.pickedUpItem = item;
                    robot.pickedUpItem.transform.SetParent(robot.transform);
                    robot.pickedUpItem.transform.position = robot.transform.position;
                }
            }
            return new ValueTask<int>(0);
        }

        protected override bool CanExecute(Robot robot, LuaFunctionExecutionContext context)
        {
            if (!base.CanExecute(robot, context))
                return false;

            if (robot.pickedUpItem != null)
                return false;

            if (Physics2D.OverlapCircle(robot.transform.position, 0.5f, LayerMask.GetMask("Items")) == null)
                return false;

            return true;
        }
    }    
}
