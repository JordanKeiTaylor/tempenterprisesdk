using Improbable.Worker;
using Improbable.Enterprise.Extension;

namespace Improbable.Enterprise.CommandHandlers
{
    public abstract class DataCommandHandler<TCommand> : BaseCommandHandler<TCommand>
            where TCommand : ICommandMetaclass, new() {
        protected EntityUtil EntityUtil;
        protected DataCommandHandler(EntityUtil entityUtil) {
            EntityUtil = entityUtil;
        }
        public abstract override void Handle(CommandRequestOp<TCommand> op);
        public abstract void HandleError(CommandRequestOp<TCommand> op);
    }

    public abstract class DataCommandHandler<TCommand, TComponent1>
        : DataCommandHandler<TCommand>
            where TComponent1 : IComponentMetaclass, new()
            where TCommand : ICommandMetaclass, new()
    {
        protected DataCommandHandler(EntityUtil entityUtil)
            : base(entityUtil)
        {}

        public override async void Handle(CommandRequestOp<TCommand> op) {
            var c1 = new TComponent1();
            var entity = await EntityUtil.FetchEntity(op.EntityId, new Improbable.Collections.List<uint>() { c1.ComponentId });
            IComponentData<TComponent1> component1;
            if (entity.TryGetComponent<TComponent1>(out component1)) {
                Handle(op, component1);
            } else {
                HandleError(op);
            }
        }
        public abstract void Handle(CommandRequestOp<TCommand> op, IComponentData<TComponent1> component1);
    }

    public abstract class DataCommandHandler<TCommand, TComponent1, TComponent2>
        : DataCommandHandler<TCommand>
            where TComponent1 : IComponentMetaclass, new()
            where TComponent2 : IComponentMetaclass, new()
            where TCommand : ICommandMetaclass, new()
    {
        protected DataCommandHandler(EntityUtil entityUtil)
            : base(entityUtil)
        { }

        public override async void Handle(CommandRequestOp<TCommand> op)
        {
            var c1 = new TComponent1();
            var c2 = new TComponent2();
            var entity = await EntityUtil.FetchEntity(op.EntityId, new Improbable.Collections.List<uint>() { c1.ComponentId, c2.ComponentId });
            IComponentData<TComponent1> component1;
            IComponentData<TComponent2> component2;
            if (entity.TryGetComponent<TComponent1>(out component1)
                && entity.TryGetComponent<TComponent2>(out component2))
            {
                Handle(op, component1, component2);
            } else {
                HandleError(op);
            }
        }
        public abstract void Handle(CommandRequestOp<TCommand> op, IComponentData<TComponent1> component1, IComponentData<TComponent2> component2);
    }

    public abstract class DataCommandHandler<TCommand, TComponent1, TComponent2, TComponent3>
        : DataCommandHandler<TCommand>
            where TComponent1 : IComponentMetaclass, new()
            where TComponent2 : IComponentMetaclass, new()
            where TComponent3 : IComponentMetaclass, new()
            where TCommand : ICommandMetaclass, new()
    {
        protected DataCommandHandler(EntityUtil entityUtil)
            : base(entityUtil)
        { }

        public override async void Handle(CommandRequestOp<TCommand> op)
        {
            var c1 = new TComponent1();
            var c2 = new TComponent2();
            var c3 = new TComponent3();
            var entity = await EntityUtil.FetchEntity(op.EntityId, new Improbable.Collections.List<uint>() { c1.ComponentId, c2.ComponentId, c3.ComponentId });
            IComponentData<TComponent1> component1;
            IComponentData<TComponent2> component2;
            IComponentData<TComponent3> component3;
            if (entity.TryGetComponent<TComponent1>(out component1)
                && entity.TryGetComponent<TComponent2>(out component2)
                && entity.TryGetComponent<TComponent3>(out component3))
            {
                Handle(op, component1, component2, component3);
            } else {
                HandleError(op);
            }
        }
        public abstract void Handle(CommandRequestOp<TCommand> op, IComponentData<TComponent1> component1, IComponentData<TComponent2> component2, IComponentData<TComponent3> component3);
    }

    public abstract class DataCommandHandler<TCommand, TComponent1, TComponent2, TComponent3, TComponent4>
        : DataCommandHandler<TCommand>
            where TComponent1 : IComponentMetaclass, new()
            where TComponent2 : IComponentMetaclass, new()
            where TComponent3 : IComponentMetaclass, new()
            where TComponent4 : IComponentMetaclass, new()
            where TCommand : ICommandMetaclass, new()
    {
        protected DataCommandHandler(EntityUtil entityUtil)
            : base(entityUtil)
        { }

        public override async void Handle(CommandRequestOp<TCommand> op)
        {
            var c1 = new TComponent1();
            var c2 = new TComponent2();
            var c3 = new TComponent3();
            var c4 = new TComponent4();
            var entity = await EntityUtil.FetchEntity(op.EntityId, new Improbable.Collections.List<uint>() { c1.ComponentId, c2.ComponentId, c3.ComponentId, c4.ComponentId });
            IComponentData<TComponent1> component1;
            IComponentData<TComponent2> component2;
            IComponentData<TComponent3> component3;
            IComponentData<TComponent4> component4;
            if (entity.TryGetComponent<TComponent1>(out component1)
                && entity.TryGetComponent<TComponent2>(out component2)
                && entity.TryGetComponent<TComponent3>(out component3)
                && entity.TryGetComponent<TComponent4>(out component4))
            {
                Handle(op, component1, component2, component3, component4);
            } else {
                HandleError(op);
            }
        }
        public abstract void Handle(CommandRequestOp<TCommand> op, IComponentData<TComponent1> component1, IComponentData<TComponent2> component2, IComponentData<TComponent3> component3, IComponentData<TComponent4> component4);
    }
}
