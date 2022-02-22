using GameBrains.FiniteStateMachine;
using UnityEngine;

namespace Microbes.States
{
    // Reproducing state. Executing in the mating state results in "contributions" from parents.
    // This state should spawn zero, one, or more new microbes with characteristics determined by
    // the parents. Be creative.
    // TODO for A2: Fill in the details.
    [CreateAssetMenu(menuName = "Microbes/States/Reproducing")]
    public class Reproducing : State
    {
        // public override void OnEnable()
        // {
        //     base.OnEnable();
        // }
        
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
        // public override void Execute(StateMachine stateMachine)
        // {
        //     base.Execute(stateMachine);
        //     
        //     // var microbe = stateMachine.Owner as Microbe;
        //     //
        //     // if (microbe == null) { return; }
        // }
        
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
        // public override bool HandleEvent<TEvent>(
        //     StateMachine stateMachine,
        //     Event<TEvent> eventArguments)
        // {
        //     // if (eventArguments.EventType == Events.MyEvent)
        //     // {
        //     //     var microbe = stateMachine.Owner as Microbe;
        //     //
        //     //     if (microbe != null && eventArguments.ReceiverId == microbe.ID)
        //     //     {
        //     //         if (VerbosityDebug)
        //     //         {
        //     //             Debug.Log($"Event {eventArguments.EventType} received by {microbe.name} at time: {Time.time}");
        //     //         }
        //     //         
        //     //         // TODO: Do stuff
        //     //
        //     //         return true;
        //     //     }
        //     // }
        //
        //     return base.HandleEvent(stateMachine, eventArguments);
        // }
    }
}