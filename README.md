# EvolvingNeuralNetwork
This project contains a simple implementation of evolving neural networks through random mutations in c#. 
Generations of birds evolve to finally master flying through obstacles. Feel free to use this Unity project for your own fun. Check it out on Youtube:
https://youtu.be/3Tjy9F2I5mU

# 1. Birds
Each bird sends out rays to 5 directions and calculates the distance to the nearest objects. This is implemented using a RaycastHit2D from Unity’s Physics2D Engine.As long as the bird flies it incrementally gains fitness. The higher the fitness the better. The fitness will later be used by the generation manager to evolve the neural networks and therefore create an ever advancing next generation.To avoid crashing into obstacles the bird can make two actions: flap and rotate the angle of the wings.
https://github.com/exoflow/EvolvingNeuralNetwork/blob/master/Assets/Scripts/Bird.cs

# 2. Neural Network
The neural network functions as the brain of the bird. The distances to the nearest obstacles are passed as inputs to the neural network. Each neuron sums the values of all neurons from the previous layer multiplied by the connection weights. The last layer then produces the output.
In this project I used a simple feed forward neural network with a linear activation function. The connection weights are initially randomly assigned and later randomly mutated.
https://github.com/exoflow/EvolvingNeuralNetwork/blob/master/Assets/Scripts/NeuralNetwork.cs

# 3. Environment
The obstacles are generated with a random height. Invariant is the gap between them. Therefore, the longer the top obstacle the shorter the bottom obstacle. Right after instantiating the obstacles they receive a force to the left which forces the birds to react.
https://github.com/exoflow/EvolvingNeuralNetwork/blob/master/Assets/Scripts/BorderCollision.cs

# 4. Generation Manager
Every bird is evaluated by its fitness. When all birds in a generation are dead the generation manager will use the neural network of the fittest bird to create a new evolved generation. The evolvement is implemented with random mutations of the weights between the neurons in the neural network. The mutation of the weights influenced the decision making of the bird which causes it to flap differently and to choose a different angle of rotation. Whether these mutations created a fitter generation is unknown. However, over time there is a clear progression of the birds fitness which means that the generation manager is working correctly and that over many generations the fitness gradually evolves.
https://github.com/exoflow/EvolvingNeuralNetwork/blob/master/Assets/Scripts/Manager.cs

# Assets
I downloaded the game background from https://opengameart.org/content/game-background-forest, it is published under the CC0 license. 
All other scripts and assets I created myself. I appreciate if you credit me and I'm interested to see your projects if this one inspired you.
