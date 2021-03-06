﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NCodeRiddian;
using NCodeRiddian.Input;
namespace FP1
{
    class Player
    {
        public string Name;
        public bool IsComputer;
        public Difficulty ComputerLevel;
        public GamePadStateManager GamePad;
        public Color PlayerColor;
        public bool isP1;

        public Player(string Name, Difficulty diff, PlayerIndex PlayerIdx)
        {
            ComputerLevel = diff;
            if (diff == Difficulty.NON_COMP)
            {
                GamePad = new ActiveController(PlayerIdx);
                IsComputer = false;
            }
            else
            {
                GamePad = new SimulatedController();
                IsComputer = true;
            }
            this.Name = Name;
            isP1 = false;
        }
        public Player(string Name, Difficulty diff, GamePadStateManager padstate)
        {
            ComputerLevel = diff;
            if (diff == Difficulty.NON_COMP)
            {
                GamePad = padstate;
                IsComputer = false;
            }
            else
            {
                GamePad = padstate;
                IsComputer = true;
            }
            this.Name = Name;
            isP1 = false;
        }
    }

    enum Difficulty : int
    {
        NON_COMP = -1,
        SEasy = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3,
        SHard = 4
    }
}
