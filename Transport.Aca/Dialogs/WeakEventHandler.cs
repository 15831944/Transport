using System;

namespace Transport.Aca.Dialogs
{
    public delegate void UnregisterCallback<TE>(EventHandler<TE> eventHandler)
    where TE : EventArgs;

    public interface IWeakEventHandler<TE>
        where TE : EventArgs
    {
        EventHandler<TE> Handler { get; }
    }

    public class WeakEventHandler<T, TE> : IWeakEventHandler<TE>
        where T : class
        where TE : EventArgs
    {
        private delegate void OpenEventHandler(T @this, object sender, TE e);

        private readonly WeakReference _mTargetRef;
        private readonly OpenEventHandler _mOpenHandler;
        private readonly EventHandler<TE> _mHandler;
        private UnregisterCallback<TE> _mUnregister;

        public WeakEventHandler(EventHandler<TE> eventHandler, UnregisterCallback<TE> unregister)
        {
            _mTargetRef = new WeakReference(eventHandler.Target);

            _mOpenHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);

            _mHandler = Invoke;
            _mUnregister = unregister;
        }

        public void Invoke(object sender, TE e)
        {
            T target = (T)_mTargetRef.Target;

            if (target != null)
                _mOpenHandler.Invoke(target, sender, e);
            else if (_mUnregister != null)
            {
                _mUnregister(_mHandler);
                _mUnregister = null;
            }
        }

        public EventHandler<TE> Handler => _mHandler;

        public static implicit operator EventHandler<TE>(WeakEventHandler<T, TE> weh)
        {
            return weh._mHandler;
        }
    }

    public static class EventHandlerUtils
    {
        public static EventHandler<TE> MakeWeak<TE>(this EventHandler<TE> eventHandler, UnregisterCallback<TE> unregister)
          where TE : EventArgs
        {
            if (eventHandler == null)
                throw new ArgumentNullException(nameof(eventHandler));

            if (eventHandler.Method.IsStatic || eventHandler.Target == null)
                throw new ArgumentException(@"Only instance methods are supported.", nameof(eventHandler));

            var wehType = typeof(WeakEventHandler<,>).MakeGenericType(eventHandler.Method.DeclaringType, typeof(TE));

            var wehConstructor = wehType.GetConstructor(new Type[] { typeof(EventHandler<TE>), typeof(UnregisterCallback<TE>) });

            IWeakEventHandler<TE> weh = (IWeakEventHandler<TE>)wehConstructor.Invoke(new object[] { eventHandler, unregister });

            return weh.Handler;
        }
    }
}
