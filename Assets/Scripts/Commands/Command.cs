using System.Threading.Tasks;
using Lua;

namespace Commands
{
    public abstract class Command
    {
        public abstract ValueTask<int> Execute(Robot robot, LuaFunctionExecutionContext context);
    
        public static void Register(string commandName, LuaState state, Robot robot, Command command)
        {
            state.Environment[commandName] = new LuaFunction((context, buffer, ct) => command.Execute(robot, context));
        }
    }    
}
