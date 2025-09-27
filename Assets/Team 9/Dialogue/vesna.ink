VAR vesna_quest_accepted = false

// START HERE: This line tells the Ink engine where to begin the story.
-> vesna_quest.intro_dialogue

=== vesna_quest ===

// --- First Dialogue Block and Choice ---
= intro_dialogue
#layout:narrator
The player goes up to Vesna and interacts with her.

#speaker:Vesna #layout:right
Oh, hello there. You must be the newcomer to our Village? It is nice to meet you.

#speaker:Player #layout:left
Yes, hello. I recently just arrived.

#layout:narrator
Vesna looks a bit surprised by the cold tone and short words but hides it behind a smile.

#speaker:Vesna #layout:right
I see. Did you come here by accident or were you running from something like most of us?

* [I don’t think that’s any of your business.]
    #speaker:Vesna #layout:right
    Forgive me, you are right. Your reasons are your reasons alone and I will respect that.
    -> after_first_choice
* [My past.]
    #speaker:Vesna #layout:right
    Ah, this is the sad reality of so many of us. I am sorry that you’ve felt the need to do that.
    -> after_first_choice
* [Don’t say anything.]
    #speaker:Vesna #layout:right
    I understand your silence, forgive me for my bluntness. I hope you do not have to keep running and feel safe enough to stay in one place.
    -> after_first_choice

// --- Continuation Dialogue Block after the first choice ---
= after_first_choice
#speaker:Vesna #layout:right
Is there anything I can help you with?

#speaker:Player #layout:left
No. I have come here to seek refuge, not ask for help from strangers.

#layout:narrator
Vesna looks unphased by the hostility of the player and keeps her warm expression.

#speaker:Vesna #layout:right
That is understandable. I also came here to seek a new home as the war progressed and I was forced to leave that which was most precious to me.

#speaker:Vesna #layout:right
I see you have not come with much? Are your sheep all you’ve got?

#layout:narrator
Player visibly tenses and quickly scans the area around them as if looking for sudden danger.

#speaker:Player #layout:left
Why are you questioning me so much? You look like you don’t have much either.

#layout:narrator
Vesna’s eyes turn slightly sad and she sighs in melancholy.

#speaker:Vesna #layout:right
You are right. I couldn’t bring my belongings with me besides the clothes on my back and flowers on my head.

#layout:narrator
She touches the wrist of her right hand and shakes her head as if trying to shoo away unwanted thoughts.

#speaker:Vesna #layout:right
I lost something very precious to me. My bracelet, gifted to me by one of the wild horses in the valley, made by their hair and the gold from the soil underneath their feet.

-> quest_pitch_choice

// --- Second Dialogue Block and Choice ---
= quest_pitch_choice
* [How precious exactly was it?]
    #speaker:Vesna #layout:right
    Very precious. It is one of a kind and worn by a Samodiva at that. If it has to be translated in human terms, it costs a fortune that can last you a lifetime.
    -> after_second_choice
* [I don’t see how this is of any relevance to me.]
    #speaker:Vesna #layout:right
    Perhaps it is not. But at the very least, you have your sheep with you who look just as precious to me. I wouldn’t wish to lose them the way I lost my bracelet.
    -> after_second_choice

// --- Final Decision Block ---
= after_second_choice
#speaker:Vesna #layout:right
Maybe you could help me find it?

#speaker:Player #layout:left
Why would I do that? As I said, I came here to seek refuge and make sure I’m safe, not become an errand boy for random people.

#speaker:Vesna #layout:right
Think of it as a favour of some kind, maybe. If you bring my bracelet, I will owe you anything you ask of me, no matter what it is. I am a creature of nature, after all, and can do things no normal human being is capable of.

#speaker:Vesna #layout:right
I am also connected to animals in a way no one is. I presume that would be handy when it comes to your sheep, no?

#layout:narrator
The player grows extremely quiet and still, thinking hard on her proposal. It is in his best interest to keep his fami- sheep safe so maybe it is not that bad of an idea.

#speaker:Player #layout:left
If I do this for you, you are in my debt, correct?

#speaker:Vesna #layout:right
Yes, precisely.

// Note: The choice text is separated from the outcome text.
+ [Accept quest: I will do it]
    ~ vesna_quest_accepted = true
    #speaker:Vesna #layout:right
    That is very helpful of you! Thank you. You will have to head out to **[Level 2]** and look around as much as possible. I went through that area before I came here and it’s the last time I remember wearing it.
    #speaker:Player #layout:left
    I will have a look. I hope this will be worth my efforts.
    -> END
+ [Decline quest: I have more important matters to attend to than this]
    ~ vesna_quest_accepted = false
    #speaker:Vesna #layout:right
    That is no problem. I will be here if you decide otherwise.
    -> END