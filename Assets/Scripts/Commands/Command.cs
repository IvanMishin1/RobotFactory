using System.Threading.Tasks;
using Lua;

namespace Commands
{
    public abstract class Command
    {
        public async ValueTask<int> CallCommand(Robot robot, LuaFunctionExecutionContext context)
        {
            while (robot != null && robot.Pause)
            {
                await Task.Yield();
            }
            if (!CanExecute(robot, context) || robot == null || robot.Stop)
            {
                robot.DisplayToggleStatus("action_error");
                await Task.Delay(1000); 
                return 1;
            }
            return await ExecuteCommand(robot, context);
        }
        protected abstract ValueTask<int> ExecuteCommand(Robot robot, LuaFunctionExecutionContext context);
    
        public static void Register(string commandName, LuaState state, Robot robot, Command command)
        {
            state.Environment[commandName] = new LuaFunction((context, buffer, ct) => command.CallCommand(robot, context));
        }
        protected virtual bool CanExecute(Robot robot, LuaFunctionExecutionContext context)
        {
            return robot != null && !robot.Stop;
        }
    }    
}
