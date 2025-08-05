using System.Threading.Tasks;
using Lua;

namespace Commands
{
    public class PauseCommand : Command
    {
        protected override ValueTask<int> ExecuteCommand(Robot robot, LuaFunctionExecutionContext context)
        {
            robot.busy = true;
            robot.Pause = true;
            return new ValueTask<int>(0);
        }
    }
}