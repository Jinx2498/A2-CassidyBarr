using GameBrains.FiniteStateMachine;
using GameBrains.Entities;
using UnityEngine;
using Microbes.Entities;
using System.Collections.Generic;

namespace Microbes.States
{
    // Reproducing state. Executing in the mating state results in "contributions" from parents.
    // This state should spawn zero, one, or more new microbes with characteristics determined by
    // the parents. Be creative.
    // TODO for A2: Fill in the details.
    [CreateAssetMenu(menuName = "Microbes/States/Reproducing")]
    public class Reproducing : State
    {
        public override void OnEnable()
        {
            base.OnEnable();
        }
        
        // This will execute when the state is entered.
        public override void Enter(StateMachine stateMachine)
        {
            base.Enter(stateMachine);
        
            var microbe = stateMachine.Owner as Microbe;
            
            if (microbe == null) { return; }
        }
        
        // This is the state's normal update function.
        public override void Execute(StateMachine stateMachine)
        {
            base.Execute(stateMachine);
            
            var microbe = stateMachine.Owner as Microbe;
            
            if (microbe == null) { return; }

            if (microbe.IsHungry) {
                var seekingFoodState = StateManager.Lookup(typeof(SeekingFood));
                if (seekingFoodState == null) { Debug.Log("Missing State"); }
                stateMachine.ChangeState(seekingFoodState);
                return;
            }

            var reproductionRadius = microbe.transform.localScale.x / 2.0f;

            List<Microbe> SurroundingMicrobes = new List<Microbe>();

            // Find all microbes in a certain radius that match any of the types of possible mates.
            foreach (Microbe aMicrobe in EntityManager.FindAll<Microbe>()) {
				if (microbe != aMicrobe && (aMicrobe.MicrobeType & microbe.DateTypes) != 0 && aMicrobe.StateMachine.CurrentState == Instance) {
                    if (Physics.Raycast(microbe.transform.position, aMicrobe.transform.position - microbe.transform.position, out RaycastHit hit, reproductionRadius)) {
                        if (hit.transform == aMicrobe.transform) {
                            SurroundingMicrobes.Add(aMicrobe);
                        }
                    }
                }
            }

            if (SurroundingMicrobes.Count > 0){
                foreach (Microbe SurroundingMicrobe in SurroundingMicrobes){
                    microbe.TryToReproduce(SurroundingMicrobe);
                }
            }
        }
        
        // This will execute when the state is exited.
        public override void Exit(StateMachine stateMachine)
        {
            base.Exit(stateMachine);
            
            var microbe = stateMachine.Owner as Microbe;
            
            if (microbe == null) { return; }
        }
        
        // This executes if the microbe receives a message from the message dispatcher.
        // public override bool HandleEvent<TEvent>(
        //     StateMachine stateMachine,
        //     Event<TEvent> eventArguments)
        // {
        //     if (eventArguments.EventType == Events.MyEvent)
        //     {
        //         var microbe = stateMachine.Owner as Microbe;
            
        //         if (microbe != null && eventArguments.ReceiverId == microbe.ID)
        //         {
        //             if (VerbosityDebug)
        //             {
        //                 Debug.Log($"Event {eventArguments.EventType} received by {microbe.name} at time: {Time.time}");
        //             }
                    
        //             // TODO: Do stuff
            
        //             return true;
        //         }
        //     }
        
        //     return base.HandleEvent(stateMachine, eventArguments);
        // }
    }
}