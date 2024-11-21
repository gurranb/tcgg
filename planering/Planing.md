# MVP

- Win or lose-mekanik.
- 1 vs. 1 (lokalt).
- Kortlek med spelbara kort.
- Webbapplikation för spelet.
- Poängsystem.
- Möjlighet att lägga och dra kort.
- Grundläggande API.
- Kortlogik (exempelvis attack/healing).
- Hårdkodade kort för att testa spelet.

# UserStories
## Tasks
- [x] Spelare ska kunna dra kort från sin kortlek.
- [x] Spelare ska kunna spela ett kort.
- [ ] Spelare ska kunna göra sin egen kortlek.
- [x] Spelare ska kunna attackera kort på spelplanen.
- [ ] Spelare ska kunna attackera motståndaren om inga kort är kvar på spelplanen.
- [ ] Spelare ska kunna skapa en användare.
- [ ] Spelare ska kunna ändra sin kortlek.
- [ ] Spelare ska kunna se sin kortlek och dess egenskaper.
- [ ] Spelare ska kunna avbryta en match.
- [ ] Spelare ska kunna se sina vinster och förluster.
- [x] Spelare ska kunna se sin hand under spelet.
- [x] Spelare ska kunna se kort på spelplanen.
- [x] Spelare ska kunna se vad motståndaren lägger för kort.
- [ ] Spelare ska kunna se antal poäng kvar att spela för.
- [ ] Spelare ska kunna erbjuda rematch.
- [ ] Spelare ska kunna avbryta sin runda i förtid.
- [ ] Spelare ska kunna se en logg över tidigare händelser.
- [ ] Spelare ska kunna se statistik (vinster, förluster, snabbaste vinsten etc.).
- [ ] Spelare ska behöva klicka på en knapp för att avsluta rundan.
- [x] Spelare ska kunna se antal kort kvar i sin kortlek.

## Prio UserStories
-  Spelare ska kunna dra kort från sin kortlek.
-  Spelare ska kunna spela ett kort.
-  Spelare ska kunna attackera kort på spelplanen.
-  Spelare ska kunna se sin hand under spelet.
-  Spelare ska kunna se kort på spelplanen.
-  Spelare ska kunna se vad motståndaren lägger för kort.
-  Spelare ska kunna se antal kort kvar i sin kortlek.

## Spelare ska kunna dra kort från sin kortlek.
`DrawCard();`

## Spelare ska kunna spela ett kort.
`PlayCard();`

## Spelare ska kunna attackera kort på spelplanen.
`AttackCardOnTable();` `AttackCardOnPlayer();`

## Spelare ska kunna se sin hand under spelet.
`DisplayCardsInHand();`

## Spelare ska kunna se kort på spelplanen.
`DisplayCardsOnTable();`

## Spelare ska kunna se vad motståndaren lägger för kort.
`DisplayCardOnTable();`

## Spelare ska kunna se antal kort kvar i sin kortlek.
`DisplayAmountOfCardsInDeck();`

## Models
- Card
- Deck
- Player
- Board
- Match

### Card (2 olika kort)
- Id
- Name
- Health
- Attack


### Deck
- Id
- Card < List >


### Player
- Id
- Name
- Deck < List >
- Health = 10
- Hand < List of cards >
- CurrentTurn Bool
- MatchDeck < List of Deck > 
- CardOnBoard < List >


### Board
- Id
- Player 1
- Player 2
- Int Turn (player 1 spelar udda nummer, player 2 spelar jämna nummer)
- CombatZone (bara kort som är på spelplanen ska kunna attackeras)


### Match
- Id
- Board
- Status (win, lose, ongoing)