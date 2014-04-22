﻿using System.Collections.Generic;
using Glimpse.Core.Message;
using Glimpse.WebApi.Core.AlternateType;

namespace Glimpse.WebApi.Core.Message
{
    public static class WebApiTimelineExtension
    {
        public static T AsWebApiTimelineMessage<T>(this T message, TimelineCategoryItem eventCategory)
            where T : ITimelineMessage
        {
            message.EventCategory = eventCategory; 

            var controllerName = string.Empty;
            var actionName = string.Empty;
            var actionMessage = message as IActionMessage;
            if (actionMessage != null)
            {
                controllerName = actionMessage.ControllerName;
                actionName = actionMessage.ActionName;
            }

            var controllerSelectorMessage = message as IHttpControllerSelector.SelectController.Message;
            if (controllerSelectorMessage != null)
            {
                message.EventName = "Controller Selector";
                return message;
            }

            var controllerActivatorMessage = message as IHttpControllerActivator.Create.Message;
            if (controllerActivatorMessage != null)
            {
                message.EventName = string.Format("Controller: {0}.{1}", controllerName, actionName);
                return message;
            }

            

            
            var activeInvokerInvokeActionResultMessage = message as IInvokeActionResultMessage;
            if (activeInvokerInvokeActionResultMessage != null)
            {
                message.EventName = string.Format("Action Result: {0}.{1}", controllerName, actionName);
                return message;
            }

            var boundedFilterMessage = message as IBoundedFilterMessage;
            if (boundedFilterMessage != null)
            {
                message.EventName = string.Format("{0} {1}: {2}.{3}", boundedFilterMessage.Category.ToString(), boundedFilterMessage.Bounds.ToString(), controllerName, actionName);
                return message;
            }

            var filterMessage = message as IFilterMessage;
            if (filterMessage != null)
            {
                message.EventName = string.Format("{0}: {1}.{2}", filterMessage.Category.ToString(), controllerName, actionName);
                return message;
            }

            if (actionMessage != null)
            {
                message.EventName = string.Format("{0}.{1}", controllerName, actionName);
                return message;
            }

            return message;
        }

        public static void BuildDetails<T>(this T message, IDictionary<string, object> details)
            where T : ITimelineMessage
        {
            var filterMessage = message as IFilterMessage;
            if (filterMessage != null)
            {
                details.Add("ResultType", filterMessage.ResultType);
            }

            var sourceMessage = message as ISourceMessage;
            if (sourceMessage != null)
            {
                if (sourceMessage.ExecutedMethod != null)
                {
                    details.Add("ExecutedMethod", sourceMessage.ExecutedMethod);
                }

                if (sourceMessage.ExecutedType != null)
                {
                    details.Add("ExecutedType", sourceMessage.ExecutedType);
                }
            }

            var cancelledFilterMessage = message as ICanceledBasedFilterMessage;
            if (cancelledFilterMessage != null)
            {
                details.Add("Canceled", cancelledFilterMessage.Canceled);
            }

            var exceptionFilterMessage = message as IExceptionFilterMessage;
            if (exceptionFilterMessage != null)
            {
                details.Add("ExceptionHandled", exceptionFilterMessage.ExceptionHandled);
                details.Add("ExceptionType", exceptionFilterMessage.ExceptionType);
            }
        }
    }
}
