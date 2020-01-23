﻿using System;
using System.Collections.Generic;

namespace Flow
{

    public interface IState : IDisposable
    {
        IState Skip();
        void PublishError(IFilteringError filteringError);
        IEnumerable<IFilteringError> Errors { get; }
        Exception Exception { get; set; }
        void Receive(IEvent @event);
    }

    public interface IState<out T> : IState
    {
        IState Next();
        IState<TR> Next<TR>(TR result);
        new IState<TR> Skip<TR>();
        
        T Result { get; }
    }
}