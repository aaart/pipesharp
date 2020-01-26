﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Flow
{
    public class Pipeline : IPipeline
    {
        private readonly Func<IState> _method;
        internal Pipeline(Func<IState> method)
        {
            _method = method;
        }

        public (IPipelineResult, Exception, IFilteringError[]) Sink() => _method().Sink<PipelineResult, IState>();
    }

    public class Pipeline<T> : IProjectablePipeline<T>
    {
        private readonly Func<IState<T>> _method;

        internal Pipeline(Func<IState<T>> method)
        {
            _method = method;
        }

        public IProjectablePipeline<TR> Project<TR>(Func<T, TR> projection) => 
            new Pipeline<TR>(() => _method.Decorate(state => projection(state.Result)));

        public (IPipelineResult<T>, Exception, IFilteringError[]) Sink() =>
            _method().Sink<PipelineResult<T>, IState<T>>((result, state) =>
            {
                if (!state.Errors.Any())
                {
                    result.Value = state.Result;
                }
            });

        (IPipelineResult, Exception, IFilteringError[]) IPipeline.Sink()
        {
            return Sink();
        }
    }
}