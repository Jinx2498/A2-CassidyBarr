using GameBrains.EventSystem;
using GameBrains.FiniteStateMachine;
using Microbes.Entities;
using UnityEngine;

namespace Microbes.States
{
    // Microbe global state class.
    // TODO for A2: fill in any additional global state details needed.
    [CreateAssetMenu(menuName = "Microbes/States/MicrobeGlobalState")]
    public class MicrobeGlobalState : State
    {
        // This will execute when the state is entered.
        // public override void Enter(StateMachine stateMachine)
        // {
        //     base.Enter(stateMachine);
        //     
        //     // var microbe = stateMachine.Owner as Microbe;
        //     //
        //     // if (microbe == null) { return; }
        // }

        // This is the state's normal update function.
        public override void Execute(StateMachine stateMachine)
        {
            base.Execute(stateMachine);
            
            var microbe = stateMachine.Owner as Microbe;
            
            if (microbe == null) { return; }

            if (Random.value < 0.05f)
            {
                microbe.Hunger += 1; // TODO for A2 (optional): Add visual indicator (maybe shrink when hungry)
            }
        }

        // This will execute when the state is exited.
        // public override void Exit(StateMachine stateMachine)
        // {
        //     base.Exit(stateMachine);
        //     
        //     // var microbe = stateMachine.Owner as Microbe;
        //     //
        //     // if (microbe == null) { return; }
        // }

        // This executes if the microbe receives a message from the message dispatcher.
        public override bool HandleEvent<TEvent>(
            StateMachine stateMachine,
            Event<TEvent> eventArguments)
        {
            if (eventArguments.EventType == Events.YouJustGotSwallowed)
            {
                var microbe = stateMachine.Owner as Microbe;
            
                if (microbe != null && eventArguments.ReceiverId == microbe.ID)
                {
                    if (VerbosityDebug)
                    {
                        Debug.Log($"Event {eventArguments.EventType} received by {microbe.name} at time: {Time.time}");
                    }
            
                    microbe.Die();
                    
                    // TODO for A2: Is the dead state needed?
                    var deadState = StateManager.Lookup(typeof(Dead));
                    if (deadState == null) { Debug.Log("Missing State"); }
                    stateMachine.ChangeState(deadState);
                    
                    return true;
                }
            }
        
            return base.HandleEvent(stateMachine, eventArguments);
        }
    }
}