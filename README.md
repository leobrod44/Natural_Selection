# Natural Selection Simulator with Genetic Algorithms and Neural Networks

## Summary
Procedurally generated biomes, creatures, and neural networks for real-time decision-making. Genetic algorithms and neural networks are used to evolve behavior and gauge environment adaptation through selective reproduction.

## Example

Parameters: 
Batch per generation: 5, 
Size of each batch: 40, 
Selection Count: top 12, 
Inner layers: 2, 
Neuron per inner layer: 5, 
Sensor neurons: 4, 
Output neurons: 3

https://github.com/leobrod44/ML_Natural_Selection_Simulator/assets/65002959/dee09375-f070-44a6-9079-c4529e565988


<img src="https://github.com/leobrod44/ML_Natural_Selection_Simulator/assets/65002959/89dfbbd7-f4be-4105-9c2d-34464eb6d785" width="2800"> 

## Performance

This simulation uses the Unity game engine but bypasses all physics engines for movement and collision detections and instead uses matrices storing locations of important objects to determine where to move or where each object is stored. Movement is then simulated by performing small teleportations to the nearest desired tile.

This allows maximized training speed since each input value is fed to each NN and their outcome relies solely on local CPU processing power and RAM, maximizing training throughput. 

The example's 1x speed demonstrates this speed.

## Biome generation

### Natural habitat generation

-Spawning tiles that can have various properties and contain elements such as food, obstacles, water and trees. Lakes tiles spawned recursively.[Engine.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/Engine.cs) and [Area.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/Area.cs)


 medium nature density     |          low nature density  |     high nature density                  
:-------------------------:|:----------------------------:|:-------------------------:
![Screenshot 2023-01-16 154057](https://user-images.githubusercontent.com/65002959/212764644-402da6e2-a8bd-4b05-8286-8141488d8536.png) |  ![Screenshot 2023-01-16 154248](https://user-images.githubusercontent.com/65002959/212764676-71fdee57-4dad-4e42-b91d-144e48b784ec.png) | ![Screenshot 2023-01-16 154337](https://user-images.githubusercontent.com/65002959/212764755-b8768792-2efd-48c9-b279-3666f32e8097.png)

### Creature generation

 - Eventually, more complex character will be generated. For now, random sized and colored cubic animals with randomly generated latin names. Current implementation [Body.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/Body.cs). The animal's character control will happen through [Animal.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/Animal.cs)

 Sulyha Sotaenis           |          Maejubis Gijolis    |     Qeshusis Raso & Pebyshaho Haeqisheqa               
:-------------------------:|:----------------------------:|:-------------------------:
![Capture](https://user-images.githubusercontent.com/65002959/212765592-271b6f71-ed96-4a74-8e43-2dab3cc0e4f5.PNG) | ![Screenshot 2023-01-16 154742](https://user-images.githubusercontent.com/65002959/212765602-ef46d13a-01dc-477f-92ae-920f123e5835.png) |![Screenshot 2023-01-16 155040](https://user-images.githubusercontent.com/65002959/212765606-4bde5723-cbfa-4f33-adf2-59cce2cc1f70.png)

# Neural Network

This approach to reinforcement learning borrows common NN characteristics such as the input, inner layers and output neurons, each interconnected through weights and having their value tweaked by a bias. However, instead of employing traditional backpropagation for optimization, genetic algorithms are used. Across generations, this algorithm selects top performers of each batch to gradually narrow down weights and biases to their optimal value,  aiming to discover an optimal solution to surviving the longest.

Each individual in a population has a unique neural network generated. Either completely random on the initial population or based on a combination from a previous population

<img src="https://github.com/leobrod44/ML_Natural_Selection_Simulator/assets/65002959/22fc3feb-791b-4699-8674-0a77514254c75" width="2800">

## Activation Function: Hyperbolic Tangent Activation

![image](https://github.com/leobrod44/ML_Natural_Selection_Simulator/assets/65002959/513d51bc-f258-49c4-9ffb-76cbb6257ec8)

# Genetic Algorithm Design

## Fitness Function
How each individual's performance is evaluated 
![image](https://github.com/leobrod44/ML_Natural_Selection_Simulator/assets/65002959/81fd2cdb-1bc9-4477-afbd-f47e04269ea8)

## Crossover Function
How each subsequent generation is generated based on the previous
![image](https://github.com/leobrod44/ML_Natural_Selection_Simulator/assets/65002959/0058b70a-f4b7-49bf-ad41-44050d9ce7d8)

## Selection
The generation is sorted by descending order according to each indiviuals fitness. The top n individuals are selected to reproduce and produce the next generation

## Persistence

Each Neural Network can be stored as a list of hexadecimals each representing a connection. This can allow for optimzed brain files to be stored and reuploaded

![image](https://github.com/leobrod44/ML_Natural_Selection_Simulator/assets/65002959/57b1392d-bc06-4e77-bf99-b06dbddf1265)





