// COPYRIGHT 2009 - 2024 by the Open Rails project.
//
// This file is part of Open Rails.
//
// Open Rails is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Open Rails is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Open Rails.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orts.Formats.Msts;
using Orts.Parsers.Msts;
using Orts.Parsers.OR;

namespace Orts.Formats.OR
{
    /// <summary>
    /// Return an Open Rails entity
    /// </summary>
    public class Consist
    {
        public string Name; // from the Name field or label field of the consist file
        public Train_Config Train;

        public Consist(string filePathWithoutExtension)
        {
            var filePath = filePathWithoutExtension + ".json";
            // try .json extension
            if (System.IO.File.Exists(filePath))
            {
                JsonReader.ReadFile(filePath, TryParse);
            }
            else
            {
                filePath = filePathWithoutExtension + ".con";
                using (var stf = new STFReader(filePath, false))
                    stf.ParseFile(new STFReader.TokenProcessor[] {
                        new STFReader.TokenProcessor("train", ()=>{ Train = new Train_Config(stf); }),
                });
            }
            Name = Train?.TrainCfg.Name;
        }

        /// <summary>
        /// Parses next item from JSON data, populating a ClockShape and issuing error messages.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool TryParse(JsonReader item)
        {
            var stringValue = "";
            switch (item.Path)
            {
                case "":
                case "[]":
                    // Ignore these items.
                    break;

                case "[].":
                    // Create an object with properties which are initially incomplete, so omissions can be detected and the object rejected later.
                    //ClockShape = new ClockShape(stringValue, null);
                    //ClockShapeList.Add(ClockShape);
                    break;

                case "[].Name":
                    // Parse the property with default value as invalid, so errors can be detected and the object rejected later.
                    stringValue = item.AsString(stringValue);
                    //var path = ShapePath + stringValue;
                    //ClockShape.Name = path;
                    //if (string.IsNullOrEmpty(stringValue))
                    //    return false;
                    //if (File.Exists(path) == false)
                    //    Trace.TraceWarning($"Non-existent shape file {path} referenced in animated.clocks-or");
                    break;

                case "[].ClockType":
                    stringValue = item.AsString(stringValue).ToLower();
                    //if (stringValue == "analog")
                    //{
                    //    ClockShape.ClockType = ClockType.Analog;
                    //}
                    //else
                    //{
                    //    Trace.TraceWarning($"ClockType \"{stringValue}\" found, but \"analog\" expected");
                    //    return false;
                    //}
                    break;

                default:
                    Trace.TraceWarning($"Unexpected entry \"{item.Path}\" found");
                    return false;
            }
            return true;
        }
    }
}
