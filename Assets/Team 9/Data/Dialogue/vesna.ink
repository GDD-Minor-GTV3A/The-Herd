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
Oh, hello there. You must be the newcomer to our Village? It is nice to meet you.

#speaker:Player
Yes, hello. I’ve recently just arrived. 

#speaker:Vesna
I see. Did you come here by accident or were you running from something like most of us?

+ [I don’t think that’s any of your business.]
    #speaker:Vesna
    Forgive me, you are right. Your reasons are your reasons alone and I will respect that. 
+ [My past.]
    #speaker:Vesna
    Ah, this is the sad reality of so many of us. I am sorry that you’ve felt the need to do that. 
+ [Don’t say anything.]
    #speaker:Vesna
    I understand your silence, forgive me for my bluntness. 
-

#speaker:Vesna
Is there anything I can help you with?

#speaker:Player
No. I have come here to seek refuge, not ask for help from strangers.

#speaker:Vesna
I see. Seek refuge from yourself or from the war? You look like a soldier to me.

#speaker:Player
…

#speaker:Vesna
Soldiers aren’t generally welcome here. The inhabitants of our Village have been through way too much and don’t trust your kind. 

#speaker:Player
I am not proud of the acts I’ve committed but that’s behind me now. 

#speaker:Player
Do not condemn me for things you know nothing of. 

#speaker:Vesna
I am not condemning you but I cannot say the same for the rest of our Village.

#speaker:Vesna
If you want to be welcomed here, you must prove to us that you are a different person than those monsters outside.

#speaker:Player
And how do you suppose I should do that?

#speaker:Vesna
Before I came here, I had a companion with me. A beautiful stag that I shared life with.

#speaker:Vesna
But the war was just as merciless to him as it was to everyone else and he got shot as I was fleeing. His corpse is still where I left it.

#speaker:Vesna
I want to try and revive him but I will need protection while I use my magic. You could be the one I’ve been waiting for.

#speaker:Player
And what exactly makes you think I’m your so-called hero?

#speaker:Vesna
You are an ex soldier. You have good endurance and have killed before. Those skills will be very useful in this case.

#speaker:Vesna
And, this might be your opportunity to show us we can trust you. I am well-known in The Village so it will give a good impression.

#speaker:Player
Why must I prove myself to people I do not even know? How come no one else is doing this.

#speaker:Vesna
Everyone has been scarred by this war. People are scared of soldiers and you used to be one.

#speaker:Vesna
Use this chance to show us you are different from them. That you are a better man.

+ [(Accept Quest) i will do it]
    ~ QUEST_001_accepted = true
    ~ QUEST_001_declined = false
    ~ StartQuest("QUEST_001")
    
    #speaker:Vesna
    Very well. I will be waiting for you by the entrance to [Lvl2].
+ [(Decline Quest) i need time to think]
    ~ QUEST_001_accepted = false
    ~ QUEST_001_declined = true
    
    #speaker:Vesna
    I will be waiting here.
-
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
This is it. This is where my beloved companion was killed by these monsters and I had to leave him behind.

#speaker:Vesna
I will start the ritual now, dear Shepherd. It is very important that I am not interrupted or disturbed in any way during the process.

#speaker:Vesna
Will that be doable for you?

#speaker:Player
Yes. I have done far worse than shooting at a few enemies.

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
