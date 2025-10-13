// SEE DOCUMENTATION AT 
// https://docs.google.com/document/d/17rfnE03GXNniznrM8fdLGHhFy1wNvdZoMoxpZ7nJq5U/edit?usp=sharing

// Replace: NPC_NAME, QUEST_ID

VAR quest_accepted = false
VAR QUEST_ID_completed = false
VAR after_quest_completed = false
VAR quest_declined = false

EXTERNAL StartQuest(string)
EXTERNAL CompleteObjective(string, string)

-> NPC_NAME_router

=== NPC_NAME_router ===
{ after_quest_completed:
    ->NPC_NAME.after_quest_completed_dialogue
- else:
    { QUEST_ID_completed:
        -> NPC_NAME.quest_completed_dialogue
    - else:
        { quest_accepted:
            -> NPC_NAME.quest_in_progress_dialogue
        - else:
            { quest_declined:
                -> NPC_NAME.quest_declined_followup_dialogue
            - else:
                -> NPC_NAME.intro_dialogue
            }
        }
    }
}
=== NPC_NAME ===

= intro_dialogue
#speaker:NPC_NAME #layout:right
[Greeting and introduction]

#speaker:Player #layout:left
[Response]

#speaker:NPC_NAME #layout:right
[Quest pitch]

+ [Accept: Yes]
    ~ quest_accepted = true
    ~ quest_declined = false
    ~ StartQuest("QUEST_ID")
    [Thank you, quest details]
    -> END
+ [Decline: No]
    ~ quest_declined = true
    [Understands]
    -> END

= quest_declined_followup_dialogue
#speaker:NPC_NAME #layout:right
Oh, hello again. Have you reconsidered my request?

#speaker:Player #layout:left
What request?

#speaker:NPC_NAME #layout:right
[Remind about quest item/task and location]

[Remind about reward/benefit]

#speaker:Player #layout:left
I see.

+ [Accept quest: Alright, I'll help you]
    ~ quest_accepted = true
    ~ quest_declined = false
    ~ StartQuest("[QUEST_ID]")
    
    #speaker:NPC_NAME #layout:right
    [Wonderful! Quest details/location]
    
    #speaker:Player #layout:left
    [Acknowledgment]
    -> END

+ [Decline quest: No, I still have other priorities]
    ~ quest_declined = true
    
    #speaker:NPC_NAME #layout:right
    I understand. The offer remains open whenever you're ready.
    -> END

= quest_in_progress_dialogue
#speaker:NPC_NAME #layout:right
Have you found [the item/completed task]?

#speaker:Player #layout:left
Not yet.

#speaker:NPC_NAME #layout:right
[Reminder/encouragement]
-> END

= quest_completed_dialogue
~ QUEST_ID_completed = true

#speaker:NPC_NAME #layout:right
[Thank you! Reward/promise]

#speaker:Player #layout:left
[Response]
~ after_quest_completed = true
-> END

= after_quest_completed_dialogue
~ after_quest_completed = true

#speaker:Vesna
Thanks but now go away please

-> END