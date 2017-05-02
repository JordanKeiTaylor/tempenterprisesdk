using Improbable.Worker;

namespace WorkerAPIFacade.CommandHandlers
{
    public abstract class BaseCommandHandler<TCommand> : CommandHandler<TCommand>
        where TCommand : ICommandMetaclass, new()
    {
        private ulong? _callbackKey = null;

        public void Register(Dispatcher dispatcher)
        {
            _callbackKey = dispatcher.OnCommandRequest<TCommand>(this.Handle);
        }
        public void Deregister(Dispatcher dispatcher)
        {
            if (_callbackKey.HasValue)
            {
                dispatcher.Remove(_callbackKey.Value);
                _callbackKey = null;
            }
        }
        public abstract void Handle(CommandRequestOp<TCommand> op);
    }
}
