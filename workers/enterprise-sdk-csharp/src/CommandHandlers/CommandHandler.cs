using Improbable.Worker;

namespace EnterpriseSDK.CommandHandlers
{
    public interface CommandHandler<TCommand>
            where TCommand : ICommandMetaclass, new() {
        void Handle(CommandRequestOp<TCommand> op);
    }
}
