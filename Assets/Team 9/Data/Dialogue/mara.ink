VAR QUEST_002_accepted = false
VAR all_QUEST_002_objectives_completed = false
VAR QUEST_002_completed = false
VAR QUEST_002_declined = false
VAR after_QUEST_002_completed = false
VAR STAGE_001_completed = false

EXTERNAL StartQuest(string)
EXTERNAL CompleteObjective(string, string)

// Entry point
-> mara_router

=== mara_router ===
{ after_QUEST_002_completed:
    -> mara.after_quest_completed_dialogue
- else:
    { QUEST_002_completed:
        -> mara.quest_completed_dialogue
    - else:
        { all_QUEST_002_objectives_completed:
            -> mara.quest_completed_dialogue
        - else:
            { STAGE_001_completed:
                -> mara.stage_002_dialogue
            - else:
                { QUEST_002_accepted:
                    -> mara.quest_in_progress_dialogue
                - else:
                    { QUEST_002_declined:
                        -> mara.quest_declined_followup_dialogue
                    - else:
                        -> mara.intro_dialogue
                    }
                }
            }
        }
    }
}

=== mara ===

= intro_dialogue
#speaker:Mara
You should not have come here… The cold follows those who seek me.

+ [I was just passing by.]
    #speaker:Mara
    No one comes here “by accident.” The woods do not allow it.
+ [Are you Mara? The villagers warned me about you.]
    #speaker:Mara
    They fear what they do not understand. One day, you will too.
+ [Say nothing.]
    #speaker:Narrator
    Mara smiles faintly, as if your silence pleased her.
-

#speaker:Mara
I can hear them… your sheep. Their wool carries the breath of life.  
But winter is approaching. And winter… takes.

#speaker:Player
What do you want from me?

#speaker:Mara
Not from you. From Her.  
*She points at something unseen.*  
Morana watches us. She demands the old ways be restored.

+ [I don’t deal with spirits.]
    #speaker:Mara
    You already are. Every night you sleep, every time your sheep bleat in the shadows… That is Her whisper.
+ [What ritual?]
    #speaker:Mara
    The effigy must be made. Straw, cloth, ribbon… and a mask. Only then will she accept the offering.
+ [Remain silent.]
    #speaker:Mara
    Silence is wise. Words can anger her.
-

#speaker:Mara
Bring me the pieces. I will carve the face of the goddess.  
And together… we will drown death itself.

+ [Accept quest: I’ll find the items you need.]
    ~ QUEST_002_accepted = true
    ~ QUEST_002_declined = false
    ~ StartQuest("QUEST_002")

    #speaker:Mara
    The old ways will thank you. Gather straw, cloth, ribbon… and the mask. When all is found, return to me.
    -> END

+ [Decline quest: I want nothing to do with this.]
    ~ QUEST_002_accepted = false
    ~ QUEST_002_declined = true

    #speaker:Mara
    Then winter will claim what it is owed. I will be here when you understand.
    -> END


= quest_declined_followup_dialogue
#speaker:Mara
You return… yet without her gifts. Have you changed your mind?

+ [Accept quest: I’ll help you this time.]
    ~ QUEST_002_accepted = true
    ~ QUEST_002_declined = false
    ~ StartQuest("QUEST_002")

    #speaker:Mara
    Good. Gather straw, cloth, ribbon, and the mask. Bring them to me when you are ready.
    -> END

+ [Decline quest: I still refuse.]
    ~ QUEST_002_accepted = false
    ~ QUEST_002_declined = true

    #speaker:Mara
    Then Her hunger will not wait forever. Go, while the frost still spares you.
    -> END


= quest_in_progress_dialogue
#speaker:Mara
Have you found what I asked for?

#speaker:Player
Not yet.
//comment
#speaker:Mara
Then keep to the paths where the snow does not whisper. Bring the pieces soon… the air grows colder with each hour.
-> END


= stage_002_dialogue
~ STAGE_001_completed = true
#speaker:Narrator
The player returns to Mara’s cabin. The mist outside is heavier than before. Inside, the air is freezing.

#speaker:Mara
You have them… I can feel their weight before you even step inside.

#speaker:Player
Yes. Here.

#speaker:Narrator
Mara touches the straw, the fabric, the ribbon. Her hand lingers on the red ribbon, caressing it as if it were alive.

#speaker:Mara
Good… good… the threads of winter are weaving together again.  
But it is not complete.

#speaker:Player
What’s missing?

#speaker:Mara
The face. Without a face, she cannot see us. Without eyes, she cannot judge us.

#speaker:Narrator
Mara picks up a sheep skull from the corner. Her hands tremble as she carves pagan symbols into it with a rusty knife, murmuring strange words.

+ [What are you whispering?]
    #speaker:Mara
    Not for your ears. To speak her name too often is to invite frost into your lungs.
+ [This is madness.]
    #speaker:Mara
    Madness is denying the cold when your bones already ache. Madness is pretending the dead do not hunger.
+ [Say nothing.]
    #speaker:Mara
    Wise again. The sheep bleat, but the shepherd knows when silence is safer.
-

#speaker:Mara
It is done. The effigy breathes now.  
You must carry it to the altar, north of the village. Cast it into the frozen water, or the hunger of winter will never end.

#speaker:Player
And if I refuse?

#speaker:Mara
Then you will feed her with your flock… and your soul.
~ CompleteObjective("QUEST_002", "NPC_MARA_STAGE_1")
~ all_QUEST_002_objectives_completed = true
-> END

    


= quest_completed_dialogue
~ QUEST_002_completed = true
~ all_QUEST_002_objectives_completed = true

#speaker:Narrator
The player carries the effigy through the storm to the frozen altar.  
The sheep panic. The dog refuses to approach. Mara appears near the altar.

#speaker:Mara
Throw it into the water… let her wake.

#speaker:Player
And if I don't?

#speaker:Mara
Then the frost will claim you… one breath at a time.


#speaker:Narrator
The player casts the effigy into the frozen waters.  

#speaker:Narrator
The ice cracks loudly. A heavy silence falls.  

#speaker:Narrator
Mara’s form begins to shift—her eyes glow pale, her skin turns to frost.

#speaker:Mara
She is here… She is me.

#speaker:Player
+ [What are you?]
    #speaker:Morena
    I am the end that walks with every step of man. I am the silence in the snow. I am death dressed in winter’s veil.
+ [Stay away from me!]
    #speaker:Morena
    You walk on my breath, shepherd. Where would you run?
+ [Say nothing.]
    #speaker:Morena
    Silent… like the grave. Fitting.
-

#speaker:Morena
Every step toward survival is also a step toward death.  
From now on, shepherd… you belong to me.
~ CompleteObjective("QUEST_002", "NPC_MARA_STAGE_2")
~ after_QUEST_002_completed = true
-> END


= after_quest_completed_dialogue
~ after_QUEST_002_completed = true

#speaker:Mara
{~The frost has quieted since your offering. | She watches through your eyes now. | The sheep sleep without dreams. | Winter is patient, shepherd. Always patient.}

-> END