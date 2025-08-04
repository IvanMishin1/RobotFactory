using System.Threading.Tasks;
using Lua;

namespace Commands
{
    public class StopCommand : Command
    {
        public override ValueTask<int> Execute(Robot robot, LuaFunctionExecutionContext context)
        {
            robot.busy = false;
            robot.stop = true;
            return new ValueTask<int>(0);
        }
    }
}