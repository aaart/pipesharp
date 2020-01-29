﻿using System;

namespace Flow
{
    public class Step<T> : IFlow<T>, IValidatedVerified<T>, IEventSource<T>
    {
        private readonly Func<IState<T>> _method;
        
        public Step(Func<IState<T>> method)
        {
            _method = method;
        }

        public IFlow<T> Validate<TR>(Func<T, TR> transform, Func<TR, bool> validator, Func<IFilteringError> error) => 
            Clone(() => _method.Decorate(transform, new LambdaFilter<TR>(validator, error)));

        public IFlow<T> Validate(Func<T, bool> validator, Func<IFilteringError> error) => 
            Clone(() => _method.Decorate(x => x, new LambdaFilter<T>(validator, error)));

        public IFlow<T> Validate<TR>(Func<T, TR> transform, IFilter<TR> filter) => 
            Clone(() => _method.Decorate(transform, filter));

        public IFlow<T> Validate(IFilter<T> filter) => 
            Clone(() => _method.Decorate(x => x, filter));

        public IValidatedVerified<T> Verify<TR>(Func<T, TR> transform, Func<TR, bool> check, Func<IFilteringError> error) => 
            Clone(() => _method.Decorate(transform, new LambdaFilter<TR>(check, error)));
        
        public IValidatedVerified<T> Verify(Func<T, bool> check, Func<IFilteringError> error) => 
            Clone(() => _method.Decorate(x => x, new LambdaFilter<T>(check, error)));

        public IValidatedVerified<T> Verify(IFilter<T> filter) => 
            Clone(() => _method.Decorate(x => x, filter));

        public IValidatedVerified<T> Verify<TR>(Func<T, TR> transform, IFilter<TR> filter) => 
            Clone(() => _method.Decorate(transform, filter));

        public IValidatedVerified<TR> Apply<TR>(Func<T, TR> apply) =>
            Clone(() => _method.Decorate(state => apply(state.Result)));
        
        public IValidatedVerified<T> Raise(Func<T, IEvent> func) =>
            Clone(() => _method.Decorate(state =>
            {
                state.EventReceiver.Receive(func(state.Result));
                return state.Result;
            }));

        public IPipeline Finalize(Action<T> execution) => 
            new Pipeline(() => _method.Decorate(state => execution(state.Result)));

        public IProjectablePipeline<TR> Finalize<TR>(Func<T, TR> execution) => 
            new Pipeline<TR>(() => _method.Decorate(state => execution(state.Result)));

        
        private Step<TR> Clone<TR>(Func<IState<TR>> method) => new Step<TR>(method);
    }
}