using System.Threading.Tasks;
using Lua;

namespace Commands
{
    public class WaitCommand : Command
    {
        protected override ValueTask<int> ExecuteCommand(Robot robot, LuaFunctionExecutionContext context)
        {
            return WaitCoroutine(context);
            async ValueTask<int> WaitCoroutine(LuaFunctionExecutionContext ctx)
            {
                float seconds = 1f;
                if (ctx.ArgumentCount > 0)
                    seconds = (float)ctx.GetArgument<double>(0);
                await Task.Delay((int)(seconds * 1000));
                return 0;
            }
        }
    }
}