using System.Threading.Tasks;
using Lua;

namespace Commands
{
    public class StopCommand : Command
    {
        protected override ValueTask<int> ExecuteCommand(Robot robot, LuaFunctionExecutionContext context)
        {
            robot.busy = false;
            robot.Stop = true;
            return new ValueTask<int>(0);
        }
    }
}