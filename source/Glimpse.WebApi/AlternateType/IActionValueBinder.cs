﻿using System.Collections.Generic;
using System.Web.Http.Controllers;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Message;
using Glimpse.WebApi.Message;
using System;
using System.Reflection;

namespace Glimpse.WebApi.AlternateType
{
    public class IActionValueBinder : AlternateType<System.Web.Http.Controllers.IActionValueBinder>
    {
        private IEnumerable<IAlternateMethod> allMethods;

        public IActionValueBinder(IProxyFactory proxyFactory)
            : base(proxyFactory)
        {
        }

        public override IEnumerable<IAlternateMethod> AllMethods
        {
            get
            {
                return allMethods ?? (allMethods = new List<IAlternateMethod>
                {
                    new GetBinding()
                });
            }
        }

        public class GetBinding : AlternateMethod
        {
            public GetBinding()
                : base(typeof(System.Web.Http.Controllers.IActionValueBinder), "GetBinding")
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
                    .AsWebApiTimelineMessage(WebApiMvcTimelineCategory.Filter);

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