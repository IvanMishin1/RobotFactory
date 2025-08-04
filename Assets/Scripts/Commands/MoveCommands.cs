using System.Threading.Tasks;
using UnityEngine;
using Lua;

namespace Commands
{
    public class MoveCommands : Command
    {
        private Vector2 direction;

        public MoveCommands(Vector2 direction)
        {
            this.direction = direction;
        }

        public override ValueTask<int> Execute(Robot robot, LuaFunctionExecutionContext context)
        {
            return MoveInDirectionAsync(robot, direction);
        }
        
        private async ValueTask<int> MoveInDirectionAsync(Robot robot, Vector2 dir)
        {
            Vector2 movePosition = (Vector2)robot.transform.position + dir;
            Collider2D collision = Physics2D.OverlapCircle(movePosition, 0.4f, LayerMask.GetMask("Walls", "Machines"));
            if (collision != null)
                return 1;

            robot.animator.SetInteger("x_movement", (int)dir.x);
            robot.animator.SetInteger("y_movement", (int)dir.y);

            if (dir.y == 1f && robot.facingDirection == -1)
                robot.facingDirection = 1;
            else if (dir.y == -1f && robot.facingDirection == 1)
                robot.facingDirection = -1;

            if (dir.x == 1f && robot.facingDirection == -1)
                robot.spriteRenderer.flipX = true;
            else if (dir.x == -1f && robot.facingDirection == -1)
                robot.spriteRenderer.flipX = false;
            else if (dir.x == -1f && robot.facingDirection == 1)
                robot.spriteRenderer.flipX = true;
            else if (dir.x == 1f && robot.facingDirection == 1)
                robot.spriteRenderer.flipX = false;

            if (dir.y == 1f)
                robot.spriteRenderer.sortingOrder = 3;
            else if (dir.y == -1f)
                robot.spriteRenderer.sortingOrder = 1;

            while (Vector2.Distance(robot.transform.position, movePosition) >= 0.01f)
            {
                robot.transform.position = Vector2.MoveTowards(robot.transform.position, movePosition, Time.deltaTime * robot.moveSpeed);
                await Task.Yield();
            }
            robot.transform.position = movePosition;
            robot.animator.SetInteger("x_movement", 0);
            robot.animator.SetInteger("y_movement", 0);
            return 0;
        }
    }
}