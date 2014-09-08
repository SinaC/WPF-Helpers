using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MVVM
{
    public sealed class Mediator : IMediator
    {
        private class Subscription
        {
            private WeakReference RecipientWeakReference { get; set; }

            public object Recipient
            {
                get
                {
                    if (RecipientWeakReference == null)
                        return null;
                    return RecipientWeakReference.Target;
                }
            }

            public object Token { get; private set; }
            public MethodInfo Method { get; private set; }
            public SynchronizationContext Context { get; private set; }

            public bool IsAlive
            {
                get
                {
                    if (RecipientWeakReference == null)
                        return false;
                    return RecipientWeakReference.IsAlive;
                }
            }

            public Subscription(object recipient, MethodInfo method, object token, SynchronizationContext context)
            {
                RecipientWeakReference = new WeakReference(recipient);
                Method = method;
                Token = token;
                Context = context;
            }
        }

        private readonly Dictionary<Type, List<Subscription>> _recipients = new Dictionary<Type, List<Subscription>>();

        private static readonly Lazy<Mediator> LazyDefault = new Lazy<Mediator>(() => new Mediator());
        public static IMediator Default
        {
            get { return LazyDefault.Value; }
        }

        #region Register
        public void Register<T>(object recipient, Action<T> action)
        {
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            if (action == null)
                throw new ArgumentNullException("action");
            Register(recipient, null, action);
        }

        public void Register<T>(object recipient, object token, Action<T> action)
        {
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            if (action == null)
                throw new ArgumentNullException("action");
            lock (_recipients)
            {
                Type messageType = typeof(T);

                List<Subscription> subscriptions;
                if (!_recipients.ContainsKey(messageType))
                {
                    subscriptions = new List<Subscription>();
                    _recipients.Add(messageType, subscriptions);
                }
                else
                    subscriptions = _recipients[messageType];

                lock (subscriptions)
                {
                    subscriptions.Add(new Subscription(recipient, action.Method, token, SynchronizationContext.Current));
                }
            }
            Cleanup();
        }
        #endregion

        #region Unregister
        public void Unregister(object recipient)
        {
            lock (_recipients)
            {
                foreach (KeyValuePair<Type, List<Subscription>> kv in _recipients)
                    UnregisterFromList(kv.Value, x => x.Recipient == recipient);
            }
        }

        public void Unregister<T>(object recipient)
        {
            lock (_recipients)
            {
                Type messageType = typeof(T);
                if (_recipients.ContainsKey(messageType))
                    UnregisterFromList(_recipients[messageType], x => x.Recipient == recipient);
            }
        }

        public void Unregister<T>(object recipient, Action<T> action)
        {
            lock (_recipients)
            {
                Type messageType = typeof(T);
                MethodInfo method = action.Method;

                if (_recipients.ContainsKey(messageType))
                    UnregisterFromList(_recipients[messageType], x => x.Recipient == recipient && x.Method == method);
            }
        }

        public void Unregister<T>(object recipient, object token)
        {
            lock (_recipients)
            {
                Type messageType = typeof(T);

                if (_recipients.ContainsKey(messageType))
                    UnregisterFromList(_recipients[messageType], x => x.Recipient == recipient && x.Token == token);
            }
        }

        public void Unregister<T>(object recipient, object token, Action<T> action)
        {
            lock (_recipients)
            {
                Type messageType = typeof(T);
                MethodInfo method = action.Method;

                if (_recipients.ContainsKey(messageType))
                    UnregisterFromList(_recipients[messageType], x => x.Recipient == recipient && x.Method == method && x.Token == token);
            }
        }

        //
        private void UnregisterFromList(List<Subscription> list, Func<Subscription, bool> filter)
        {
            lock (list)
            {
                List<Subscription> toRemove = list.Where(filter).ToList();
                foreach (Subscription item in toRemove)
                    list.Remove(item);
            }
            Cleanup();
        }
        #endregion

        #region Send
        public void Send<T>(T message)
        {
            Send(message, null);
        }

        public void Send<T>(T message, object token)
        {
            List<Subscription> clone = null;
            lock (_recipients)
            {
                Type messageType = typeof(T);

                if (_recipients.ContainsKey(messageType))
                {
                    // Clone to avoid problem if register/unregistering in "receive message" method
                    lock (_recipients[messageType])
                    {
                        clone = _recipients[messageType].Where(x => (x.Token == null && token == null)
                                                                   ||
                                                                   (x.Token != null && x.Token.Equals(token))
                            ).ToList();
                    }
                }
            }
            if (clone != null)
                SendToList(clone, message);
        }

        private void SendToList<T>(IEnumerable<Subscription> list, T message)
        {
            // Send message to matching recipients
            List<Exception> exceptions = new List<Exception>();
            foreach (Subscription item in list)
            {
                try
                {
                    if (item.IsAlive)
                    {
                        //http://stackoverflow.com/questions/4843010/net-how-do-i-invoke-a-delegate-on-a-specific-thread-isynchronizeinvoke-disp
                        // Execute on thread which performed Register if possibe
                        Subscription subscription = item; // avoid closure problem
                        if (subscription.Context != null)
                        {
                            subscription.Context.Post(
                                _ => subscription.Method.Invoke(subscription.Recipient, new object[] { message }), null);
                        }
                        else // no context specified while registering, create a delegate and BeginInvoke
                        {
                            Func<object, object[], object> delegateToMethod = subscription.Method.Invoke;
                            delegateToMethod.BeginInvoke(subscription.Recipient, new object[] { message }, null, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Any())
                throw new AggregateException("Send problem", exceptions);
            Cleanup();
        }
        #endregion

        #region Cleanup
        private void Cleanup()
        {
            // Clean dead recipients
            lock (_recipients)
            {
                foreach (KeyValuePair<Type, List<Subscription>> kv in _recipients)
                {
                    List<Subscription> list = kv.Value;
                    List<Subscription> toRemove = list.Where(x => !x.IsAlive || x.Recipient == null).ToList();
                    foreach (Subscription item in toRemove)
                        list.Remove(item);
                }
            }
        }
        #endregion
    }
}
