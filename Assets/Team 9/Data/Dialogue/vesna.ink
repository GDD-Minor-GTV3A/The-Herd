VAR quest_accepted = false
VAR all_objectives_completed = false
VAR QUEST_001_completed = false
VAR quest_declined = false
VAR after_quest_completed = false

EXTERNAL StartQuest(string)
EXTERNAL CompleteObjective(string, string)

// Entry point
-> vesna_router

=== vesna_router ===
{ after_quest_completed:
    -> vesna.after_quest_completed_dialogue
- else:
    { QUEST_001_completed:
        -> vesna.quest_completed_dialogue
    - else:
        { all_objectives_completed:
            -> vesna.quest_completed_dialogue
        - else:
            { quest_accepted:
                -> vesna.quest_in_progress_dialogue
            - else:
                { quest_declined:
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
Yes, hello. I recently just arrived.

#speaker:Narrator
Vesna looks a bit surprised by the cold tone and short words but hides it behind a smile.

#speaker:Vesna
I see. Did you come here by accident or were you running from something like most of us?

+ [I don't think that's any of your business.]
    #speaker:Narrator
    Vesna’s smile drops a little and she bows her head in apology.
    #speaker:Vesna
    Forgive me, you are right. Your reasons are your reasons alone and I will respect that.
+ [My past.]
    #speaker:Vesna
    Ah, this is the sad reality of so many of us. I am sorry that you've felt the need to do that.
+ [Don't say anything.]
    #speaker:Vesna
    I understand your silence, forgive me for my bluntness. I hope you do not have to keep running and feel safe enough to stay in one place.

-

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

+ [How precious exactly was it?]
    #speaker:Vesna
    Very precious. It is one of a kind and worn by a Samodiva at that. If it has to be translated in human terms, it costs a fortune that can last you a lifetime.
+ [I don't see how this is of any relevance to me.]
    #speaker:Vesna
    Perhaps it is not. But at the very least, you have your sheep with you who look just as precious to me. I wouldn't wish to lose them the way I lost my bracelet.
+ [Kueze 3]
    #speaker:Vesna
    goede keuze
-

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

= quest_declined_followup_dialogue
#speaker:Vesna
Oh, hello again. Have you reconsidered my request?

#speaker:Player
What request?

#speaker:Vesna
My bracelet. The one I lost in **[Level 2]**. I understand you were busy before, but I still need help finding it.

#speaker:Vesna
As I mentioned, I would be in your debt. My connection to nature and animals could be invaluable to you and your sheep.

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
    ~ quest_accepted = false
    ~ quest_declined = true

    #speaker:Vesna
    I understand. The offer remains open whenever you're ready.
    -> END

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

= quest_completed_dialogue
~ QUEST_001_completed = true

#speaker:Vesna
Welcome back! I can see by the look in your eyes that you have good news for me.

#speaker:Player
I found your bracelet. Here it is.

#speaker:Vesna
Wonderful! Ah, thank you so much! You have done me a great favor. 

#speaker:Player
Now my part is done. You will help me when I need it, correct? 

#speaker:Narrator
Something flashes in Vesna’s eyes but her usual smile stays unchanging

#speaker:Vesna
Well yes, of course. A samodiva never forgets those who have lent her a hand.

#speaker:Vesna
In fact, in order to prove my sincerity, I will give you one of my Blessings. 

//Give the player a reward !!!!

~ after_quest_completed = true
-> END

= after_quest_completed_dialogue
~ after_quest_completed = true

#speaker:Vesna
{~Well, hello dear. I’m glad to see that you’ve familiarized yourself with the place. | Hi, there. | Ah, look at these adorable sweetings. How I love them! | If you ever need me for something, don’t feel too shy to ask.}

-> END
