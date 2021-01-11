Purpose:
- create an actor tree
- delegate actor creation with Props
- watching actors terminations
- supervising child actors
- learn how to design/create actor tree to solve parallel, concurrent, messages based systems

AdminActor -> 
when "n" hit, randomize 2 names and spawns ChannelActor as supervised child.
watches all spawned ChannelActors for their termination.
monitors amount of open ChannelActors with info.

ChannelActor -> 
when created, create two child ChatActors for names received, randomize greeter and instructs it to greet other.
watches both children for their termination. if one terminates, it instructs the other to terminate.
if no children, terminates itself.

ChatActor -> 
awaits any ChatMessage, on receival it responds to it (if it was question) and randomize if ask own question to sender. 
finishes it's existence when randomized that not in the mood of talk.


Hierarchy:

AdminActor [one - "root"]
 |
 V
ChannelActor [many]
 |
 +--> ChatActor [first child]
 |
 +--> ChatActor [second child]