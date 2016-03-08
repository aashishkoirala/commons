using System.Text;
using System.Threading.Tasks;

namespace AK.Commons.Commands
{
    public interface ICommand
    {
        CommandDefinition Definition { get; }
        CommandState State { get; set; }
        CommandParameters Parameters { get; set; }
    }
}