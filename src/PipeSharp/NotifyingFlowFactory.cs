﻿using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PipeSharp.Internal;

namespace PipeSharp
{
    public class NotifyingFlowFactory<TFilteringError> : INotifyingFlowFactory<TFilteringError>
    {
        private readonly IEventReceiverFactory _eventReceiverFactory;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public NotifyingFlowFactory(IEventReceiverFactory eventReceiverFactory)
        {
            _eventReceiverFactory = eventReceiverFactory;
            _logger = NullLogger.Instance;
        }

        public NotifyingFlowFactory(IEventReceiverFactory eventReceiverFactory, ILogger logger)
        {
            _eventReceiverFactory = eventReceiverFactory;
            _logger = logger;
        }
        
        public NotifyingFlowFactory(IEventReceiverFactory eventReceiverFactory, ILoggerFactory loggerFactory)
        {
            _eventReceiverFactory = eventReceiverFactory;
            _loggerFactory = loggerFactory;
        }

        public INotifyingFlow<T, TFilteringError> For<T>(T target) => For(target, () => { }, () => { });

        public INotifyingFlow<T, TFilteringError> For<T>(T target, Action onDoing, Action onDone) =>
            new Step<T, TFilteringError>(
                () => new State<T, TFilteringError>(
                    target,
                    new StateData<TFilteringError>(
                        _logger ?? _loggerFactory.CreateLogger<Step<T, TFilteringError>>(),
                        _eventReceiverFactory.Create())),
                onDoing,
                onDone);
    }
}