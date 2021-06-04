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
        Dictionary<string, Action> commands;
        MyCommandLine commandLine;

        public Program()
        {
            commandLine = new MyCommandLine();
            commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);
            commands["init"] = InitSensors;
            InitSensors();
        }

        public void Save()
        {

        }

        public void InitSensors()
        {
            List<IMySensorBlock> sensors = new List<IMySensorBlock>();
            GridTerminalSystem.GetBlocksOfType(sensors);
            foreach (var sensor in sensors)
            {
                string customData = sensor.CustomData;
                string[] customDataLines = customData.Split('\n');
                Echo($"Handling sensor: {sensor.CustomName}");
                if (customDataLines != null && customDataLines.Count() > 1)
                {
                    foreach (var sensorRange in customDataLines)
                    {
                        if (sensorRange.Contains('='))
                        {
                            string rangeSide = sensorRange.Split('=')[0];
                            float range = float.Parse(sensorRange.Split('=')[1]);
                            Echo($"Setting: {rangeSide} to: {range}");
                            switch (rangeSide.ToLower())
                            {
                                case "top":
                                    sensor.TopExtend = range;
                                    break;
                                case "bottom":
                                    sensor.BottomExtend = range;
                                    break;
                                case "left":
                                    sensor.LeftExtend = range;
                                    break;
                                case "right":
                                    sensor.RightExtend = range;
                                    break;
                                case "front":
                                    sensor.FrontExtend = range;
                                    break;
                                case "back":
                                    sensor.BackExtend = range;
                                    break;
                                default:
                                    Echo("Not a valid target extend");
                                    break;
                            }
                        }
                    }
                } else
                {
                    Echo("No custom data found for this sensor, ignoring");
                }
            }
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
