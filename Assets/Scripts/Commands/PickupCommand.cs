using System;
using System.Threading.Tasks;
using UnityEngine;
using Lua;

namespace Commands
{
    public class PickupCommand : Command
    {
        public override ValueTask<int> Execute(Robot robot, LuaFunctionExecutionContext context)
        {
            string itemType = null;
            if (robot.pickedUpItem != null)
                return new ValueTask<int>(1);
            if (context.ArgumentCount > 0)
                itemType = context.GetArgument<string>(0);
            if (Physics2D.OverlapCircle(robot.transform.position, 0.5f, LayerMask.GetMask("Items")) is Collider2D itemCollider)
            {
                Item item = itemCollider.GetComponent<Item>();
                if (!String.IsNullOrEmpty(itemType) && item.itemType != itemType && !item.transform.parent.CompareTag("Machine"))
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
    }    
}
