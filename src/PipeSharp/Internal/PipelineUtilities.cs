﻿using System;
using Microsoft.Extensions.Logging;

namespace PipeSharp.Internal
{
    internal static class PipelineUtilities
    {

        public static bool Execute<T, TError>(this Func<IState<T, TError>> step, out IState<T, TError> state)
        {
            state = step();
            return !state.Invalid && !state.Broken;
        }

        public static IState<TError> Decorate<T, TError>(this Func<IState<T, TError>> step, Action<IState<T, TError>> target, Action onDoing, Action onDone) =>
            step.Execute(out IState<T, TError> state) ? TryCatch(state, target, onDoing, onDone) : state.Fail();

        public static IState<TR, TError> Decorate<T, TR, TError>(this Func<IState<T, TError>> method, Func<IState<T, TError>, TR> target, Action onDoing, Action onDone) =>
            method.Execute(out IState<T, TError> state) ? TryCatch(state, target, onDoing, onDone) : state.Fail<TR>();

        public static IState<T, TError> Decorate<T, TK, TError>(this Func<IState<T, TError>> step, Func<T, TK> transform, IFilter<TK, TError> filter)
        {
            var state = step();
            try
            {
                state.LogDebug($"Validating {typeof(T)} object.");
                TK target = transform(state.StepResult);
                if (!state.Broken && !filter.Check(target, out TError error))
                {
                    state.LogError($"{typeof(T)} is invalid. {typeof(TError)} error registered.");
                    return state.Invalidate(error);
                }
                state.LogDebug("Validated");
                return state.Next(state.StepResult);
            }
            catch (Exception ex)
            {
                state.LogError($"An exception was thrown during validation of {typeof(T)}.");
                state.LogError(ex, ex.Message);
                return state.Fail<T>(ex);
            }
        }

        public static IState<TError> TryCatch<T, TError>(IState<T, TError> state, Action<IState<T, TError>> step, Action onDoing, Action onDone)
        {
            try
            {
                state.LogDebug($"Executing step for {typeof(T)}");
                onDoing();
                step(state);
                onDone();
                return state.Next();
            }
            catch (Exception ex)
            {
                state.LogError($"An exception was thrown during execution of step for {typeof(T)}.");
                return state.Fail(ex);
            }
        }

        public static IState<TR, TError> TryCatch<T, TR, TError>(IState<T, TError> state, Func<IState<T, TError>, TR> step, Action onDoing, Action onDone)
        {
            try
            {
                state.LogDebug($"Executing step for {typeof(T)}");
                onDoing();
                var r = step(state);
                onDone();
                return state.Next(r);
            }
            catch (Exception ex)
            {
                state.LogError($"An exception was thrown during execution of step for {typeof(T)}.");
                return state.Fail<TR>(ex);
            }
        }

        public static TPipelineResult Sink<TPipelineResult, TState, TError>(this TState state, Action<TPipelineResult, TState> setup = null)
            where TPipelineResult : IPipelineSummary<TError>, new()
            where TState : IState<TError>
        {
            state.LogDebug("All steps executed. Building result object");
            var result = new TPipelineResult();
            setup?.Invoke(result, state);
            state.Done();
            return result;
        }
    }
}