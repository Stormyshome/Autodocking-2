﻿using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class ShipSystemsController
        {
            private readonly Program parent_program;

            public ShipSystemsController(Program _parent_program)
            {
                parent_program = _parent_program;
            }

            /// <summary>
            /// Overrides all the gyros on the ship and sets them to these specific speeds.
            /// </summary>
            /// <param name="pitch_speed"></param>
            /// <param name="yaw_speed"></param>
            /// <param name="roll_speed"></param>
            /// <param name="gyro_list"></param>
            /// <param name="b_WorldMatrix"></param>
            public void ApplyGyroOverride(double pitch_speed, double yaw_speed, double roll_speed, List<IMyGyro> gyro_list, MatrixD b_WorldMatrix)
            {
                var rotationVec = new Vector3D(-pitch_speed, yaw_speed, roll_speed); //because keen does some weird stuff with signs 
                var relativeRotationVec = Vector3D.TransformNormal(rotationVec, b_WorldMatrix);
                bool hasDetected = false;

                foreach (var thisGyro in gyro_list)
                {
                if (thisGyro.IsWorking)
                {
                    var gyroMatrix = thisGyro.WorldMatrix;
                    var transformedRotationVec = Vector3D.TransformNormal(relativeRotationVec, Matrix.Transpose(gyroMatrix));

                    thisGyro.Pitch = (float)transformedRotationVec.X;
                    thisGyro.Yaw = (float)transformedRotationVec.Y;
                    thisGyro.Roll = (float)transformedRotationVec.Z;
                    thisGyro.GyroOverride = true;
                    }
                else if(!hasDetected)
                {
                    parent_program.systemsAnalyzer.basicDataGatherRequired = true;
                    parent_program.shipIOHandler.Echo("Warning:\nGyro damage detected, recomputing.");
                    hasDetected = true;
                }
            }
            }
            public void SetThrusterForces(ThrusterGroup thrusterGroup, double thrustToApply)
            {
                double thrustProportion = thrustToApply / thrusterGroup.MaxThrust;
                bool hasDetected = false;
                foreach (IMyThrust thisThruster in thrusterGroup.thrusters)
                {
                    if (thisThruster.IsWorking)
                    {
                        thisThruster.ThrustOverride = (float)(thisThruster.MaxThrust * thrustProportion);
                    }
                    else if (!hasDetected)
                    {
                        parent_program.systemsAnalyzer.basicDataGatherRequired = true;
                        parent_program.shipIOHandler.Echo("Warning:\nThruster damage detected, recomputing.");
                        hasDetected = true;
                    }
                }
            }
        }
    }
}