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

#speaker:Player
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
    I will be waiting here.
-
-> END

= quest_in_progress_dialogue
#speaker:Vesna
From here on you can make your own way, I’m sure. Just go in the general direction of where we were going and you’ll be fine.
-> END

= quest_completed_dialogue
~ QUEST_001_completed = true

#speaker:Vesna
I am deeply thankful for your help and won’t forget this. Neither will the villager as word spreads fast.

#speaker:Player
So I can assume that I will not be met with further hostility?

#speaker:Vesna
Yes, and to show you that we always pay back those who have lent us a hand, you can have one of my Blessings.

#speaker:Player
Thank you.

#speaker:Vesna
Take care, dear Shepherd. Find me in the village and we will talk again.
~ after_QUEST_001_completed = true
-> END

= after_quest_completed_dialogue
~ after_QUEST_001_completed = true

#speaker:Vesna
Oh… you’re back. I’ve been thinking about what we saw in the forest.

#speaker:Vesna
That old woman, the one standing between the trees. She wasn’t… ordinary.


+ [You know her?]
    #speaker:Vesna
    Know her? No… but the villagers whisper about her. They call her Mara. The Snow Widow. She lives near the woods… when she wants to.
+ [I thought she was just a villager.]
    #speaker:Vesna
    If she were just a villager, the forest wouldn’t have gone silent like that.
+ [Say nothing.]
    #speaker:Vesna
    She appears when winter wants something. When the cold chooses someone.

#speaker:Vesna
When I saw her, I… felt something. As if she was watching you, not me.
 And then the fog grew colder.

+ [Why would she watch me?]
+ [What does she want?]
+ [Where can I find her?]
-

#speaker:Vesna
I don’t know. No one really does.

#speaker:Vesna
She lost her family long ago, some say winter itself took them. And ever since… the cold follows her.

#speaker:Vesna
But I believe she knows more about this winter than she lets anyone see.

#speaker:Vesna
If you want real answers… you should find her cabin

#speaker:Vesna
Follow the river north. When the fog clings low to the ground, her home is near

#speaker:Player
And talk to her? Just like that?

#speaker:Vesna
Be careful. Old things live in those trees. But… if anyone can help you, it’s her.

#speaker:Vesna
She doesn’t speak to most villagers. But she might speak to you.

#speaker:Vesna
The woods led you to her once, maybe they want you to find her again.

~ StartQuest("QUEST_011")

-> END
