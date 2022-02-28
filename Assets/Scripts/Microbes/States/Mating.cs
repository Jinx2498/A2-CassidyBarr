using GameBrains.EventSystem;
using GameBrains.Entities;
using Microbes.Entities;
using GameBrains.FiniteStateMachine;
using UnityEngine;
using System.Collections.Generic;

namespace Microbes.States
{
    // When in this state, the microbe should try attracting a mate of a suitable type. When a
    // potential mate is close, we should send a LetsMakeABaby message. If a LetsMakeABaby message
    // is received, we should change to the Reproducing state. Or if a YouAreNotMyType message is
    // received, either change to the Sleep state (say 50% chance) or seek mate of a different type
    // (say 40%) or keep trying (say 10%). Note: the above is not a strict requirement, just one
    // possibility. You might make your microbe seek partners of several types before reproducing
    // (3 or 4 parents!). You might make the type of microbe that gets spawned depend on the type
    // of its parents. Use your imagination. Then take a cold shower :-)
    // TODO for A2: Fill in the details of this state.
    [CreateAssetMenu(menuName = "Microbes/States/Mating")]
    public class Mating : State
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

            EventManager.Instance.Fire(Events.Message, $"{microbe.name}: I want To Mate!");
            
            if (microbe == null) { return; }
        }

        // This is the state's normal update function.
        public override void Execute(StateMachine stateMachine)
        {
            base.Execute(stateMachine);
            
            var microbe = stateMachine.Owner as Microbe;
            
            if (microbe == null) { return; }

            if (microbe.IsHungry)
            {
                var seekingFoodState = StateManager.Lookup(typeof(SeekingFood));
                if (seekingFoodState == null) { Debug.Log("Missing State"); }
                stateMachine.ChangeState(seekingFoodState);
                return;
            }

            var matingRadius = microbe.transform.localScale.x / 2.0f;

            List<Microbe> SurroundingMicrobes = new List<Microbe>();

            RaycastHit hit;

            foreach (Microbe aMicrobe in EntityManager.FindAll<Microbe>()) {
                if (microbe != aMicrobe && (aMicrobe.MicrobeType & microbe.DateTypes) != 0) {
                    if (Physics.Raycast(microbe.transform.position, aMicrobe.transform.position - microbe.transform.position, out hit, matingRadius)) {
                        if (hit.transform == aMicrobe.transform) {
                            SurroundingMicrobes.Add(aMicrobe);
                        }
                    }
                }
            }

            if (SurroundingMicrobes.Count > 0) {
                foreach (Microbe SurroundingMicrobe in SurroundingMicrobes) {
                    
                    EventManager.Instance.Fire(
                        Events.LetsMakeABaby, 
                        microbe.ID, 
                        SurroundingMicrobe.ID, 
                        string.Empty);
                    
                    EventManager.Instance.Fire(Events.Message, $"{microbe.name} mated with: {SurroundingMicrobe.name}");
                }
            }

	        microbe.Attractor.radius = 500; 
	        microbe.Attractor.AttractTypes = microbe.DateTypes;
 
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
                
        //         if (microbe == null) { return base.HandleEvent(stateMachine, eventArguments);; }
                
        //         if (eventArguments.ReceiverId == microbe.ID)
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