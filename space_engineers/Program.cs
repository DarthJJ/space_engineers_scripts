using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        IMyBlockGroup negUpRotorGroup;
        IMyBlockGroup posUpRotorGroup;
        List<IMyMotorStator> negUpRotors;
        List<IMyMotorStator> posUpRotors;
        Dictionary<string, Action> commands;
        int rotorVelocity = 10;
        MyCommandLine commandLine;

        public Program()
        {
            negUpRotorGroup = GridTerminalSystem.GetBlockGroupWithName("Neg Up");
            posUpRotorGroup = GridTerminalSystem.GetBlockGroupWithName("Pos Up");
            commandLine = new MyCommandLine();
            commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
            commands["stop"] = Stop;
            commands["up"] = Up;
            commands["down"] = Down;
            if (negUpRotorGroup == null || posUpRotorGroup == null)
            {
                Echo("Could not initialize");
                return;
            }
            Echo("Rotor groups found.");
        }

        public void GetRotors()
        {
            negUpRotors = new List<IMyMotorStator>();
            posUpRotors = new List<IMyMotorStator>();
            negUpRotorGroup.GetBlocksOfType(negUpRotors);
            posUpRotorGroup.GetBlocksOfType(posUpRotors);
            if (negUpRotors.Count == 0 || posUpRotors.Count == 0)
            {
                Echo("Rotors empty, error");
                return;
            }
        }

        public void Up()
        {
            GetRotors();
            foreach (var rotor in negUpRotors)
            {
                rotor.TargetVelocityRPM = -rotorVelocity;
            }
            foreach (var rotor in posUpRotors)
            {
                rotor.TargetVelocityRPM = rotorVelocity;
            }
        }

        public void Down()
        {
            GetRotors();
            foreach (var rotor in negUpRotors)
            {
                rotor.TargetVelocityRPM = rotorVelocity;
            }
            foreach (var rotor in posUpRotors)
            {
                rotor.TargetVelocityRPM = -rotorVelocity;
            }
        }

        public void Stop()
        {
            GetRotors();
            foreach (var rotor in negUpRotors)
            {
                rotor.TargetVelocityRPM = 0;
            }
            foreach (var rotor in posUpRotors)
            {
                rotor.TargetVelocityRPM = 0;
            }
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            MyCommandLine _commandLine = new MyCommandLine();
            if (commandLine.TryParse(argument))
            {
                Action commandAction;

                string command = commandLine.Argument(0);
                if (command == null)
                {
                    Echo("No command specified");
                }
                else if (commands.TryGetValue(command, out commandAction))
                {
                    commandAction();
                }
                else
                {
                    Echo($"Unknown command {command}");
                }
            }
            else
            {
                Echo("Please provide an argument");
                return;
            }
        }
    }
}
