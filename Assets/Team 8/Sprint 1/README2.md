Hello integration team , please read README1 before this.

## User story
" To be introduced to the shaman and realize everything comes with cost"

## What have we done
 - full economy system where you trade/ purchase items from the shaman.
 - Shaman 3D model introudcing it
 ## Classes
 - `CurrencyData`: Generic type representing any tradable object  
 - `Wallet`: Container that holds different currencies  
 - `Trading`: Static class to transfer currency 
 - `GetCurrency`: Generates Currency
 - `ShopItemData`: ScriptableObject representing a shop item (name, currency, cost, success message).
 - `CurrencyUI`: Binds (shows) a CurrencyData balance to a TMP_Text field.
 - `ShopSystem`: Main shop logic: hooks to the player’s wallet, updates UI, processes purchases.

## Notes
- There are 2 more classes `Player` and `Shaman` which are used only for testing purposes( Moving currenices from wallet to another).
- The `GetCurrency` class would be adjusted later when it is decided how/where to get currency from in the game. but for now we just made it that you can generate
  unlimited currency by clicking x and c.
- You will find the main Core.Economy scripts in                    Sprint 1 -> Economy -> Scripts
- You will find a `Player` and `Shaman` + testing scenes in         Sprint 1 -> Economy -> Example
- Player is NOT our responsibility, the class is just for testing.
- Besides the Economy folder, there are " Sprites" and " Materials" which are plaeholders for some UI Elements.( Yes we do understand there should not be any
  placeholders but these are not part of the user story, and not something that needs to be done for sprint 1, it's also for testing purposes :D )


If there is something unclear, don't hesitate to contact us. Ping us on discord and we gonna help
Have a good day!

Team 8: Shaman 