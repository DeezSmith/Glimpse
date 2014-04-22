﻿using System.Collections.Generic;
using System.Web.Http.Controllers;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Message;
using Glimpse.WebApi.Core.Message;
using System;
using System.Reflection;

namespace Glimpse.WebApi.Core.AlternateType
{
    public class IAssembliesResolver : AlternateType<System.Web.Http.Dispatcher.IAssembliesResolver>
    {
        private IEnumerable<IAlternateMethod> allMethods;

        public IAssembliesResolver(IProxyFactory proxyFactory)
            : base(proxyFactory)
        {
        }

        public override IEnumerable<IAlternateMethod> AllMethods
        {
            get
            {
                return allMethods ?? (allMethods = new List<IAlternateMethod>
                {
                    new GetAssemblies()
                });
            }
        }

        public class GetAssemblies : AlternateMethod
        {
            public GetAssemblies()
                : base(typeof(System.Web.Http.Dispatcher.IAssembliesResolver), "GetAssemblies")
            {
            }

            public override void PostImplementation(IAlternateMethodContext context, TimerResult timerResult)
            {
                var actionContext = (HttpControllerContext)context.Arguments[0];
                var message = new Message()
                    .AsTimedMessage(timerResult)
                    .AsSourceMessage(actionContext.Controller.GetType(), context.MethodInvocationTarget)
//                    .AsActionMessage(actionContext.ControllerDescriptor..RequestContext)
                    .AsFilterMessage(FilterCategory.Action, actionContext.GetTypeOrNull())
                    .AsBoundedFilterMessage(FilterBounds.Executing)
                    .AsWebApiTimelineMessage(WebApiTimelineCategory.Filter);

                context.MessageBroker.Publish(message);
            }

            public class Message : MessageBase, IBoundedFilterMessage, IExecutionMessage
            {
                public string ControllerName { get; set; }

                public string ActionName { get; set; }

                public FilterCategory Category { get; set; }

                public Type ResultType { get; set; }

                public FilterBounds Bounds { get; set; }

                public bool IsChildAction { get; set; }

                public Type ExecutedType { get; set; }

                public MethodInfo ExecutedMethod { get; set; }

                public TimeSpan Offset { get; set; }

                public TimeSpan Duration { get; set; }

                public DateTime StartTime { get; set; }

                public string EventName { get; set; }

                public TimelineCategoryItem EventCategory { get; set; }

                public string EventSubText { get; set; }
            }
        }
    }
}
