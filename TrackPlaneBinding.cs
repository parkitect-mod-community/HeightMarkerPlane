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

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TrackHeightPlane
{
    public class TrackPlaneBinding : MonoBehaviour
    {
        private TrackBuilder TrackBuilder { get; set; }
        private TrackedRide TrackRide { get; set; }
        private readonly List<HeightMarkerPlane> _planes = new List<HeightMarkerPlane>();
        private TrackSegment4 _selectedSegment;

        private void Awake()
        {

        }

        private void Start()
        {
            TrackBuilder = gameObject.GetComponentInChildren<TrackBuilder>();
        }

        private TrackedRide GetTrackRide()
        {
            return (TrackedRide) TrackBuilder.GetType()
                .GetField("trackedRide", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(TrackBuilder);
        }

        private TrackSegment4 GetSelectedSegment()
        {
            int trackCursorPosition = (int) TrackBuilder.GetType().GetField("trackCursorPosition",
                BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(TrackBuilder);
            return GetTrackRide().Track.trackSegments[trackCursorPosition];
        }

        private TrackSegment4 GetGhostSegment()
        {
            GameObject go =  (GameObject) TrackBuilder.GetType()
                .GetField("ghost", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(TrackBuilder);
            if (go == null)
                return null;
            return go.GetComponent<TrackSegment4>();
            
        }

        private void OnDestroy()
        {
            ClearPlanes();
        }

        private void ClearPlanes()
        {
            foreach (var plane in _planes)
            {
                Destroy(plane);
            }

            _planes.Clear();
        }

        private void Update()
        {
            if (TrackRide != null)
            {
                if (!TrackRide.isBeingEdited)
                {
                    ClearPlanes();
                }

                TrackSegment4 segment = GetSelectedSegment();
                if (segment != _selectedSegment)
                {
                    ClearPlanes();
                    if(!segment.gameObject.GetComponent<HeightMarkerPlane>() != null)
                     _planes.Add(segment.gameObject.AddComponent<HeightMarkerPlane>());
                   
                }
                
                TrackSegment4 ghost = GetGhostSegment();
                if (ghost != null)
                {
                    if (!ghost.gameObject.GetComponent<HeightMarkerPlane>() != null)
                        ghost.gameObject.AddComponent<HeightMarkerPlane>();
                }

                
                _selectedSegment = segment;
            }

            var ride = GetTrackRide();
            if (ride != TrackRide)
            {
                ClearPlanes();
                TrackRide = ride;
            }

        }

    }
}