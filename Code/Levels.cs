namespace BoulderDash
{
    public static class Levels
    {
        public class LevelData
        {
            public string Title;
            public string Hint;
            public int DiamondsNeeded;
            public int KillsNeeded;
            public float DriftSeconds;
            public bool Tutorial; 
            public bool RandomExit;  
            public int TimeSeconds;
        }

        public static readonly LevelData[] All =
        {
            new LevelData
            {
                Title = "Welcome to Hell",
                Tutorial = true,
                // by Yolly: begin first-level warning about falling pentagrams
                Hint = "Move with the arrow keys or WASD. You dig through dirt just by walking into it. Collect 3 rings (the spinning pentagrams) to open the gate, then walk into the gate. Falling pentagrams can crush you too.",
                // by Yolly: end first-level warning about falling pentagrams
                DiamondsNeeded = 3,
                TimeSeconds = 150,
            },
            new LevelData
            {
                Title = "Falling Rocks",
                Tutorial = true,
                // by Yolly: begin explain rolling skulls and pentagrams
                Hint = "Skulls and pentagrams fall into empty spaces. Both can roll off another skull or pentagram if the side and space below it are clear; either side may be chosen when both are open. A falling skull crushes you. A resting skull or pentagram is safe only while you keep moving. Stand still under either and it wobbles, then drops. You can push a skull sideways. Dig out the rings and reach the gate.",
                // by Yolly: end explain rolling skulls and pentagrams
                DiamondsNeeded = 3,
                TimeSeconds = 150,
            },
            new LevelData
            {
                Title = "Demons",
                Tutorial = true,
                Hint = "Demons patrol open tunnels and kill you on touch. They cannot dig through dirt. Drop a rock on one to defeat it. The gate needs 2 rings AND 1 defeated demon.",
                DiamondsNeeded = 2,
                KillsNeeded = 1,
                TimeSeconds = 150,
            },
            new LevelData
            {
                Title = "Keys and Gates",
                Tutorial = true,
                Hint = "Pick up the key and walk into the gold door to open it. Each key opens one door. Collect 3 rings, open the door, then enter the gate.",
                DiamondsNeeded = 3,
                TimeSeconds = 150,
            },
            new LevelData
            {
                Title = "Ashfields",
                RandomExit = true,
                Hint = "Demons wander the tunnels, and every 30 seconds a ring drifts away and respawns somewhere else. Keep moving. Press M for the full map.",
                DiamondsNeeded = 8,
                DriftSeconds = 30,
                TimeSeconds = 200,
            },
            new LevelData
            {
                Title = "Walled Pits",
                RandomExit = true,
                Hint = "Two demons this time, and the walls trap rocks. Rings drift every 25 seconds. Plan your route.",
                DiamondsNeeded = 8,
                DriftSeconds = 25,
                TimeSeconds = 200,
            },
            new LevelData
            {
                Title = "Crossroads",
                RandomExit = true,
                Hint = "The cave is shaped like a cross, and the gate is in a different place every time. Rings drift every 30 seconds.",
                DiamondsNeeded = 10,
                DriftSeconds = 30,
                TimeSeconds = 420,
            },
            new LevelData
            {
                Title = "The Ring",
                RandomExit = true,
                Hint = "Circle the pit to find the gate. It moves every time you start.",
                DiamondsNeeded = 12,
                DriftSeconds = 25,
                TimeSeconds = 450,
            },
            new LevelData
            {
                Title = "Caverns",
                RandomExit = true,
                Hint = "A twisting cave with hidden demons and a hidden gate. Use the minimap.",
                DiamondsNeeded = 12,
                DriftSeconds = 25,
                TimeSeconds = 450,
            },
            new LevelData
            {
                Title = "The Abyss",
                RandomExit = true,
                Hint = "A big level. Rings drift every 20 seconds. Watch the minimap in the corner, and press M for the whole map.",
                DiamondsNeeded = 20,
                DriftSeconds = 20,
                TimeSeconds = 500,
            },
        };
    }
}
