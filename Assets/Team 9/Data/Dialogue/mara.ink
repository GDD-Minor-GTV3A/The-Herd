VAR intro_complete = false

EXTERNAL StartQuest(string)
EXTERNAL CompleteObjective(string, string)

// Entry point
-> mara_router

=== mara_router ===
{ intro_complete:
    -> END
- else:
    -> mara.intro_dialogue
}

=== mara ===

= intro_dialogue

#speaker:Mara
Lost already? The shaman tends people in circle

#speaker:Player
He said that they were sheep here

#speaker:Mara
They were. Once before the winter changed

#speaker:Player
So where are they now?

#speaker:Mara
... In the ruins. I hear them sometimes. Faint bleating carried by the wind

#speaker:Player
You are sure?

#speaker:Mara
As sure as one can be about anything in this cold

#speaker:Player
How do I get there?

#speaker:Mara
Take the path behind the well. Don’t stray from it. The forest doesn’t forgive wandering

#speaker:Mara
If the shaman wants sheep, let's fetch them himself. But since he sent you, you would better hurry

-> END