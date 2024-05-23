# Netcode for Entities - Web Build

## Build Configuration
- Unity Version: 2023.2.20f1
- Entities Version: 1.2.0
- Server Config: Run in Unity Editor
- Client Config: Web build deployed/served on a local https instance

## Bug/Error
When connecting to a unity server instance of a game, the web client has two errors in the browsers inspector console log.

```
NotSupportedException: To marshal a managed method, please add an attribute named 'MonoPInvokeCallback' 
to the method definition. The method we're attempting to marshal is: 
Unity.NetCode.ComponentSerializationHelper`3[[Unity.Physics.PhysicsVelocity, 
Unity.Physics, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],
```

and 

```
9f0ebc4b-d105-4b8d-87a3-a37962b7ea7d:9  Received a ghost - EN_Player - from the server which
has a different hash on the client (got 6806886325826531417 but expected 10301392053713532870). 
GhostPrefab: Entity(139:1) ('')
```

the game then disconnects.

## How to Reproduce
1. In the unity editor, enter play mode then click the "Server" button, the editor is now the server
2. Ensure **serve** is installed on your local system. At the terminal enter
```npm install -g serve```
3. Navigate to the folder BasicNetcodeWebGame/Builds/Client in your terminal and run the following command
```serve -s -l 3000 --ssl-cert server.cert --ssl-key server.key```
4. You can now open a browser and navigate to https://localhost:3000
5. Hit "Client" button in the web build of the browser
6. A blue circle player should be appearing but instead we get the previously mentioned errors in the inspector console
7. If running additional client builds do not delete the server.crt, server.key abd server,csr files