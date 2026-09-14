 16BarLogicGame — Interactive Music System (FSM + FMOD)

An event-driven, quantized adaptive music system built in **Unity (C#)** with **FMOD Studio**. Game state changes (idle, explore, combat, anxiety, epic, win, die) drive music transitions that always land on the bar, not the moment the trigger fires.

Core systems

- **Finite state machine** — seven music states (`Idle`, `Explore`, `Combat`, `Anxiety`, `Epic`, `Win`, `Die`), each with a dedicated FMOD transition loop for musical continuity between states.

- **Quantized transitions** — a C# routine reads the FMOD timeline position, calculates BPM-derived bar length in milliseconds, and holds any queued state change until the exact end of the current bar before firing it. No mid-bar cuts.

- **Custom timers** — state-specific logic, e.g. an Inactive Timer that governs entry into Idle, and a Last Combat Timer that governs safely exiting Combat rather than dropping out of it mid-encounter.

- **Priority locking** — higher-intensity states (Anxiety) can override and lock out lower-tier state changes until they resolve, so a queued Explore/Combat transition can't interrupt a state that needs to finish first.

Stack

Unity 2022.3, C#, FMOD Studio.

Development notes

The state machine design, the quantization approach, and the timer/priority logic are my own. Implementation was done iteratively with AI pair-programming, particularly for the UI, combat, and spawner scaffolding used to test the system in a playable scene. I can walk through and explain any part of this repository on request.
