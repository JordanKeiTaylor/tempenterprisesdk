using Improbable.Worker;

namespace Improbable.Enterprise.CommandHandlers
{
    public interface CommandHandler<TCommand>
            where TCommand : ICommandMetaclass, new() {
        void Handle(CommandRequestOp<TCommand> op);
    }
}
