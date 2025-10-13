// Replace: NPC_NAME, [QUEST_ID]

VAR NPC_NAME_quest_accepted = false
VAR NPC_NAME_quest_completed = false
VAR NPC_NAME_quest_declined = false

EXTERNAL StartQuest(string)
EXTERNAL CompleteObjective(string, string)

-> NPC_NAME_router

=== NPC_NAME_router ===
{ NPC_NAME_quest_completed:
    -> NPC_NAME.quest_completed
- else:
    { NPC_NAME_quest_accepted:
        -> NPC_NAME.quest_in_progress
    - else:
        { NPC_NAME_quest_declined:
            -> NPC_NAME.quest_declined_followup
        - else:
            -> NPC_NAME.intro
        }
    }
}

=== NPC_NAME ===

= intro
#speaker:NPC_NAME #layout:right
[Greeting and introduction]

#speaker:Player #layout:left
[Response]

#speaker:NPC_NAME #layout:right
[Quest pitch]

+ [Accept: Yes]
    ~ NPC_NAME_quest_accepted = true
    ~ NPC_NAME_quest_declined = false
    ~ StartQuest("[QUEST_ID]")
    [Thank you, quest details]
    -> END
+ [Decline: No]
    ~ NPC_NAME_quest_declined = true
    [Understands]
    -> END

= quest_declined_followup
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
    ~ NPC_NAME_quest_accepted = true
    ~ NPC_NAME_quest_declined = false
    ~ StartQuest("[QUEST_ID]")
    #speaker:NPC_NAME #layout:right
    [Wonderful! Quest details/location]
    #speaker:Player #layout:left
    [Acknowledgment]
    -> END
+ [Decline quest: No, I still have other priorities]
    ~ NPC_NAME_quest_declined = true
    #speaker:NPC_NAME #layout:right
    I understand. The offer remains open whenever you're ready.
    -> END

= quest_in_progress
#speaker:NPC_NAME #layout:right
Have you found [the item/completed task]?

#speaker:Player #layout:left
Not yet.

#speaker:NPC_NAME #layout:right
[Reminder/encouragement]
-> END

= quest_completed
~ NPC_NAME_quest_completed = true
#speaker:NPC_NAME #layout:right
[Thank you! Reward/promise]

#speaker:Player #layout:left
[Response]
-> END