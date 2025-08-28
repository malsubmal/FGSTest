using System;

namespace FGSTest.Payload
{
    [Serializable]
    public enum GameState : byte
    {
        None = 0, 
        Running = 1,
        EndGame = 2,
    }
    
    
    [Serializable]
    public struct GameStateUpdatePayload
    {
        public GameState GameState;
        
        public GameStateUpdatePayload(GameState gameState)
        {
            GameState = gameState;
        }
    }
}