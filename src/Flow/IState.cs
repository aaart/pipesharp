﻿using System.Collections.Generic;

namespace Flow
{

    public interface IState
    {
        IState Skip();
        void PublishError(IError error);
        IEnumerable<IError> Errors { get; }
        IEventReceiver EventReceiver { get; }
    }

    public interface IState<out T> : IState
    {
        IState<TR> Clone<TR>(TR result);
        new IState<TR> Skip<TR>();
        IState ToVoid();
        T Result { get; }
    }
}