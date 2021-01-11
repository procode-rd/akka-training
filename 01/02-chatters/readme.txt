Purpose:
- create instance of an actor
- send simple messages from one actor to another
- respond to message

ChatActor -> 
awaits any ChatMessage. on receival, responds to it (if it was question) and randomize if ask own question to sender. 
finishes it's existence when randomized that not in the mood of talk.


Hierarchy:

user [system actor]
 |
 +--> ChatActor [first child]
 |
 +--> ChatActor [second child]