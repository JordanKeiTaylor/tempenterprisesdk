using Improbable.Worker;

namespace WorkerAPIFacade.CommandHandlers
{
    public interface CommandHandler<TCommand>
            where TCommand : ICommandMetaclass, new() {
        void Handle(CommandRequestOp<TCommand> op);
    }
}
