VAR QUEST_001_accepted = false
VAR all_QUEST_001_objectives_completed = false
VAR QUEST_001_completed = false
VAR QUEST_001_declined = false
VAR after_QUEST_001_completed = false

EXTERNAL StartQuest(string)
EXTERNAL CompleteObjective(string, string)

// Entry point
-> vesna_router

=== vesna_router ===
{ after_QUEST_001_completed:
    -> vesna.after_quest_completed_dialogue
- else:
    { QUEST_001_completed:
        -> vesna.quest_completed_dialogue
    - else:
        { all_QUEST_001_objectives_completed:
            -> vesna.quest_completed_dialogue
        - else:
            { QUEST_001_accepted:
                -> vesna.quest_in_progress_dialogue
            - else:
                { QUEST_001_declined:
                    -> vesna.quest_declined_followup_dialogue
                - else:
                    -> vesna.intro_dialogue
                }
            }
        }
    }
}

=== vesna ===

= intro_dialogue
#speaker:Vesna
Hello, there. You look like a newcomer. What brings you here? 

#speaker:Player
I was told by your so-called Shaman to gather sheep from here. Where can I find them? 

#speaker:Vesna
The Shaman? Sheep? Here? I’m afraid you might have misunderstood him, there are no sheep here.

#speaker:Player
How come? I was instructed to collect sheep from here, was I lied to?

#speaker:Vesna
I’m sorry to hear that but I can promise you there are no sheep here. I do know where you might find some.

#speaker:Player
And where would that be?

#speaker:Vesna
Go to the nearby ruins outside the village. It hasn’t been explored or set foot in for a long time so there should be something interesting there. 

#speaker:Vesna
I can show you the way. Follow me.
~ QUEST_001_accepted = true
~ QUEST_001_declined = false
~ StartQuest("QUEST_001")
-> END

= quest_declined_followup_dialogue

#speaker:Vesna
Are you ready to head out now?

+ [(Accept Quest) Yes, let’s get this over with.]
    ~ QUEST_001_accepted = true
    ~ QUEST_001_declined = false
    ~ StartQuest("QUEST_001")
    
    #speaker:Vesna
    Very well. I will be waiting for you by the entrance to [Lvl2].
+ [(Decline Quest) No, I am not ready to do this yet.]
    ~ QUEST_001_accepted = false
    ~ QUEST_001_declined = true
    
    #speaker:Vesna
   - I will be waiting here.

-> END

= quest_in_progress_dialogue
#speaker:Vesna
From here on you can make your own way, I’m sure. Just go in the general direction of where we were going and you’ll be fine.
-> END

= quest_completed_dialogue
~ QUEST_001_completed = true

-> END
