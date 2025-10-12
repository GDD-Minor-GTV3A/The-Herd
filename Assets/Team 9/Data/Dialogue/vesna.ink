VAR quest_accepted = false
VAR QUEST_001_completed = false
VAR quest_declined = false

EXTERNAL StartQuest(string)
EXTERNAL CompleteObjective(string, string)

// START HERE: Entry point that routes to correct dialogue based on state
-> vesna_quest_router

=== vesna_quest_router ===
// Check quest state and route to appropriate dialogue
{ QUEST_001_completed:
    -> vesna_quest.quest_completed_dialogue
- else:
    { quest_accepted:
        -> vesna_quest.quest_in_progress_dialogue
    - else:
        { quest_declined:
            -> vesna_quest.quest_declined_followup
        - else:
            -> vesna_quest.intro_dialogue
        }
    }
}

=== vesna_quest ===

// --- Initial Meeting Dialogue ---
= intro_dialogue
#speaker:Vesna
Oh, hello there. You must be the newcomer to our Village? It is nice to meet you.

#speaker:Player
Yes, hello. I recently just arrived.

#speaker:Narrator
Vesna looks a bit surprised by the cold tone and short words but hides it behind a smile.

#speaker:Vesna
I see. Did you come here by accident or were you running from something like most of us?

+ [I don't think that's any of your business.]
    #speaker:Vesna
    Forgive me, you are right. Your reasons are your reasons alone and I will respect that.
    -> after_first_choice
+ [My past.]
    #speaker:Vesna
    Ah, this is the sad reality of so many of us. I am sorry that you've felt the need to do that.
    -> after_first_choice
+ [Don't say anything.]
    #speaker:Vesna
    I understand your silence, forgive me for my bluntness. I hope you do not have to keep running and feel safe enough to stay in one place.
    -> after_first_choice

// --- Continuation Dialogue Block after the first choice ---
= after_first_choice
#speaker:Vesna
Is there anything I can help you with?

#speaker:Player
No. I have come here to seek refuge, not ask for help from strangers.

#speaker:Narrator
Vesna looks unphased by the hostility of the player and keeps her warm expression.

#speaker:Vesna
That is understandable. I also came here to seek a new home as the war progressed and I was forced to leave that which was most precious to me.

#speaker:Vesna
I see you have not come with much? Are your sheep all you've got?

#speaker:Narrator
Player visibly tenses and quickly scans the area around them as if looking for sudden danger.

#speaker:Player
Why are you questioning me so much? You look like you don't have much either.

#speaker:Narrator
Vesna's eyes turn slightly sad and she sighs in melancholy.

#speaker:Vesna
You are right. I couldn't bring my belongings with me besides the clothes on my back and flowers on my head.

#speaker:Narrator
She touches the wrist of her right hand and shakes her head as if trying to shoo away unwanted thoughts.

#speaker:Vesna
I lost something very precious to me. My bracelet, gifted to me by one of the wild horses in the valley, made by their hair and the gold from the soil underneath their feet.

-> quest_pitch_choice

// --- Second Dialogue Block and Choice ---
= quest_pitch_choice
+ [How precious exactly was it?]
    #speaker:Vesna
    Very precious. It is one of a kind and worn by a Samodiva at that. If it has to be translated in human terms, it costs a fortune that can last you a lifetime.
    -> after_second_choice
+ [I don't see how this is of any relevance to me.]
    #speaker:Vesna
    Perhaps it is not. But at the very least, you have your sheep with you who look just as precious to me. I wouldn't wish to lose them the way I lost my bracelet.
    -> after_second_choice
+ [Kueze 3]
    #speaker:Vesna
    goede keuze
    -> after_second_choice

// --- Final Decision Block ---
= after_second_choice
#speaker:Vesna
Maybe you could help me find it?

#speaker:Player
Why would I do that? As I said, I came here to seek refuge and make sure I'm safe, not become an errand boy for random people.

#speaker:Vesna
Think of it as a favour of some kind, maybe. If you bring my bracelet, I will owe you anything you ask of me, no matter what it is. I am a creature of nature, after all, and can do things no normal human being is capable of.

#speaker:Vesna
I am also connected to animals in a way no one is. I presume that would be handy when it comes to your sheep, no?

#speaker:Narrator
The player grows extremely quiet and still, thinking hard on her proposal. It is in his best interest to keep his fami- sheep safe so maybe it is not that bad of an idea.

#speaker:Player
If I do this for you, you are in my debt, correct?

#speaker:Vesna
Yes, precisely.

+ [Accept quest: I will do it]
    ~ quest_accepted = true
    ~ quest_declined = false
    ~ StartQuest("QUEST_001")
    #speaker:Vesna
    That is very helpful of you! Thank you. You will have to head out to **[Level 2]** and look around as much as possible. I went through that area before I came here and it's the last time I remember wearing it.
    #speaker:Player
    I will have a look. I hope this will be worth my efforts.
    -> END
+ [Decline quest: I have more important matters to attend to than this]
    ~ quest_accepted = false
    ~ quest_declined = true
    #speaker:Vesna
    That is no problem. I will be here if you decide otherwise.
    -> END

// --- Quest Declined Follow-up Dialogue ---
= quest_declined_followup
#speaker:Vesna
Oh, hello again. Have you reconsidered my request?

#speaker:Player
What request?

#speaker:Vesna
My bracelet. The one I lost in **[Level 2]**. I understand you were busy before, but I still need help finding it.

#speaker:Vesna
As I mentioned, I would be in your debt. My connection to nature and animals could be invaluable to you and your sheep.

#speaker:Player
I see.

+ [Accept quest: Alright, I'll help you find it]
    ~ quest_accepted = true
    ~ quest_declined = false
    ~ StartQuest("QUEST_001")
    #speaker:Vesna
    Wonderful! I truly appreciate this. Please search **[Level 2]** thoroughly. I'm certain it must be there somewhere.
    #speaker:Player
    I'll see what I can find.
    -> END
+ [Decline quest: No, I still have other priorities]
    ~ quest_accepted = true
    #speaker:Vesna
    I understand. The offer remains open whenever you're ready.
    -> END

// --- Quest In Progress Dialogue ---
= quest_in_progress_dialogue
#speaker:Vesna
Oh, hello again! Have you found my bracelet yet?

#speaker:Player
Not yet. I'm still searching.

#speaker:Vesna
I understand. Please take your time. Remember, it should be somewhere in **[Level 2]**. I have faith you will find it.

#speaker:Player
I'll keep looking.

-> END

// --- Quest Completed Dialogue ---
= quest_completed_dialogue
~ QUEST_001_completed = true
#speaker:Vesna
Welcome back! I can see by the look in your eyes that you have good news for me.

#speaker:Player
I found your bracelet. Here it is.

#speaker:Vesna
Oh, thank you so much! You have no idea how much this means to me. As promised, I am now in your debt.

#speaker:Narrator
Vesna carefully takes the bracelet and puts it on her wrist, her eyes glistening with unshed tears.

#speaker:Vesna
If you ever need my help with your sheep or anything else within my power, please do not hesitate to ask.

-> END