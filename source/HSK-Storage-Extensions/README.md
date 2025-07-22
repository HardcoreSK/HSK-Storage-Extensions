# Life Lessons

Rimworld mod that adds the concept of proficiency/capability and learning mechanisms.

## Project Scope
The intent of this mod is to create a new system that simulates an additional layer of knowledge at the individual level. To explain this layer, it's important to define the other layers of the system. To this end we define:
- Knowledge - I know or have access to a collective knowledge that describes some understanding or technology. This is Rimworld's Research system.
- Ability - I am able to utilize my knowledge through use of skills, abilities, or understanding of concepts. 
- Aptitude - I am skilled at utilizing my abilities. This is Rimworld's skill system
With the above definitions, we can create this example: 
A colony that has ship building technology discovered has the knowledge of ship building (and prereqs)  
An individual does not necessarily have the ability to build ship components. They will need the ability to use advanced construction techniques and tools, which may also require other abilities and individual knowledge, such as mathematics or the ability to read.  
Just because a person knows how to build ships and has the necessary abilities, does not mean they are good at it, and their individual skill will still apply to how effective they are at it.

This mod is the answer to the middle layer, between knowledge and skill is ability. How can a surgeon transplant a heart if they don't even know where a heart is?

**Knowledge versus ability, and granularity**
The scope of this mod includes both individual knowledge and ability. The ability to read, or understanding the scientific process are both in scope. What falls out of scope is broadly speaking a matter of granularity. The goal is to provide enough granularity (or extensibility) that other mods may utilize these proficiencies, while still being abstract enough that it doesn't heavily impede players.  
In regards to granularity, it will be case-by-case and may become more granular over time.  
As an example, Bow Crafting would be too granular, as it can be more readily lumped in as Basic Fabrication, along with a large number of other less specific items. Meanwhile having the only science proficiency be, broadly, Science, would be far too monolithic. There are many abilities that fall under that category that it makes more sense to split them out into separate proficiencies.

**Skill gain versus Proficiency gain**
Both skill gain and proficiency gain are in scope for the learning and teaching modules. Classes and workshops may be configured such that a specific proficency is taught (mathematics) or a specific skill is taught (crafting) in order to train it as a group setting. Alternatively, teaching proficiencies might also teach skills.

## Driving tenets and principles
The following principles will drive decisions regarding scope, content, etc.

1. Believability > Realism - This mod will not strive for realism, but for believability. It is realistic that crafting a knit sweater uses different skills than sewing a linen jacket. It is believable that someone with the ability to craft one can figure out how to craft the other.
2. The colony lives - The colony is a living community of people who strive to succeed and grow. It is assumed that players using this mod will be seeking out a long term colony, and the mod will therefor not be balanced around the "vanilla game length" which tends to run on the short side. It is probable that many colonies will be wiped out before fully exploring the content of this mod if the player is not using other mods to lengthen their game.
3. Losing is fun - Enhancing the complexity and difficulty of the game through barriers is the entire point of the mod, no effort will be made to make the mod more forgiving. It is entirely possible that a tribal start may never make it to the stars purely because they cannot gain access to the abilities necessary to build a spaceship. It is also possible they may never find someone to teach them how to read.
