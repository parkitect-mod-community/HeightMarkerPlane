/**
* Copyright 2019 Michael Pollind
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System.Linq;
using UnityEngine;

namespace TrackHeightPlane
{
    
    public class Main : IMod
    {

        public string Path
        {
            get
            {
                var path = ModManager.Instance.getModEntries().First(x => x.mod == this).path;
                return path;
            }
        }

  
        public void onEnabled()
        {

            ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.AddComponent<TrackPlaneBinding>();
        }

        public void onDisabled()
        {
            Object.Destroy(ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject
                .GetComponent<TrackPlaneBinding>());
        }

        public string Name => "Height Marker Plane";

        public string Description => "Draws a height marker plane for the selected track segment";

        string IMod.Identifier => "Height_Marker_Plane";

    }
}