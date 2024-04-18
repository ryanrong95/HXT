using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Flow.Event
{
    public sealed class EventBuilder<T>
    {
        private event EventHandler<T> Event;

        private List<EventHandler<T>> Handlers = new List<EventHandler<T>>();

        public EventBuilder()
        {

        }

        public EventBuilder(List<EventHandler<T>> handlers)
        {
            this.ReLoad(handlers);
        }

        private void ReLoad(List<EventHandler<T>> handlers)
        {
            this.Event = null;
            this.Handlers = handlers;
            if (handlers != null && handlers.Any())
            {
                foreach (var handler in this.Handlers)
                {
                    this.Event += handler;
                }
            }
        }

        public EventBuilder<T> Append(EventHandler<T> handler)
        {
            if (!this.Handlers.Contains(handler))
            {
                this.Handlers.Add(handler);
                this.Event += handler;
            }
            return this;
        }

        public EventBuilder<T> Append(EventHandler<T> handler, bool force)
        {
            if (force)
            {
                this.Handlers.Add(handler);
                this.Event += handler;
            }
            else
            {
                this.Append(handler);
            }
            return this;
        }

        public EventBuilder<T> Distinct()
        {
            this.ReLoad(this.Handlers.Distinct().ToList());
            return this;
        }

        public object Execute(T param)
        {
            if (this.Event != null)
            {
                return this.Event(param, new EventArgs<T>(param));
            }

            return null;
        }

        public List<HandlerInfo> HandlerInfos
        {
            get
            {
                List<HandlerInfo> listHandlerInfo = new List<HandlerInfo>();
                for (int i = 0; i < this.Handlers.Count; i++)
                {
                    listHandlerInfo.Add(new HandlerInfo()
                    {
                        SerialNo = i,
                        Namespace = this.Handlers[i].Method.DeclaringType.Namespace,
                        ClassName = this.Handlers[i].Method.DeclaringType.Name,
                        MethodName = this.Handlers[i].Method.Name,
                    });
                }
                return listHandlerInfo;
            }
        }

        public EventBuilder<T> ReOrder(HandlerInfo[] handlerInfos, bool match = false)
        {
            var newHandlers = new List<EventHandler<T>>();
            var oldHandlers = this.Handlers;

            if (handlerInfos == null || handlerInfos.Length == 0)
            {
                return this;
            }

            foreach (var handlerInfo in handlerInfos)
            {
                var targetHandlerQuery = oldHandlers.AsQueryable();
                if (!string.IsNullOrEmpty(handlerInfo.Namespace))
                {
                    targetHandlerQuery = targetHandlerQuery.Where(t => t.Method.DeclaringType.Namespace == handlerInfo.Namespace);
                }
                if (!string.IsNullOrEmpty(handlerInfo.ClassName))
                {
                    targetHandlerQuery = targetHandlerQuery.Where(t => t.Method.DeclaringType.Name == handlerInfo.ClassName);
                }
                if (!string.IsNullOrEmpty(handlerInfo.MethodName))
                {
                    targetHandlerQuery = targetHandlerQuery.Where(t => t.Method.Name == handlerInfo.MethodName);
                }

                var targetHandler = targetHandlerQuery.FirstOrDefault();
                if (targetHandler == null)
                {
                    if (match)
                    {
                        throw new Exception(handlerInfo.Namespace + "." + handlerInfo.ClassName + "." + handlerInfo.MethodName + " 不存在");
                    }
                    else
                    {
                        continue;
                    }
                }

                oldHandlers.Remove(targetHandler);
                newHandlers.Add(targetHandler);
            }

            if (oldHandlers != null && oldHandlers.Any())
            {
                newHandlers.AddRange(oldHandlers);
            }

            this.ReLoad(newHandlers);

            return this;
        }
    }
}
