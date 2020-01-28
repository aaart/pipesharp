﻿using Flow.Tests.TestUtilities;
using Xunit;

namespace Flow.Tests
{
    public class FlowEventsTests
    {
        [Fact]
        public void Flow_WhenTestingEventPublished_ExpectEventReceived1()
        {
            bool received = false;
            new Step<int>(() => new StepResult<int, TestingEventReceiver>(default, new StepState<TestingEventReceiver>(new TestingEventReceiver(() => received = true))))
                .Publish((x, er) => er.Receive(new TestingEvent()))
                .Finalize(x => { })
                .Sink();

            Assert.True(received);
        }

        [Fact]
        public void Flow_WhenTestingEventPublished_ExpectEventReceived2()
        {
            bool received = false;
            new Step<int>(() => new StepResult<int, TestingEventReceiver>(default, new StepState<TestingEventReceiver>(new TestingEventReceiver(() => received = true))))
                .Publish((x, er) => er.Receive(new TestingEvent()))
                .Finalize(x => x)
                .Sink();
            
            Assert.True(received);
        }

        [Fact]
        public void Flow_WhenTestingEventPublished_ExpectEventReceived3()
        {
            bool received = false;
            new Step<int>(() => new StepResult<int, TestingEventReceiver>(default, new StepState<TestingEventReceiver>(new TestingEventReceiver(() => received = true))))
                .Publish((x, er) => er.Receive(new TestingEvent()))
                .Finalize(x => x)
                .Project(x => x)
                .Sink();
            
            Assert.True(received);
        }
    }
}