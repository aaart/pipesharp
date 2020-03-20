﻿using System;

namespace PipeSharp
{
    public interface INotifyingFlowFactory<TFilteringError>
    {
        INotifyingFlow<T, TFilteringError> For<T>(T target);
        INotifyingFlow<T, TFilteringError> For<T>(T target, Action onDoing, Action onDone);
    }
}